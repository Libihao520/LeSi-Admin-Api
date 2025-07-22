using LeSi.Admin.Contracts.Config;
using LeSi.Admin.Domain.Enums;
using LeSi.Admin.Domain.Interfaces;
using LeSi.Admin.Infrastructure.Data.Database;

namespace LeSi.Admin.Infrastructure.Repository;

public class RepositoryFactory: IRepositoryFactory
{
    /// <summary>
    /// 根据数据库分类获取对应的 Repository 实例
    /// </summary>
    /// <param name="category">数据库分类枚举值</param>
    /// <returns>对应的 Repository 实例</returns>
    public IRepository GetRepository(DatabaseCategory category)
    {
        IDatabase database = null;
        string dbType = GlobalContext.SystemConfig.DbProvider;
        string dbConnectionString;
        int dbTimeout;

        switch (category)
        {
            case DatabaseCategory.User:
                dbConnectionString = GlobalContext.SystemConfig.UserDb.DbConnectionString;
                dbTimeout = GlobalContext.SystemConfig.UserDb.DbCommandTimeout;
                break;
            case DatabaseCategory.Dictionary:
                dbConnectionString = GlobalContext.SystemConfig.DictionaryDb.DbConnectionString;
                dbTimeout = GlobalContext.SystemConfig.DictionaryDb.DbCommandTimeout;
                break;
            default:
                throw new ArgumentException("不支持的数据库分类", nameof(category));
        }

        switch (dbType)
        {
            case "SqlServer":
                // TODO 
                // database = new SqlServerDatabase(dbConnectionString);
                break;
            case "MySql":
                DbHelper.DbType = DatabaseType.MySql;
                database = new MySqlDatabase(category,dbConnectionString,dbTimeout);
                break;
            case "Oracle":
                // TODO
                break;
            default:
                throw new Exception("未找到数据库配置");
        }

        return new Repository(database);
    }

    /// <summary>
    /// 获取用户库的 Repository 实例
    /// </summary>
    /// <returns>用户库的 Repository 实例</returns>
    public IRepository UserRepository()
    {
        return GetRepository(DatabaseCategory.User);
    }

    /// <summary>
    /// 获取字典库的 Repository 实例
    /// </summary>
    /// <returns>字典库的 Repository 实例</returns>
    public IRepository DictionaryRepository()
    {
        return GetRepository(DatabaseCategory.Dictionary);
    }
}