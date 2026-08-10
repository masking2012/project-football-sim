using Microsoft.Extensions.Logging;

namespace ProjectFootballSim.GamePersistence.Application;

internal static partial class Log
{
    [LoggerMessage(Level = LogLevel.Information, Message = "User {UserId} created a new game with ID {GameId}")]
    public static partial void NewGameCreated(ILogger logger, Guid userId, Guid gameId);
}
