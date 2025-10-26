using LeSi.Admin.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Dtos = LeSi.Admin.Contracts.Models.UserManagement.Dtos;
using Queries = LeSi.Admin.Contracts.Models.UserManagement.Queries;
using Commands = LeSi.Admin.Contracts.Models.UserManagement.Commands;

namespace LeSi.Admin.WebApi.Controllers;

/// <inheritdoc />
[ApiController]
[Route("api/[controller]")]
public class UserManagementController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICache _cache;

    /// <summary>
    /// 用户管理控制器
    /// </summary>
    /// <param name="mediator"></param>
    public UserManagementController(IMediator mediator, ICache cache)
    {
        _mediator = mediator;
        _cache = cache;
    }

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>用户信息</returns>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<Dtos.UserDto>> GetUser([FromRoute] long id)
    {
        var query = new Queries.GetUserByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>用户列表</returns>
    [HttpGet]
    public async Task<ActionResult<Dtos.UserPagedDto>> GetUserList([FromQuery] Queries.GetUserListQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="command">创建用户命令</param>
    /// <returns>创建的用户信息</returns>
    [HttpPost]
    public async Task<ActionResult> CreateUser([FromBody] Commands.CreateUserCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    /// <summary>
    /// 更新用户信息
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="command">更新用户命令</param>
    /// <returns>更新后的用户信息</returns>
    [HttpPut]
    public async Task<ActionResult> UpdateUser( [FromBody] Commands.UpdateUserCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{id:long}")]
    public async Task<ActionResult> DeleteUser(long id)
    {
        var command = new Commands.DeleteUserCommand { Id = id };
        await _mediator.Send(command);
        return Ok();
    }
}