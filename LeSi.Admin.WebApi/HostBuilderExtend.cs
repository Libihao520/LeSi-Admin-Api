using LeSi.Admin.Application;
using LeSi.Admin.Application.Dictionary.QueryHandlers;
using LeSi.Admin.Infrastructure.CaChe;
using LeSi.Admin.Infrastructure.Config;
using StackExchange.Redis;


namespace LeSi.Admin.WebApi;

public static class HostBuilderExtend
{
    public static void Register(this WebApplicationBuilder builder)
    {
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
        builder.Services.AddHostedService(provider => provider.GetRequiredService<IKeyPairManager>() as KeyPairManager ?? throw new InvalidOperationException("KeyPairManager not found"));
    }
}