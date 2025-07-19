using Microsoft.Extensions.Caching.Memory;
using LeSi.Admin.Infrastructure.Logging;

namespace LeSi.Admin.Infrastructure.CaChe;

public class MemoryCacheImp : ICache
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheImp(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        LogHelper.Debug("内存缓存实例初始化完成");
    }

    public T Get<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            var ex = new ArgumentException("缓存键不能为空或空白", nameof(key));
            LogHelper.Error("无效的缓存键", ex);
            throw ex;
        }

        var value = _memoryCache.Get<T>(key);
        if (value != null)
        {
            LogHelper.Debug($"成功获取缓存键 [{key}]");
        }
        else
        {
            LogHelper.Debug($"缓存键 [{key}] 未找到");
        }
        return value;
    }

    public void Set<T>(string key, T value, TimeSpan expiration)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            var ex = new ArgumentException("缓存键不能为空或空白", nameof(key));
            LogHelper.Error("无效的缓存键", ex);
            throw ex;
        }

        if (value == null)
        {
            var ex = new ArgumentNullException(nameof(value), "缓存值不能为null");
            LogHelper.Error("无效的缓存值", ex);
            throw ex;
        }

        _memoryCache.Set(key, value, expiration);
        LogHelper.Debug($"成功设置缓存键 [{key}]，过期时间 {expiration}");
    }
}