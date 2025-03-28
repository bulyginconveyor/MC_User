using StackExchange.Redis;
using user_service.application.dto;
using user_service.infrastructure.repository.interfaces;
using user_service.infrastructure.repository.redis.@base;

namespace user_service.infrastructure.repository.redis;

public static class ServiceProviderExtensionsRedisCache
{
    public static void AddRedisCache(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();
        
        string connectionRedis = Environment.GetEnvironmentVariable("REDIS_CONNECTION");
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(connectionRedis));
        services.AddScoped<ICacheRepository<RegisterData>, BaseCacheRepository<RegisterData>>();
    }

    public static void AddConfirmCodeStorage(this IServiceCollection services)
    {
        services.AddScoped<IConfirmCodeStorage, ConfirmCodeRedisStorage>();
    }
}
