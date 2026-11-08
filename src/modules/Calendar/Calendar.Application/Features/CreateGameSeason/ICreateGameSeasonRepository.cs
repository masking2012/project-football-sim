using ProjectFootballSim.Calendar.Domain.Entities;

namespace ProjectFootballSim.Calendar.Application.Features.CreateGameSeason;

public interface ICreateGameSeasonRepository
{
    Task<GameSeason?> GetCurrentSeasonAsync(Guid userId, Guid gameId, CancellationToken cancellationToken);

    void Add(GameSeason gameSeason);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
