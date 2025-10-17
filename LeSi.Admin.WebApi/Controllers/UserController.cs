using LeSi.Admin.Application.User.QueryHandlers;
using LeSi.Admin.Domain.Interfaces;
using LeSi.Admin.Infrastructure.Cache;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Dtos = LeSi.Admin.Contracts.Models.User.Dtos;
using Queries = LeSi.Admin.Contracts.Models.User.Queries;

namespace LeSi.Admin.WebApi.Controllers;

/// <inheritdoc />
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICache _cache;

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="cache"></param>
    public UserController(IMediator mediator, ICache cache)
    {
        _mediator = mediator;
        _cache = cache;
    }

    /// <summary>
    /// 获取用户公钥
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<Dtos.GetPublicKeyDto>> GetPublicKeyAsync(
        [FromQuery] Queries.GetPublicKeyDtoQuery query)
    {
        if (IsRequestTooFrequent())
        {
            return StatusCode(429, "请求过于频繁，请稍后再试");
        }

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<Dtos.LoginDto>> Login([FromBody] Queries.LoginDtoQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    private bool IsRequestTooFrequent()
    {
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();

        if (string.IsNullOrEmpty(clientIp))
        {
            clientIp = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        }

        if (string.IsNullOrEmpty(clientIp))
        {
            return false;
        }

        var cacheKey = CacheKeys.PublicKeyRequestRateLimit(clientIp);

        if (_cache.TryGetValue<bool>(cacheKey, out _))
        {
            return true; // 请求过于频繁
        }

        // 设置5分钟限制
        _cache.Set(cacheKey, true, TimeSpan.FromMinutes(5));
        return false;
    }
}