using ProjectFootballSim.Calendar.Contracts.Features.GetGameSeasons;

namespace ProjectFootballSim.Calendar.Application.Features.GetGameSeasons;

internal sealed class GetGameSeasonsQueryHandler(IGetGameSeasonsRepository repository) : IGetGameSeasonsQueryHandler
{
    public async Task<IReadOnlyList<GetGameSeasonDto>> HandleAsync(
        GetGameSeasonsQuery query,
        CancellationToken cancellationToken)
    {
        var gameSeasons = await repository.GetAsync(query.UserId, query.GameId, cancellationToken).ConfigureAwait(false);
        if (gameSeasons.Count == 0)
            throw new InvalidOperationException($"No game seasons found for UserId: {query.UserId}, GameId: {query.GameId}");

        return gameSeasons
            .Select(s => new GetGameSeasonDto(
                Id: s.Id,
                StartDate: s.StartDate,
                EndDate: s.EndDate,
                Order: s.Order,
                IsCurrent: s.IsCurrent))
            .ToList();
    }
}
