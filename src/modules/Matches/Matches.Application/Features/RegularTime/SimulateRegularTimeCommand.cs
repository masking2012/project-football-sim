using ProjectFootballSim.Matches.Application.Common.Models;

namespace ProjectFootballSim.Matches.Application.Features.RegularTime;

public sealed record SimulateRegularTimeCommand(
    MatchTeamDto Home, MatchTeamDto Away, MatchSettingsDto MatchSettings);
