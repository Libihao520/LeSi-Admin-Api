using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using LeSi.Admin.Infrastructure.Data.DbContexts;
using LeSi.Admin.Infrastructure.Extensions;
using LeSi.Admin.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LeSi.Admin.Infrastructure.Data.Database;

public class PostgreSqlDatabase : IDatabase
{
    /// <summary>
    /// PostgreSQL数据库操作类
    /// </summary>
    public DbContext DbContext { get; }

    public IDbContextTransaction DbContextTransaction { get; set; }

    public PostgreSqlDatabase(DatabaseCategory category, string connString, int dbTimeout)
    {
        DbContext = new PgsqlDbContext(category, connString, dbTimeout);
    }

    public async Task<IDatabase> BeginTransactionAsync()
    {
        var dbConnection = DbContext.Database.GetDbConnection();
        if (dbConnection.State == ConnectionState.Closed)
        {
            await dbConnection.OpenAsync();
        }

        DbContextTransaction = await DbContext.Database.BeginTransactionAsync();
        return this;
    }

    public async Task CommitAsync()
    {
        await DbContextTransaction.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        await DbContextTransaction.RollbackAsync();
    }

    public async Task<T?> FindEntityAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
    {
        return await DbContext.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class, new()
    {
        return await DbContext.Set<T>().ToListAsync();
    }

    /// <summary>
    /// 根据SQL语句查询数据列表
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="strSql">SQL语句</param>
    /// <param name="dbParameter">参数数组</param>
    /// <returns>符合条件的数据列表</returns>
    public async Task<IEnumerable<T>> QueryAsync<T>(string strSql, DbParameter[] dbParameter) where T : class
    {
        var reader = await new DbHelper(DbContext).ExecuteReaderAsync(strSql, dbParameter);
        return DatabasesExtension.IDataReaderToList<T>(reader);
    }
}