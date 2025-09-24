using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Domain.Abstractions;

public interface IBaseRepository<T> where T : class
{
    Task<List<T>> FindAllAsync(bool trackChanges, CancellationToken cancellationToken = default);
    Task<List<T>> FindByConditionAsync(Expression<Func<T, bool>> expression, bool trackChanges, CancellationToken cancellationToken = default);
    Task<T?> FindByIdAsync(Guid id, bool trackChanges, CancellationToken cancellationToken = default);
    ValueTask<EntityEntry<T>> CreateAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}