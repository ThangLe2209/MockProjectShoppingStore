namespace ShoppingStore.API.Services
{
    public class NotUseResponseCacheService : IResponseCacheService
    {
        public async Task<string> GetCacheResponseAsync(string cacheKey)
        {
            return await Task.FromResult(String.Empty);
        }

        public async Task RemoveCacheResponseAsync(string pattern)
        {
            await Task.CompletedTask;
        }

        public async Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeOut)
        {
            await Task.CompletedTask;
        }
    }
}
