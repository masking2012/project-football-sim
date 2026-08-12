using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Calendar.Domain.Entities;

namespace ProjectFootballSim.Calendar.Infrastructure.Database;

public class CalendarDbContext(DbContextOptions<CalendarDbContext> options) : DbContext(options)
{
    public DbSet<GameCalendar> GameCalendars => Set<GameCalendar>();
    public DbSet<GameSeason> GameSeasons => Set<GameSeason>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<GameCalendar>(entity =>
        {
            entity.HasKey(x => new { x.UserId, x.GameId });
            entity.Property(x => x.UserId).IsRequired();
            entity.Property(x => x.GameId).IsRequired();
            entity.Property(x => x.CurrentDate).IsRequired();
        });

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
