namespace LeSi.Admin.Infrastructure.Cache;

public static class CacheKeys
{
    public static string PublicKeyRequestRateLimit(string clientIp)
    {
        return $"publickey_request_limit_{clientIp}";
    }
}