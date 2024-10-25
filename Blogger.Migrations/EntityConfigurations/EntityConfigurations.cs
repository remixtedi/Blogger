using Blogger.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blogger.Migrations.EntityConfigurations;

public class EntityConfigurations : IEntityTypeConfiguration<BaseEntity>
{
    public void Configure(EntityTypeBuilder<BaseEntity> builder)
    {
        builder.Property(e => e.Created)
            .HasConversion(
                d => d.UtcDateTime,
                d => new DateTimeOffset(d, TimeSpan.Zero)
            );
    }
}

public class AuditableEntityConfigurations : IEntityTypeConfiguration<AuditableEntity>
{
    public void Configure(EntityTypeBuilder<AuditableEntity> builder)
    {
        builder.Property(e => e.LastModified)
            .HasConversion(
                d => d != null ? d.Value.UtcDateTime : (DateTime?)null,
                d => d != null ? new DateTimeOffset(d.Value, TimeSpan.Zero) : null
            );
    }
}