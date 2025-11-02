using System.Collections.Concurrent;
using System.Security.Cryptography;
using LeSi.Admin.Contracts.Logging;
using LeSi.Admin.Domain.Interfaces;
using LeSi.Admin.Domain.Interfaces.Cache;
using LeSi.Admin.Domain.Interfaces.Security;
using Microsoft.Extensions.Hosting;
using LeSi.Admin.Infrastructure.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;

namespace LeSi.Admin.Infrastructure.Cache
{
    public class KeyPairManager : IHostedService, IDisposable, IKeyPairManager
    {
        private readonly IAppLogger _logger;
        private readonly ICache _cache; // 缓存接口
        private Timer _timer; // 定期检查的计时器
        private readonly CancellationTokenSource _cancellationTokenSource = new(); // 取消令牌源
        private const string BackupCacheKey = "BackupKeyPairs"; // 备用密钥对的缓存键
        private readonly SemaphoreSlim _semaphore = new(1, 1); // 用于线程同步的信号量
        private const int InitialKeyPairCount = 30; // 初始密钥对数量
        private const int RefillThreshold = 15; // 补充阈值
        private const int UsedKeyExpirationMinutes = 30; // 已使用密钥过期时间(分钟)
        private const int CheckIntervalMinutes = 1; // 检查间隔(分钟)
        private bool _disposed = false; // 是否已释放资源

        public KeyPairManager( ICache cache, IAppLogger logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await InitializeKeyPairsAsync();
            _timer = new Timer(
                async _ => await CheckAndRefillKeyPairsAsync(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(CheckIntervalMinutes));
        }

        private async Task InitializeKeyPairsAsync()
        {
            try
            {
                await _semaphore.WaitAsync(_cancellationTokenSource.Token);

                var currentPairs =
                    _cache.Get<ConcurrentDictionary<string, string>>(BackupCacheKey)
                    ?? new ConcurrentDictionary<string, string>();

                if (currentPairs.Count >= InitialKeyPairCount)
                    return;

                int needToGenerate = InitialKeyPairCount - currentPairs.Count;
                if (needToGenerate > 0)
                {
                    var generatedPairs = await GenerateKeyPairsAsync(needToGenerate);

                    foreach (var pair in generatedPairs)
                    {
                        currentPairs.TryAdd(pair.Key, pair.Value);
                    }

                    _cache.Set(BackupCacheKey, currentPairs, TimeSpan.FromDays(365 * 10));
                    _logger.Info($"密钥对初始化完成。当前备用密钥数量：{currentPairs.Count}");
                }
            }
            catch (OperationCanceledException) when (_cancellationTokenSource.IsCancellationRequested)
            {
                // 服务正在停止
            }
            catch (Exception ex)
            {
                _logger.Error("密钥对初始化失败", ex);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task CheckAndRefillKeyPairsAsync()
        {
            if (_cancellationTokenSource.IsCancellationRequested)
                return;

            try
            {
                await _semaphore.WaitAsync(_cancellationTokenSource.Token);

                var currentPairs =
                    _cache.Get<ConcurrentDictionary<string, string>>(BackupCacheKey)
                    ?? new ConcurrentDictionary<string, string>();
                _logger.Info($"定时任务：当前备用密钥数量：{currentPairs.Count}");

                if (currentPairs.Count < RefillThreshold)
                {
                    int needToGenerate = InitialKeyPairCount - currentPairs.Count;
                    var generatedPairs = await GenerateKeyPairsAsync(needToGenerate);

                    foreach (var pair in generatedPairs)
                    {
                        currentPairs.TryAdd(pair.Key, pair.Value);
                    }

                    _cache.Set(BackupCacheKey, currentPairs, TimeSpan.FromDays(365 * 10));
                    _logger.Info($"已补充 {needToGenerate} 个密钥对。当前备用密钥数量：{currentPairs.Count}");
                }
            }
            catch (OperationCanceledException) when (_cancellationTokenSource.IsCancellationRequested)
            {
                // 服务正在停止
            }
            catch (Exception ex)
            {
                _logger.Error("密钥对检查补充失败", ex);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<ConcurrentDictionary<string, string>> GenerateKeyPairsAsync(int count)
        {
            var keyPairs = new ConcurrentDictionary<string, string>();
            var tasks = Enumerable.Range(0, count)
                .Select(async _ =>
                {
                    var pair = await GenerateKeyPairAsync();
                    keyPairs.TryAdd(pair.PublicKey, pair.PrivateKey);
                });

            await Task.WhenAll(tasks);
            return keyPairs;
        }

        private Task<(string PublicKey, string PrivateKey)> GenerateKeyPairAsync()
        {
            return Task.Run(() =>
            {
                using var rsa = RSA.Create(2048);
                string publicKey = rsa.ExportSubjectPublicKeyInfoPem();
                string privateKey = rsa.ExportPkcs8PrivateKeyPem();
                return (publicKey, privateKey);
            });
        }

        public async Task<(string PublicKey, string PrivateKey)?> GetAndMoveKeyPairAsync()
        {
            try
            {
                await _semaphore.WaitAsync(_cancellationTokenSource.Token);

                var backupPairs =
                    _cache.Get<ConcurrentDictionary<string, string>>(BackupCacheKey);
                if (backupPairs == null || backupPairs.Count == 0)
                {
                    _logger.Warn("备用密钥对不足，无法获取");
                    return null;
                }

                var firstPair = backupPairs.First();
                if (backupPairs.TryRemove(firstPair.Key, out var privateKey))
                {
                    if (string.IsNullOrEmpty(firstPair.Key))
                    {
                        _logger.Warn("获取的密钥对公钥为空，跳过存储");
                        return null;
                    }

                    _cache.Set(firstPair.Key, privateKey, TimeSpan.FromMinutes(UsedKeyExpirationMinutes));
                    _cache.Set(BackupCacheKey, backupPairs, TimeSpan.FromDays(365 * 10));

                    return (firstPair.Key, privateKey);
                }

                return null;
            }
            catch (OperationCanceledException) when (_cancellationTokenSource.IsCancellationRequested)
            {
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error("获取并转移密钥对失败", ex);
                return null;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _timer?.Dispose();
                    _cancellationTokenSource?.Dispose();
                    _semaphore?.Dispose();
                }

                _disposed = true;
            }
        }
    }
}