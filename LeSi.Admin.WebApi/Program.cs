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


        // 配置 NLog
        builder.Logging.ClearProviders();
        builder.Host.UseNLog();

        builder.Services.AddControllers(options => { options.Filters.Add<ApiResponseFilter>(); });
        
        builder.WebHost.ConfigureKestrel(options =>
        {
            // 端口1：HTTP/1.1（REST API/Swagger）
            options.ListenAnyIP(5158, listenOptions => 
            {
                listenOptions.Protocols = HttpProtocols.Http1;
            });

            // 端口2：HTTP/2（gRPC）
            options.ListenAnyIP(5159, listenOptions => 
            {
                listenOptions.Protocols = HttpProtocols.Http2;
                // listenOptions.UseHttps(); // 生产环境启用 HTTPS
            });
        });
        builder.Services.AddGrpc();
        builder.Services.AddEndpointsApiExplorer();
        builder.Register();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseMiddleware<ExceptionHandlerMiddleware>();
        app.MapControllers();

        // 映射 gRPC 服务
        app.MapGrpcService<LeSi.Admin.WebApi.ProtoService.PublicKeyService>();

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