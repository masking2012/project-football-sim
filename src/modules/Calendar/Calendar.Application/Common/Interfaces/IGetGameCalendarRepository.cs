using ProjectFootballSim.Calendar.Application.Common.Models;

namespace ProjectFootballSim.Calendar.Application.Common.Interfaces;

public interface IGetGameCalendarRepository
{
    Task<GameCalendarDto?> GetAsync(Guid userId, Guid gameId, CancellationToken cancellationToken);
}
