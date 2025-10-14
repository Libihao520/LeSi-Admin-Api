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
using StackExchange.Redis;


namespace LeSi.Admin.WebApi;

public static class HostBuilderExtend
{
    public static void Register(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IAppLogger, NLogLogger>();
        
        GlobalContext.SystemConfig = builder.Configuration
            .GetSection("SystemConfig")
            .Get<SystemConfig>() ?? throw new InvalidOperationException("SystemConfig 配置缺失或格式错误");
        //auto mapper
        builder.Services.AddAutoMapper(typeof(AutoMapperConfigs));

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<DictionaryQueryHandler>());


        // 注册内存缓存
        builder.Services.AddMemoryCache();
        builder.Services.AddKeyedSingleton<ICache, MemoryCacheImp>("MemoryCache");

        // 注册 Redis 缓存
        builder.Services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(GlobalContext.SystemConfig.RedisConnectionString));
        builder.Services.AddKeyedSingleton<ICache, RedisCacheImp>("RedisCache");

        // 作为普通服务注册（通过接口）
        builder.Services.AddSingleton<IKeyPairManager, KeyPairManager>();
        // 作为托管服务注册（复用同一个实例）
        builder.Services.AddHostedService(provider =>
            provider.GetRequiredService<IKeyPairManager>() as KeyPairManager ??
            throw new InvalidOperationException("KeyPairManager not found"));
        
        // 注册 TokenService
        builder.Services.AddTransient<ITokenService, TokenService>();
        builder.Services.AddScoped<IDatabaseParameterFactory, DatabaseParameterFactory>();
        builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();
    }
}