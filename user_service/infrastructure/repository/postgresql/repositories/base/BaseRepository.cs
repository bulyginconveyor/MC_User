using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using user_service.domain.models.@base;
using user_service.infrastructure.repository.interfaces;
using user_service.services.result;
using user_service.services.result.errors;
using user_service.services.result.errors.@base;

namespace user_service.infrastructure.repository.postgresql.repositories.@base;

public class BaseRepository<T>(DbContext context)
    : IDbRepository<T> where T : class, IDbModel, IEntity<Guid>
{
    protected readonly DbContext _context = context;
    
    public async Task<Result> Save()
    {
        try
        {
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("Repository.Save.TryException", ex.Message));
        }
    }
    
    public virtual async Task<Result<IEnumerable<T>>> GetAll()
    {
        var result = await _context.Set<T>().Where(e => e.DeletedAt == null).ToListAsync();
        
        return Result<IEnumerable<T>>.Success(result);
    }
    public virtual async Task<Result<IEnumerable<T>>> GetAll(Expression<Func<T, bool>> filter)
    {
        var result = await _context.Set<T>()
            .AsNoTracking()
            .Where(filter).Where(e => e.DeletedAt == null)
            .ToListAsync();
        
        return Result<IEnumerable<T>>.Success(result);
    }
    
    public virtual async Task<Result<T>> GetOne(Guid id)
    {
        var result = await _context.Set<T>().Where(e => e.DeletedAt == null).FirstOrDefaultAsync(e => e.Id == id);
        
        if(result == null)
            return Result<T>.Failure(Errors.Repository.NotFoundGetOneById);
        
        return Result<T>.Success(result);
    }
    public virtual async Task<Result<T>> GetOne(Expression<Func<T, bool>> filter)
    {
        var result = await _context.Set<T>()
            .Where(e => e.DeletedAt == null)
            .FirstOrDefaultAsync(filter);

        if (result == null)
            return Result<T>.Failure(Errors.Repository.NotFoundGetOneById);
        
        return Result<T>.Success(result);
    }
    
    public virtual async Task<Result> Add(T entity)
    {
        try
        {
            entity.CreatedAt = DateTime.UtcNow;
            await _context.Set<T>().AddAsync(entity);
            //await Task.Run(() => _context.Set<T>().Attach(entity));
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("Repository.TryException", ex.Message));
        }
    }
    public virtual async Task AddRange(IEnumerable<T> entities)
    {
        List<Task> tasks = [];
        tasks.AddRange(entities.Select(this.Add));

        await Task.WhenAll(tasks);
    }

    public virtual async Task<Result> Update(T entity)
    {
        try
        {
            // Кажется, что это костыль и есть решение лучше
            entity.CreatedAt = _context.Set<T>()
                .Select(c => 
                    new
                    {
                        Id = c.Id, 
                        CreatedAt = c.CreatedAt
                    })
                .First(e => e.Id == entity.Id)
                .CreatedAt;
            entity.UpdatedAt = DateTime.UtcNow;

            //await Task.Run(() => _context.Set<T>().Attach(entity));
            await Task.Run(() => _context.Set<T>().Update(entity));
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(Errors.Repository.TryException);
        }
    }
    public virtual async Task UpdateRange(IEnumerable<T> entities)
    {
        List<Task> tasks = [];
        tasks.AddRange(entities.Select(this.Update));

        await Task.WhenAll(tasks);
    }
    
    public virtual async Task<Result> Delete(Guid id)
    {
        try
        {
            await _context
                .Set<T>()
                .Where(e => e.Id == id)
                .ExecuteUpdateAsync(e => 
                    e.SetProperty(model => model.DeletedAt, DateTime.UtcNow)
                );
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(Errors.Repository.TryException);
        }
    }
    public virtual async Task<Result> Delete(T entity)
    {
        try
        {
            await _context
                .Set<T>()
                .Where(e => e.Id == entity.Id)
                .ExecuteUpdateAsync(e => 
                    e.SetProperty(model => model.DeletedAt, DateTime.UtcNow)
                );
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(Errors.Repository.TryException);
        }
    }
    public virtual async Task DeleteRange(IEnumerable<T> entities)
    {
        //TODO: Сделать более оптимизированным вариантом!!!
        // Вариант с собиранием всех tasks в список и вызов метода Task.WhenAll(tasks) не работает!!!
        
        foreach (var entity in entities)
            await Delete(entity);
    }

    public virtual async Task<Result<T>> LoadData(T entity) => await Task.FromResult(Result<T>.Success(entity));

    public virtual async Task<Result<bool>> Exists(Expression<Func<T, bool>> filter)
    {
        try
        {
            var result = await _context.Set<T>()
                .AsNoTracking()
                .Where(e => e.DeletedAt == null)
                .AnyAsync(filter);
            
            return result 
                ? Result<bool>.Success(true)
                : Result<bool>.Success(false);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(new Error("Repository.TryExcepthion", ex.Message));
        }
    }
    public virtual async Task<Result<long>> Count(Expression<Func<T, bool>> filter)
    {
        var res = await _context.Set<T>()
            .AsNoTracking()
            .Where(e => e.DeletedAt == null)
            .CountAsync(filter);
        if (res == 0)
            return Result<long>.Failure(Errors.Repository.NotFoundCount);
        
        return Result<long>.Success(res);
    }

    public virtual async Task<Result<ulong>> PagesCount(uint countPerPage, Expression<Func<T, bool>> filter = null)
    {
        var countPerPageDec = (decimal)countPerPage;
        var count = filter is null ? 
            (decimal)await _context.Set<T>().AsNoTracking().Where(e => e.DeletedAt == null).CountAsync()
            : (decimal)await _context.Set<T>().AsNoTracking().Where(e => e.DeletedAt == null).CountAsync(filter);
        
        if(count == 0)
            return Result<ulong>.Failure(Errors.Repository.NotFoundCount);
        
        var pageCount = Math.Ceiling(count / countPerPageDec);
        return Result<ulong>.Success((ulong)pageCount);
    }

    public virtual async Task<Result<IEnumerable<T>>> GetByPage(uint countPerPage, uint pageNumber,
        Expression<Func<T, bool>> filter = null)
    {
        var countPerPageDec = (decimal)countPerPage;
        var count = filter is null ? 
            (decimal)await _context.Set<T>().AsNoTracking().Where(e => e.DeletedAt == null).CountAsync() 
            : (decimal)await _context.Set<T>().AsNoTracking().Where(e => e.DeletedAt == null).CountAsync(filter);

        if (countPerPage * --pageNumber >= count)
            return Result<IEnumerable<T>>.Failure(Errors.Repository.NullArgument);
        
        var res = filter == null ? 
            await _context.Set<T>().AsNoTracking().Where(e => e.DeletedAt == null).Skip((int)(countPerPage * pageNumber)).Take((int)countPerPage).ToListAsync()
            : await _context.Set<T>().AsNoTracking().Where(e => e.DeletedAt == null).Where(filter).Skip((int)(countPerPage * pageNumber)).Take((int)countPerPage).ToListAsync();
        
        return Result<IEnumerable<T>>.Success(res);
    }
}
