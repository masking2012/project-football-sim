using ProjectFootballSim.Calendar.Application.Common.Models;

namespace ProjectFootballSim.Calendar.Application.Common.Interfaces;

public interface IGameCalendarRepository
{
    Task<GameCalendarDto?> GetCurrentCalendarAsync(Guid userId, Guid gameId, CancellationToken cancellationToken);
    Task<GameSeasonDto?> GetCurrentSeasonAsync(Guid userId, Guid gameId, CancellationToken cancellationToken);
}
