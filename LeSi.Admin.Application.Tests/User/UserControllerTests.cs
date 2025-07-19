using System.Net;
using System.Text;
using System.Text.Json;
using LeSi.Admin.Contracts.User.Dtos;
using LeSi.Admin.Contracts.User.Queries;
using LeSi.Admin.WebApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace LeSi.Admin.Application.Tests.User;

public class UserControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public UserControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithEncryptedCredentials_ShouldReturnToken()
    {
        // Step 1: 获取公钥
        var publicKeyResponse = await _client.GetAsync("/api/User?timestamp=123456"); // 假设需要时间戳参数，根据实际接口调整
        publicKeyResponse.EnsureSuccessStatusCode();
        var publicKeyDto = await JsonSerializer.DeserializeAsync<GetPublicKeyDto>(
            await publicKeyResponse.Content.ReadAsStreamAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(publicKeyDto?.PublicKey);

        // Step 2: 使用公钥加密测试凭证（示例RSA加密，需根据实际加密方式调整）
        var plaintext = "username=test&password=123456";
        var encryptedData = RsaEncrypt(plaintext, publicKeyDto.PublicKey);

            [Fact]
    public async Task GetPublicKeyAsync_ShouldReturnValidPublicKey()
    {
        // 调用获取公钥接口
        var response = await _client.GetAsync("/api/User?timestamp=123456"); // 根据实际接口参数调整
        response.EnsureSuccessStatusCode();

        // 反序列化响应结果
        var result = await JsonSerializer.DeserializeAsync<GetPublicKeyDto>(
            await response.Content.ReadAsStreamAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // 断言公钥不为空
        Assert.NotNull(result?.PublicKey);
        Assert.NotEmpty(result.PublicKey);
    }

        // Step 3: 构造登录请求
        var loginQuery = new LoginDtoQuery { EncryptedData = encryptedData };
        var loginContent = new StringContent(
            JsonSerializer.Serialize(loginQuery),
            Encoding.UTF8,
            "application/json");

        // Step 4: 调用登录接口
        var loginResponse = await _client.PostAsync("/api/User", loginContent);
        loginResponse.EnsureSuccessStatusCode();

        var loginDto = await JsonSerializer.DeserializeAsync<LoginDto>(
            await loginResponse.Content.ReadAsStreamAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Step 5: 断言token存在
        Assert.NotNull(loginDto?.Token);
    }

    // 示例RSA加密方法（需根据项目实际加密实现调整）
    private string RsaEncrypt(string plaintext, string publicKey)
    {
        // 这里应替换为项目实际使用的RSA加密逻辑
        // 示例代码仅示意，实际需使用System.Security.Cryptography.RSACryptoServiceProvider等实现
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(plaintext)); 
    }
}