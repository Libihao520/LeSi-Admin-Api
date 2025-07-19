using System.Data.Common;
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