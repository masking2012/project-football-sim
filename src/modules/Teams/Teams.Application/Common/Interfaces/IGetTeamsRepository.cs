using ProjectFootballSim.Teams.Application.Common.Models;

namespace ProjectFootballSim.Teams.Application.Common.Interfaces;

public interface IGetTeamsRepository
{
    Task<IReadOnlyList<TeamDto>> GetAllAsync(CancellationToken cancellationToken);
}
