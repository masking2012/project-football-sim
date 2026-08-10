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

        var startDate = new DateTime(query.Date.Year, query.Date.Month, query.Date.Day);
        var endDate = startDate.AddDays(1);

        return await dbContext.GameLeagueMatches
            .AsNoTracking()
            .Include(x => x.GameLeague)
            .Where(x => x.GameLeague.UserId == query.UserId
                && x.GameLeague.GameId == query.GameId
                && x.Date >= startDate && x.Date < endDate)
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
