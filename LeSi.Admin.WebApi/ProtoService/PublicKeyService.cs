using Grpc.Core;
using LeSi.Admin.Contracts.User;
using MediatR;


namespace LeSi.Admin.WebApi.ProtoService;

public class PublicKeyService : WebApi.PublicKeyService.PublicKeyServiceBase
{
    private readonly IMediator _mediator;

    public PublicKeyService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<PublicKeyResponse> GetPublicKey(
        GetPublicKeyRequest request,
        ServerCallContext context)
    {
        var result = await _mediator.Send(new Queries.GetPublicKeyDtoQuery());

        string _publicKeyPem = result.PublicKey;

        return new PublicKeyResponse
        {
            PublicKey = _publicKeyPem
        };
    }
}