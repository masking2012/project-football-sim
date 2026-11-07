using ProjectFootballSim.Calendar.Application.Common.Interfaces;
using ProjectFootballSim.Calendar.Application.Common.Models;
using ProjectFootballSim.Calendar.Contracts.Features.GetCalendarDay;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Domain.Enums;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueMatchesByIds;

namespace ProjectFootballSim.Calendar.Application.Features.GetCalendarDay;

internal sealed class GetCalendarDayQueryHandler(
    IGameCalendarRepository gameCalendarRepository,
    IGetCalendarDayFixturesRepository gameFixturesRepository,
    IGetGameLeagueMatchesByIdsQueryHandler getGameLeagueMatchesByIdsQueryHandler)
    : IGetCalendarDayQueryHandler
{
    public async Task<GetCalendarDayQueryResult> HandleAsync(GetCalendarDayQuery query, CancellationToken cancellationToken)
    {
        GameCalendarDto gameCalendar = await GetGameCalendarAsync(query, cancellationToken).ConfigureAwait(false);

        DateTime date = query.Date ?? gameCalendar.CurrentDate;
        GameDayStatus dayStatus = GetDayStatus(gameCalendar, date);

        IReadOnlyList<GameFixture> fixtures = await GetFixturesAsync(query, date, cancellationToken).ConfigureAwait(false);
        if (fixtures.Count == 0)
            return CreateResult(date, dayStatus, []);

        List<Guid> leagueMatchIds = GetLeagueMatchIds(fixtures);
        if (leagueMatchIds.Count == 0)
            return CreateResult(date, dayStatus, []);

        IReadOnlyList<GetGameLeagueMatchDto> matches = await GetLeagueMatchesAsync(query, leagueMatchIds, cancellationToken)
            .ConfigureAwait(false);

        return CreateResult(date, dayStatus, matches);
    }

    private async Task<GameCalendarDto> GetGameCalendarAsync(GetCalendarDayQuery query, CancellationToken cancellationToken)
    {
        GameCalendarDto? gameCalendar = await gameCalendarRepository
            .GetCurrentCalendarAsync(query.UserId, query.GameId, cancellationToken)
            .ConfigureAwait(false);

        return gameCalendar ?? throw new InvalidOperationException(
            $"Game calendar not found for user ID '{query.UserId}' and game ID '{query.GameId}'.");
    }

    private static GameDayStatus GetDayStatus(GameCalendarDto gameCalendar, DateTime date)
    {
        DateTime currentDate = gameCalendar.CurrentDate;

        return date switch
        {
            var value when value == currentDate => gameCalendar.DayStatus,
            var value when value < currentDate => GameDayStatus.Completed,
            var value when value > currentDate => GameDayStatus.NotStarted,
            _ => throw new InvalidOperationException("Invalid date comparison.")
        };
    }

    private async Task<IReadOnlyList<GameFixture>> GetFixturesAsync(
        GetCalendarDayQuery query,
        DateTime date,
        CancellationToken cancellationToken)
    {
        DateTime startDate = date.Date;

        return await gameFixturesRepository
            .GetFixturesAsync(query.UserId, query.GameId, startDate, startDate.AddDays(1), cancellationToken)
            .ConfigureAwait(false);
    }

    private static List<Guid> GetLeagueMatchIds(IReadOnlyList<GameFixture> fixtures) =>
        fixtures
            .Where(fixture => fixture.CompetitionType == CompetitionType.League)
            .Select(fixture => fixture.MatchId)
            .ToList();

    private async Task<IReadOnlyList<GetGameLeagueMatchDto>> GetLeagueMatchesAsync(
        GetCalendarDayQuery query,
        List<Guid> matchIds,
        CancellationToken cancellationToken) =>
        await getGameLeagueMatchesByIdsQueryHandler
            .HandleAsync(
                new GetGameLeagueMatchesByIdsQuery(
                    UserId: query.UserId,
                    GameId: query.GameId,
                    Ids: matchIds),
                cancellationToken)
            .ConfigureAwait(false);

    private static GetCalendarDayQueryResult CreateResult(
        DateTime date,
        GameDayStatus dayStatus,
        IReadOnlyList<GetGameLeagueMatchDto> matches) =>
        new(
            Date: date,
            DayStatus: dayStatus.ToString(),
            LeagueMatches: matches.Select(MapLeagueMatch).ToList());

    private static LeagueMatchDto MapLeagueMatch(GetGameLeagueMatchDto match) =>
        new(
            Id: match.Id,
            HomeTeamId: match.HomeTeamId,
            AwayTeamId: match.AwayTeamId,
            HomeTeamName: match.HomeTeamName,
            AwayTeamName: match.AwayTeamName,
            HomeTeamScore: match.HomeTeamScore,
            AwayTeamScore: match.AwayTeamScore,
            LeagueVersionId: match.LeagueVersionId,
            LeagueName: match.LeagueName,
            LeagueRound: match.LeagueRound,
            LeagueOrder: match.LeagueOrder,
            CountryId: match.CountryId,
            CountryName: match.CountryName);
}
