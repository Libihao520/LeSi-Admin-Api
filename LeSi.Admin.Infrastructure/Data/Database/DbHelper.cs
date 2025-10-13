using System.Data;
using System.Data.Common;
using System.Diagnostics;
using LeSi.Admin.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;

namespace LeSi.Admin.Infrastructure.Data.Database;

/// <summary>
/// 数据库帮助类（通用）
/// </summary>
public class DbHelper
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public static DatabaseType DbType { get; set; }

    private readonly DbContext _dbContext;

    public DbHelper(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// 执行SQL查询并返回DataReader
    /// </summary>
    public async Task<DbDataReader> ExecuteReaderAsync(string sql, params DbParameter[]? dbParameters)
    {
        var command = _dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = sql;
        command.CommandType = CommandType.Text;
        command.CommandTimeout = 180;

        // 添加参数
        if (dbParameters != null)
        {
            command.Parameters.Clear();
            foreach (var param in dbParameters)
            {
                command.Parameters.Add(param);
            }
        }

        // 确保连接已打开
        if (command.Connection.State != ConnectionState.Open)
        {
            await command.Connection.OpenAsync();
        }

        try
        {
            // 使用ExecuteReaderAsync执行查询
            return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        }
        catch
        {
            // 异常时关闭连接
            if (command.Connection.State == ConnectionState.Open)
            {
                command.Connection.Close();
            }

            throw;
        }
    }
}