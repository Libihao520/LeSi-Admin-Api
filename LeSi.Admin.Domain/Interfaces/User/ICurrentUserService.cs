namespace LeSi.Admin.Domain.Interfaces.User;

public interface ICurrentUserService
{
    long UserId { get; }
    string UserName { get; }
}