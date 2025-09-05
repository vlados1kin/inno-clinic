using Domain.Entities;

namespace Application.Contracts.Repositories;

public interface IRefreshTokenRepository : IRepositoryBase<RefreshToken>
{
    Task<RefreshToken?> GetFreshToken(Guid userId, CancellationToken cancellationToken);
}