using LeSi.Admin.Domain.Entities.User;

namespace LeSi.Admin.Domain.Interfaces.User;

public interface ITokenService
{
    public string GetToken(UsersEntity user, string privateKey);
}