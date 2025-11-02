using LeSi.Admin.Contracts.Logging;
using LeSi.Admin.Domain.Interfaces;
using LeSi.Admin.Domain.Interfaces.Cache;
using Microsoft.Extensions.Caching.Memory;
using LeSi.Admin.Infrastructure.Logging;
using Microsoft.IdentityModel.Logging;

namespace LeSi.Admin.Infrastructure.Cache;

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
        ValidateKey(key);

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

    public bool TryGetValue<T>(string key, out T value)
    {
        ValidateKey(key);

        var result = _memoryCache.TryGetValue<T>(key, out value);
        if (result)
        {
            _logger.Debug($"成功获取缓存键 [{key}]");
        }
        else
        {
            _logger.Debug($"缓存键 [{key}] 未找到");
        }

        return result;
    }

    public void Set<T>(string key, T value, TimeSpan expiration)
    {
        ValidateKey(key);

        _memoryCache.Set(key, value, expiration);
        _logger.Debug($"设置缓存键 [{key}]，过期时间 {expiration}，值类型: {value?.GetType().Name ?? "null"}");
    }

    public void Remove(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            _logger.Debug("尝试删除空缓存键");
            return;
        }

        _memoryCache.Remove(key);
        _logger.Debug($"成功删除内存缓存键 [{key}]");
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<T?>(Get<T>(key));
    }

    public Task<(bool Success, T? Value)> TryGetValueAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var result = _memoryCache.TryGetValue<T>(key, out var value);

        return Task.FromResult((Success: result, Value: value));
    }

    public Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        Set(key, value, expiration);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        Remove(key);
        return Task.CompletedTask;
    }

    private void ValidateKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            var ex = new ArgumentException("缓存键不能为空或空白", nameof(key));
            _logger.Error("无效的缓存键", ex);
            throw ex;
        }
    }
}