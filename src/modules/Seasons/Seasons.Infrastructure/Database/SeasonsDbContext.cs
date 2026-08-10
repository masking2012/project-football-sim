using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Seasons.Domain.Entities;

namespace ProjectFootballSim.Seasons.Infrastructure.Database;

public class SeasonsDbContext(DbContextOptions<SeasonsDbContext> options) : DbContext(options)
{
    public DbSet<GameSeason> GameSeasons => Set<GameSeason>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<GameSeason>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
            entity.Property(x => x.GameId).IsRequired();
            entity.Property(x => x.UserId).IsRequired();
            entity.Property(x => x.StartDate).IsRequired();
            entity.Property(x => x.EndDate).IsRequired();
            entity.Property(x => x.Order).IsRequired();
            entity.Property(x => x.IsCurrent).IsRequired();

            entity.HasIndex(x => new { x.GameId, x.UserId, x.Order }).IsUnique();
        });
    }
}
