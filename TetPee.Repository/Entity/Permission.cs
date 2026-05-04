using TetPee.Repository.Abtraction;

namespace TetPee.Repository.Entity;

public class Permission : BaseEntity<Guid>, IAuditableEntity
{
    public required string Name { get; set; }
    public required string Code { get; set; }
    public string? Description { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
