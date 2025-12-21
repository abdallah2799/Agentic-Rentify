using System.Linq.Expressions;
using Agentic_Rentify.Core.Common;
using Agentic_Rentify.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Agentic_Rentify.Infrastructure.Persistence;
using Agentic_Rentify.Infrastructure.Specifications;

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

    public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
    {
        var query = ApplySpecification(spec);
        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetPagedResponseAsync(int page, int size)
    {
        return await _context.Set<T>()
            .OrderBy(x => x.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAppAsync(int page, int size, Expression<Func<T, bool>>? predicate = null)
    {
        var query = _context.Set<T>().AsNoTracking();

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        int totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(x => x.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<int> CountAsync(ISpecification<T> spec)
    {
        var query = ApplySpecification(spec);
        return await query.CountAsync();
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

    private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
        return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
    }
}
