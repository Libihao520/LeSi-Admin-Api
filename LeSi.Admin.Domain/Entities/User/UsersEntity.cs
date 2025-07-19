using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LeSi.Admin.Domain.Attributes;

namespace LeSi.Admin.Domain.Entities.User;

[Table("Users")]
[Database("user_db")]
public class UsersEntity : AuditableBaseEntity
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [Required]
    public string PassWord { get; set; }

    /// <summary>
    /// 用户角色
    /// </summary>
    public int Role { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [Required]
    public string Email { get; set; }

    /// <summary>
    /// 头像id
    /// </summary>
    public long? PhotosId { get; set; }
}