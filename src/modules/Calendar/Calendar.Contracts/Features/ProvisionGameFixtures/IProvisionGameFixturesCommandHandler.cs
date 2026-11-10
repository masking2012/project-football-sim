namespace ProjectFootballSim.Calendar.Contracts.Features.ProvisionGameFixtures;

public interface IProvisionGameFixturesCommandHandler
{
    Task HandleAsync(
        ProvisionGameFixturesCommand command,
        CancellationToken cancellationToken);
}
