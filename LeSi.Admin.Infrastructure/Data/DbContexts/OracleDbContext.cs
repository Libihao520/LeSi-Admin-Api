using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using LeSi.Admin.Domain.Attributes;
using LeSi.Admin.Infrastructure.Data.Interceptors;
using LeSi.Admin.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LeSi.Admin.Infrastructure.Data.DbContexts;

public class OracleDbContext : DbContext
{
    private string ConnectionString { get; set; }

    private int CommandTimeout { get; set; }

    private DatabaseCategory Category { get; set; }

    public OracleDbContext(DatabaseCategory category, string connectionString, int commandTimeout)
    {
        Category = category;
        ConnectionString = connectionString;
        CommandTimeout = commandTimeout;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseOracle(ConnectionString, options =>
        {
            options.CommandTimeout(CommandTimeout);
            // Oracle 特定配置
            options.UseOracleSQLCompatibility("11"); // 根据实际Oracle版本调整
        });
        optionsBuilder.AddInterceptors(new DbCommandCustomInterceptor());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        string targetDatabaseName = GetDatabaseNameByCategory(Category);
        Assembly entityAssembly = Assembly.Load(new AssemblyName("LeSi.Admin.Domain"));
        IEnumerable<Type> typesToRegister = entityAssembly.GetTypes().Where(p => !string.IsNullOrEmpty(p.Namespace))
            .Where(t => t.GetCustomAttribute<DatabaseAttribute>()?.DatabaseName == targetDatabaseName)
            .Where(p => !string.IsNullOrEmpty(p.GetCustomAttribute<TableAttribute>()?.Name));

        foreach (Type type in typesToRegister)
        {
            modelBuilder.Model.AddEntityType(type);
        }

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            string currentTableName = modelBuilder.Entity(entity.Name).Metadata.GetTableName();
            modelBuilder.Entity(entity.Name).ToTable(currentTableName);

            // Oracle 列名约定 - 默认转换为大写
            if (Category == DatabaseCategory.Dictionary)
            {
                var properties = entity.GetProperties();
                foreach (var property in properties)
                {
                    ColumnConvention.SetColumnName(modelBuilder, entity.Name, property.Name);
                }
            }

            // Oracle 特定的配置
            ConfigureOracleSpecificEntities(modelBuilder, entity);
        }

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Oracle 特定的实体配置
    /// </summary>
    private void ConfigureOracleSpecificEntities(ModelBuilder modelBuilder, IMutableEntityType entity)
    {
        // Oracle 表名和列名默认大写
        var tableName = modelBuilder.Entity(entity.Name).Metadata.GetTableName();
        if (!string.IsNullOrEmpty(tableName))
        {
            modelBuilder.Entity(entity.Name).ToTable(tableName.ToUpper());
        }

        // 配置序列用于自增主键
        var primaryKey = entity.FindPrimaryKey();
        if (primaryKey?.Properties.Count == 1)
        {
            var property = primaryKey.Properties[0];
            if (property.ClrType == typeof(int) || property.ClrType == typeof(long))
            {
                var sequenceName = $"{tableName}_SEQ";
                modelBuilder.Entity(entity.Name)
                    .Property(property.Name)
                    .ValueGeneratedOnAdd()
                    .UseOracleIdentityColumn(); // Oracle 12c+ 自增列

                // 对于旧版本Oracle，可以使用序列
                // .HasDefaultValueSql($"{sequenceName}.NEXTVAL");
            }
        }

        // 配置Oracle数据类型映射
        var properties = entity.GetProperties();
        foreach (var property in properties)
        {
            var clrType = property.ClrType;
            if (clrType == typeof(string))
            {
                // 字符串类型配置
                modelBuilder.Entity(entity.Name)
                    .Property(property.Name)
                    .HasColumnType("VARCHAR2")
                    .HasMaxLength(4000); // Oracle VARCHAR2 最大长度
            }
            else if (clrType == typeof(decimal) || clrType == typeof(decimal?))
            {
                // 十进制类型配置
                modelBuilder.Entity(entity.Name)
                    .Property(property.Name)
                    .HasColumnType("NUMBER");
            }
            else if (clrType == typeof(DateTime) || clrType == typeof(DateTime?))
            {
                // 日期时间类型配置
                modelBuilder.Entity(entity.Name)
                    .Property(property.Name)
                    .HasColumnType("DATE");
            }
        }
    }

    /// <summary>
    /// 根据数据库分类获取对应的数据库名称
    /// </summary>
    private string GetDatabaseNameByCategory(DatabaseCategory category)
    {
        switch (category)
        {
            case DatabaseCategory.User:
                return "user_db";
            case DatabaseCategory.Dictionary:
                return "dictionary_db";
            default:
                throw new ArgumentException("不支持的数据库分类", nameof(category));
        }
    }
}