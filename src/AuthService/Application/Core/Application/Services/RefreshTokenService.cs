using System.Security.Cryptography;
using Application.Abstractions;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Application.Services;

public sealed class RefreshTokenService(
    IRefreshTokenRepository refreshTokenRepository,
    IHttpContextAccessor httpContextAccessor,
    IOptions<JwtOptions> jwtOptions) : IRefreshTokenService
{
    public async Task<RefreshToken> GenerateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var randomNumber = new byte[32];
        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(randomNumber);
        
        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Issued = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(jwtOptions.Value.RefreshTokenExpires),
            Token = Convert.ToBase64String(randomNumber),
            IpAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString()
        };
                
        await refreshTokenRepository.CreateAsync(refreshToken, cancellationToken);
        await refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return refreshToken;
    }
}