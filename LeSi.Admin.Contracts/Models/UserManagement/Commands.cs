using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using MediatR;

namespace LeSi.Admin.Contracts.Models.UserManagement;

public class Commands
{
    /// <summary>
    /// 创建用户命令
    /// </summary>
    public class CreateUserCommand : IRequest
    {
        [Required(ErrorMessage = "名称不可为空！")] public string? Name { get; set; }
        [Required(ErrorMessage = "密码不可为空！")] public string? PassWord { get; set; }
        [Required(ErrorMessage = "邮箱不可为空！")] public string? Email { get; set; }
        [Required(ErrorMessage = "角色不可为空！")] public int Role { get; set; }
    }

    /// <summary>
    /// 更新用户命令
    /// </summary>
    public class UpdateUserCommand : IRequest
    {
        [Required(ErrorMessage = "ID不可为空！")] public long Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PassWord { get; set; }

        public int Role { get; set; }
    }

    /// <summary>
    /// 删除用户命令
    /// </summary>
    public class DeleteUserCommand : IRequest
    {
        [Required(ErrorMessage = "ID不可为空！")] public long Id { get; set; }
    }
}