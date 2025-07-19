using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LeSi.Admin.Infrastructure.Data.Database;

public interface IDatabase
{
    public DbContext DbContext { get; }
    public IDbContextTransaction dbContextTransaction { get; set; }

    /// <summary>
    /// 开始事务
    /// </summary>
    /// <returns></returns>
    Task<IDatabase> BeginTrans();

    /// <summary>
    /// 提交事务
    /// </summary>
    /// <returns></returns>
    Task Commit();

    /// <summary>
    /// 回滚事务
    /// </summary>
    /// <returns></returns>
    Task Rollback();

    /// <summary>
    /// 查询
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<IEnumerable<T>> FindList<T>() where T : class, new();

    /// <summary>
    /// SQL查询
    /// </summary>
    /// <param name="strSql"></param>
    /// <param name="dbParameter"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<IEnumerable<T>> FindList<T>(string strSql, DbParameter[] dbParameter) where T : class;
}