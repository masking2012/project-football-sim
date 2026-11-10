using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.UpdateGameLeagueRoundStatus;
using ProjectFootballSim.Leagues.Domain.GameState.Enums;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.UpdateGameLeagueRoundStatus;

internal sealed class UpdateGameLeagueRoundStatusCommandHandler(IUpdateGameLeagueRoundStatusRepository repository)
    : IUpdateGameLeagueRoundStatusCommandHandler
{
    public async Task HandleAsync(UpdateGameLeagueRoundStatusCommand command, CancellationToken cancellationToken)
    {
        var requestedRounds = command.Rounds
            .Select(reference => (reference.GameLeagueId, reference.Round))
            .ToList();

        var rounds = await repository
            .GetRoundsAsync(command.UserId, command.GameId, requestedRounds, cancellationToken)
            .ConfigureAwait(false);

        var status = MapStatus(command.Status);

        foreach (var (gameLeagueId, roundNumber) in requestedRounds)
        {
            var round = rounds.SingleOrDefault(r => r.GameLeagueId == gameLeagueId && r.Round == roundNumber)
                ?? throw new KeyNotFoundException(
                    $"Round {roundNumber} of game league '{gameLeagueId}' was not found.");

            round.UpdateStatus(status);
        }

        await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private static GameLeagueRoundStatus MapStatus(GameLeagueRoundStatusContract status) => status switch
    {
        GameLeagueRoundStatusContract.Provisional => GameLeagueRoundStatus.Provisional,
        GameLeagueRoundStatusContract.Scheduled => GameLeagueRoundStatus.Scheduled,
        GameLeagueRoundStatusContract.InProgress => GameLeagueRoundStatus.InProgress,
        GameLeagueRoundStatusContract.Completed => GameLeagueRoundStatus.Completed,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unsupported round status.")
    };
}
