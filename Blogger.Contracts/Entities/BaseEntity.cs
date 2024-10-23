using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Blogger.Contracts.Enums;

namespace Blogger.Contracts.Entities;

public abstract class BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public virtual int Id { get; set; }

    public virtual DateTimeOffset Created { get; set; }
    public virtual EntityStatus EntityStatus { get; set; }
}

public abstract class AuditableEntity : BaseEntity
{
    public virtual string CreatedBy { get; set; }
    public virtual string? ModifiedBy { get; set; }
    public virtual DateTimeOffset? LastModified { get; set; }
}

public abstract class BaseGuidEntity
{
    [Key]
    public virtual Guid Id { get; set; } = Guid.NewGuid();
    public virtual DateTimeOffset Created { get; set; }
    public virtual EntityStatus EntityStatus { get; set; }
}

public abstract class AuditableGuidEntity : BaseGuidEntity
{
    public virtual string CreatedBy { get; set; }
    public virtual string? ModifiedBy { get; set; }
    public virtual DateTimeOffset? LastModified { get; set; }
}