using ProjectFootballSim.Calendar.Application.Common.Services;
using ProjectFootballSim.Leagues.Application.GameFeatures.GetGameLeagueMatchesByDate;

namespace ProjectFootballSim.Calendar.Application.Features.GetEventsByDate;

public sealed class GetEventsByDateQueryHandler(
    IGameCalendarRetriever gameCalendarRetriever,
    GetGameLeagueMatchesByDateQueryHandler getGameLeagueMatchesByDateQueryHandler)
{
    public async Task<IEnumerable<MatchEventDto>> HandleAsync(GetEventsByDateQuery query, CancellationToken cancellationToken)
    {
        DateTime? date = query.Date;
        if (date is null)
        {
            var gameCalendar = await gameCalendarRetriever
                .GetCurrentGameDateAsync(query.UserId, query.GameId, cancellationToken)
                .ConfigureAwait(false);
            date = gameCalendar.CurrentDate;
        }

        var getGameLeagueMatchesByDateQuery = new GetGameLeagueMatchesByDateQuery(GameId: query.GameId, Date: date.Value);
        var matches = await getGameLeagueMatchesByDateQueryHandler
            .HandleAsync(getGameLeagueMatchesByDateQuery, cancellationToken)
            .ConfigureAwait(false);

        return matches.Select(x => new MatchEventDto(
            Id: x.Id,
            HomeTeamId: x.HomeTeamId,
            AwayTeamId: x.AwayTeamId,
            HomeTeamScore: x.HomeTeamScore,
            AwayTeamScore: x.AwayTeamScore,
            Round: x.Round,
            LeagueName: x.LeagueName,
            LeagueId: x.LeagueId,
            CountryId: x.CountryId));
    }
}
