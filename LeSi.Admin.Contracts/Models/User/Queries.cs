using System.ComponentModel.DataAnnotations;

namespace LeSi.Admin.Contracts.Models.User;

public class Queries
{
    public class GetPublicKeyQuery : MediatR.IRequest<Dtos.GetPublicKeyDto>
    {
    }
    public class LoginQuery : MediatR.IRequest<Dtos.LoginDto>
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Public key is required.")]
        public string PublicKey { get; set; } = string.Empty;
    }
}