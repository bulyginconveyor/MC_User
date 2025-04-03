using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using user_service.domain.models;
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
    public override async Task<Result<IEnumerable<User>>> GetAll(Expression<Func<User, bool>> filter)
    {
        var result = await _context.Set<User>()
                .AsNoTracking()
                .Include(e => e.Role)
                .Where(filter).Where(e => e.DeletedAt == null)
                .ToListAsync();
        
        return Result<IEnumerable<User>>.Success(result);
    }
    
    public override async Task<Result<User>> GetOne(Guid id)
    {
        var result = await _context.Set<User>().Include(e => e.Role).Where(e => e.DeletedAt == null).FirstOrDefaultAsync(e => e.Id == id);
        
        if(result == null)
            return Result<User>.Failure(Errors.Repository.NotFoundGetOneById);
        
        return Result<User>.Success(result);
    }
    public override async Task<Result<User>> GetOne(Expression<Func<User, bool>> filter)
    {
        var result = await _context.Set<User>()
                .AsNoTracking()
                .Include(e => e.Role)
                .Where(e => e.DeletedAt == null)
                .FirstOrDefaultAsync(filter); 

        if (result == null)
            return Result<User>.Failure(Errors.Repository.NotFoundGetOneById);
        
        return Result<User>.Success(result);
    }
}
