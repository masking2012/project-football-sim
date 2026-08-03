using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Domain.Entities;
using ProjectFootballSim.Leagues.Domain.ValueObjects;

namespace ProjectFootballSim.Leagues.Infrastructure.Database;

public class LeaguesDbContext(DbContextOptions<LeaguesDbContext> options) : DbContext(options)
{
    public DbSet<League> Leagues => Set<League>();
    public DbSet<LeagueTeam> LeagueTeams => Set<LeagueTeam>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<League>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            entity.Property(x => x.Name).IsRequired();
            entity.Property(x => x.Order).IsRequired();
            entity.Property(x => x.CountryId).IsRequired();
        });

        modelBuilder.Entity<LeagueTeam>(entity =>
        {
            entity.Property(x => x.LeagueId).IsRequired();
            entity.Property(x => x.TeamId).IsRequired();

            entity.HasKey(x => new { x.LeagueId, x.TeamId });
        });
    }
}
