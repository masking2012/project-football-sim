namespace ProjectFootballSim.Calendar.Contracts.Features.CompleteGameSeason;

public interface ICompleteGameSeasonCommandHandler
{
    Task HandleAsync(CompleteGameSeasonCommand command, CancellationToken cancellationToken);
}
