using Microsoft.Extensions.Logging;

namespace ProjectFootballSim.Calendar.Application.Features.CreateGameCalendar;

internal static partial class CreateGameCalendarLogs
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Game calendar was created for UserId: {UserId}, GameId: {GameId}")]
    public static partial void Created(ILogger logger, Guid userId, Guid gameId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Game calendar already exists for UserId: {UserId}, GameId: {GameId}. Exception: {Exception}")]
    public static partial void Exists(ILogger logger, Guid userId, Guid gameId, Exception? exception);
}
