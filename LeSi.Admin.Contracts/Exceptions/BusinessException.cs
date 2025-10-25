namespace LeSi.Admin.Contracts.Exceptions;

/// <summary>
/// 业务异常
/// 示例 throw new BusinessException("邮箱已被注册", "EMAIL_ALREADY_EXISTS");
/// </summary>
public class BusinessException : Exception
{
    public string Code { get; set; }

    public BusinessException(string message)
        : base(message)
    {
        Code = "BUSINESS_ERROR";
    }

    public BusinessException(string message, string code)
        : base(message)
    {
        Code = code;
    }
}