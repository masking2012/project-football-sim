namespace ProjectFootballSim.Calendar.Contracts.Features.GetGameSeasons;

public interface IGetGameSeasonsQueryHandler
{
    Task<IReadOnlyList<GetGameSeasonDto>> HandleAsync(GetGameSeasonsQuery query, CancellationToken cancellationToken);
}
