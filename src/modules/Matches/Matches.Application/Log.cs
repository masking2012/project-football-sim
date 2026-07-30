using Microsoft.Extensions.Logging;

namespace ProjectFootballSim.Matches.Application;

internal static partial class Log
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Penalty shootout result: Home ({HomeTeamId}) {HomeScore} - Away ({AwayTeamId}) {AwayScore}")]
    public static partial void PenaltyShootoutResult(ILogger logger, int homeTeamId, int homeScore, int awayTeamId, int awayScore);
}
