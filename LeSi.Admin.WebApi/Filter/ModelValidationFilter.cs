using LeSi.Admin.Contracts.ApiResponse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LeSi.Admin.WebApi.Filter;

/// <summary>
/// 模型验证过滤器
/// </summary>
public class ModelValidationFilter : IAsyncActionFilter
{
    /// <summary>
    /// 执行模型验证，若模型状态无效则返回400错误
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            var response = new ApiResponse<object>(
                400,
                "请求参数验证失败",
                errors
            );

            context.Result = new BadRequestObjectResult(response);
            return;
        }

        await next();
    }
}