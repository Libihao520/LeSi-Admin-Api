using LeSi.Admin.Application.User.QueryHandlers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Dtos = LeSi.Admin.Contracts.Models.User.Dtos;
using Queries = LeSi.Admin.Contracts.Models.User.Queries;

namespace LeSi.Admin.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<Dtos.GetPublicKeyDto>> GetPublicKeyAsync(
        [FromQuery] Queries.GetPublicKeyDtoQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Dtos.LoginDto>> Login([FromBody] Queries.LoginDtoQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}