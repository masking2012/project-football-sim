using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Team.Domain.Entities;

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
            entity.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            entity.Property(x => x.Name).IsRequired();
            entity.Property(x => x.Attack.Value).IsRequired().HasColumnName("Attack");
            entity.Property(x => x.Midfield.Value).IsRequired().HasColumnName("Midfield");
            entity.Property(x => x.Defence.Value).IsRequired().HasColumnName("Defence");
            entity.Property(x => x.CountryId).IsRequired();
        });
    }
}
