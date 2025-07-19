using MediatR;

namespace LeSi.Admin.Contracts.Dictionary;

public class Queries
{
    public class GetDictionaryDtoQuery : IRequest<List<Dtos.DictionaryDto>>
    {
        
    }
        
}