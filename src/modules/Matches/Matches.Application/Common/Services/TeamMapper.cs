using ProjectFootballSim.Matches.Application.Common.Models;
using ProjectFootballSim.Matches.Domain.ValueObjects;

namespace ProjectFootballSim.Matches.Application.Common.Services;

internal static class TeamMapper
{
    public static MatchTeam Map(MatchTeamDto dto)
    {
        return new MatchTeam(dto.Id, dto.Attack, dto.Defence, dto.Midfield);
    }
}
