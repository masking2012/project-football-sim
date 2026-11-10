using ProjectFootballSim.Calendar.Domain.Entities;

namespace ProjectFootballSim.Calendar.Application.Features.ProvisionGameFixtures;

public interface IProvisionGameFixturesRepository
{
    void AddRange(IEnumerable<GameFixture> gameFixtures);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
