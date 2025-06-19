using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using UserService.DAL.Interfaces.Caching;
using UserService.DAL.Options;

namespace UserService.DAL.Data.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly CacheOptions _options;

        public RedisCacheService(IDistributedCache cache, IOptions<CacheOptions> options)
        {
            _cache = cache;
            _options = options.Value;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var rawData = await _cache.GetStringAsync(key, cancellationToken);

            return rawData is null ? default : JsonSerializer.Deserialize<T>(rawData);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }

        public async Task SetAsync<T>(string key, T data, CancellationToken cancellationToken = default)
        {
            var cacheOptions = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.TTL),
            };

            var rawData = JsonSerializer.Serialize(data);

            await _cache.SetStringAsync(key, rawData, cacheOptions, cancellationToken);
        }
    }
}
