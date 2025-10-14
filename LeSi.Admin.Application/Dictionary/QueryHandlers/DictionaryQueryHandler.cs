using AutoMapper;
using LeSi.Admin.Contracts;
using LeSi.Admin.Contracts.Models.Dictionary;
using LeSi.Admin.Domain.Entities.SystemManage;
using LeSi.Admin.Domain.Entities.User;
using LeSi.Admin.Domain.Interfaces;
using LeSi.Admin.Domain.Interfaces.Repository;
using MediatR;

namespace LeSi.Admin.Application.Dictionary.QueryHandlers;

public class DictionaryQueryHandler(IMapper mapper, IRepositoryFactory repositoryFactory)
    : IRequestHandler<Queries.GetDictionaryDtoQuery, List<Dtos.DictionaryDto>>
{
    public async Task<List<Dtos.DictionaryDto>> Handle(Queries.GetDictionaryDtoQuery command,
        CancellationToken cancellationToken)
    {
        var dictionaryEntities = await repositoryFactory.DictionaryRepository().FindList<DictionaryEntity>();

        var dictionaryDto = mapper.Map<List<Dtos.DictionaryDto>>(dictionaryEntities);

        return dictionaryDto;
    }
}