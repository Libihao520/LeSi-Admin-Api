using LeSi.Admin.Domain.Entities.User;

namespace LeSi.Admin.Domain.Interfaces;

public interface ITokenService
{
    public string GetToken(UsersEntity user, string privateKey);
}