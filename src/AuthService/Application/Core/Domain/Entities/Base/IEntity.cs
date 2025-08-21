namespace Domain.Entities.Base;

/// <summary>
/// The <c>IEntity</c> interface represents a base entity with unique identifier
/// </summary>
public interface IEntity
{
    public Guid Id { get; set; }
}