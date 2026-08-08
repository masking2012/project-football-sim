using ProjectFootballSim.Calendar.Application.Common.Services;
using ProjectFootballSim.Calendar.Application.Features.GetEventsByDate;
using ProjectFootballSim.Calendar.Domain.Enums;
using ProjectFootballSim.Leagues.Application.GameFeatures.GetGameLeagueMatchesByDate;

namespace ProjectFootballSim.Calendar.Application.Features.GetDayWithEvents;

public sealed class GetDayWithEventsQueryHandler(
    IGameCalendarRetriever gameCalendarRetriever,
    GetGameLeagueMatchesByDateQueryHandler getGameLeagueMatchesByDateQueryHandler)
{
    public async Task<DayDetailsDto> HandleAsync(GetDayWithEventsQuery query, CancellationToken cancellationToken)
    {
        DateTime? date = query.Date;

        var gameCalendar = await gameCalendarRetriever
            .GetCurrentGameDateAsync(query.UserId, query.GameId, cancellationToken)
            .ConfigureAwait(false);
        DayState dayState;

        if (date is null || date == gameCalendar.CurrentDate)
        {
            dayState = gameCalendar.State;
            date = gameCalendar.CurrentDate;
        }
        else if (date < gameCalendar.CurrentDate)
        {
            dayState = DayState.Completed;
        }
        else if (date > gameCalendar.CurrentDate)
        {
            dayState = DayState.NotStarted;
        }
        else
        {
            throw new InvalidOperationException("Invalid date comparison.");
        }

        var getGameLeagueMatchesByDateQuery = new GetGameLeagueMatchesByDateQuery(GameId: query.GameId, Date: date.Value);
        var matches = await getGameLeagueMatchesByDateQueryHandler
            .HandleAsync(getGameLeagueMatchesByDateQuery, cancellationToken)
            .ConfigureAwait(false);

        var result = new DayDetailsDto(
            Date: date.Value,
            DayState: dayState.ToString(),
            MatchEvents: matches.Select(x => new MatchEventDto(
                Id: x.Id,
                HomeTeamId: x.HomeTeamId,
                AwayTeamId: x.AwayTeamId,
                HomeTeamScore: x.HomeTeamScore,
                AwayTeamScore: x.AwayTeamScore,
                Round: x.Round,
                LeagueName: x.LeagueName,
                LeagueId: x.LeagueId,
                CountryId: x.CountryId))
        );
        return result;
    }
}
