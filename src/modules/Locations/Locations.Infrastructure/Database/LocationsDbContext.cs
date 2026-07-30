using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Locations.Domain.Entities;

namespace ProjectFootballSim.Locations.Infrastructure.Database;

public class LocationsDbContext(DbContextOptions<LocationsDbContext> options) : DbContext(options)
{
    public DbSet<Country> Countries => Set<Country>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            entity.Property(x => x.Name).IsRequired();
        });

        modelBuilder.Entity<Country>()
            .HasIndex(u => u.Name)
            .IsUnique();
    }
}
