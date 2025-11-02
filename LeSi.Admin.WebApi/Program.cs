using LeSi.Admin.Domain.Interfaces;
using LeSi.Admin.Domain.Interfaces.Cache;
using LeSi.Admin.Infrastructure.Services;
using LeSi.Admin.WebApi.Filter;
using LeSi.Admin.WebApi.Middleware;
using LeSi.Admin.WebApi;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using NLog.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddProgramExtensions();

        var app = builder.Build();

        // 初始化 KeyResolverService
        var cache = app.Services.GetRequiredService<ICache>();
        KeyResolverService.Initialize(cache);

        app.UseMiddleware<ExceptionHandlerMiddleware>();

        #region 鉴权授权

        //通过 ASP.NET Core 中配置的授权认证，读取客户端中的身份标识(Cookie,Token等)并解析出来，存储到 context.User 中
        app.UseAuthentication();
        //判断当前访问 Endpoint (Controller或Action)是否使用了 [Authorize]以及配置角色或策略，然后校验 Cookie 或 Token 是否有效
        app.UseAuthorization();

        #endregion

        app.MapControllers();

        // 映射 gRPC 服务
        app.MapGrpcService<LeSi.Admin.WebApi.ProtoService.AuthService>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }
}