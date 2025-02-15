using user_service.domain.models.@base;
using user_service.services.result;

namespace user_service.infrastructure.repository.interfaces;

public interface IRepository<T> where T : class, IEntity<Guid>
{
    public Task<Result<IEnumerable<T>>> GetAll();
    public Task<Result<T>> GetOne(Guid id);
    public Task<Result> Add(T entity);
    public Task<Result> Update(T entity);
    public Task<Result> Delete(Guid id);
    public Task<Result> Delete(T entity);
    public Task<Result> Save();
}
