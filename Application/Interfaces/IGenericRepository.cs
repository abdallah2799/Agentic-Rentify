using System.Linq.Expressions;
using Agentic_Rentify.Core.Common;

namespace Agentic_Rentify.Application.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> ListAllAsync();
    Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);
    Task<IReadOnlyList<T>> GetPagedResponseAsync(int page, int size);
    Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAppAsync(int page, int size, Expression<Func<T, bool>>? predicate = null);
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
    Task<int> CountAsync(ISpecification<T> spec);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
