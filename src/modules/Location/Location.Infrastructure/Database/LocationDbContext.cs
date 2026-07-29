using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Location.Domain.Entities;

namespace ProjectFootballSim.Location.Infrastructure.Database;

public class LocationDbContext(DbContextOptions<LocationDbContext> options) : DbContext(options)
{
    public DbSet<CountryEntity> Countries => Set<CountryEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CountryEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            entity.Property(x => x.Name).IsRequired();
        });

        modelBuilder.Entity<CountryEntity>()
            .HasIndex(u => u.Name)
            .IsUnique();
    }
}
