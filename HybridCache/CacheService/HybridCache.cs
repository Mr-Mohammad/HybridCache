using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace HybridCache.CacheService
{
    public class HybridCache : IHybridCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;

        public HybridCache(IMemoryCache memoryCache, IDistributedCache distributedCache)
        {
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            // Check in Memory Cache
            if (_memoryCache.TryGetValue(key, out T value))
            {
                return value;
            }

            // Check in Distributed Cache
            var cachedValue = await _distributedCache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(cachedValue))
            {
                value = System.Text.Json.JsonSerializer.Deserialize<T>(cachedValue);
                _memoryCache.Set(key, value, TimeSpan.FromMinutes(5)); // Update Memory Cache
                return value;
            }

            return default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow)
        {
            // Add to Memory Cache
            _memoryCache.Set(key, value, absoluteExpirationRelativeToNow);

            // Add to Distributed Cache
            var serializedValue = System.Text.Json.JsonSerializer.Serialize(value);
            await _distributedCache.SetStringAsync(key, serializedValue, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
            });
        }

        public async Task RemoveAsync(string key)
        {
            // Remove from Memory Cache
            _memoryCache.Remove(key);

            // Remove from Distributed Cache
            await _distributedCache.RemoveAsync(key);
        }
    }

}
