using System.Text.Json;
using LeSi.Admin.Contracts.Logging;
using LeSi.Admin.Domain.Interfaces;
using LeSi.Admin.Infrastructure.Logging;
using Microsoft.IdentityModel.Logging;
using StackExchange.Redis;

namespace LeSi.Admin.Infrastructure.CaChe;

public class RedisCacheImp : ICache, IDisposable, IAsyncDisposable
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly IAppLogger _logger;

    public RedisCacheImp(
        IConnectionMultiplexer redis, IAppLogger logger, JsonSerializerOptions? serializerOptions = null)
    {
        _logger = logger;
        _redis = (ConnectionMultiplexer)(redis ?? throw new ArgumentNullException(nameof(redis)));
        _database = _redis.GetDatabase();
        _serializerOptions = serializerOptions ?? new JsonSerializerOptions();
        
        _logger.Debug("Redis缓存实例初始化完成");
    }

    public T? Get<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            var ex = new ArgumentException("缓存键不能为空或空白", nameof(key));
            _logger.Error("无效的缓存键", ex);
            throw ex;
        }

        try
        {
            var value = _database.StringGet(key);
            if (value.HasValue)
            {
                _logger.Debug($"成功获取键值 [{key}]");
                return JsonSerializer.Deserialize<T>(value, _serializerOptions);
            }
            
            _logger.Debug($"缓存中未找到键 [{key}]");
            return default;
        }
        catch (Exception ex)
        {
            _logger.Error($"获取键 [{key}] 的值时发生错误", ex);
            throw;
        }
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            var ex = new ArgumentException("缓存键不能为空或空白", nameof(key));
            _logger.Error("无效的缓存键", ex);
            throw ex;
        }

        try
        {
            var value = await _database.StringGetAsync(key);
            if (value.HasValue)
            {
                return JsonSerializer.Deserialize<T>(value, _serializerOptions);
            }
            
            _logger.Debug($"缓存中未找到键 [{key}]");
            return default;
        }
        catch (Exception ex)
        {
            _logger.Error($"获取键 [{key}] 的值时发生错误", ex);
            throw;
        }
    }

    public void Set<T>(string key, T value, TimeSpan expiration)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            var ex = new ArgumentException("缓存键不能为空或空白", nameof(key));
            _logger.Error("无效的缓存键", ex);
            throw ex;
        }

        if (value is null)
        {
            var ex = new ArgumentNullException(nameof(value));
            _logger.Error("缓存值不能为null", ex);
            throw ex;
        }

        try
        {
            var serializedValue = JsonSerializer.Serialize(value, _serializerOptions);
            _database.StringSet(key, serializedValue, expiration);
            _logger.Debug($"成功设置键 [{key}]，过期时间: {expiration}，序列化后的值: {serializedValue}");
        }
        catch (Exception ex)
        {
            _logger.Error($"设置键 [{key}] 的值时发生错误", ex);
            throw;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            var ex = new ArgumentException("缓存键不能为空或空白", nameof(key));
            _logger.Error("无效的缓存键", ex);
            throw ex;
        }

        if (value is null)
        {
            var ex = new ArgumentNullException(nameof(value));
            _logger.Error("缓存值不能为null", ex);
            throw ex;
        }

        try
        {
            var serializedValue = JsonSerializer.Serialize(value, _serializerOptions);
            await _database.StringSetAsync(key, serializedValue, expiration);
            _logger.Debug($"成功设置键 [{key}]，过期时间: {expiration}，序列化后的值: {serializedValue}");
        }
        catch (Exception ex)
        {
            _logger.Error($"设置键 [{key}] 的值时发生错误", ex);
            throw;
        }
    }

    public void Dispose()
    {
        try
        {
            _redis?.Dispose();
            GC.SuppressFinalize(this);
            _logger.Debug("Redis缓存实例已释放");
        }
        catch (Exception ex)
        {
            _logger.Error("释放Redis缓存时发生错误", ex);
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_redis != null)
            {
                await _redis.DisposeAsync().ConfigureAwait(false);
                _logger.Debug("Redis缓存实例已异步释放");
            }
        }
        catch (Exception ex)
        {
            _logger.Error("异步释放Redis缓存时发生错误", ex);
        }
    }
}