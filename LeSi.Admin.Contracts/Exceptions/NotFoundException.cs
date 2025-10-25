namespace LeSi.Admin.Contracts.Exceptions;

/// <summary>
/// 资源未找到异常
/// 示例 throw new NotFoundException("用户不存在", "User");
/// </summary>
public class NotFoundException : Exception
{
    public string Resource { get; set; }

    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string message, string resource)
        : base(message)
    {
        Resource = resource;
    }
}