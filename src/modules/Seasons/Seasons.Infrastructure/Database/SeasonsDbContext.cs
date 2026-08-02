using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Seasons.Domain.Entities;

namespace ProjectFootballSim.Seasons.Infrastructure.Database;

public class SeasonsDbContext(DbContextOptions<SeasonsDbContext> options) : DbContext(options)
{
    public DbSet<PlayerSeason> PlayerSeasons => Set<PlayerSeason>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PlayerSeason>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).IsRequired().ValueGeneratedNever();
            entity.Property(x => x.UserId).IsRequired();
            entity.Property(x => x.StartDate).IsRequired();
            entity.Property(x => x.EndDate).IsRequired();
            entity.Property(x => x.Order).IsRequired();
            entity.Property(x => x.IsCurrent).IsRequired();
        });
    }
}
