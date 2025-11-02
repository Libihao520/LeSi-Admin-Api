using Microsoft.Extensions.Hosting;

namespace LeSi.Admin.Domain.Interfaces.Security
{
    public interface IKeyPairManager : IHostedService, IDisposable
    {
        /// <summary>
        /// 获取并转移一个密钥对到已使用缓存
        /// </summary>
        /// <returns>密钥对或null（无可用密钥时）</returns>
        Task<(string PublicKey, string PrivateKey)?> GetAndMoveKeyPairAsync();
    }
}