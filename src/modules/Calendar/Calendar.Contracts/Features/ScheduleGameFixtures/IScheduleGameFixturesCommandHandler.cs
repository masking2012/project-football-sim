namespace ProjectFootballSim.Calendar.Contracts.Features.ScheduleGameFixtures;

public interface IScheduleGameFixturesCommandHandler
{
    Task HandleAsync(
        ScheduleGameFixturesCommand command,
        CancellationToken cancellationToken);
}
