using StackExchange.Redis;

namespace user_service.infrastructure.repository.redis;

public static class ServiceProviderExtensionsRedisCache
{
    public static void AddRedisCache(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();
        
        string connectionRedis = Environment.GetEnvironmentVariable("REDIS_CONNECTION");
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(connectionRedis));
    }
}
