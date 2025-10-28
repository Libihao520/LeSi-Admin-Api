using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using LeSi.Admin.Domain.Entities.User;
using LeSi.Admin.Domain.Interfaces;
using LeSi.Admin.Infrastructure.Config;
using Microsoft.IdentityModel.Tokens;

namespace LeSi.Admin.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly string _issuer = GlobalContext.SystemConfig.Authentication.Issuer;
    private readonly string _audience = GlobalContext.SystemConfig.Authentication.Audience;

    public string GetToken(UsersEntity user, string privateKey)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(privateKey);

        var expires = DateTime.Now.AddSeconds(GlobalContext.SystemConfig.Authentication.AccessTokenExpiration);

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
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// 验证Token
    /// </summary>
    /// <param name="token"></param>
    /// <param name="publicKey"></param>
    /// <returns></returns>
    public ClaimsPrincipal ValidateToken(string token, string publicKey)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(publicKey);

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(rsa),
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal =
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return principal;
        }
        catch (Exception ex)
        {
            throw new SecurityTokenException("Token validation failed", ex);
        }
    }
}