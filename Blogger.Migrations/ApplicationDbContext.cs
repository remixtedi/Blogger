using Blogger.Contracts.Entities;
using Blogger.Contracts.Enums;
using Blogger.Contracts.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blogger.Migrations;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IUserService userService)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Blog> Blogs { get; set; }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var item in ChangeTracker.Entries<AuditableEntity>().AsEnumerable())
            switch (item.State)
            {
                case EntityState.Added:
                    item.Entity.Created = DateTime.UtcNow;
                    item.Entity.CreatedBy = string.IsNullOrWhiteSpace(item.Entity.CreatedBy)
                        ? userService.UserId
                        : item.Entity.CreatedBy;
                    item.Entity.EntityStatus = EntityStatus.Active;
                    break;
                case EntityState.Modified:
                    item.Entity.LastModified = DateTime.UtcNow;
                    item.Entity.ModifiedBy = userService.UserId;
                    break;
                case EntityState.Detached:
                    break;
                case EntityState.Unchanged:
                    break;
                case EntityState.Deleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        foreach (var item in ChangeTracker.Entries<AuditableGuidEntity>().AsEnumerable())
            switch (item.State)
            {
                case EntityState.Added:
                    item.Entity.Created = DateTime.UtcNow;
                    item.Entity.CreatedBy = string.IsNullOrWhiteSpace(item.Entity.CreatedBy)
                        ? userService.UserId
                        : item.Entity.CreatedBy;
                    item.Entity.EntityStatus = EntityStatus.Active;
                    break;
                case EntityState.Modified:
                    item.Entity.LastModified = DateTime.UtcNow;
                    item.Entity.ModifiedBy = userService.UserId;
                    break;
                case EntityState.Detached:
                    break;
                case EntityState.Unchanged:
                    break;
                case EntityState.Deleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        return base.SaveChangesAsync(cancellationToken);
    }
}