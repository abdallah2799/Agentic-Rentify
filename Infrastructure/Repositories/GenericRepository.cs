using System.Linq.Expressions;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Agentic_Rentify.Infrastructure.Persistence;

namespace Agentic_Rentify.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<IReadOnlyList<T>> ListAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().Where(predicate).ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetPagedResponseAsync(int page, int size)
    {
        return await _context.Set<T>()
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        // Careful handling for simple updates.
        // For complex types with OwnsMany/ToJson, EF Core Change Tracking should handle it
        // if the entity is tracked. If we are attaching, we need to set state.
        
        _context.Entry(entity).State = EntityState.Modified;
        
        // No SaveChangesAsync here as we will use UnitOfWork
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        await Task.CompletedTask;
    }
}
