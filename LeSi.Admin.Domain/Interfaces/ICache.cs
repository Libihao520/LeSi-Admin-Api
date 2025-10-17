namespace LeSi.Admin.Domain.Interfaces;

public interface ICache
{
    T? Get<T>(string key);
    bool TryGetValue<T>(string key, out T? value);
    void Set<T>(string key, T value, TimeSpan expiration);
    void Remove(string key);

    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task<(bool Success, T? Value)> TryGetValueAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}