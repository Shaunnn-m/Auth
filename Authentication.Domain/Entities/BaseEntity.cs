namespace Authentication.Domain.Entities
{
   public abstract class BaseEntity
{
    public Guid Id { get; set; } = new Guid();

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
}
