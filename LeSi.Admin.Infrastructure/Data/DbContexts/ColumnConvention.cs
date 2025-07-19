using Microsoft.EntityFrameworkCore;

namespace LeSi.Admin.Infrastructure.Data.DbContexts
{
    public static class ColumnConvention
    {
        // 将属性名（如 ParentId）转为下划线风格（如 parent_id）并设置列名
        public static void SetColumnName(ModelBuilder modelBuilder, string entityName, string propertyName)
        {
            var entity = modelBuilder.Model.FindEntityType(entityName);
            if (entity == null) return;
            var property = entity.FindProperty(propertyName);
            if (property == null) return;
            var columnName = ToSnakeCase(propertyName);
            property.SetColumnName(columnName);
        }

        // 驼峰转下划线
        private static string ToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            var chars = new List<char>();
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsUpper(input[i]) && i > 0)
                {
                    chars.Add('_');
                }
                chars.Add(char.ToLowerInvariant(input[i]));
            }
            return new string(chars.ToArray());
        }
    }
}

