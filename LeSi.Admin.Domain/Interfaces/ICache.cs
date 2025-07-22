namespace LeSi.Admin.Domain.Interfaces;

public interface ICache
{
    T Get<T>(string key);
    void Set<T>(string key, T value, TimeSpan expiration);
}