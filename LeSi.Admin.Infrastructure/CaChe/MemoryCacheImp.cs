using LeSi.Admin.Contracts.Logging;
using LeSi.Admin.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using LeSi.Admin.Infrastructure.Logging;
using Microsoft.IdentityModel.Logging;

namespace LeSi.Admin.Infrastructure.CaChe;

public class MemoryCacheImp : ICache
{
    private readonly IMemoryCache _memoryCache;
    private readonly IAppLogger _logger;
    public MemoryCacheImp(IMemoryCache memoryCache, IAppLogger logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
        _logger.Debug("内存缓存实例初始化完成");
    }

    public T Get<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            var ex = new ArgumentException("缓存键不能为空或空白", nameof(key));
            _logger.Error("无效的缓存键", ex);
            throw ex;
        }

        var value = _memoryCache.Get<T>(key);
        if (value != null)
        {
            _logger.Debug($"成功获取缓存键 [{key}]");
        }
        else
        {
            _logger.Debug($"缓存键 [{key}] 未找到");
        }
        return value;
    }

    public void Set<T>(string key, T value, TimeSpan expiration)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            var ex = new ArgumentException("缓存键不能为空或空白", nameof(key));
            _logger.Error("无效的缓存键", ex);
            throw ex;
        }

        if (value == null)
        {
            var ex = new ArgumentNullException(nameof(value), "缓存值不能为null");
            _logger.Error("无效的缓存值", ex);
            throw ex;
        }

        _memoryCache.Set(key, value, expiration);
        _logger.Debug($"成功设置缓存键 [{key}]，过期时间 {expiration}");
    }
}