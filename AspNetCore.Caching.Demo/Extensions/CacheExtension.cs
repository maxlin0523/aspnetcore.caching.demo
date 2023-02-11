using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AspNetCore.Caching.Demo.Extensions
{
    public static class CacheExtension
    {
        public static async Task<TItem> GetOrCreateAsync<TItem>(this IDistributedCache cache, string key, Func<Task<TItem>> entity, DistributedCacheEntryOptions options = null)
        {
            if (options == null)
            {
                options = new DistributedCacheEntryOptions()
                {
                    // 預設 5 分鐘快取
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5)
                };
            }

            var value = await cache.GetStringAsync(key);
            if (value == null)
            {
                var source = await entity();
                var jsonSource = JsonSerializer.Serialize(source);
                await cache.SetStringAsync(key, jsonSource, options);
                return source;
            }
            return JsonSerializer.Deserialize<TItem>(value);
        }
    }
}
