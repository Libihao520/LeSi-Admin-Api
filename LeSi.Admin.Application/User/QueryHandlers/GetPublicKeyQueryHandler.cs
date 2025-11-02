using AutoMapper;
using LeSi.Admin.Contracts.Logging;
using LeSi.Admin.Contracts.Models.User;
using LeSi.Admin.Domain.Interfaces;
using LeSi.Admin.Domain.Interfaces.Security;
using MediatR;
using Queries = LeSi.Admin.Contracts.Models.User.Queries;

namespace LeSi.Admin.Application.User.QueryHandlers;

public class GetPublicKeyQueryHandler(IMapper mapper, IKeyPairManager keyPairManager, IAppLogger logger)
    : IRequestHandler<Queries.GetPublicKeyQuery, Dtos.GetPublicKeyDto>
{
    public async Task<Dtos.GetPublicKeyDto> Handle(Queries.GetPublicKeyQuery command,
        CancellationToken cancellationToken)
    {
        var keyPairResult = await keyPairManager.GetAndMoveKeyPairAsync();

        if (keyPairResult == null)
        {
            throw new InvalidOperationException("无法生成密钥对，请稍后重试");
        }

        var (publicKey, _) = keyPairResult.Value;
        return new Dtos.GetPublicKeyDto() { PublicKey = publicKey };
    }
}