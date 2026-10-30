using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Calendar.Application.Features.GetGameSeasons;
using ProjectFootballSim.Calendar.Infrastructure.Database;

namespace ProjectFootballSim.Calendar.Infrastructure.Repositories;

internal sealed class GetGameSeasonsRepository(CalendarDbContext calendarDbContext) : IGetGameSeasonsRepository
{
    public async Task<IReadOnlyList<GameSeasonDto>> GetAsync(Guid userId, Guid gameId, CancellationToken cancellationToken)
    {
        var seasons = await calendarDbContext.GameSeasons
            .AsNoTracking()
            .Where(s => s.UserId == userId && s.GameId == gameId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return seasons.Select(s => new GameSeasonDto(
            Id: s.Id,
            StartDate: s.StartDate,
            EndDate: s.EndDate,
            Order: s.Order,
            IsCurrent: s.IsCurrent))
            .ToList();
    }
}
