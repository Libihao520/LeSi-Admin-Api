using LeSi.Admin.Shared.Enums;

namespace LeSi.Admin.Infrastructure.Data.DbContexts;

internal static class DatabaseHelper
{
    public static string GetDatabaseNameByCategory(DatabaseCategory category)
    {
        switch (category)
        {
            case DatabaseCategory.User:
                return "user_db";
            case DatabaseCategory.Dictionary:
                return "dictionary_db";
            default:
                throw new ArgumentException("不支持的数据库分类", nameof(category));
        }
    }
}