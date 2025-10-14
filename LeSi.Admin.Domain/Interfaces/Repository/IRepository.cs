using System.Data.Common;
using System.Linq.Expressions;

namespace LeSi.Admin.Domain.Interfaces.Repository;

public interface IRepository
{
    Task<T?> FindEntityAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new();
    Task<IEnumerable<T>> GetAllAsync<T>() where T : class, new();
    Task<IEnumerable<T>> QueryAsync<T>(string strSql, DbParameter[] dbParameter) where T : class;
    Task<IRepository> BeginTransactionAsync();
}