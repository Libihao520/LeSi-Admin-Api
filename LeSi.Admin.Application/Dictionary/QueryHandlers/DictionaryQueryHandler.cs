using AutoMapper;
using LeSi.Admin.Contracts;
using LeSi.Admin.Contracts.Dictionary;
using LeSi.Admin.Domain.Entities.SystemManage;
using LeSi.Admin.Domain.Entities.User;
using LeSi.Admin.Infrastructure.Repository;
using MediatR;

namespace LeSi.Admin.Application.Dictionary.QueryHandlers;

public class DictionaryQueryHandler : RepositoryFactory,
    IRequestHandler<Queries.GetDictionaryDtoQuery, List<Dtos.DictionaryDto>>
{
    private readonly IMapper _mapper;

    public DictionaryQueryHandler(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task<List<Dtos.DictionaryDto>> Handle(Queries.GetDictionaryDtoQuery command,
        CancellationToken cancellationToken)
    {
        var dictionaryEntities = await DictionaryRepository().FindList<DictionaryEntity>();

        var dictionaryDto = _mapper.Map<List<Dtos.DictionaryDto>>(dictionaryEntities);

        return dictionaryDto;
    }
}