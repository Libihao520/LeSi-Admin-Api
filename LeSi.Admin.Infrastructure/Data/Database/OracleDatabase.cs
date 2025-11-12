using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using LeSi.Admin.Infrastructure.Data.DbContexts;
using LeSi.Admin.Infrastructure.Extensions;
using LeSi.Admin.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LeSi.Admin.Infrastructure.Data.Database;

public class OracleDatabase : IDatabase
{
    public DbContext DbContext { get; }

    public IDbContextTransaction DbContextTransaction { get; set; }

    public OracleDatabase(DatabaseCategory category, string connString, int dbTimeout)
    {
        DbContext = new OracleDbContext(category, connString, dbTimeout);
    }

    public async Task<IDatabase> BeginTransactionAsync()
    {
        var dbConnection = DbContext.Database.GetDbConnection();
        if (dbConnection.State == ConnectionState.Closed)
        {
            await dbConnection.OpenAsync();
        }

        DbContextTransaction = await DbContext.Database.BeginTransactionAsync();
        return this;
    }

    public async Task CommitAsync()
    {
        if (DbContextTransaction != null)
        {
            await DbContextTransaction.CommitAsync();
            await DbContextTransaction.DisposeAsync();
            DbContextTransaction = null;
        }
    }

    public async Task RollbackAsync()
    {
        if (DbContextTransaction != null)
        {
            await DbContextTransaction.RollbackAsync();
            await DbContextTransaction.DisposeAsync();
            DbContextTransaction = null;
        }
    }

    public async Task<T> AddAsync<T>(T entity) where T : class, new()
    {
        var entry = await DbContext.Set<T>().AddAsync(entity);
        await SaveChangesAsync();
        return entry.Entity;
    }

    public async Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class, new()
    {
        await DbContext.Set<T>().AddRangeAsync(entities);
        await SaveChangesAsync();
    }

    public async Task DeleteAsync<T>(T entity) where T : class, new()
    {
        DbContext.Set<T>().Remove(entity);
        await SaveChangesAsync();
    }

    public async Task DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
    {
        var entities = DbContext.Set<T>().Where(predicate);
        DbContext.Set<T>().RemoveRange(entities);
        await SaveChangesAsync();
    }

    public async Task DeleteRangeAsync<T>(IEnumerable<T> entities) where T : class, new()
    {
        DbContext.Set<T>().RemoveRange(entities);
        await SaveChangesAsync();
    }

    public async Task<T?> FindEntityAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
    {
        return await DbContext.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class, new()
    {
        return await DbContext.Set<T>().ToListAsync();
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string strSql, DbParameter[] dbParameter) where T : class
    {
        var reader = await new DbHelper(DbContext).ExecuteReaderAsync(strSql, dbParameter);
        return DatabasesExtension.IDataReaderToList<T>(reader);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await DbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        DbContextTransaction?.Dispose();
        DbContext?.Dispose();
    }
}