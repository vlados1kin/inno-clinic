using System.Linq.Expressions;
using Domain.Abstractions;
using Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Repositories;

public abstract class BaseRepository<T>(AuthDbContext dbContext) : IBaseRepository<T> where T : class, IEntity
{
    public async Task<List<T>> FindAllAsync(bool trackChanges, CancellationToken cancellationToken = default)
    {
        return !trackChanges
            ? await dbContext.Set<T>().AsNoTracking().ToListAsync(cancellationToken)
            : await dbContext.Set<T>().ToListAsync(cancellationToken);
    }

    public async Task<List<T>> FindByConditionAsync(Expression<Func<T, bool>> expression, bool trackChanges, CancellationToken cancellationToken = default)
    {
        return !trackChanges
            ? await dbContext.Set<T>().Where(expression).AsNoTracking().ToListAsync(cancellationToken)
            : await dbContext.Set<T>().Where(expression).ToListAsync(cancellationToken);
    }

    public Task<T?> FindByIdAsync(Guid id, bool trackChanges, CancellationToken cancellationToken = default)
    {
        return !trackChanges 
                ? dbContext.Set<T>().AsNoTracking().SingleOrDefaultAsync(i => i.Id == id, cancellationToken)
                : dbContext.Set<T>().SingleOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public ValueTask<EntityEntry<T>> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        return dbContext.Set<T>().AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        dbContext.Set<T>().Update(entity);
    }

    public void Delete(T entity)
    {
        dbContext.Set<T>().Remove(entity);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}