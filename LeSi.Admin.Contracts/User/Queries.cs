using System.ComponentModel.DataAnnotations;
using MediatR;

namespace LeSi.Admin.Contracts.User;

public class Queries
{
    public class GetPublicKeyDtoQuery : MediatR.IRequest<Dtos.GetPublicKeyDto>
    {
    }
    public class LoginDtoQuery : MediatR.IRequest<Dtos.LoginDto>
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Public key is required.")]
        public string PublicKey { get; set; } = string.Empty;
    }
}