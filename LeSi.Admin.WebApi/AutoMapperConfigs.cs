using AutoMapper;
using LeSi.Admin.Contracts;
using LeSi.Admin.Contracts.Dictionary;
using LeSi.Admin.Domain.Entities.SystemManage;

namespace LeSi.Admin.WebApi;

public class AutoMapperConfigs : Profile
{
    public AutoMapperConfigs()
    {
        CreateMap<DictionaryEntity, Dtos.DictionaryDto>();
    }
}