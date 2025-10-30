using System.Security.Cryptography;
using LeSi.Admin.Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace LeSi.Admin.Infrastructure.Services;

public static class KeyResolverService
{
    private static ICache _cache;

    /// <summary>
    /// 初始化缓存实例（在程序启动时调用一次）
    /// </summary>
    public static void Initialize(ICache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// 从动态源获取公钥 - 直接从缓存中通过token获取
    /// </summary>
    public static RSA GetPublicKeyFromDynamicSource(string token)
    {
        if (_cache == null)
        {
            throw new InvalidOperationException("KeyResolverService 未初始化，请先调用 Initialize 方法");
        }

        // 直接用token作为key从缓存中获取公钥
        var publicKeyPem = _cache.Get<string>(token);

        if (string.IsNullOrEmpty(publicKeyPem))
        {
            throw new SecurityTokenValidationException("Token对应的公钥不存在或已过期");
        }

        // 将PEM格式的公钥转换为RSA
        var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyPem);

        return rsa;
    }
}