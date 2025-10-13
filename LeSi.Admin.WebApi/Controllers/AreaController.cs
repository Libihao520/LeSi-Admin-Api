using MediatR;
using Microsoft.AspNetCore.Mvc;
using Dtos = LeSi.Admin.Contracts.Models.Area.Dtos;
using Queries = LeSi.Admin.Contracts.Models.Area.Queries;

namespace LeSi.Admin.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AreaController : ControllerBase
{
    private readonly IMediator _mediator;

    public AreaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 获取行政区列表
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<List<Dtos.AreaDto>>> GetAreasAsync([FromQuery] Queries.GetAreaDtoQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}