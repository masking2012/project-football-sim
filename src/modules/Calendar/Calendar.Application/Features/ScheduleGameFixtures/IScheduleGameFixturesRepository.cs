using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Domain.Enums;

namespace ProjectFootballSim.Calendar.Application.Features.ScheduleGameFixtures;

public interface IScheduleGameFixturesRepository
{
    Task<IReadOnlyList<GameFixture>> GetProvisionalFixturesAsync(
        Guid userId,
        Guid gameId,
        Guid gameCompetitionId,
        CompetitionType competitionType,
        int roundId,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
