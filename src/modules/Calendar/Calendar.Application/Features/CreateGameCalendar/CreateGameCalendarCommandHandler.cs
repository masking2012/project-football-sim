using Microsoft.Extensions.Logging;
using ProjectFootballSim.Calendar.Contracts.Features.CreateGameCalendar;
using ProjectFootballSim.Shared.Data.Entities.Seasons;

namespace ProjectFootballSim.Calendar.Application.Features.CreateGameCalendar;

internal sealed class CreateGameCalendarCommandHandler(
    ILogger<CreateGameCalendarCommandHandler> logger,
    ICreateGameCalendarRepository repository) : ICreateGameCalendarCommandHandler
{
    public async Task HandleAsync(
        CreateGameCalendarCommand command,
        CancellationToken cancellationToken)
    {
        var seasonDefinition = SeasonsDataProvider.GetSeasonDefinition();
        var initialDate = new DateTime(seasonDefinition.FirstSeasonYear, seasonDefinition.StartSeasonMonth, seasonDefinition.StartSeasonDay);

        try
        {
            await repository.CreateAndSaveAsync(command.UserId, command.GameId, initialDate, cancellationToken).ConfigureAwait(false);
            CreateGameCalendarLogs.Created(logger, command.UserId, command.GameId);
        }
        catch (GameCalendarExistsException ex)
        {
            CreateGameCalendarLogs.Exists(logger, command.UserId, command.GameId, ex.InnerException);
            throw;
        }
    }
}
