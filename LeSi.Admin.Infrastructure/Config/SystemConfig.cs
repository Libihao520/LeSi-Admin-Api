namespace LeSi.Admin.Infrastructure.Config;

public class SystemConfig
{
    /// <summary>
    /// 数据库提供者
    /// </summary>
    public string DbProvider { get; set; } = null!;

    /// <summary>
    /// 系统固定数据数据库配置
    /// </summary>
    public DictionaryDbConfig DictionaryDb { get; set; } = null!; // 添加 null 断言

    /// <summary>
    /// 用户数据库配置
    /// </summary>
    public UserDbConfig UserDb { get; set; } = null!; // 添加 null 断言

    /// <summary>
    /// 缓存提供者
    /// </summary>
    public string RedisConnectionString { get; set; } = null!;

    /// <summary>
    /// 认证配置
    /// </summary>
    public Authentication Authentication { get; set; } = null!;
}

/// <summary>
/// 行政区数据库配置类
/// </summary>
public class DictionaryDbConfig
{
    /// <summary>
    /// 数据库连接字符串
    /// </summary>
    public string DbConnectionString { get; set; } = null!;

    /// <summary>
    /// 数据库命令超时时间，单位秒
    /// </summary>
    public int DbCommandTimeout { get; set; }
}

/// <summary>
/// 用户数据库配置类
/// </summary>
public class UserDbConfig
{
    /// <summary>
    /// 数据库连接字符串
    /// </summary>
    public string DbConnectionString { get; set; } = null!;

    /// <summary>
    /// 数据库命令超时时间，单位秒
    /// </summary>
    public int DbCommandTimeout { get; set; }
}

public class Authentication
{
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int AccessTokenExpiration { get; set; }
}