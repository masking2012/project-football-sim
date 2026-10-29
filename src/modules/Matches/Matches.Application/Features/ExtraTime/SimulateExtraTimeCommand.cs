using ProjectFootballSim.Matches.Application.Common.Models;

namespace ProjectFootballSim.Matches.Application.Features.ExtraTime;

public sealed record SimulateExtraTimeCommand
    (MatchTeamDto HomeDto, MatchTeamDto AwayDto, MatchSettingsDto MatchSettings);
