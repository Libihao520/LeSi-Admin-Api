using AutoMapper;
using LeSi.Admin.Contracts;
using LeSi.Admin.Domain.Entities.SystemManage;
using LeSi.Admin.Domain.Entities.User;
using LeSi.Admin.Shared.Utilities.Encryption;
using UserManagement = LeSi.Admin.Contracts.Models.UserManagement;
using Dictionary = LeSi.Admin.Contracts.Models.Dictionary;

namespace LeSi.Admin.WebApi;

/// <summary>
/// 自动映射配置类
/// </summary>
public class AutoMapperConfigs : Profile
{
    /// <summary>
    /// 字典映射配置
    /// </summary>
    public AutoMapperConfigs()
    {
        #region Entity to DTO

        // 用户实体映射到DTO
        CreateMap<UsersEntity, UserManagement.Dtos.UserDto>();

        // Dictionary实体映射到DTO
        CreateMap<DictionaryEntity, Dictionary.Dtos.DictionaryDto>();

        #endregion

        #region DTO to Entity

        // 创建用户DTO映射到实体
        CreateMap<UserManagement.Commands.CreateUserCommand, UsersEntity>()
            .AfterMap((src, dest) =>
            {
                dest.Email = AesUtilities.Encrypt(src.Email);
                dest.PassWord = Md5Utilities.GetMd5Hash(src.PassWord);
            });

        #endregion
    }
}