using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using LeSi.Admin.Domain.Entities;
using LeSi.Admin.Infrastructure.Data.DbContexts;
using LeSi.Admin.Infrastructure.Extensions;
using LeSi.Admin.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LeSi.Admin.Infrastructure.Data.Database;

public class SqLiteDatabase : IDatabase
{
    public DbContext DbContext { get; }

    public IDbContextTransaction DbContextTransaction { get; set; }

    public SqLiteDatabase(DatabaseCategory category, string connString, int dbTimeout)
    {
        DbContext = new SqliteDbContext(category, connString, dbTimeout);
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
        await DbContextTransaction.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        await DbContextTransaction.RollbackAsync();
    }

    public async Task<T> AddAsync<T>(T entity) where T : class, new()
    {
        await DbContext.Set<T>().AddAsync(entity);
        return entity;
    }

    public Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class, new()
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync<T>(T entity) where T : class, new()
    {
        if (entity is AuditableBaseEntity auditableEntity)
        {
            auditableEntity.IsDeleted = 1;
        }
    }

    public async Task DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
    {
        var entities = await DbContext.Set<T>()
            .Where(predicate)
            .ToListAsync();

        foreach (var entity in entities)
        {
            await DeleteAsync(entity);
        }
    }

    public async Task DeleteRangeAsync<T>(IEnumerable<T> entities) where T : class, new()
    {
        foreach (var entity in entities)
        {
            await DeleteAsync(entity);
        }
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
}