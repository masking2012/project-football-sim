using Microsoft.Extensions.Logging;

namespace ProjectFootballSim.Matches.Application.Features.RegularTime;

internal static partial class Log
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Regular time statistics. Home ({HomeTeamId}) possession:{HomePossession} chances:{HomeChances} - Away ({AwayTeamId}) possession:{AwayPossession} chances:{AwayChances}")]
    public static partial void RegularTimeStatistics(ILogger logger, int homeTeamId, double homePossession, int homeChances, int awayTeamId, double awayPossession, int awayChances);

    [LoggerMessage(Level = LogLevel.Information, Message = "Regular time result: Home ({HomeTeamId}) {HomeScore} - Away ({AwayTeamId}) {AwayScore}")]
    public static partial void RegularTimeResult(ILogger logger, int homeTeamId, int homeScore, int awayTeamId, int awayScore);
}
