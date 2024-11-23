using ShoppingStore.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingStore.API.Tests.Controllers
{
    public class FakeResponseCacheService : IResponseCacheService
    {
        public Task<string> GetCacheResponseAsync(string cacheKey)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveCacheResponseAsync(string pattern)
        {
        }

        public Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeOut)
        {
            throw new NotImplementedException();
        }
    }
}
