using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Seasons.Infrastructure.Database;

namespace ProjectFootballSim.Seasons.Application.Features.GetPlayerSeasons;

public sealed class GetGameSeasonsQueryHandler(SeasonsDbContext dbContext)
{
    public async Task<IReadOnlyList<GameSeasonDto>> HandleAsync(
        GetGameSeasonsQuery query,
        CancellationToken cancellationToken)
    {
        return await dbContext.GameSeasons
            .AsNoTracking()
            .Where(s => s.UserId == query.UserId && s.GameId ==  query.GameId)
            .Select(s => new GameSeasonDto(
                Id: s.Id,
                StartDate: s.StartDate,
                EndDate: s.EndDate,
                Order: s.Order,
                IsCurrent: s.IsCurrent))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
