using System.Security.Cryptography;
using System.Text;
using LeSi.Admin.Contracts.Models.User;
using LeSi.Admin.Domain.Entities.User;
using LeSi.Admin.Domain.Interfaces;
using LeSi.Admin.Domain.Interfaces.Repository;
using LeSi.Admin.Shared.Utilities.Encryption;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace LeSi.Admin.Application.User.QueryHandlers;

public class GetTokenQueryHandler(ICache cache, ITokenService tokenService, IRepositoryFactory repositoryFactory)
    : IRequestHandler<Queries.LoginQuery, Dtos.LoginDto>
{
    public async Task<Dtos.LoginDto> Handle(Queries.LoginQuery request, CancellationToken cancellationToken)
    {
        var privateKey = await cache.GetAsync<string>(request.PublicKey, cancellationToken);

        string decryptedUsername;
        string decryptedPassword;

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

        var userRepository = repositoryFactory.UserRepository();

        var user = await userRepository.FindEntityAsync<UsersEntity>(u => u.Name == decryptedUsername);
        if (user == null)
        {
            throw new ArgumentException("用户不存在");
        }

        if (user.PassWord != Md5Utilities.GetMd5Hash(decryptedPassword))
        {
            throw new ArgumentException("密码错误");
        }

        var token = tokenService.GetToken(user, privateKey);

        cache.Set(token, request.PublicKey, TimeSpan.FromMinutes(30));
        return new Dtos.LoginDto { Token = token };
    }
}