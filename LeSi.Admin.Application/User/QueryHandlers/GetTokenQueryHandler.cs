using System.Security.Cryptography;
using System.Text;
using LeSi.Admin.Contracts.User;
using LeSi.Admin.Domain.Entities.User;
using LeSi.Admin.Domain.Interfaces;
using LeSi.Admin.Infrastructure.CaChe;
using LeSi.Admin.Infrastructure.Repository;
using LeSi.Admin.Shared.Utilities.Encryption;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace LeSi.Admin.Application.User.QueryHandlers;

public class GetTokenQueryHandler : RepositoryFactory, IRequestHandler<Queries.LoginDtoQuery, Dtos.LoginDto>
{
    private readonly ICache _memoryCache;
    private readonly ICache _redisCache;
    private readonly ITokenService _tokenService;

    public GetTokenQueryHandler([FromKeyedServices("MemoryCache")] ICache memoryCache,
        [FromKeyedServices("RedisCache")] ICache redisCache, ITokenService tokenService)
    {
        _memoryCache = memoryCache;
        _redisCache = redisCache;
        _tokenService = tokenService;
    }

    public async Task<Dtos.LoginDto> Handle(Queries.LoginDtoQuery request, CancellationToken cancellationToken)
    {
        var privateKey = _redisCache.Get<string>(request.PublicKey);

        string decryptedUsername = string.Empty;
        string decryptedPassword = string.Empty;

        // 使用私钥进行RSA解密
        try
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(privateKey); // 导入PEM格式私钥

            var decryptedUsernameBytes =
                rsa.Decrypt(Convert.FromBase64String(request.Username), RSAEncryptionPadding.Pkcs1);
            decryptedUsername = Encoding.UTF8.GetString(decryptedUsernameBytes);

            var decryptedPasswordBytes =
                rsa.Decrypt(Convert.FromBase64String(request.Password), RSAEncryptionPadding.Pkcs1);
            decryptedPassword = Encoding.UTF8.GetString(decryptedPasswordBytes);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException("输入的用户名或密码非Base64格式，解密失败", ex);
        }
        catch (CryptographicException ex)
        {
            throw new InvalidOperationException("解密失败：私钥无效或数据格式错误", ex);
        }

        var userRepository = UserRepository();

        var user = await userRepository.FindEntity<UsersEntity>(u => u.Name == decryptedUsername);
        if (user == null)
        {
            throw new ArgumentException("用户不存在");
        }

        if (user.PassWord != AesUtilities.Encrypt(decryptedPassword))
        {
            throw new ArgumentException("密码错误");
        }

        var token = _tokenService.GetToken(user, privateKey);
        _redisCache.Set(user.Name, token, TimeSpan.FromDays(1));
        return new Dtos.LoginDto { Token = token };
    }
}