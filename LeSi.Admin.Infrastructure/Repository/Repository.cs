using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LeSi.Admin.Domain.Interfaces;
using LeSi.Admin.Domain.Interfaces.Repository;
using LeSi.Admin.Infrastructure.Data.Database;

namespace LeSi.Admin.Infrastructure.Repository
{
    /// <summary>
    /// 通用仓储基类，定义数据标准操作
    /// </summary>
    public class Repository(IDatabase iDatabase) : IRepository
    {
        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        public async Task<IRepository> BeginTransactionAsync()
        {
            await iDatabase.BeginTransactionAsync();
            return this;
        }

        /// <summary>
        /// 根据条件获取单条数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="predicate">查询条件</param>
        /// <returns>符合条件的单条数据，如果没有则返回null</returns>
        public async Task<T?> FindEntityAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            return await iDatabase.FindEntityAsync(predicate);
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class, new()
        {
            return await iDatabase.GetAllAsync<T>();
        }

        /// <summary>
        /// SQL查询
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="dbParameter"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string strSql, DbParameter[] dbParameter) where T : class
        {
            return await iDatabase.QueryAsync<T>(strSql, dbParameter);
        }
    }
}