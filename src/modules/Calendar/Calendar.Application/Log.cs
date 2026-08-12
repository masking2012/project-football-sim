using Microsoft.Extensions.Logging;

namespace ProjectFootballSim.Calendar.Application;

internal static partial class Log
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Game calendar was created for UserId: {UserId}, GameId: {GameId}")]
    public static partial void GameCalendarCreated(ILogger logger, Guid userId, Guid gameId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Game calendar already exists for UserId: {UserId}, GameId: {GameId}. Exception: {Exception}")]
    public static partial void GameCalendarExists(ILogger logger, Guid userId, Guid gameId, Exception exception);
}

