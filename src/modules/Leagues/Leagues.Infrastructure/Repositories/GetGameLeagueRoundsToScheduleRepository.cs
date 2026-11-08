using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueRoundsToSchedule;
using ProjectFootballSim.Leagues.Domain.GameState.Enums;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Infrastructure.Repositories;

internal sealed class GetGameLeagueRoundsToScheduleRepository(LeaguesDbContext dbContext)
    : IGetGameLeagueRoundsToScheduleRepository
{
    public async Task<IReadOnlyList<ProvisionalRoundDto>> GetProvisionalRoundsAsync(Guid userId, Guid gameId, CancellationToken cancellationToken)
    {
        return await dbContext.GameLeagueRounds
            .AsNoTracking()
            .Where(round => round.RoundStatus == GameLeagueRoundStatus.Provisional)
            .Join(
                dbContext.GameLeagues.AsNoTracking()
                    .Where(gameLeague => gameLeague.UserId == userId && gameLeague.GameId == gameId),
                round => round.GameLeagueId,
                gameLeague => gameLeague.Id,
                (round, gameLeague) => new ProvisionalRoundDto(
                    round.GameLeagueId,
                    gameLeague.LeagueVersionId,
                    round.Round,
                    round.BaseDate))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
