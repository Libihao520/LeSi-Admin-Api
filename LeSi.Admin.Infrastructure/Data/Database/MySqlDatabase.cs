using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using LeSi.Admin.Infrastructure.Data.DbContexts;
using LeSi.Admin.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LeSi.Admin.Infrastructure.Data.Database;

public class MySqlDatabase : IDatabase
{
    /// <summary>
    /// MySQL数据库操作类
    /// </summary>
    public DbContext DbContext { get; }

    public IDbContextTransaction dbContextTransaction { get; set; }

    public MySqlDatabase(DatabaseCategory category, string connString, int dbTimeout)
    {
        DbContext = new MySqlDbContext(category, connString, dbTimeout);
    }


    /// <summary>
    /// 开始事务
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IDatabase> BeginTrans()
    {
        var dbConnection = DbContext.Database.GetDbConnection();
        if (dbConnection.State == ConnectionState.Closed)
        {
            await dbConnection.OpenAsync();
        }

        dbContextTransaction = await DbContext.Database.BeginTransactionAsync();
        return this;
    }

    public Task Commit()
    {
        throw new NotImplementedException();
    }

    public Task Rollback()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 根据条件获取单条数据
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="predicate">查询条件</param>
    /// <returns>符合条件的单条数据，如果没有则返回null</returns>
    public async Task<T?> FindEntity<T>(Expression<Func<T, bool>> predicate) where T : class, new()
    {
        return await DbContext.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<T>> FindList<T>() where T : class, new()
    {
        return await DbContext.Set<T>().ToListAsync();
    }

    public async Task<IEnumerable<T>> FindList<T>(string strSql, DbParameter[] dbParameter) where T : class
    {
        var reader = await new DbHelper(DbContext).ExecuteReaderAsync(strSql, dbParameter);
        return DatabasesExtension.IDataReaderToList<T>(reader);
    }
}