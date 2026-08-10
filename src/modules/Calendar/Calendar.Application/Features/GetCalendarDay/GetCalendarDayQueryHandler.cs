using ProjectFootballSim.Calendar.Application.Common.Services;
using ProjectFootballSim.Calendar.Domain.Enums;
using ProjectFootballSim.Leagues.Application.GameFeatures.GetGameLeagueMatchesByDate;

namespace ProjectFootballSim.Calendar.Application.Features.GetCalendarDay;

public sealed class GetCalendarDayQueryHandler(
    IGameCalendarRetriever gameCalendarRetriever,
    GetGameLeagueMatchesByDateQueryHandler getGameLeagueMatchesByDateQueryHandler)
{
    public async Task<CalendarDayDto> HandleAsync(GetCalendarDayQuery query, CancellationToken cancellationToken)
    {
        DateTime? date = query.Date;

        var gameCalendar = await gameCalendarRetriever
            .GetCurrentGameDateAsync(query.UserId, query.GameId, cancellationToken)
            .ConfigureAwait(false);
        CalendarDayStatus dayState;

        if (date is null || date == gameCalendar.CurrentDate)
        {
            dayState = gameCalendar.DayStatus;
            date = gameCalendar.CurrentDate;
        }
        else if (date < gameCalendar.CurrentDate)
        {
            dayState = CalendarDayStatus.Completed;
        }
        else if (date > gameCalendar.CurrentDate)
        {
            dayState = CalendarDayStatus.NotStarted;
        }
        else
        {
            throw new InvalidOperationException("Invalid date comparison.");
        }

        var getGameLeagueMatchesByDateQuery = new GetGameLeagueMatchesByDateQuery(UserId: query.UserId, GameId: query.GameId, Date: date.Value);
        var matches = await getGameLeagueMatchesByDateQueryHandler
            .HandleAsync(getGameLeagueMatchesByDateQuery, cancellationToken)
            .ConfigureAwait(false);

        var result = new CalendarDayDto(
            Date: date.Value,
            DayStatus: dayState.ToString(),
            LeagueMatches: matches.Select(x => new LeagueMatchDto(
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
