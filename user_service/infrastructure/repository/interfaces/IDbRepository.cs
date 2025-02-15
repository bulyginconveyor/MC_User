using System.Linq.Expressions;
using user_service.domain.models.@base;
using user_service.infrastructure.repository.enums;
using user_service.services.result;

namespace user_service.infrastructure.repository.interfaces;

public interface IDbRepository<T> : IRepository<T> where T : class, IDbModel, IEntity<Guid>
{
    public Task<Result<IEnumerable<T>>> GetAll(Expression<Func<T, bool>> filter, Tracking tracking = Tracking.Yes);
    public Task<Result<IEnumerable<T>>> GetAll(Tracking tracking);
    public Task<Result<T>> GetOne(Expression<Func<T, bool>> filter, Tracking tracking = Tracking.Yes);
    public Task<Result<T>> GetOne(Guid id, Tracking tracking = Tracking.Yes);
    public Task AddRange(IEnumerable<T> entities);
    public Task UpdateRange(IEnumerable<T> entities);
    public Task DeleteRange(IEnumerable<T> entities);

    public Task<Result<T>> LoadData(T entity);
    
    public Task<Result<bool>> Exists(Expression<Func<T, bool>> filter);
    public Task<Result<long>> Count(Expression<Func<T, bool>> filter);

    public Task<Result<ulong>> PagesCount(uint countPerPage, Expression<Func<T, bool>> filter = null);

    public Task<Result<IEnumerable<T>>> GetByPage(uint countPerPage, uint pageNumber,
        Expression<Func<T, bool>> filter = null);
}
