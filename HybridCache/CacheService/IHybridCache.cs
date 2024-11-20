namespace HybridCache.CacheService
{
    public interface IHybridCache
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow);
        Task RemoveAsync(string key);
    }
}
