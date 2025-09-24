using Domain.Entities;

namespace Domain.Abstractions;

public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
{
    Task<RefreshToken?> GetFreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);
}