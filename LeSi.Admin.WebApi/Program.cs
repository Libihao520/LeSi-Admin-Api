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

        app.UseMiddleware<ExceptionHandlerMiddleware>();
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