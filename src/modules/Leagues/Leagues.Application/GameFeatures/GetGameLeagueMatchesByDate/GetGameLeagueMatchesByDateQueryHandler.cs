using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Application.Common.Services;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Application.GameFeatures.GetGameLeagueMatchesByDate;

public sealed class GetGameLeagueMatchesByDateQueryHandler(
    LeaguesDbContext dbContext,
    ILeaguesCatalog leaguesCatalog)
{
    public async Task<IReadOnlyList<GameLeagueMatchDto>> HandleAsync(
        GetGameLeagueMatchesByDateQuery query,
        CancellationToken cancellationToken)
    {
        var leagues = await leaguesCatalog.GetAllAsync(cancellationToken).ConfigureAwait(false);

        return await dbContext.GameLeagueMatches
            .AsNoTracking()
            .Include(x => x.GameLeague)
            .Where(x => x.GameLeague.GameId == query.GameId && x.Date == query.Date)
            .Select(x => new GameLeagueMatchDto(
                Id: x.Id,
                HomeTeamId: x.HomeTeamId,
                AwayTeamId: x.AwayTeamId,
                HomeTeamScore: x.HomeTeamScore,
                AwayTeamScore: x.AwayTeamScore,
                Round: x.Round,
                LeagueName: leagues[x.GameLeague.LeagueId].Name,
                LeagueId: x.GameLeague.LeagueId,
                CountryId: leagues[x.GameLeague.LeagueId].CountryId))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
