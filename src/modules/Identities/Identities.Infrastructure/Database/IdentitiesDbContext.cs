using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Identities.Domain.Entities;

namespace ProjectFootballSim.Identities.Infrastructure.Database;

public class IdentitiesDbContext(DbContextOptions<IdentitiesDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            entity.Property(x => x.Username).IsRequired().HasMaxLength(100);
            entity.HasIndex(x => x.Username).IsUnique();
            entity.Property(x => x.PasswordHash).IsRequired();
        });
    }
}
