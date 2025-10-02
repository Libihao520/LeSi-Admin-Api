using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Reflection;
using LeSi.Admin.Domain.Interfaces;
using LeSi.Admin.Infrastructure.Config;
using LeSi.Admin.Infrastructure.Data.Database;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using Oracle.ManagedDataAccess.Client;

namespace LeSi.Admin.Infrastructure.Extensions
{
    public static class DatabasesExtension
    {
        /// <summary>
        /// 将DataReader数据转为Dynamic对象
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static dynamic DataFillDynamic(IDataReader reader)
        {
            using (reader)
            {
                dynamic d = new ExpandoObject();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    try
                    {
                        ((IDictionary<string, object>)d).Add(reader.GetName(i), reader.GetValue(i));
                    }
                    catch
                    {
                        ((IDictionary<string, object>)d).Add(reader.GetName(i), null);
                    }
                }

                return d;
            }
        }

        /// <summary>
        /// 将IDataReader转换为集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static List<T> IDataReaderToList<T>(IDataReader reader)
        {
            var list = new List<T>();
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            using (reader)
            {
                while (reader.Read())
                {
                    var obj = Activator.CreateInstance<T>();
                    foreach (var prop in props)
                    {
                        if (!reader.HasColumn(prop.Name) || reader[prop.Name] is DBNull) continue;
                        prop.SetValue(obj, Convert.ChangeType(reader[prop.Name], prop.PropertyType));
                    }

                    list.Add(obj);
                }
            }

            return list;
        }

        // 辅助方法：判断列是否存在
        private static bool HasColumn(this IDataRecord reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 根据配置文件中所配置的数据库类型
        /// 来创建相应数据库的参数对象
        /// </summary>
        /// <returns></returns>
        public static DbParameter CreateDbParameter()
        {
            return GlobalContext.SystemConfig.DbProvider switch
            {
                "SqlServer" => new SqlParameter(),
                "MySql" => new MySqlParameter(),
                "Oracle" => new OracleParameter(),
                _ => throw new Exception("数据库类型目前不支持！")
            };
        }

        /// <summary>
        /// 根据配置文件中所配置的数据库类型
        /// 来创建相应数据库的参数对象
        /// </summary>
        /// <returns></returns>
        public static DbParameter CreateDbParameter(string paramName, object value)
        {
            DbParameter param = CreateDbParameter();
            param.ParameterName = paramName;
            param.Value = value;
            return param;
        }
    }
}