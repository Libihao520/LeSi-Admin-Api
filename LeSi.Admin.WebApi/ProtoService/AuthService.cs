using Grpc.Core;
using LeSi.Admin.Contracts.User;
using MediatR;


namespace LeSi.Admin.WebApi.ProtoService;

public class AuthService : WebApi.AuthService.AuthServiceBase
{
    private readonly IMediator _mediator;

    public AuthService(IMediator mediator)
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

    public override async Task<LoginResponse> Login(
        LoginRequest request,
        ServerCallContext context)
    {
        var loginDtoQuery = new Queries.LoginDtoQuery()
        {
            Username = request.Username,
            Password = request.Password,
            PublicKey = request.PublicKey
        };
        var loginResponse = new LoginResponse();
        try
        {
            var result = await _mediator.Send(loginDtoQuery);
            loginResponse.Code = 0;
            loginResponse.Token = result.Token;
        }
        catch (Exception e)
        {
            loginResponse.Code = 1;
            loginResponse.Message = "登录失败：" + e.Message;
        }

        return loginResponse;
    }
}