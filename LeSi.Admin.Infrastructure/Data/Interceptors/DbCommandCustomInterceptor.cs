using System.Data.Common;
using System.Diagnostics;
using LeSi.Admin.Infrastructure.Logging;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LeSi.Admin.Infrastructure.Data.Interceptors;

/// <summary>
/// Sql执行拦截器
/// </summary>
public class DbCommandCustomInterceptor : DbCommandInterceptor
{
    
    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        // 打印 SQL 和参数
        // LogHelper.Info($"Executing SQL: {command.CommandText}");
        if (command.Parameters.Count > 0)
        {
            // LogHelper.Info("Parameters:");
            foreach (DbParameter p in command.Parameters)
            {
                // LogHelper.Info($"  {p.ParameterName} = {p.Value}");
            }
        }

        // 记录执行时间
        var stopwatch = Stopwatch.StartNew();
        var originalResult = base.ReaderExecuting(command, eventData, result);
        stopwatch.Stop();

        // LogHelper.Info($"Execution Time: {stopwatch.ElapsedMilliseconds}ms");
        return originalResult;
    }
    
    
    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        // LogHelper.Info($"Executing SQL (Async): {command.CommandText}");
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }
}