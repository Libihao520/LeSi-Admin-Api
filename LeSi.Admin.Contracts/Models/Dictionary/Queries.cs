using MediatR;

namespace LeSi.Admin.Contracts.Models.Dictionary;

public class Queries
{
    public class GetDictionaryQuery : IRequest<List<Dtos.DictionaryDto>>
    {
    }
}