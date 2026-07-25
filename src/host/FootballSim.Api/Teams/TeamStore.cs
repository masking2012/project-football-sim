using ProjectFootballSim.Api.Match;
using ProjectFootballSim.Match.Domain.ValueObjects;

namespace ProjectFootballSim.Api.Teams;

internal static class TeamStore
{
    private static readonly List<(TeamDto Dto, Team Domain)> _teams =
    [
        Create("Manchester City",   88, 82, 85),
        Create("Real Madrid",       86, 80, 84),
        Create("Bayern Munich",     87, 81, 83),
        Create("Arsenal",           82, 79, 80),
        Create("Barcelona",         85, 75, 86),
        Create("Liverpool",         84, 78, 82),
        Create("PSG",               89, 74, 80),
        Create("Inter Milan",       80, 83, 78),
        Create("Atletico Madrid",   76, 88, 79),
        Create("Borussia Dortmund", 83, 74, 77),
    ];

    public static IReadOnlyList<TeamDto> GetAll() => _teams.Select(t => t.Dto).ToList();

    public static (TeamDto Dto, Team Domain)? FindById(Guid id) =>
        _teams.FirstOrDefault(t => t.Dto.Id == id) is { Dto.Id: var tid } pair && tid == id
            ? pair
            : null;

    private static (TeamDto, Team) Create(string name, int attack, int defence, int midfield)
    {
        var id = Guid.NewGuid();
        return (new TeamDto(id, name, attack, defence, midfield),
                new Team(id, attack, defence, midfield));
    }
}
