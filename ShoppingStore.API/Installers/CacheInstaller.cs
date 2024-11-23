
using ShoppingStore.API.Configurations;
using ShoppingStore.API.Services;
using StackExchange.Redis;

namespace ShoppingStore.API.Installers
{
    public class CacheInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            var redisConfiguration = new RedisConfiguration();
            configuration.GetSection("RedisConfiguration").Bind(redisConfiguration);

            services.AddSingleton(redisConfiguration);

            if (!redisConfiguration.Enabled)
            {
                services.AddSingleton<IResponseCacheService, NotUseResponseCacheService>();
                return;
            }

            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConfiguration.ConnectionString));
            services.AddStackExchangeRedisCache(option => option.Configuration = redisConfiguration.ConnectionString);

            services.AddSingleton<IResponseCacheService, ResponseCacheService>();

        }
    }
}
