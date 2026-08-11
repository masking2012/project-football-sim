using Microsoft.Extensions.Logging;

namespace ProjectFootballSim.Seasons.Application;

internal static partial class Log
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Game season with Id: {GameSeasonId} was created for UserId: {UserId}, GameId: {GameId}")]
    public static partial void GameSeasonCreated(ILogger logger, Guid gameSeasonId, Guid userId, Guid gameId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Game season already exists for UserId: {UserId}, GameId: {GameId}, Order: {Order}. Exception: {Exception}")]
    public static partial void GameSeasonExists(ILogger logger, Guid userId, Guid gameId, int order, Exception exception);
}
