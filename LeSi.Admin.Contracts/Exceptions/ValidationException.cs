namespace LeSi.Admin.Contracts.Exceptions;

/// <summary>
/// 验证异常
/// 示例 throw new ValidationException("邮箱不能为空", "EMAIL_REQUIRED", nameof(request.Email));
/// </summary>
public class ValidationException : Exception
{
    public string Code { get; set; }
    public string? Field { get; set; }

    public ValidationException(string message)
        : base(message)
    {
        Code = "VALIDATION_ERROR";
    }

    public ValidationException(string message, string code)
        : base(message)
    {
        Code = code;
    }

    public ValidationException(string message, string code, string field)
        : base(message)
    {
        Code = code;
        Field = field;
    }
}