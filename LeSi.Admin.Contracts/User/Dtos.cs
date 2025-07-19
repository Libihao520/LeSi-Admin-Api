namespace LeSi.Admin.Contracts.User;

public class Dtos
{
    public class GetPublicKeyDto
    {
        public string PublicKey { get; set; } = string.Empty;
    }
    public class LoginDto
    {
        public string Token { get; set; } = string.Empty;
    }
}