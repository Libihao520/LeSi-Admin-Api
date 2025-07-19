using MediatR;
using Microsoft.AspNetCore.Mvc;
using Dtos = LeSi.Admin.Contracts.Area.Dtos;
using Queries = LeSi.Admin.Contracts.Area.Queries;

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

    [HttpGet]
    public async Task<ActionResult<List<Dtos.AreaDto>>> GetAreasAsync([FromQuery] Queries.GetAreaDtoQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}