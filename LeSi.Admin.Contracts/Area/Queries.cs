using System.ComponentModel.DataAnnotations;

namespace LeSi.Admin.Contracts.Area;

public class Queries
{
    public class GetAreaDtoQuery : MediatR.IRequest<List<Dtos.AreaDto>>
    {
        [Required]
        public string Code { get; set; } =  "44";
    }
}