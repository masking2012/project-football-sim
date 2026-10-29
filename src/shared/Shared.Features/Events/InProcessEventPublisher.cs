using Microsoft.Extensions.DependencyInjection;

namespace ProjectFootballSim.Shared.Features.Events;

internal sealed class InProcessEventPublisher(IServiceProvider serviceProvider) : IEventPublisher
{
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(@event);

        foreach (IEventListener<TEvent> listener in serviceProvider.GetServices<IEventListener<TEvent>>())
        {
            await listener.HandleAsync(@event, cancellationToken).ConfigureAwait(false);
        }
    }
}
