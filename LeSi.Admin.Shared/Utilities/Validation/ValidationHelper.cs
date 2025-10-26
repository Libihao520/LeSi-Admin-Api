using System.Text.RegularExpressions;

namespace LeSi.Admin.Shared.Utilities.Validation;

public class ValidationHelper
{
    /// <summary>
    /// 验证邮箱格式是否正确
    /// </summary>
    /// <param name="email">待验证的邮箱地址</param>
    /// <returns>邮箱格式是否正确</returns>
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            // 使用正则表达式验证邮箱格式
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    /// <summary>
    /// 验证手机号格式是否正确
    /// </summary>
    /// <param name="phone">待验证的手机号</param>
    /// <returns>手机号格式是否正确</returns>
    public static bool IsValidPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        try
        {
            // 简单的手机号验证（以1开头的11位数字）
            return Regex.IsMatch(phone,
                @"^1[3-9]\d{9}$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    /// <summary>
    /// 验证密码强度
    /// </summary>
    /// <param name="password">待验证的密码</param>
    /// <returns>密码是否符合强度要求</returns>
    public static bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        // 密码长度至少8位，包含字母和数字
        return password.Length >= 8 &&
               Regex.IsMatch(password, @"[a-zA-Z]") &&
               Regex.IsMatch(password, @"[0-9]");
    }
}