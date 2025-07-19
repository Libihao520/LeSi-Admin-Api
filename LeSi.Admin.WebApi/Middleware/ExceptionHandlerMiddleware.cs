using System.Net;
using System.Text.Json;
using LeSi.Admin.Infrastructure.Shared;

namespace LeSi.Admin.WebApi.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
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

