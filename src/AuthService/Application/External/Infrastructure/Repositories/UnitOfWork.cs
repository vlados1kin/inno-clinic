using Application.Contracts.Repositories;

namespace Infrastructure.Repositories;

internal sealed class UnitOfWork(AuthDbContext context) : IUnitOfWork
{
    private readonly Lazy<IRefreshTokenRepository> _refreshTokenRepository = new(() => new RefreshTokenRepository(context));

    public IRefreshTokenRepository RefreshTokenRepository => _refreshTokenRepository.Value;
    
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        return context.Database.CurrentTransaction is not null ? context.Database.CurrentTransaction.CommitAsync(cancellationToken) : Task.CompletedTask;
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        return context.Database.CurrentTransaction is not null ? context.Database.CurrentTransaction.RollbackAsync(cancellationToken) : Task.CompletedTask;
    }
}