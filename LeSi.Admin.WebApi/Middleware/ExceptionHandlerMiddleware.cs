using System.Net;
using System.Text.Json;
using LeSi.Admin.Contracts.ApiResponse;
using LeSi.Admin.Contracts.Logging;

namespace LeSi.Admin.WebApi.Middleware;

/// <summary>
/// 全局异常处理中间件
/// </summary>
public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IAppLogger _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, IAppLogger logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.Error("全局异常捕获：{ErrorMessage}", ex);
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new ApiResponse<object>(
            context.Response.StatusCode,
            "系统内部错误，请稍后重试",
            new { Error = ex.Message }
        );

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}