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

    public IDbContextTransaction dbContextTransaction { get; set; }

    public PostgreSqlDatabase(DatabaseCategory category, string connString, int dbTimeout)
    {
        DbContext = new PgsqlDbContext(category, connString, dbTimeout);
    }

    public Task<IDatabase> BeginTrans()
    {
        throw new NotImplementedException();
    }

    public Task Commit()
    {
        throw new NotImplementedException();
    }

    public Task Rollback()
    {
        throw new NotImplementedException();
    }

    public Task<T?> FindEntity<T>(Expression<Func<T, bool>> predicate) where T : class, new()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> FindList<T>() where T : class, new()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 根据SQL语句查询数据列表
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="strSql">SQL语句</param>
    /// <param name="dbParameter">参数数组</param>
    /// <returns>符合条件的数据列表</returns>
    public async Task<IEnumerable<T>> FindList<T>(string strSql, DbParameter[] dbParameter) where T : class
    {
        var reader = await new DbHelper(DbContext).ExecuteReaderAsync(strSql, dbParameter);
        return DatabasesExtension.IDataReaderToList<T>(reader);
    }
}