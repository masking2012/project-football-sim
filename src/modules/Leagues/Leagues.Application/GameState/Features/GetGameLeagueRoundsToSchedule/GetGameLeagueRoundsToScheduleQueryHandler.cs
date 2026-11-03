using ProjectFootballSim.Leagues.Application.Catalog.Interfaces;
using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Application.GameState.Interfaces;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueRoundsToSchedule;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueRoundsToSchedule;

internal sealed class GetGameLeagueRoundsToScheduleQueryHandler(
    IGetGameLeagueRoundsToScheduleRepository repository,
    IGameLeaguesService gameLeaguesService,
    ICachedLeaguesRetriever cachedLeaguesRetriever) : IGetGameLeagueRoundsToScheduleQueryHandler
{
    public async Task<IReadOnlyList<GameLeagueRoundDto>> HandleAsync(
        GetGameLeagueRoundsToScheduleQuery query,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<ProvisionalRoundDto> provisionalRounds = await repository
            .GetProvisionalRoundsAsync(query.UserId, query.GameId, cancellationToken)
            .ConfigureAwait(false);
        if (provisionalRounds.Count == 0)
            return [];

        var leagues = (await gameLeaguesService
            .GetGameLeaguesAsync(query.UserId, query.GameId, cancellationToken)
            .ConfigureAwait(false))
            .ToDictionary(league => league.LeagueVersionId);

        List<ProvisionalRoundDto> readyToBeScheduledRounds = provisionalRounds
            .Where(round =>
                round.BaseDate <= query.CurrentDate.AddDays(leagues[round.LeagueVersionId].MatchdaySchedulingLeadDays))
            .ToList();

        IReadOnlyDictionary<int, IReadOnlyList<LeagueRoundDto>> leagueRounds = await cachedLeaguesRetriever
            .GetLeagueRoundsAsync(readyToBeScheduledRounds.Select(round => round.LeagueVersionId).ToList(), cancellationToken)
            .ConfigureAwait(false);

        return provisionalRounds
            .Where(round =>
                round.BaseDate <= query.CurrentDate.AddDays(leagues[round.LeagueVersionId].MatchdaySchedulingLeadDays))
            .Select(round => new GameLeagueRoundDto(
                round.GameLeagueId,
                round.LeagueVersionId,
                round.Round,
                round.BaseDate,
                leagueRounds[round.LeagueVersionId].Single(lr => lr.Round == round.Round).AvailableDaysOfWeek))
            .ToList();
    }
}
