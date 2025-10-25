using System.Data.Common;
using System.Linq.Expressions;

namespace LeSi.Admin.Domain.Interfaces.Repository;

public interface IRepository

{
    // <summary>
    /// 开启事务
    /// </summary>
    /// <returns></returns>
    Task<IRepository> BeginTransactionAsync();

    /// <summary>
    /// 提交事务
    /// </summary>
    /// <returns></returns>
    Task CommitAsync();

    /// <summary>
    /// 回滚事务
    /// </summary>
    /// <returns></returns>
    Task RollbackAsync();

    /// <summary>
    /// 新增实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entity">实体对象</param>
    /// <returns></returns>
    Task<T> AddAsync<T>(T entity) where T : class, new();

    /// <summary>
    /// 批量新增实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entities">实体集合</param>
    /// <returns></returns>
    Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class, new();

    /// <summary>
    /// 删除实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entity">实体对象</param>
    /// <returns></returns>
    Task DeleteAsync<T>(T entity) where T : class, new();

    /// <summary>
    /// 根据条件删除
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="predicate">删除条件</param>
    /// <returns></returns>
    Task DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new();

    /// <summary>
    /// 批量删除实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="entities">实体集合</param>
    /// <returns></returns>
    Task DeleteRangeAsync<T>(IEnumerable<T> entities) where T : class, new();

    /// <summary>
    /// 根据条件获取单条数据
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="predicate">查询条件</param>
    /// <returns>符合条件的单条数据，如果没有则返回null</returns>
    Task<T?> FindEntityAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new();

    /// <summary>
    /// 查询所有数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<IEnumerable<T>> GetAllAsync<T>() where T : class, new();

    /// <summary>
    /// SQL查询
    /// </summary>
    /// <param name="strSql"></param>
    /// <param name="dbParameter"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<IEnumerable<T>> QueryAsync<T>(string strSql, DbParameter[] dbParameter) where T : class;

    /// <summary>
    /// 保存更改
    /// </summary>
    /// <returns>影响的行数</returns>
    Task<int> SaveChangesAsync();
}