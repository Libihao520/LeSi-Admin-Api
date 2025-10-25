namespace LeSi.Admin.Contracts.Exceptions;

/// <summary>
/// 未授权异常
/// 示例 throw new UnauthorizedException("用户未登录");
/// </summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message)
        : base(message)
    {
    }
}