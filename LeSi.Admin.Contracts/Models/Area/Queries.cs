using System.ComponentModel.DataAnnotations;

namespace LeSi.Admin.Contracts.Models.Area;

public class Queries
{
    public class GetAreaDtoQuery : MediatR.IRequest<List<Models.Area.Dtos.AreaDto>>
    {
        public string? Code { get; set; }
    }
}