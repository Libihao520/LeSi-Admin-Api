namespace LeSi.Admin.Domain.Interfaces;

public interface ICurrentUserService
{
    long UserId { get; }
    string UserName { get; }
}