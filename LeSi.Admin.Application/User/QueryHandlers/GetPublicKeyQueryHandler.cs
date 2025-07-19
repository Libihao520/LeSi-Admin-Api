using AutoMapper;
using LeSi.Admin.Contracts.Dictionary;
using LeSi.Admin.Domain.Entities.SystemManage;
using LeSi.Admin.Infrastructure.CaChe;
using LeSi.Admin.Infrastructure.Repository;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Queries = LeSi.Admin.Contracts.User.Queries;

namespace LeSi.Admin.Application.User.QueryHandlers;

public class GetPublicKeyQueryHandler : RepositoryFactory,
    IRequestHandler<Contracts.User.Queries.GetPublicKeyDtoQuery, Contracts.User.Dtos.GetPublicKeyDto>
{
    private readonly IMapper _mapper;
    private readonly ICache _memoryCache;
    private readonly ICache _redisCache;
    private readonly IKeyPairManager _keyPairManager;

    public GetPublicKeyQueryHandler(IMapper mapper, [FromKeyedServices("MemoryCache")] ICache memoryCache,
        [FromKeyedServices("RedisCache")] ICache redisCache, IKeyPairManager keyPairManager)
    {
        _mapper = mapper;
        _memoryCache = memoryCache;
        _redisCache = redisCache;
        _keyPairManager = keyPairManager;
    }

    public async Task<Contracts.User.Dtos.GetPublicKeyDto> Handle(Queries.GetPublicKeyDtoQuery command,
        CancellationToken cancellationToken)
    {
        var keyPairResult = await _keyPairManager.GetAndMoveKeyPairAsync();

        var (publicKey, _) = keyPairResult.Value;
        return new Contracts.User.Dtos.GetPublicKeyDto() { PublicKey = publicKey };
    }
}