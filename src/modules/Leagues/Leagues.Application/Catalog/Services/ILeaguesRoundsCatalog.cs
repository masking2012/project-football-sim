using ProjectFootballSim.Leagues.Application.Catalog.Models;

namespace ProjectFootballSim.Leagues.Application.Catalog.Services;

public interface ILeaguesRoundsCatalog
{
    ValueTask<IReadOnlyList<LeagueRoundDto>> GetRoundsForLeagueVersionAsync(
        int leagueVersionId,
        CancellationToken cancellationToken);
}
