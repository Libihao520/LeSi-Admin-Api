using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using LeSi.Admin.Domain.Attributes;
using LeSi.Admin.Infrastructure.Data.Database;
using LeSi.Admin.Infrastructure.Data.Interceptors;
using LeSi.Admin.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace LeSi.Admin.Infrastructure.Data.DbContexts;

public class SqliteDbContext : DbContext
{
    private string ConnectionString { get; set; }
    private int CommandTimeout { get; set; }
    private DatabaseCategory Category { get; set; } 

    public SqliteDbContext(DatabaseCategory category, string connectionString, int commandTimeout)
    {
        Category = category;
        ConnectionString = connectionString;
        CommandTimeout = commandTimeout;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(ConnectionString, p => p.CommandTimeout(CommandTimeout));
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
            // SQLite 表名处理
            modelBuilder.Entity(entity.Name).ToTable(currentTableName);

            // 列名约定，适用于 SQLite
            if (Category == DatabaseCategory.Dictionary)
            {
                var properties = entity.GetProperties();
                foreach (var property in properties)
                {
                    // 使用相同的列名转换逻辑
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
