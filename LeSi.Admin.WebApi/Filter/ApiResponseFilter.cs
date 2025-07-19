using LeSi.Admin.Infrastructure.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LeSi.Admin.WebApi.Filter;

public class ApiResponseFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executedContext = await next();

        // 处理成功响应
        if (executedContext.Result is OkObjectResult okResult)
        {
            executedContext.Result = new OkObjectResult(new ApiResponse<object>(okResult.Value));
        }
        // 处理无返回值的成功响应
        else if (executedContext.Result is OkResult)
        {
            executedContext.Result = new OkObjectResult(new ApiResponse<object>(null));
        }
        // 处理自定义错误响应（如 400、404、500 等）
        else if (executedContext.Result is ObjectResult objectResult && objectResult.StatusCode.HasValue)
        {
            var errorCode = objectResult.StatusCode.Value;
            var errorMessage = objectResult.Value?.ToString() ?? "操作失败";

            executedContext.Result = new ObjectResult(
                new ApiResponse<object>(errorCode, errorMessage, objectResult.Value)
            )
            {
                StatusCode = errorCode,
            };
        }
    }
}