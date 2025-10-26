namespace LeSi.Admin.Contracts.Models.UserManagement;

public class Dtos
{
    /// <summary>
    /// 用户信息DTO
    /// </summary>
    public class UserDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int Role { get; set; }
        public long CreateUserId { get; set; }
        public DateTime CreateDate { get; set; }
        public long? LastModifiedUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }

    /// <summary>
    /// 分页结果DTO
    /// </summary>
    public class UserPagedDto : Common.Dtos.PagedResult<UserPagedDto>
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int Role { get; set; }
    }
}