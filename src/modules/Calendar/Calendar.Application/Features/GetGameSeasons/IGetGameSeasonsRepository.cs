namespace ProjectFootballSim.Calendar.Application.Features.GetGameSeasons;

public interface IGetGameSeasonsRepository
{
    Task<IReadOnlyList<GameSeasonDto>> GetAsync(Guid userId, Guid gameId, CancellationToken cancellationToken);
}
