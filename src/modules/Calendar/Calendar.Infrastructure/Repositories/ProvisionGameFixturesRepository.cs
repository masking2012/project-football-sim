using ProjectFootballSim.Calendar.Application.Features.ProvisionGameFixtures;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Infrastructure.Database;

namespace ProjectFootballSim.Calendar.Infrastructure.Repositories;

internal sealed class ProvisionGameFixturesRepository(CalendarDbContext dbContext) : IProvisionGameFixturesRepository
{
    public void AddRange(IEnumerable<GameFixture> gameFixtures)
    {
        dbContext.GameFixtures.AddRange(gameFixtures);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
