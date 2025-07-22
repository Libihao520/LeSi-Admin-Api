using AutoMapper;
using LeSi.Admin.Contracts.Logging;
using LeSi.Admin.Domain.Interfaces;
using MediatR;
using Queries = LeSi.Admin.Contracts.User.Queries;

namespace LeSi.Admin.Application.User.QueryHandlers;

public class GetPublicKeyQueryHandler : IRequestHandler<Contracts.User.Queries.GetPublicKeyDtoQuery,
    Contracts.User.Dtos.GetPublicKeyDto>
{
    private readonly IAppLogger _logger;

    private readonly IMapper _mapper;
    private readonly IKeyPairManager _keyPairManager;
    private readonly IRepositoryFactory _repositoryFactory;

    public GetPublicKeyQueryHandler(IMapper mapper,
        IKeyPairManager keyPairManager, IAppLogger logger)
    {
        _mapper = mapper;
        _keyPairManager = keyPairManager;
        _logger = logger;
    }

    public async Task<Contracts.User.Dtos.GetPublicKeyDto> Handle(Queries.GetPublicKeyDtoQuery command,
        CancellationToken cancellationToken)
    {
        var keyPairResult = await _keyPairManager.GetAndMoveKeyPairAsync();

        var (publicKey, _) = keyPairResult.Value;
        return new Contracts.User.Dtos.GetPublicKeyDto() { PublicKey = publicKey };
    }
}