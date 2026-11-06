using ProjectFootballSim.Calendar.Domain.Entities;

namespace ProjectFootballSim.Calendar.Application.Features.GetCalendarDay;

public interface IGetCalendarDayFixturesRepository
{
    Task<IReadOnlyList<GameFixture>> GetFixturesAsync(
        Guid userId,
        Guid gameId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken);
}
