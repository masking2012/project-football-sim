using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Domain.Entities;
using ProjectFootballSim.Leagues.Domain.ValueObjects;

namespace ProjectFootballSim.Leagues.Infrastructure.Database;

public class LeaguesDbContext(DbContextOptions<LeaguesDbContext> options) : DbContext(options)
{
    public DbSet<League> Leagues => Set<League>();
    public DbSet<LeagueTeam> LeagueTeams => Set<LeagueTeam>();

    public DbSet<GameLeague> GameLeagues => Set<GameLeague>();
    public DbSet<GameLeagueTeam> GameLeagueTeams => Set<GameLeagueTeam>();

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

        modelBuilder.Entity<GameLeague>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            entity.Property(x => x.UserId).IsRequired();
            entity.Property(x => x.GameId).IsRequired();
            entity.Property(x => x.SeasonId).IsRequired();

            entity.HasOne(x => x.League)
                .WithMany()
                .HasForeignKey(x => x.LeagueId)
                .IsRequired();
        });

        modelBuilder.Entity<GameLeague>()
            .HasMany(e => e.GameLeagueTeams)
            .WithOne(e => e.GameLeague)
            .HasForeignKey(e => e.GameLeagueId)
            .IsRequired();

        modelBuilder.Entity<GameLeagueTeam>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).IsRequired();
            entity.Property(x => x.GameLeagueId).IsRequired();
            entity.Property(x => x.TeamId).IsRequired();
            entity.Property(x => x.Wins).IsRequired().HasDefaultValue(0);
            entity.Property(x => x.Draws).IsRequired().HasDefaultValue(0);
            entity.Property(x => x.Losses).IsRequired().HasDefaultValue(0);
            entity.Property(x => x.GoalsFor).IsRequired().HasDefaultValue(0);
            entity.Property(x => x.GoalsAgainst).IsRequired().HasDefaultValue(0);
            entity.Property(x => x.Points).IsRequired().HasDefaultValue(0);
        });

        modelBuilder.Entity<GameLeague>()
            .HasMany(x => x.GameLeagueTeams)
            .WithOne(e => e.GameLeague)
            .HasForeignKey(e => e.GameLeagueId)
            .IsRequired();

    }
}
