using AutoMapper;
using LeSi.Admin.Contracts.Exceptions;
using LeSi.Admin.Contracts.Models.UserManagement;
using LeSi.Admin.Domain.Entities.User;
using LeSi.Admin.Domain.Interfaces.Repository;
using LeSi.Admin.Shared.Utilities.Encryption;
using LeSi.Admin.Shared.Utilities.Validation;
using MediatR;

namespace LeSi.Admin.Application.UserManagement.CommandHandlers;

public class CreateUserCommandHandler(IMapper mapper, IRepositoryFactory repositoryFactory)
    : IRequestHandler<Commands.CreateUserCommand>

{
    public async Task Handle(Commands.CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userRepository = repositoryFactory.UserRepository();

        var findEntityAsync = await userRepository.FindEntityAsync<UsersEntity>(u => u.Name == request.Name);
        if (findEntityAsync != null)
        {
            throw new BusinessException("用户已存在", "USER_EXISTS");
        }

        if (!ValidationHelper.IsValidEmail(request.Email))
        {
            throw new ValidationException("邮箱格式错误", "EMAIL_INVALID", nameof(request.Email));
        }

        findEntityAsync =
            await userRepository.FindEntityAsync<UsersEntity>(u => u.Email == AesUtilities.Encrypt(request.Email));
        if (findEntityAsync != null)
        {
            throw new BusinessException("邮箱已被注册", "EMAIL_ALREADY_EXISTS");
        }

        if (!ValidationHelper.IsValidPassword(request.PassWord))
        {
            throw new ValidationException("密码格式错误", "PASSWORD_INVALID", nameof(request.PassWord));
        }

        var userEntity = mapper.Map<UsersEntity>(request);
        await userRepository.AddAsync(userEntity);
        await userRepository.SaveChangesAsync();
    }
}