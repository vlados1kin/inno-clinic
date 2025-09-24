using Domain.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser<Guid>, IEntity
{
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}