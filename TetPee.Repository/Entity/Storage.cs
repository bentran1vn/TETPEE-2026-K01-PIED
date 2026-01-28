using TetPee.Repository.Abtraction;

namespace TetPee.Repository.Entity;

public class Storage: BaseEntity<Guid>, IAuditableEntity
{
    public decimal Price { get; set; }
    public required string Type { get; set; } // Export, Import
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    
    public ICollection<ProductStorage> ProductStorages { get; set; } = new List<ProductStorage>();
}