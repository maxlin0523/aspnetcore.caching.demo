using AspNetCore.Caching.Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace AspNetCore.Caching.Demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MemoryCacheController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        [HttpGet("{userId}")]
        public ActionResult<UserInfo> Get(int userId)
        {
            var info = _memoryCache.GetOrCreate(userId, (entry) =>
            {
                // Set AbsoluteExpiration for 1 day
                entry.AbsoluteExpiration = DateTimeOffset.Now.AddDays(1);
                // Set SlidingExpiration for 1 hour
                entry.SlidingExpiration = TimeSpan.FromHours(1);

                return GetUserInfoFromDb();
            });

            return info;
        }


        [HttpPost]
        public ActionResult Set([FromBody] UserInfo info)
        {
            // 依 id 儲存資料於快取
            _memoryCache.Set(info.Id, info);
            return NoContent();
        }

        [HttpDelete("{userId}")]
        public ActionResult Remove(int userId)
        {
            // 依 id 儲存資料於快取
            _memoryCache.Remove(userId);
            return NoContent();
        }

        private UserInfo GetUserInfoFromDb()
        {
            return new UserInfo
            {
                Id = 1,
                Name = "Test",
                Email = "Test@Gmail.com",
            };
        }
    }
}
