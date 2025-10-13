
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using LeSi.Admin.Domain.Attributes;
using LeSi.Admin.Infrastructure.Data.Database;
using LeSi.Admin.Infrastructure.Data.Interceptors;
using LeSi.Admin.Shared.Enums;
using Microsoft.EntityFrameworkCore;
// 添加 Npgsql 引用
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace LeSi.Admin.Infrastructure.Data.DbContexts;

public class PgsqlDbContext : DbContext
{
    private string ConnectionString { get; set; }
    private int CommandTimeout { get; set; }
    private DatabaseCategory Category { get; set; } 

    public PgsqlDbContext(DatabaseCategory category, string connectionString, int commandTimeout)
    {
        Category = category;
        ConnectionString = connectionString;
        CommandTimeout = commandTimeout;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // 使用 UseNpgsql 替代 UseMySql
        optionsBuilder.UseNpgsql(ConnectionString, p => p.CommandTimeout(CommandTimeout));
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
            // PostgreSQL 默认使用蛇形命名，可以考虑使用 ToTable 指定具体名称
            modelBuilder.Entity(entity.Name).ToTable(currentTableName);

            // 列名约定，适用于 PostgreSQL 的蛇形命名
            if (Category == DatabaseCategory.Dictionary)
            {
                var properties = entity.GetProperties();
                foreach (var property in properties)
                {
                    // 可能需要调整为适合 PostgreSQL 的列名转换逻辑
                    ColumnConvention.SetColumnName(modelBuilder, entity.Name, property.Name);
                }
            }
        }

        base.OnModelCreating(modelBuilder);
    }

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