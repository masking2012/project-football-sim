using ProjectFootballSim.Leagues.Application.Common.Models;
using ProjectFootballSim.Leagues.Application.Common.Services;

namespace ProjectFootballSim.Leagues.Application.Features.GetLeagueById;

public sealed class GetLeagueByIdQueryHandler(ILeaguesCatalog leagueCatalog)
{
    public ValueTask<LeagueDto?> HandleAsync(
        int leagueId,
        CancellationToken cancellationToken) =>
        leagueCatalog.GetByIdAsync(leagueId, cancellationToken);
}
