using ProjectFootballSim.Calendar.Contracts.Features.CreateGameCalendar;
using ProjectFootballSim.Calendar.Contracts.Features.CreateGameSeason;
using ProjectFootballSim.GamePersistence.Contracts.Features.CreateGame;
using ProjectFootballSim.Shared.Features.Events;

namespace ProjectFootballSim.Calendar.Application.EventListeners;

internal sealed class GameCreatedEventListener
    (ICreateGameCalendarCommandHandler createGameCalendarCommandHandler,
    ICreateGameSeasonCommandHandler createGameSeasonCommandHandler) : IEventListener<GameCreatedEvent>
{
    public async Task HandleAsync(GameCreatedEvent @event, CancellationToken cancellationToken)
    {
        var createGameCalendarCommand = new CreateGameCalendarCommand(@event.UserId, @event.GameId);
        await createGameCalendarCommandHandler.HandleAsync(createGameCalendarCommand, cancellationToken).ConfigureAwait(false);

        var createGameSeasonCommand = new CreateGameSeasonCommand(@event.UserId, @event.GameId);
        await createGameSeasonCommandHandler.HandleAsync(createGameSeasonCommand, cancellationToken).ConfigureAwait(false);
    }
}
