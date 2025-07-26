using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using LeSi.Admin.Domain.Entities.User;
using LeSi.Admin.Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace LeSi.Admin.Infrastructure.Services;

public class TokenService : ITokenService
{
    public string GetToken(UsersEntity user, string privateKey)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(privateKey);

        // TODO 后面改成URL
        var issuer = "LeSi.Admin";
        var audience = "LeSi.Client";
        var expires = DateTime.Now.AddDays(2); // 2天有效期

        var credentials = new SigningCredentials(
            new RsaSecurityKey(rsa),
            SecurityAlgorithms.RsaSha256 // 使用RSA算法
        );

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("Id", user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("Name", user.Name),
            new Claim("RoleName", user.Role.ToString()),

        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}