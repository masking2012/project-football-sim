using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Calendar.Application.Features.ScheduleGameFixtures;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Domain.Enums;
using ProjectFootballSim.Calendar.Infrastructure.Database;

namespace ProjectFootballSim.Calendar.Infrastructure.Repositories;

internal sealed class ScheduleGameFixturesRepository(CalendarDbContext dbContext) : IScheduleGameFixturesRepository
{
    public async Task<IReadOnlyList<GameFixture>> GetProvisionalFixturesAsync(
        Guid userId,
        Guid gameId,
        Guid gameCompetitionId,
        CompetitionType competitionType,
        int roundId,
        CancellationToken cancellationToken)
    {
        return await dbContext.GameFixtures
            .Where(fixture => fixture.UserId == userId
                && fixture.GameId == gameId
                && fixture.Status == FixtureStatus.Provisional
                && fixture.CompetitionType == competitionType
                && fixture.GameCompetitionId == gameCompetitionId
                && fixture.RoundId == roundId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
