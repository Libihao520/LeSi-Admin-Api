using LeSi.Admin.Application;
using LeSi.Admin.Contracts;
using LeSi.Admin.Contracts.Models.Dictionary;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LeSi.Admin.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DictionaryController : ControllerBase
{
    private readonly IMediator _mediator;

    public DictionaryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 获取所有数据字典类型
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<List<Dtos.DictionaryDto>>> GetAllAsync([FromQuery] Queries.GetDictionaryQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}