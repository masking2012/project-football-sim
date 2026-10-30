using ProjectFootballSim.Calendar.Domain.Entities;

namespace ProjectFootballSim.Calendar.Application.Common.Interfaces;

public interface IGameCalendarWriteRepository
{
    Task<GameCalendar?> GetCurrentCalendarAsync(Guid userId, Guid gameId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
