using System;

namespace LeSi.Admin.Infrastructure.CaChe;

public interface ICache
{
    T Get<T>(string key);
    void Set<T>(string key, T value, TimeSpan expiration);
}