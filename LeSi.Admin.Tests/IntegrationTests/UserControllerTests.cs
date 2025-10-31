using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using LeSi.Admin.Contracts.ApiResponse;
using LeSi.Admin.Contracts.Models.User;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace LeSi.Admin.Test.IntegrationTests
{
    [TestFixture]
    public class UserControllerTests
    {
        [Test]
        public async Task GetPublicKeyAsync_ShouldReturnPublicKey()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            //获取公钥
            var response = await client.GetAsync("/api/user");

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<Dtos.GetPublicKeyDto>>(responseContent,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            //使用公钥加密凭证
            var username = "lbhlbh";
            var password = "1qazZAQ!";
            using var rsa = RSA.Create();
            rsa.ImportFromPem(result.Data.PublicKey); // 导入PEM格式公钥


            var encryptedUsername =
                Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(username), RSAEncryptionPadding.Pkcs1));
            var encryptedPassword =
                Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(password), RSAEncryptionPadding.Pkcs1));

            var loginQuery = new Queries.LoginQuery
            {
                Username = encryptedUsername,
                Password = encryptedPassword,
                PublicKey = result.Data.PublicKey
            };
            var loginContent =
                new StringContent(JsonSerializer.Serialize(loginQuery), Encoding.UTF8, "application/json");

            var loginResponse = await client.PostAsync("/api/user", loginContent);
            loginResponse.EnsureSuccessStatusCode();
            var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            var loginResult = JsonSerializer.Deserialize<ApiResponse<Dtos.LoginDto>>(loginResponseContent,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            Console.WriteLine(loginResult.Data.Token);
        }
    }
}