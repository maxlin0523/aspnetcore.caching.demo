using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AspNetCore.Caching.Demo.Extensions
{
    public static class CacheExtension
    {
        public static async Task<TItem> GetOrCreateAsync<TItem>(this IDistributedCache cache, string key, Func<Task<TItem>> entity, DistributedCacheEntryOptions options = null)
        {
            // 如果快取 options 沒傳入則自動設定預設快取
            if (options == null)
            {
                options = new DistributedCacheEntryOptions()
                {
                    // 預設 5 分鐘快取
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5)
                };
            }

            // 取得快取資料，不存在則返回 null
            var value = await cache.GetStringAsync(key);

            // 不存在的情況
            if (value == null)
            {
                // 透過傳入的委派 entity 取得 resource
                var source = await entity();

                // 序列化 json
                var jsonSource = JsonSerializer.Serialize(source);

                // 存入快取並同時回傳
                await cache.SetStringAsync(key, jsonSource, options);
                return source;
            }

            return JsonSerializer.Deserialize<TItem>(value);
        }
    }
}
