using user_service.services.result;

namespace user_service.infrastructure.repository.interfaces;

public interface ICacheRepository<T> where T : class
{
    public Task<Result<T>> Get(string key);
    public Task<Result> Add(string key, T entity);
    public Task<Result> Add(string key, T entity, TimeSpan timeLife);
    public Task<Result> Update(string key, T entity);
    public Task<Result> Update(string key, T entity, TimeSpan timeLife);
    public Task<Result> UnSet(string key);

    protected string ToJsonString(T entity);
}
