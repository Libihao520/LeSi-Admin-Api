using System.Security.Cryptography;
using System.Text;
using LeSi.Admin.Contracts.User;
using LeSi.Admin.Infrastructure.CaChe;
using LeSi.Admin.Infrastructure.Repository;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace LeSi.Admin.Application.User.QueryHandlers;

public class GetTokenQueryHandler: RepositoryFactory, IRequestHandler<Queries.LoginDtoQuery,Dtos.LoginDto>
{
    private readonly ICache _memoryCache;
    private readonly ICache _redisCache;

    public GetTokenQueryHandler([FromKeyedServices("MemoryCache")] ICache memoryCache,
        [FromKeyedServices("RedisCache")] ICache redisCache )
    {
        _memoryCache = memoryCache;
        _redisCache = redisCache;
    }

    public async Task<Dtos.LoginDto> Handle(Queries.LoginDtoQuery request, CancellationToken cancellationToken)
    {

        var privateKey =  _redisCache.Get<string>(request.PublicKey);
        
        string decryptedUsername = string.Empty;
        string decryptedPassword = string.Empty;
        
        // 使用私钥进行RSA解密
        try {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(privateKey); // 导入PEM格式私钥

            var decryptedUsernameBytes = rsa.Decrypt(Convert.FromBase64String(request.Username), RSAEncryptionPadding.Pkcs1);
             decryptedUsername = Encoding.UTF8.GetString(decryptedUsernameBytes);

            var decryptedPasswordBytes = rsa.Decrypt(Convert.FromBase64String(request.Password), RSAEncryptionPadding.Pkcs1);
             decryptedPassword = Encoding.UTF8.GetString(decryptedPasswordBytes);


        } catch (FormatException ex) {
            throw new InvalidOperationException("输入的用户名或密码非Base64格式，解密失败", ex);
        } catch (CryptographicException ex) {
            throw new InvalidOperationException("解密失败：私钥无效或数据格式错误", ex);
        }

        // 验证解密后的值（示例：简单判断非空）
        if (string.IsNullOrWhiteSpace(decryptedUsername) || string.IsNullOrWhiteSpace(decryptedPassword))
            throw new ArgumentException("用户名或密码无效");

        // 用私钥生成token（假实现）
        var token = $"FakeToken_{privateKey}_Generated";          

        return  new Dtos.LoginDto { Token = token } ;
    }
}