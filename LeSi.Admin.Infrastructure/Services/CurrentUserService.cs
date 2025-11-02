using LeSi.Admin.Domain.Interfaces;
using LeSi.Admin.Domain.Interfaces.User;
using Microsoft.AspNetCore.Http;

namespace LeSi.Admin.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public long UserId
    {
        get
        {
            var claim = httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(c => c.Type == "Id")?.Value;
            return claim != null ? long.Parse(claim) : 0;
        }
    }

    public string UserName => httpContextAccessor.HttpContext?.User?.Claims
        .FirstOrDefault(c => c.Type == "UserName")?.Value ?? string.Empty;
}