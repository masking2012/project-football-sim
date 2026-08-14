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
        var gameCalendar = await gameCalendarRetriever
            .GetCurrentGameDateAsync(query.UserId, query.GameId, cancellationToken)
            .ConfigureAwait(false);

        DateTime currentDate = gameCalendar.CurrentDate;
        DateTime date = query.Date ?? currentDate;
        CalendarDayStatus dayStatus = date switch
        {
            var value when value == currentDate => gameCalendar.DayStatus,
            var value when value < currentDate => CalendarDayStatus.Completed,
            var value when value > currentDate => CalendarDayStatus.NotStarted,
            _ => throw new InvalidOperationException("Invalid date comparison.")
        };

        var matches = await getGameLeagueMatchesByDateQueryHandler
            .HandleAsync(
                new GetGameLeagueMatchesByDateQuery(
                    UserId: query.UserId,
                    GameId: query.GameId,
                    Date: date),
                cancellationToken)
            .ConfigureAwait(false);

        return new CalendarDayDto(
            Date: date,
            DayStatus: dayStatus.ToString(),
            LeagueMatches: matches.Select(x => new LeagueMatchDto(
                Id: x.Id,
                HomeTeamId: x.HomeTeamId,
                AwayTeamId: x.AwayTeamId,
                HomeTeamScore: x.HomeTeamScore,
                AwayTeamScore: x.AwayTeamScore,
                Round: x.Round,
                LeagueName: x.LeagueName,
                LeagueId: x.LeagueId,
                CountryId: x.CountryId,
                Order: x.Order))
        );
    }
}
