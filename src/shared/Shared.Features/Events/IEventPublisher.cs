namespace ProjectFootballSim.Shared.Features.Events;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken);
}
