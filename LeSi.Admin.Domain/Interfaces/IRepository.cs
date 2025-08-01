using System.Data.Common;
using System.Linq.Expressions;

namespace LeSi.Admin.Domain.Interfaces;

public interface IRepository
{
    Task<T?> FindEntity<T>(Expression<Func<T, bool>> predicate) where T : class, new();
    Task<IEnumerable<T>> FindList<T>() where T : class, new();
    Task<IEnumerable<T>> FindList<T>(string strSql, DbParameter[] dbParameter) where T : class;
    Task<IRepository> BeginTrans();
}