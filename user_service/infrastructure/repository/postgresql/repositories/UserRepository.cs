using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using user_service.domain.models;
using user_service.infrastructure.repository.enums;
using user_service.infrastructure.repository.postgresql.repositories.@base;
using user_service.services.result;
using user_service.services.result.errors;

namespace user_service.infrastructure.repository.postgresql.repositories;

public class UserRepository(DbContext context) : BaseRepository<User>(context)
{
    public override async Task<Result<IEnumerable<User>>> GetAll()
    {
        var result = await _context.Set<User>()
            .Include(e => e.Role)
            .Where(e => e.DeletedAt == null)
            .ToListAsync();
        
        return Result<IEnumerable<User>>.Success(result);
    }
    public override async Task<Result<IEnumerable<User>>> GetAll(Tracking tracking)
    {
        if(tracking == Tracking.No)
        {
            var resNoTracking = await _context.Set<User>()
                .AsNoTracking()
                .Include(e => e.Role)
                .Where(e => e.DeletedAt == null)
                .ToListAsync();
            return Result<IEnumerable<User>>.Success(resNoTracking);
        }
        
        return await this.GetAll();
    }
    public override async Task<Result<IEnumerable<User>>> GetAll(Expression<Func<User, bool>> filter, Tracking tracking = Tracking.Yes)
    {
        var result = tracking == Tracking.No
            ? await _context.Set<User>().AsNoTracking().Include(e => e.Role).Where(filter).Where(e => e.DeletedAt == null).ToListAsync()
            : await _context.Set<User>().Where(filter).Where(e => e.DeletedAt == null).Include(e => e.Role).ToListAsync();
        
        return Result<IEnumerable<User>>.Success(result);
    }
    
    public override async Task<Result<User>> GetOne(Guid id)
    {
        var result = await _context.Set<User>().Include(e => e.Role).Where(e => e.DeletedAt == null).FirstOrDefaultAsync(e => e.Id == id);
        
        if(result == null)
            return Result<User>.Failure(Errors.Repository.NotFoundGetOneById);
        
        return Result<User>.Success(result);
    }
    public override async Task<Result<User>> GetOne(Guid id, Tracking tracking = Tracking.Yes)
    {
        if (tracking == Tracking.No)
        {
            var resNoTracking = await _context.Set<User>().AsNoTracking().Include(e => e.Role).Where(e => e.DeletedAt == null).FirstOrDefaultAsync(e => e.Id == id);
            if(resNoTracking == null)
                return Result<User>.Failure(Errors.Repository.NotFoundGetOneById);
            
            return Result<User>.Success(resNoTracking);
        }

        return await this.GetOne(id);
    }
    public override async Task<Result<User>> GetOne(Expression<Func<User, bool>> filter, Tracking tracking = Tracking.Yes)
    {
        var result = tracking == Tracking.No ?
            await _context.Set<User>().AsNoTracking().Include(e => e.Role).Where(e => e.DeletedAt == null).FirstOrDefaultAsync(filter) 
            :
            await _context.Set<User>().Include(e => e.Role).Where(e => e.DeletedAt == null).FirstOrDefaultAsync(filter);

        if (result == null)
            return Result<User>.Failure(Errors.Repository.NotFoundGetOneById);
        
        return Result<User>.Success(result);
    }
}
