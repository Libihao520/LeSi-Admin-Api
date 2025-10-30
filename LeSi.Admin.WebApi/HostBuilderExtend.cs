using LeSi.Admin.Application.Dictionary.QueryHandlers;
using LeSi.Admin.Contracts.Logging;
using LeSi.Admin.Domain.Interfaces;
using LeSi.Admin.Domain.Interfaces.Repository;
using LeSi.Admin.Infrastructure.Cache;
using LeSi.Admin.Infrastructure.Config;
using LeSi.Admin.Infrastructure.Data.Database;
using LeSi.Admin.Infrastructure.Logging;
using LeSi.Admin.Infrastructure.Repository;
using LeSi.Admin.Infrastructure.Services;
using LeSi.Admin.WebApi.Filter;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using NLog.Web;
using StackExchange.Redis;


namespace LeSi.Admin.WebApi;

/// <summary>
/// 程序扩展方法
/// </summary>
public static class HostBuilderExtend
{
    /// <summary>
    /// 扩展方法，总入口
    /// </summary>
    /// <param name="builder"></param>
    public static void AddProgramExtensions(this WebApplicationBuilder builder)
    {
        builder.AddConfiguration(); // 配置管理
        builder.AddLoggingServices(); // 日志服务
        builder.AddAutoMapperServices(); // AutoMapper
        builder.AddMediatRServices(); // MediatR
        builder.AddCacheServices(); // 缓存服务
        builder.AddInfrastructureServices(); // 基础设施服务
        builder.AddApplicationServices(); // 应用服务
        builder.AddWebApiServices(); // Web API 服务
        builder.AddGrpcServices(); // gRPC 服务
        builder.AddSwaggerServices(); // Swagger 文档
    }

    /// <summary>
    /// 配置管理
    /// </summary>
    /// <param name="builder"></param>
    private static void AddConfiguration(this WebApplicationBuilder builder)
    {
        builder.Configuration
            .AddJsonFile("appsettings.json")
            .AddJsonFile(
                $"appsettings.{(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true" ? "Docker" : "Development")}.json",
                optional: true);

        GlobalContext.SystemConfig = builder.Configuration
            .GetSection("SystemConfig")
            .Get<SystemConfig>() ?? throw new InvalidOperationException("SystemConfig 配置缺失或格式错误");
    }

    /// <summary>
    /// 日志服务
    /// </summary>
    /// <param name="builder"></param>
    private static void AddLoggingServices(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Host.UseNLog();
        builder.Services.AddSingleton<IAppLogger, NLogLogger>();
    }

    /// <summary>
    /// Web API 服务配置
    /// </summary>
    /// <param name="builder"></param>
    private static void AddWebApiServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                // 禁用自动模型验证，让过滤器来处理
                options.SuppressModelStateInvalidFilter = true;
            });
        
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<ModelValidationFilter>();
            options.Filters.Add<ApiResponseFilter>();
        });

        builder.WebHost.ConfigureKestrel(options =>
        {
            // 端口1：HTTP/1.1（REST API/Swagger）
            options.ListenAnyIP(5158, listenOptions => { listenOptions.Protocols = HttpProtocols.Http1; });

            // 端口2：HTTP/2（gRPC）
            options.ListenAnyIP(5159, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
                // listenOptions.UseHttps(); // 生产环境启用 HTTPS
            });
        });

        builder.Services.AddEndpointsApiExplorer();
    }

    /// <summary>
    /// gRPC 服务配置
    /// </summary>
    /// <param name="builder"></param>
    private static void AddGrpcServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddGrpc();
    }

    /// <summary>
    /// Swagger 文档配置
    /// </summary>
    /// <param name="builder"></param>
    private static void AddSwaggerServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(options =>
        {
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "LeSi.Admin.WebApi.xml"));

            // 可以在这里添加更多的 Swagger 配置
            // options.SwaggerDoc("v1", new OpenApiInfo { Title = "Admin API", Version = "v1" });
            // 添加 JWT 认证支持等
        });
    }

    /// <summary>
    /// AutoMapper配置
    /// </summary>
    /// <param name="builder"></param>
    private static void AddAutoMapperServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(typeof(AutoMapperConfigs));
    }


    /// <summary>
    /// MediatR配置
    /// </summary>
    /// <param name="builder"></param>
    private static void AddMediatRServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<DictionaryQueryHandler>());
    }

    /// <summary>
    /// 缓存服务
    /// </summary>
    /// <param name="builder"></param>
    private static void AddCacheServices(this WebApplicationBuilder builder)
    {
        //可选缓存配置

        // 内存缓存
        // builder.Services.AddMemoryCache();
        // builder.Services.AddSingleton<ICache, MemoryCacheImp>();

        // Redis缓存配置
        builder.AddRedisCacheServices();
    }

    /// <summary>
    /// Redis缓存服务（可选）
    /// </summary>
    /// <param name="builder"></param>
    private static void AddRedisCacheServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(GlobalContext.SystemConfig.RedisConnectionString));
        builder.Services.AddSingleton<ICache, RedisCacheImp>();
    }

    /// <summary>
    /// 基础设施服务
    /// </summary>
    /// <param name="builder"></param>
    private static void AddInfrastructureServices(this WebApplicationBuilder builder)
    {
        // KeyPairManager 同时作为服务接口和托管服务
        builder.Services.AddSingleton<IKeyPairManager, KeyPairManager>();
        builder.Services.AddHostedService(provider =>
            provider.GetRequiredService<IKeyPairManager>() as KeyPairManager ??
            throw new InvalidOperationException("KeyPairManager not found"));
    }

    /// <summary>
    /// 应用服务
    /// </summary>
    /// <param name="builder"></param>
    private static void AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ITokenService, TokenService>();
        builder.Services.AddScoped<IDatabaseParameterFactory, DatabaseParameterFactory>();
        builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();
    }

    /// <summary>
    /// 认证
    /// </summary>
    /// <param name="builder"></param>
    public static void AddAuthentication(this WebApplicationBuilder builder)
    {
        var authentication = GlobalContext.SystemConfig.Authentication;
        
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience =  authentication.Audience,
                    ValidIssuer = authentication.Issuer,
                    IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                    {
                        var publicKey = KeyResolverService.GetPublicKeyFromDynamicSource(token);
                        return new[] { new RsaSecurityKey(publicKey) };
                    }
                };
                
            });
    }
}