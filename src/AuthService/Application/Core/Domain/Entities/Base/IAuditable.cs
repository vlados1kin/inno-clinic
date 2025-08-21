namespace Domain.Entities.Base;

/// <summary>
/// The <c>IAuditable</c> interface is used for auditing.
/// </summary>
public interface IAuditable
{
    public Guid? CreatedById { get; set; }
    
    public User? CreatedBy { get; set; }
    
    public DateTimeOffset CreatedOn { get; set; }
    
    public Guid? UpdatedById { get; set; }
    
    public User? UpdatedBy { get; set; }
    
    public DateTimeOffset UpdatedOn { get; set; }
}