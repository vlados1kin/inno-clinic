using Application.Contracts.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal sealed class RefreshTokenRepository(AuthDbContext dbContext) : BaseRepository<RefreshToken>(dbContext), IRefreshTokenRepository
{
    private readonly AuthDbContext _dbContext = dbContext;

    public Task<RefreshToken?> GetFreshToken(Guid userId, CancellationToken cancellationToken)
    {
        return _dbContext.RefreshTokens
            .Where(token => token.UserId == userId &&
                            token.Expires > DateTimeOffset.UtcNow &&
                            token.Revoked == null)
            .OrderByDescending(token => token.Issued)
            .FirstOrDefaultAsync(cancellationToken);
    }
}