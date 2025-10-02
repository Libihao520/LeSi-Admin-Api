using AutoMapper;
using LeSi.Admin.Contracts.Logging;
using LeSi.Admin.Contracts.Models.User;
using LeSi.Admin.Domain.Interfaces;
using LeSi.Admin.Domain.Interfaces.Repository;
using MediatR;
using Queries = LeSi.Admin.Contracts.Models.User.Queries;

namespace LeSi.Admin.Application.User.QueryHandlers;

public class GetPublicKeyQueryHandler : IRequestHandler<Queries.GetPublicKeyDtoQuery,
    Dtos.GetPublicKeyDto>
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

    public async Task<Dtos.GetPublicKeyDto> Handle(Queries.GetPublicKeyDtoQuery command,
        CancellationToken cancellationToken)
    {
        var keyPairResult = await _keyPairManager.GetAndMoveKeyPairAsync();

        var (publicKey, _) = keyPairResult.Value;
        return new Dtos.GetPublicKeyDto() { PublicKey = publicKey };
    }
}