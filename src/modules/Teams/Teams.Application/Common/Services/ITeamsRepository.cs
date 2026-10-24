using ProjectFootballSim.Teams.Domain.Entities;

namespace ProjectFootballSim.Teams.Application.Common.Services;

public interface ITeamsRepository
{
    Task<IReadOnlyList<Team>> GetAllAsync(CancellationToken cancellationToken);
}
