using LeSi.Admin.Contracts.Exceptions;
using LeSi.Admin.Contracts.Models.UserManagement;
using LeSi.Admin.Domain.Entities.User;
using LeSi.Admin.Domain.Interfaces.Database.Repository;
using MediatR;

namespace LeSi.Admin.Application.UserManagement.CommandHandlers;

public class DeleteUserCommandHandler(IRepositoryFactory repositoryFactory)
    : IRequestHandler<Commands.DeleteUserCommand>
{
    public async Task Handle(Commands.DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var userRepository = repositoryFactory.UserRepository();

        var userEntity = await userRepository.FindEntityAsync<UsersEntity>(u => u.Id == request.Id && u.IsDeleted == 0);
        if (userEntity == null)
        {
            throw new NotFoundException("用户不存在", "USER_NOT_FOUND");
        }

        await userRepository.DeleteAsync(userEntity);
        await userRepository.SaveChangesAsync();
    }
}