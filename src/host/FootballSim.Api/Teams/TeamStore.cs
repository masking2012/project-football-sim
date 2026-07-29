using ProjectFootballSim.Api.Match;
using ProjectFootballSim.Team.Application.Features;
using System.Security.Cryptography;
using MatchTeam = ProjectFootballSim.Match.Domain.ValueObjects.Team;

namespace ProjectFootballSim.Api.Teams;

internal sealed class TeamStore(GetTeamsByCountryFeature getTeamsByCountryFeature)
{
    private static readonly List<(TeamDto Dto, MatchTeam Domain)> _teams =
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

    public async Task<IReadOnlyList<TeamDto>> GetAllAsync(CancellationToken cancellationToken) {
        var predefined = _teams.Select(t => t.Dto).ToList();
        var teamsByCountry = await getTeamsByCountryFeature.HandleAsync(1, cancellationToken).ConfigureAwait(false);
        predefined.AddRange(teamsByCountry.Select(t => new TeamDto(t.Id, t.Name, t.Attack.Value, t.Defence.Value, t.Midfield.Value)));
        return predefined;
    } 

    public static (TeamDto Dto, MatchTeam Domain)? FindById(int id) =>
        _teams.FirstOrDefault(t => t.Dto.Id == id) is { Dto.Id: var tid } pair && tid == id
            ? pair
            : null;

    private static (TeamDto, MatchTeam) Create(string name, int attack, int defence, int midfield)
    {
        var id = RandomNumberGenerator.GetInt32(10, 1000);
        return (new TeamDto(id, name, attack, defence, midfield),
                new MatchTeam(id, attack, defence, midfield));
    }
}
