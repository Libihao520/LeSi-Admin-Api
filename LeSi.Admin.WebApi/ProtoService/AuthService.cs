using Grpc.Core;
using LeSi.Admin.Contracts.Models.User;
using MediatR;


namespace LeSi.Admin.WebApi.ProtoService;

/// <summary>
/// （gRPC）认证服务
/// </summary>
public class AuthService(IMediator mediator) : WebApi.AuthService.AuthServiceBase
{
    /// <summary>
    /// 获取公钥
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<PublicKeyResponse> GetPublicKey(
        GetPublicKeyRequest request,
        ServerCallContext context)
    {
        var result = await mediator.Send(new Queries.GetPublicKeyDtoQuery());

        string publicKeyPem = result.PublicKey;

        return new PublicKeyResponse
        {
            PublicKey = publicKeyPem
        };
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
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
            var result = await mediator.Send(loginDtoQuery);
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