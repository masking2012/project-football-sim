namespace ProjectFootballSim.Teams.Contracts.Features.GetTeamsByIds;

public interface IGetTeamsByIdsQueryHandler
{
    Task<IReadOnlyDictionary<int, TeamByIdsDto>> HandleAsync(IReadOnlyList<int> teamIds, CancellationToken cancellationToken);
}
