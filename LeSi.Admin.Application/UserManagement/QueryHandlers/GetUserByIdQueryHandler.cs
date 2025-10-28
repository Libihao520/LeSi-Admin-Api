using AutoMapper;
using LeSi.Admin.Contracts.Exceptions;
using LeSi.Admin.Contracts.Models.UserManagement;
using LeSi.Admin.Domain.Entities.User;
using LeSi.Admin.Domain.Interfaces.Repository;
using MediatR;

namespace LeSi.Admin.Application.UserManagement.QueryHandlers;

public class GetUserByIdQueryHandler(IMapper mapper, IRepositoryFactory repositoryFactory)
    : IRequestHandler<Queries.GetUserByIdQuery, Dtos.UserDto>
{
    public async Task<Dtos.UserDto> Handle(Queries.GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var userRepository = repositoryFactory.UserRepository();
        var userEntity = await userRepository.FindEntityAsync<UsersEntity>(u => u.Id == request.Id);
        if (userEntity == null)
        {
            throw new BusinessException("用户不存在", "USER_NOT_FOUND");
        }
        var userDto = mapper.Map<Dtos.UserDto>(userEntity);
        return userDto;
    }
}