using user_service.services.result;

namespace user_service.infrastructure.repository.interfaces;

public interface ICacheRepositoryWithList<T> : ICacheRepository<T> where T : class
{
    public Task<Result<List<T>>> GetAll();
    public Task<Result> Add(List<T> entities);
    public Task<Result> Add(List<T> entities, TimeSpan timeLife);
    
    public Task<Result> Update(List<T> entities);
    public Task<Result> Update(List<T> entities, TimeSpan timeLife);

    public Task<Result> UnSetCollection();
}
