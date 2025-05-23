using StackExchange.Redis;
using user_service.infrastructure.repository.interfaces;

namespace user_service.infrastructure.repository.redis;

public sealed class ConfirmCodeRedisStorage(IConnectionMultiplexer mux) : IConfirmCodeStorage
{
    private readonly IDatabase _redis = mux.GetDatabase();

    public string PREFIX => "confirm_code-";

    public async Task<string> GenerateAndSaveCode(string email, TimeSpan? lifeTime = null)
    {
        if(lifeTime is null)
            lifeTime = TimeSpan.FromMinutes(5);
        
        //var code = Guid.NewGuid().ToString();
        var code = Random.Shared.Next(10000, 99999).ToString();
        await _redis.StringSetAsync($"{PREFIX}{email}", code, lifeTime);
        
        return code;
    }

    public async Task<bool> ConfirmEmail(string email, string code)
    {
        var codeFromRedis = await _redis.StringGetAsync($"{PREFIX}{email}");
        
        return codeFromRedis == code;
    }
}
