using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class RefreshTokenRepository(AuthDbContext dbDbContext) : BaseRepository<RefreshToken>(dbDbContext), IRefreshTokenRepository
{
    public Task<RefreshToken?> GetFreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        return dbDbContext.RefreshTokens
            .Where(token => token.UserId == userId &&
                            token.Expires > DateTimeOffset.UtcNow &&
                            token.Revoked == null)
            .OrderByDescending(token => token.Issued)
            .FirstOrDefaultAsync(cancellationToken);
    }
}