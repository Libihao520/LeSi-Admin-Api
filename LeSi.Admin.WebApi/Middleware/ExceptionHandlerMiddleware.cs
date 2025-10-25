using System.Net;
using System.Text.Json;
using LeSi.Admin.Contracts.ApiResponse;
using LeSi.Admin.Contracts.Exceptions;
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
        // 根据异常类型设置不同的HTTP状态码
        var (statusCode, message, errorCode) = ex switch
        {
            ValidationException validationEx =>
                (400, validationEx.Message, validationEx.Code),
            BusinessException businessEx =>
                (422, businessEx.Message, businessEx.Code),
            NotFoundException =>
                (404, ex.Message, "NOT_FOUND"),
            UnauthorizedException =>
                (401, ex.Message, "UNAUTHORIZED"),
            ForbiddenException =>
                (403, ex.Message, "FORBIDDEN"),
            _ =>
                (500, "系统内部错误，请稍后重试", "INTERNAL_ERROR")
        };

        context.Response.StatusCode = statusCode;

        var response = new ApiResponse<object>(
            context.Response.StatusCode,
            message,
            new { Error = ex.Message, Code = errorCode }
        );

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}