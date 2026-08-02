using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.GamePersistence.Domain.Entities;

namespace ProjectFootballSim.GamePersistence.Infrastructure.Database;

public class GamePersistenceDbContext(DbContextOptions<GamePersistenceDbContext> options) : DbContext(options)
{
    public DbSet<GameSave> GameSaves => Set<GameSave>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<GameSave>(entity =>
        {
            entity.HasKey(x => new { x.UserId, x.SlotId });
            entity.Property(x => x.UserId).IsRequired();
            entity.Property(x => x.GameId).IsRequired();
            entity.Property(x => x.SlotId).IsRequired();
            entity.Property(x => x.Name).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
        });
    }
}
