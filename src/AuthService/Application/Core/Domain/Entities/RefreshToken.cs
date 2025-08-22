using System.Net;
using Domain.Entities.Base;

namespace Domain.Entities;

public class RefreshToken : IEntity
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public string? Token { get; set; }
    
    public IPAddress IpAddress { get; set; } = IPAddress.None;
    
    public DateTimeOffset Issued { get; set; }
    
    public DateTimeOffset Expires { get; set; }
    
    public DateTimeOffset? Revoked { get; set; }
    
    public User? User { get; set; }
}