using MediatR;

namespace LeSi.Admin.Contracts.Models.Dictionary;

public class Queries
{
    public class GetDictionaryDtoQuery : IRequest<List<Models.Dictionary.Dtos.DictionaryDto>>
    {
        
    }
        
}