using LeSi.Admin.Infrastructure.Config;
using LeSi.Admin.WebApi;
using LeSi.Admin.WebApi.Filter;
using LeSi.Admin.WebApi.Middleware;
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

builder.Services.AddEndpointsApiExplorer();
builder.Register();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

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