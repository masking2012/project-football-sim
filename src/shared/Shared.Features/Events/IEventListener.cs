namespace ProjectFootballSim.Shared.Features.Events;

public interface IEventListener<in TEvent>
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
}
