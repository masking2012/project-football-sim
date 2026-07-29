using ProjectFootballSim.Api.Match;
using ProjectFootballSim.Team.Application.Features;
using System.Globalization;

namespace ProjectFootballSim.Api.Teams;

internal sealed class TeamStore(GetTeamsByCountryFeature getTeamsByCountryFeature)
{
    private static IReadOnlyList<TeamDto>? _teams; 

    public async Task<IReadOnlyList<TeamDto>> GetAllAsync(CancellationToken cancellationToken) {
        if (_teams is not null)
            return _teams;

        var teamsByCountry = await getTeamsByCountryFeature.HandleAsync(1, cancellationToken).ConfigureAwait(false);
        var predefined = teamsByCountry.Select(t => new TeamDto(t.Id.ToString(CultureInfo.InvariantCulture), t.Name, t.Attack.Value, t.Defence.Value, t.Midfield.Value)).ToList();
        _teams = predefined;
        return _teams;
    }

    public static TeamDto? FindById(string id) => _teams?.FirstOrDefault(t => t.Id == id);
}
