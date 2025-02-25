using StackExchange.Redis;
using user_service.infrastructure.repository.interfaces;

namespace user_service.infrastructure.repository.redis;

public static class ServiceProviderExtensionsRedisCache
{
    public static void AddRedisCache(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();
        
        string connectionRedis = Environment.GetEnvironmentVariable("REDIS_CONNECTION");
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(connectionRedis));
    }

    public static void AddConfirmCodeStorage(this IServiceCollection services)
    {
        services.AddScoped<IConfirmCodeStorage, ConfirmCodeRedisStorage>();
    }
}
