using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Country.Domain.Entities;

namespace ProjectFootballSim.Country.Infrastructure.Database;

public class CountryDbContext(DbContextOptions<CountryDbContext> options) : DbContext(options)
{
    public DbSet<CountryEntity> Countries => Set<CountryEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CountryEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).IsRequired();
            entity.Property(x => x.Name).IsRequired();
        });

        modelBuilder.Entity<CountryEntity>()
            .HasIndex(u => u.Name)
            .IsUnique();
    }
}
