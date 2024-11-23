using ShoppingStore.API.Configurations;
using ShoppingStore.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Extensions.Primitives;

namespace ShoppingStore.API.Attributes
{
    public class CacheAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveSeconds;

        public CacheAttribute(int timeToLiveSeconds = 1000)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheConfiguration = context.HttpContext.RequestServices.GetRequiredService<RedisConfiguration>();
            if(!cacheConfiguration.Enabled)
            {
                Console.WriteLine("Before Attribute");
                await next();
                Console.WriteLine("After Attribute"); // that how middleware work ! after controller done it back to this line and then we return to finish this attribute function
                return;
            }

            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            var cacheResponse = await cacheService.GetCacheResponseAsync(cacheKey);

            if(!string.IsNullOrEmpty(cacheResponse))
            {
                if (cacheResponse.Contains($"item2"))
                {
                    var responseData = JsonConvert.DeserializeObject<dynamic>(cacheResponse);
                    cacheResponse = JsonConvert.SerializeObject(responseData.item1);
                    context.HttpContext.Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(responseData.item2));
                }

                var contentResult = new ContentResult
                {
                    Content = cacheResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = contentResult;
                return;
            }

            var executedContext = await next();
            if(executedContext.Result is OkObjectResult objectResult)
            {
                // if directly using headerPaginateJson line below but not toString then StringValues Datatype will make Serialize have array in headerPaginateJson (see line 62)
                var isHavePagingHeader = executedContext.HttpContext.Response.Headers.TryGetValue("X-Pagination", out var headerPaginateJson);
                //await cacheService.SetCacheResponseAsync(cacheKey, isHavePagingHeader ? (objectResult.Value, headerPaginateJson.ToString()) : objectResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds));
                
                var headerPaginateValue = isHavePagingHeader == true ? JsonConvert.DeserializeObject<dynamic>(headerPaginateJson) : null;
                await cacheService.SetCacheResponseAsync(cacheKey, isHavePagingHeader ? (objectResult.Value, headerPaginateValue) : objectResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds));

            }

        }

        private static string GenerateCacheKeyFromRequest(HttpRequest request) // here we will use endpoints and arguments params from api controller to make it our key to access redis db
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append($"{request.Path}");
            foreach (var (key,value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }
            return keyBuilder.ToString();
        }
    }
}
