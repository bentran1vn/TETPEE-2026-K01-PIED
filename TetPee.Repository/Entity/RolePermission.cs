using TetPee.Repository.Abtraction;

namespace TetPee.Repository.Entity;

public class RolePermission : BaseEntity<Guid>, IAuditableEntity
{
    public Guid RoleId { get; set; }
    public Role Role { get; set; }

    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
