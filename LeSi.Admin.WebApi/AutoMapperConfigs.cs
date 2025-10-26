using AutoMapper;
using LeSi.Admin.Contracts;
using LeSi.Admin.Contracts.Models.Dictionary;
using LeSi.Admin.Domain.Entities.SystemManage;
using LeSi.Admin.Domain.Entities.User;
using LeSi.Admin.Shared.Utilities.Encryption;
using UserManagement = LeSi.Admin.Contracts.Models.UserManagement;

namespace LeSi.Admin.WebApi;

public class AutoMapperConfigs : Profile
{
    /// <summary>
    /// 字典映射配置
    /// </summary>
    public AutoMapperConfigs()
    {
        // Dictionary实体映射到DTO
        CreateMap<DictionaryEntity, Dtos.DictionaryDto>();
        // 创建用户DTO映射到实体
        CreateMap<UserManagement.Commands.CreateUserCommand, UsersEntity>()
            .AfterMap((src, dest) =>
            {
                dest.Email = AesUtilities.Encrypt(src.Email);
                dest.PassWord = Md5Utilities.GetMd5Hash(src.PassWord);
            });
    }
}