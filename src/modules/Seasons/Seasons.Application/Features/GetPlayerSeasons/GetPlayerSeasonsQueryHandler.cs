using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Seasons.Infrastructure.Database;

namespace ProjectFootballSim.Seasons.Application.Features.GetPlayerSeasons;

public sealed class GetPlayerSeasonsQueryHandler(SeasonsDbContext dbContext)
{
    public async Task<IReadOnlyList<PlayerSeasonDto>> HandleAsync(
        GetPlayerSeasonsQuery query,
        CancellationToken cancellationToken)
    {
        var seasons = await dbContext.PlayerSeasons
            .AsNoTracking()
            .Where(s => s.UserId == query.UserId && s.GameId ==  query.GameId)
            .Select(s => new PlayerSeasonDto(
                Id: s.Id,
                StartDate: s.StartDate,
                EndDate: s.EndDate,
                IsCurrent: s.IsCurrent))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return seasons;
    }
}
