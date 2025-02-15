using Newtonsoft.Json;
using StackExchange.Redis;
using user_service.infrastructure.repository.interfaces;
using user_service.services.result;
using user_service.services.result.errors;
using user_service.services.string_extensions;

namespace user_service.infrastructure.repository.redis.@base;

public class BaseCacheRepositoryWithList<T>(IConnectionMultiplexer mux) : BaseCacheRepository<T>(mux), ICacheRepositoryWithList<T> where T : class
{
    protected virtual string PREFIX_COLLECTION
    {
        get => typeof(T).Name.Pluralize().ToSnakeCase();
    }
    
    public async Task<Result<List<T>>> GetAll()
    {
        List<T> entities;
        
        try
        {
            var jsonStr = await _redis.StringGetAsync(PREFIX_COLLECTION);
            if (jsonStr.IsNullOrEmpty)
                return Result<List<T>>.Failure(Errors.Repository.NotFoundGetAll);
            
            entities = JsonConvert.DeserializeObject<List<T>>(jsonStr);
        }
        catch (Exception ex)
        {
            return Result<List<T>>.Failure(Errors.Repository.TryException);
        }
        
        return Result<List<T>>.Success(entities);
    }

    public async Task<Result> Add(List<T> entities)
    {
        try
        {
            await _redis.StringSetAsync(PREFIX_COLLECTION, JsonConvert.SerializeObject(entities, Formatting.None));
        }
        catch (Exception ex)
        {
            return Result.Failure(Errors.Repository.TryException);
        }
        
        return Result.Success();
    }

    public async Task<Result> Add(List<T> entities, TimeSpan timeLife)
    {
        try
        {
            await _redis.StringSetAsync(PREFIX_COLLECTION, JsonConvert.SerializeObject(entities, Formatting.None), timeLife);
        }
        catch (Exception ex)
        {
            return Result.Failure(Errors.Repository.TryException);
        }
        
        return Result.Success();
    }

    public async Task<Result> Update(List<T> entities)
        => await Add(entities);

    public async Task<Result> Update(List<T> entities, TimeSpan timeLife)
        => await Add(entities, timeLife);
    
    public async Task<Result> UnSetCollection()
    {
        try
        {
            await _redis.KeyDeleteAsync(PREFIX_COLLECTION);
        }
        catch (Exception ex)
        {
            return Result.Failure(Errors.Repository.TryException);
        }
        
        return Result.Success();
    }
}
