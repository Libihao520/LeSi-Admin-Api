using System.ComponentModel.DataAnnotations;

namespace LeSi.Admin.Domain.Entities;

public class AuditableBaseEntity: BaseEntity
{
    /// <summary>
    /// 版本
    /// </summary>
    [Timestamp]
    public byte[] RowVersion { get; set; }

    /// <summary>
    /// 创建人Id
    /// </summary>
    [Required]
    public long CreateUserId { get; set; }

    /// <summary>
    /// 创建日期
    /// </summary>
    [Required]
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// 是否删除
    /// </summary>
    [Required]
    public int IsDeleted { get; set; }
}