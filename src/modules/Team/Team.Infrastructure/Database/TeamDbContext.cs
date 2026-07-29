using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Team.Domain.Entities;
using ProjectFootballSim.Team.Domain.ValueObjects;

namespace ProjectFootballSim.Team.Infrastructure.Database;

public class TeamDbContext(DbContextOptions<TeamDbContext> options) : DbContext(options)
{
    public DbSet<TeamEntity> Teams => Set<TeamEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TeamEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).IsRequired();
            entity.Property(x => x.Name).IsRequired();
            entity.Property(x => x.Attack)
                .IsRequired()
                .HasColumnName("Attack")
                .HasColumnType("INTEGER")
                .HasConversion(v => (int)v.Value, v => new AttributeValue(v));
            entity.Property(x => x.Midfield)
                .IsRequired()
                .HasColumnName("Midfield")
                .HasColumnType("INTEGER")
                .HasConversion(v => (int)v.Value, v => new AttributeValue(v));
            entity.Property(x => x.Defence)
                .IsRequired()
                .HasColumnName("Defence")
                .HasColumnType("INTEGER")
                .HasConversion(v => (int)v.Value, v => new AttributeValue(v));
            entity.Property(x => x.CountryId).IsRequired();
        });
    }
}
