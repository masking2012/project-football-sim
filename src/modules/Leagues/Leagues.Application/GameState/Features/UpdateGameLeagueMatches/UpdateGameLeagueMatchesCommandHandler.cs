using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.UpdateGameLeagueMatches;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.UpdateGameLeagueMatches;

internal sealed class UpdateGameLeagueMatchesCommandHandler(IUpdateGameLeagueMatchesRepository repository)
    : IUpdateGameLeagueMatchesCommandHandler
{
    public async Task HandleAsync(UpdateGameLeagueMatchesCommand command, CancellationToken cancellationToken)
    {
        var matchIds = command.Matches
            .Select(match => match.MatchId)
            .ToList();

        var matches = await repository
            .GetMatchesAsync(command.UserId, command.GameId, matchIds, cancellationToken)
            .ConfigureAwait(false);

        var missingIds = matchIds.Except(matches.Select(match => match.Id)).ToList();
        if (missingIds.Count > 0)
            throw new KeyNotFoundException($"League matches were not found: {string.Join(", ", missingIds)}.");

        var datesByMatchId = command.Matches.ToDictionary(match => match.MatchId, match => match.Date);

        foreach (var match in matches)
            match.UpdateDate(datesByMatchId[match.Id]);

        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
