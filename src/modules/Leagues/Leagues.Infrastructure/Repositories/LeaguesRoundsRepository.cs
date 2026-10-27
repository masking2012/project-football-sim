using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Application.Catalog.Services;
using ProjectFootballSim.Leagues.Infrastructure.Database;
using LeagueRoundDomain = ProjectFootballSim.Leagues.Domain.Catalog.ValueObjects.LeagueRound;

namespace ProjectFootballSim.Leagues.Infrastructure.Repositories;

internal sealed class LeaguesRoundsRepository(LeaguesDbContext dbContext) : ILeaguesRoundsRepository
{
    public async Task<IReadOnlyDictionary<int, IReadOnlyList<LeagueRoundDto>>> GetActiveAsync(
        CancellationToken cancellationToken)
    {
        var activeRounds = await dbContext.LeagueRounds
            .AsNoTracking()
            .Join(
                dbContext.LeagueActivations.AsNoTracking(),
                round => round.LeagueVersionId,
                activation => activation.LeagueVersionId,
                (round, activation) => round)
            .Join(
                dbContext.LeagueVersions.AsNoTracking(),
                round => round.LeagueVersionId,
                version => version.LeagueVersionId,
                (round, version) => new { round, version })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return activeRounds
            .GroupBy(item => item.round.LeagueVersionId)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<LeagueRoundDto>)group
                    .Select(item => CreateRoundDto(item.version.LeagueId, item.round))
                    .ToList());
    }

    public async Task<IReadOnlyList<LeagueRoundDto>> GetForVersionAsync(
        int leagueVersionId,
        CancellationToken cancellationToken)
    {
        var rounds = await dbContext.LeagueRounds
            .AsNoTracking()
            .Where(round => round.LeagueVersionId == leagueVersionId)
            .Join(
                dbContext.LeagueVersions.AsNoTracking(),
                round => round.LeagueVersionId,
                version => version.LeagueVersionId,
                (round, version) => new { round, version })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return rounds
            .Select(item => CreateRoundDto(item.version.LeagueId, item.round))
            .ToList();
    }

    private static LeagueRoundDto CreateRoundDto(int leagueId, LeagueRoundDomain round) =>
        new(leagueId, round.LeagueVersionId, round.Round, round.Week, round.IsMidweek);
}
