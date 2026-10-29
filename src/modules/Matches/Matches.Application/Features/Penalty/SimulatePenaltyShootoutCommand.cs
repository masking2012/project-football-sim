using ProjectFootballSim.Matches.Application.Common.Models;

namespace ProjectFootballSim.Matches.Application.Features.Penalty;

public sealed record SimulatePenaltyShootoutCommand
    (MatchTeamDto HomeDto, MatchTeamDto AwayDto);
