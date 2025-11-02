using AutoMapper;
using LeSi.Admin.Contracts.Exceptions;
using LeSi.Admin.Contracts.Models.UserManagement;
using LeSi.Admin.Domain.Entities.User;
using LeSi.Admin.Domain.Interfaces.Database.Repository;
using LeSi.Admin.Shared.Utilities.Encryption;
using LeSi.Admin.Shared.Utilities.Validation;
using MediatR;

namespace LeSi.Admin.Application.UserManagement.CommandHandlers;

public class UpdateUserCommandHandler(IMapper mapper, IRepositoryFactory repositoryFactory)
    : IRequestHandler<Commands.UpdateUserCommand>
{
    public async Task Handle(Commands.UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var userRepository = repositoryFactory.UserRepository();

        // 检查用户是否存在
        var existingUser = await userRepository.FindEntityAsync<UsersEntity>(u => u.Id == request.Id);
        if (existingUser == null)
        {
            throw new BusinessException("用户不存在", "USER_NOT_FOUND");
        }

        // 如果提供了新用户名，检查是否与其他用户重复
        if (!string.IsNullOrEmpty(request.Name) && request.Name != existingUser.Name)
        {
            var userWithName = await userRepository.FindEntityAsync<UsersEntity>(u => u.Name == request.Name);
            if (userWithName != null)
            {
                throw new BusinessException("用户名已被使用", "USER_NAME_EXISTS");
            }

            existingUser.Name = request.Name;
        }

        // 如果提供了新邮箱，验证并检查是否已被其他用户使用
        if (!string.IsNullOrEmpty(request.Email) && request.Email != AesUtilities.Decrypt(existingUser.Email))
        {
            if (!ValidationHelper.IsValidEmail(request.Email))
            {
                throw new ValidationException("邮箱格式错误", "EMAIL_INVALID", nameof(request.Email));
            }

            var encryptedEmail = AesUtilities.Encrypt(request.Email);
            var userWithEmail = await userRepository.FindEntityAsync<UsersEntity>(u => u.Email == encryptedEmail);
            if (userWithEmail != null)
            {
                throw new BusinessException("邮箱已被注册", "EMAIL_ALREADY_EXISTS");
            }

            existingUser.Email = encryptedEmail;
        }

        // 如果提供了新密码，验证密码强度
        if (!string.IsNullOrEmpty(request.PassWord))
        {
            if (!ValidationHelper.IsValidPassword(request.PassWord))
            {
                throw new ValidationException("密码格式错误", "PASSWORD_INVALID", nameof(request.PassWord));
            }

            existingUser.PassWord = Md5Utilities.GetMd5Hash(request.PassWord);
        }

        await userRepository.UpdateAsync(existingUser);
        await userRepository.SaveChangesAsync();
    }
}