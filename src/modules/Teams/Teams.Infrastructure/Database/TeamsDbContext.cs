using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Teams.Domain.Entities;
using ProjectFootballSim.Teams.Domain.ValueObjects;

namespace ProjectFootballSim.Teams.Infrastructure.Database;

public class TeamsDbContext(DbContextOptions<TeamsDbContext> options) : DbContext(options)
{
    public DbSet<Team> Teams => Set<Team>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            entity.Property(x => x.Name).IsRequired();
            entity.Property(x => x.Attack)
                .IsRequired()
                .HasColumnName("Attack")
                .HasColumnType("INTEGER")
                .HasConversion(v => (int)v.Value, v => new TeamAttributeValue(v));
            entity.Property(x => x.Midfield)
                .IsRequired()
                .HasColumnName("Midfield")
                .HasColumnType("INTEGER")
                .HasConversion(v => (int)v.Value, v => new TeamAttributeValue(v));
            entity.Property(x => x.Defence)
                .IsRequired()
                .HasColumnName("Defence")
                .HasColumnType("INTEGER")
                .HasConversion(v => (int)v.Value, v => new TeamAttributeValue(v));
            entity.Property(x => x.CountryId).IsRequired();
        });
    }
}
