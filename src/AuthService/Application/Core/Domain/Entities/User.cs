using Domain.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser<Guid>, IEntity, IAuditable
{
    public Guid? CreatedById { get; set; }
    
    public User? CreatedBy { get; set; }
    
    public DateTimeOffset CreatedOn { get; set; }

    public Guid? UpdatedById { get; set; }
    
    public User? UpdatedBy { get; set; }
    
    public DateTimeOffset UpdatedOn { get; set; }
}