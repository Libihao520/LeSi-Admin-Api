using System.Security.Cryptography;
using System.Text;
using LeSi.Admin.Contracts.User;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Test
{
    [TestFixture]
    public class GetPublicKeyIntegrationTests
    {
        private IMediator _mediator;
        private WebApplicationFactory<Program> _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>();
            _mediator = _factory.Services.GetRequiredService<IMediator>();
        }

        [Test]
        public async Task GetPublicKey_ShouldReturnFromRealCache()
        {
            // 获取公钥 
            var publicKeyResult = await _mediator.Send(new Queries.GetPublicKeyDtoQuery());


            // 准备登录数据
            string username = "lbhlbh";
            string password = "123456";
            
            // 使用公钥加密用户名和密码
            var encryptedUsername = EncryptWithPublicKey(publicKeyResult.PublicKey, username);
            var encryptedPassword = EncryptWithPublicKey(publicKeyResult.PublicKey, password);

            // 创建登录请求
            var loginQuery = new Queries.LoginDtoQuery
            {
                Username = encryptedUsername,
                Password = encryptedPassword,
                PublicKey = publicKeyResult.PublicKey
            };
            var loginDto = await _mediator.Send(loginQuery);
        }

        private string EncryptWithPublicKey(string publicKey, string data)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicKey.ToCharArray());
            
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] encryptedBytes = rsa.Encrypt(dataBytes, RSAEncryptionPadding.Pkcs1);
            return Convert.ToBase64String(encryptedBytes);
        }

        [TearDown]
        public void TearDown()
        {
            _factory?.Dispose();
        }
    }
    
}