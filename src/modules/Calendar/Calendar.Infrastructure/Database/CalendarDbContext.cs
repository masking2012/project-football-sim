using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Calendar.Domain.Entities;

namespace ProjectFootballSim.Calendar.Infrastructure.Database;

public class CalendarDbContext(DbContextOptions<CalendarDbContext> options) : DbContext(options)
{
    public DbSet<GameCalendar> GameCalendars => Set<GameCalendar>();

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
    }
}
