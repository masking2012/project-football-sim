using Microsoft.Extensions.Logging;

namespace ProjectFootballSim.Matches.Application.Features.ExtraTime;

internal static partial class Log
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Extra time statistics. Home ({HomeTeamId}) possession:{HomePossession} chances:{HomeChances} - Away ({AwayTeamId}) possession:{AwayPossession} chances:{AwayChances}")]
    public static partial void ExtraTimeStatistics(ILogger logger, int homeTeamId, double homePossession, int homeChances, int awayTeamId, double awayPossession, int awayChances);

    [LoggerMessage(Level = LogLevel.Information, Message = "Extra time result: Home ({HomeTeamId}) {HomeScore} - Away ({AwayTeamId}) {AwayScore}")]
    public static partial void ExtraTimeResult(ILogger logger, int homeTeamId, int homeScore, int awayTeamId, int awayScore);
}
