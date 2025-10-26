using MediatR;

namespace LeSi.Admin.Contracts.Models.UserManagement;

public class Queries
{
    /// <summary>
    /// 根据ID获取用户查询
    /// </summary>
    public class GetUserByIdQuery : IRequest<Dtos.UserDto>
    {
        public long Id { get; set; }
    }

    /// <summary>
    /// 获取用户列表查询
    /// </summary>
    public class GetUserListQuery : IRequest<Dtos.UserPagedDto>
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}