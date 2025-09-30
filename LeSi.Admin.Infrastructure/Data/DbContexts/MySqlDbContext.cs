using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using LeSi.Admin.Domain.Attributes;
using LeSi.Admin.Infrastructure.Data.Database;
using LeSi.Admin.Infrastructure.Data.Interceptors;
using LeSi.Admin.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace LeSi.Admin.Infrastructure.Data.DbContexts;

public class MySqlDbContext : DbContext
{
    private string ConnectionString { get; set; }

    private int CommandTimeout { get; set; }

    private DatabaseCategory Category { get; set; }

    public MySqlDbContext(DatabaseCategory category, string connectionString, int commandTimeout)
    {
        Category = category;
        ConnectionString = connectionString;
        CommandTimeout = commandTimeout;
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString),
            p => p.CommandTimeout(CommandTimeout));
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

            //列名约定，比如属性ParentId，映射到数据库字段parent_id
            if (Category == DatabaseCategory.Dictionary)
            {
                var properties = entity.GetProperties();
                foreach (var property in properties)
                {
                    ColumnConvention.SetColumnName(modelBuilder, entity.Name, property.Name);
                }
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// 根据数据库分类获取对应的数据库名称
    /// </summary>
    /// <param name="category">数据库分类</param>
    /// <returns>对应的数据库名称</returns>
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