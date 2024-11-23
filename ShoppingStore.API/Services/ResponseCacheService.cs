
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;
using System.Collections;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShoppingStore.API.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConnectionMultiplexer _connectionMultiplexer; // multiple redis server or redis db( google IConnectionMultiplexer)

        public ResponseCacheService(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer)
        {
            _distributedCache = distributedCache;
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task<string> GetCacheResponseAsync(string cacheKey)
        {
            var cacheResponse = await _distributedCache.GetStringAsync(cacheKey);
            return string.IsNullOrEmpty(cacheResponse) ? null : cacheResponse;
        }

        public async Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeOut)
        {
            if (response == null) return;

            var serializeRespone = JsonConvert.SerializeObject(response, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver() // default redis response return lowercase first letter here is setting it become camel case
            });

            await _distributedCache.SetStringAsync(cacheKey, serializeRespone, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = timeOut,
            });
        }

        public async Task RemoveCacheResponseAsync(string pattern)
        {
            if(string.IsNullOrWhiteSpace(pattern))
            {
                throw new ArgumentException("Value cannot be null or whitespace");
            }

            await foreach (var key in GetKeyAsyncByPattern(pattern + "*")) // * mean delete all api endpoints match the pattern (also query parameter). Ex: WeatherForecast/getall(pattern), WeatherForecast/getall?search="Thang"&pageNumber=1, ....
            {
                await _distributedCache.RemoveAsync(key);
            }
        }

        private async IAsyncEnumerable<string> GetKeyAsyncByPattern(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                throw new ArgumentException("Value cannot be null or whitespace");
            }

            foreach (var endPoint in _connectionMultiplexer.GetEndPoints()) // multiple redis server
            {
                var server = _connectionMultiplexer.GetServer(endPoint);
                foreach (var key in server.Keys(pattern: pattern))
                {
                    yield return key.ToString();
                }
            }
        }

        //public string GenerateCacheKeyFromRequest(HttpRequest request) // here we will use endpoints and arguments params from api controller to make it our key to access redis db
        //{
        //    var keyBuilder = new StringBuilder();
        //    keyBuilder.Append($"{request.Path}");
        //    foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
        //    {
        //        keyBuilder.Append($"|{key}-{value}");
        //    }
        //    return keyBuilder.ToString();
        //}
    }

}
