using ProjectFootballSim.Api.Match;
using ProjectFootballSim.Team.Application.Features;

namespace ProjectFootballSim.Api.Teams;

internal sealed class TeamStore(GetTeamsByCountryFeature getTeamsByCountryFeature)
{
    private static IReadOnlyList<TeamDto>? _teams; 

    public async Task<IReadOnlyList<TeamDto>> GetAllAsync(CancellationToken cancellationToken) {
        var teamsByCountry = await getTeamsByCountryFeature.HandleAsync(1, cancellationToken).ConfigureAwait(false);
        var predefined = teamsByCountry.Select(t => new TeamDto(t.Id, t.Name, t.Attack.Value, t.Defence.Value, t.Midfield.Value)).ToList();
        _teams = predefined;
        return _teams;
    } 

    public static TeamDto? FindById(int id) => _teams?.FirstOrDefault(t => t.Id == id);
}
