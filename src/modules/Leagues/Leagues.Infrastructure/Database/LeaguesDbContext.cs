using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Common.Features.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Domain.Entities;
using ProjectFootballSim.Leagues.Domain.ValueObjects;

namespace ProjectFootballSim.Leagues.Infrastructure.Database;

public class LeaguesDbContext(DbContextOptions<LeaguesDbContext> options) : DbContext(options)
{
    public DbSet<League> Leagues => Set<League>();
    public DbSet<LeagueTeam> LeagueTeams => Set<LeagueTeam>();
    public DbSet<LeagueRound> LeagueRounds => Set<LeagueRound>();

    public DbSet<GameLeague> GameLeagues => Set<GameLeague>();
    public DbSet<GameLeagueTeam> GameLeagueTeams => Set<GameLeagueTeam>();
    public DbSet<GameLeagueMatch> GameLeagueMatches => Set<GameLeagueMatch>();

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
            entity.Property(x => x.TeamsCount).IsRequired();

            entity.Property(x => x.PromotionPositions).UseJsonCollection();
            entity.Property(x => x.PromotionPlayOffPositions).UseJsonCollection();
            entity.Property(x => x.RelegationPositions).UseJsonCollection();
            entity.Property(x => x.RelegationPlayOffPositions).UseJsonCollection();
            entity.Property(x => x.UefaChampionsLeaguePositions).UseJsonCollection();
            entity.Property(x => x.UefaEuropaLeaguePositions).UseJsonCollection();
            entity.Property(x => x.UefaConferenceLeaguePositions).UseJsonCollection();
        });

        modelBuilder.Entity<LeagueTeam>(entity =>
        {
            entity.HasKey(x => new { x.LeagueId, x.TeamId });
            entity.Property(x => x.TeamId).IsRequired();
            entity.HasOne<League>()
                .WithMany()
                .HasForeignKey(x => x.LeagueId)
                .IsRequired();
        });

        modelBuilder.Entity<LeagueRound>(entity =>
        {
            entity.HasKey(x => new { x.LeagueId, x.Round });
            entity.Property(x => x.Round).IsRequired();
            entity.Property(x => x.Week).IsRequired();
            entity.Property(x => x.IsMidweek).IsRequired();
            entity.HasOne<League>()
                .WithMany()
                .HasForeignKey(x => x.LeagueId)
                .IsRequired();
            entity.HasIndex(x => new { x.LeagueId, x.Week, x.IsMidweek }).IsUnique();
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

            entity.HasMany(e => e.GameLeagueMatches)
                .WithOne(e => e.GameLeague)
                .HasForeignKey(e => e.GameLeagueId)
                .IsRequired();

            entity.HasIndex(x => new { x.GameId, x.UserId, x.SeasonId, x.LeagueId }).IsUnique();
        });            

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

        modelBuilder.Entity<GameLeagueMatch>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            entity.Property(x => x.Date).IsRequired();
            entity.Property(x => x.HomeTeamId).IsRequired();
            entity.Property(x => x.AwayTeamId).IsRequired();
            entity.Property(x => x.HomeTeamScore).IsRequired(false);
            entity.Property(x => x.AwayTeamScore).IsRequired(false);
            entity.Property(x => x.Round).IsRequired();

            entity.HasOne(x => x.GameLeague)
                .WithMany(x => x.GameLeagueMatches)
                .HasForeignKey(x => x.GameLeagueId)
                .IsRequired();
        });
    }
}
