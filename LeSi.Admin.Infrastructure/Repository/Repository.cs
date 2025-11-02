using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LeSi.Admin.Domain.Entities;
using LeSi.Admin.Domain.Interfaces;
using LeSi.Admin.Domain.Interfaces.Database.Repository;
using LeSi.Admin.Domain.Interfaces.User;
using LeSi.Admin.Infrastructure.Data.Database;
using LeSi.Admin.Shared.Utilities.RandomId;
using Microsoft.AspNetCore.Http;

namespace LeSi.Admin.Infrastructure.Repository
{
    /// <summary>
    /// 通用仓储基类，定义数据标准操作
    /// </summary>
    public class Repository(IDatabase database, ICurrentUserService currentUserService) : IRepository
    {
        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        public async Task<IRepository> BeginTransactionAsync()
        {
            await database.BeginTransactionAsync();
            return this;
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <returns></returns>
        public async Task CommitAsync()
        {
            await database.CommitAsync();
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <returns></returns>
        public async Task RollbackAsync()
        {
            await database.RollbackAsync();
        }

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public async Task<T> AddAsync<T>(T entity) where T : class, new()
        {
            if (entity is AuditableBaseEntity auditableBase)
            {
                auditableBase.CreateDate = DateTime.UtcNow;
                auditableBase.IsDeleted = 0;
                auditableBase.CreateUserId = currentUserService.UserId;
            }

            if (entity is BaseEntity baseEntity)
            {
                baseEntity.Id = TimeBasedIdGeneratorUtil.GenerateId();
            }

            return await database.AddAsync(entity);
        }

        /// <summary>
        /// 批量新增实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        public async Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class, new()
        {
            await database.AddRangeAsync(entities);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public async Task DeleteAsync<T>(T entity) where T : class, new()
        {
            await database.DeleteAsync(entity);
        }

        /// <summary>
        /// 根据条件删除
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="predicate">删除条件</param>
        /// <returns></returns>
        public async Task DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            await database.DeleteAsync(predicate);
        }

        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        public async Task DeleteRangeAsync<T>(IEnumerable<T> entities) where T : class, new()
        {
            await database.DeleteRangeAsync(entities);
        }

        /// <summary>
        /// 根据条件获取单条数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="predicate">查询条件</param>
        /// <returns>符合条件的单条数据，如果没有则返回null</returns>
        public async Task<T?> FindEntityAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            return await database.FindEntityAsync(predicate);
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class, new()
        {
            return await database.GetAllAsync<T>();
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
            return await database.QueryAsync<T>(strSql, dbParameter);
        }

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>影响的行数</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await database.SaveChangesAsync();
        }
    }
}