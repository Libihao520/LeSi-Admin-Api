using System.Data;
using System.Data.Common;
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

    public MySqlDatabase(DatabaseCategory category,string connString,int dbTimeout)
    {
        DbContext = new MySqlDbContext(category,connString,dbTimeout);
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
