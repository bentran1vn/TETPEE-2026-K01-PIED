using TetPee.Repository.Abtraction;

namespace TetPee.Repository.Entity;

public class Inventory : BaseEntity<Guid>, IAuditableEntity
{
    public int TotalSell { get; set; }
    public int TotalInStock { get; set; }
    
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}