namespace LeSi.Admin.Contracts.Exceptions;

/// <summary>
/// 权限不足异常
/// 示例 throw new ForbiddenException("没有创建用户的权限");
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message)
        : base(message)
    {
    }
}