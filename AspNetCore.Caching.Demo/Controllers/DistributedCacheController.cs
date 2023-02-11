using AspNetCore.Caching.Demo.Extensions;
using AspNetCore.Caching.Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace AspNetCore.Caching.Demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DistributedCacheController : ControllerBase
    {
        private readonly IDistributedCache _distributedCache;

        public DistributedCacheController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserInfo>> Get(int userId)
        {
            var source = await _distributedCache.GetAsync(userId.ToString());
            if (source == null)
            {
                return NotFound();         
            }

            var info = JsonSerializer.Deserialize<UserInfo>(source);
            return info;
        }

        [HttpPost]
        public async Task<ActionResult> Set([FromBody] UserInfo info)
        {
            await _distributedCache.SetStringAsync(info.Id.ToString(), JsonSerializer.Serialize(info));
            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> Remove(int userId)
        {
            await _distributedCache.RemoveAsync(userId.ToString());
            return NoContent();
        }

        [HttpPost("GetOrCreate/{userId}")]
        public async Task<ActionResult<UserInfo>> GetOrCreate(int userId)
        {
            var cacheEntryOptions = new DistributedCacheEntryOptions
            {
                // Set AbsoluteExpiration for 3 mins
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(3),
                // Set SlidingExpiration for 5 s
                SlidingExpiration = TimeSpan.FromSeconds(5)
            };

            var info = await _distributedCache.GetOrCreateAsync(userId.ToString(), () => GetUserInfoFromDbAsync(), cacheEntryOptions);
            return Ok(info);
        }

        private async Task<UserInfo> GetUserInfoFromDbAsync()
        {
            return await Task.FromResult(new UserInfo
            {
                Id = 1,
                Name = "Test",
                Email = "Test@Gmail.com",
            });
        }
    }
}
