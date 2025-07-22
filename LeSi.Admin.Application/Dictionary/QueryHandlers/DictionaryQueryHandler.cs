using AutoMapper;
using LeSi.Admin.Contracts;
using LeSi.Admin.Contracts.Dictionary;
using LeSi.Admin.Domain.Entities.SystemManage;
using LeSi.Admin.Domain.Entities.User;
using LeSi.Admin.Domain.Interfaces;
using MediatR;

namespace LeSi.Admin.Application.Dictionary.QueryHandlers;

public class DictionaryQueryHandler : IRequestHandler<Queries.GetDictionaryDtoQuery, List<Dtos.DictionaryDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryFactory _repositoryFactory;
    public DictionaryQueryHandler(IMapper mapper, IRepositoryFactory repositoryFactory)
    {
        _mapper = mapper;
        _repositoryFactory = repositoryFactory;
    }

    public async Task<List<Dtos.DictionaryDto>> Handle(Queries.GetDictionaryDtoQuery command,
        CancellationToken cancellationToken)
    {
        var dictionaryEntities = await _repositoryFactory.DictionaryRepository().FindList<DictionaryEntity>();

        var dictionaryDto = _mapper.Map<List<Dtos.DictionaryDto>>(dictionaryEntities);

        return dictionaryDto;
    }
}