using System.Data.Common;
using System.Linq.Expressions;
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
    /// 根据条件获取单条数据
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="predicate">查询条件</param>
    /// <returns>符合条件的单条数据，如果没有则返回null</returns>
    Task<T?> FindEntity<T>(Expression<Func<T, bool>> predicate) where T : class, new();

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