using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Application.GameState.Services;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueMatchesByDate;

public sealed class GetGameLeagueMatchesByDateQueryHandler(
    LeaguesDbContext dbContext,
    IGameLeagueCatalog gameLeagueCatalog)
{
    public async Task<IReadOnlyList<GameLeagueMatchDto>> HandleAsync(
        GetGameLeagueMatchesByDateQuery query,
        CancellationToken cancellationToken)
    {
        var leagues = (await gameLeagueCatalog
            .GetByGameAsync(query.UserId, query.GameId, cancellationToken)
            .ConfigureAwait(false))
            .ToDictionary(league => league.LeagueVersionId);

        var startDate = new DateTime(query.Date.Year, query.Date.Month, query.Date.Day);
        var endDate = startDate.AddDays(1);

        var matches = await dbContext.GameLeagueMatches
            .AsNoTracking()
            .Join(
                dbContext.GameLeagues.AsNoTracking(),
                match => match.GameLeagueId,
                gameLeague => gameLeague.Id,
                (match, gameLeague) => new { match, gameLeague })
            .Join(
                dbContext.LeagueVersions.AsNoTracking(),
                item => item.gameLeague.LeagueVersionId,
                version => version.LeagueVersionId,
                (item, version) => new { item.match, item.gameLeague, version })
            .Where(x => x.gameLeague.UserId == query.UserId
                && x.gameLeague.GameId == query.GameId
                && x.match.Date >= startDate && x.match.Date < endDate)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return matches
            .Select(x => new GameLeagueMatchDto(
                Id: x.match.Id,
                HomeTeamId: x.match.HomeTeamId,
                AwayTeamId: x.match.AwayTeamId,
                HomeTeamScore: x.match.HomeTeamScore,
                AwayTeamScore: x.match.AwayTeamScore,
                Round: x.match.Round,
                 LeagueName: leagues[x.version.LeagueVersionId].Name,
                 LeagueId: x.version.LeagueId,
                 CountryId: leagues[x.version.LeagueVersionId].CountryId,
                 Order: leagues[x.version.LeagueVersionId].Order))
            .ToList();
    }
}
