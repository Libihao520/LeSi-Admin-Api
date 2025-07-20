using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LeSi.Admin.Infrastructure.Data.Database;

namespace LeSi.Admin.Infrastructure.Repository
{
    /// <summary>
    /// 通用仓储基类，定义数据标准操作
    /// </summary>
    public class Repository
    {
        public IDatabase db;

        public Repository(IDatabase iDatabase)
        {
            this.db = iDatabase;
        }

        public async Task<Repository> BeginTrans()
        {
            await db.BeginTrans();
            return this;
        }

        /// <summary>
        /// 根据条件获取单条数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="predicate">查询条件</param>
        /// <returns>符合条件的单条数据，如果没有则返回null</returns>
        public async Task<T?> FindEntity<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            return await db.FindEntity(predicate);
        }

        public async Task<IEnumerable<T>> FindList<T>() where T : class, new()
        {
            return await db.FindList<T>();
        }

        public async Task<IEnumerable<T>> FindList<T>(string strSql, DbParameter[] dbParameter) where T : class
        {
            return await db.FindList<T>(strSql, dbParameter);
        }
    }
}