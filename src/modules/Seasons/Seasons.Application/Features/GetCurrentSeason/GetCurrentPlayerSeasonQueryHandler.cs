using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Seasons.Infrastructure.Database;

namespace ProjectFootballSim.Seasons.Application.Features.GetCurrentSeason;

public sealed class GetCurrentPlayerSeasonQueryHandler(SeasonsDbContext dbContext)
{
    public async Task<CurrentSeasonDto?> HandleAsync(Guid userId, CancellationToken cancellationToken)
    {
        var currentSeason = await dbContext.PlayerSeasons
           .FirstOrDefaultAsync(s => s.UserId == userId && s.IsCurrent, cancellationToken)
           .ConfigureAwait(false);
        if (currentSeason is null)
            return null;

        return new CurrentSeasonDto(
            Id: currentSeason.Id,
            StartDate: currentSeason.StartDate,
            EndDate: currentSeason.EndDate);
    }
}
