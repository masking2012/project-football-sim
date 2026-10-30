using ProjectFootballSim.Calendar.Application.Common.Interfaces;
using ProjectFootballSim.Calendar.Application.Common.Models;
using ProjectFootballSim.Calendar.Contracts.Features.GetCalendarDay;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueMatchesByDate;

namespace ProjectFootballSim.Calendar.Application.Features.GetCalendarDay;

internal sealed class GetCalendarDayQueryHandler(
    IGetGameCalendarRepository gameCalendarRepository,
    IGetGameLeagueMatchesByDateQueryHandler getGameLeagueMatchesByDateQueryHandler)
    : IGetCalendarDayQueryHandler
{
    public async Task<GetCalendarDayQueryResult> HandleAsync(GetCalendarDayQuery query, CancellationToken cancellationToken)
    {
        GameCalendarDto? gameCalendar = await gameCalendarRepository
            .GetAsync(query.UserId, query.GameId, cancellationToken)
            .ConfigureAwait(false);
        if (gameCalendar is null)
            throw new InvalidOperationException(
                $"Game calendar not found for user ID '{query.UserId}' and game ID '{query.GameId}'.");

        DateTime currentDate = gameCalendar.CurrentDate;
        DateTime date = query.Date ?? currentDate;
        GameDayStatus dayStatus = date switch
        {
            var value when value == currentDate => gameCalendar.DayStatus,
            var value when value < currentDate => GameDayStatus.Completed,
            var value when value > currentDate => GameDayStatus.NotStarted,
            _ => throw new InvalidOperationException("Invalid date comparison.")
        };

        IReadOnlyList<GetGameLeagueMatchDto> matches = await getGameLeagueMatchesByDateQueryHandler
            .HandleAsync(
                new GetGameLeagueMatchesByDateQuery(
                    UserId: query.UserId,
                    GameId: query.GameId,
                    Date: date),
                cancellationToken)
            .ConfigureAwait(false);

        return new GetCalendarDayQueryResult(
            Date: date,
            DayStatus: dayStatus.ToString(),
            LeagueMatches: matches.Select(m => new LeagueMatchDto(
                Id: m.Id,
                HomeTeamId: m.HomeTeamId,
                AwayTeamId: m.AwayTeamId,
                HomeTeamName: m.HomeTeamName,
                AwayTeamName: m.AwayTeamName,
                HomeTeamScore: m.HomeTeamScore,
                AwayTeamScore: m.AwayTeamScore,
                LeagueVersionId: m.LeagueVersionId,
                LeagueName: m.LeagueName,
                LeagueRound: m.LeagueRound,
                LeagueOrder: m.LeagueOrder,
                CountryId: m.CountryId,
                CountryName: m.CountryName)).ToList()
        );
    }
}
