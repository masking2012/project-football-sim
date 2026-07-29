using ProjectFootballSim.Match.Application.Common.Dtos;
using ProjectFootballSim.Match.Domain.ValueObjects;

namespace ProjectFootballSim.Match.Application.Common.Services;

internal static class TeamMapper
{
    public static Team Map(MatchTeamDto dto)
    {
        return new Team(dto.Id, dto.Attack, dto.Defence, dto.Midfield);
    }
}
