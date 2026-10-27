using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Application.Catalog.Interfaces;
using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Infrastructure.Database;
using LeagueDomain = ProjectFootballSim.Leagues.Domain.Catalog.ValueObjects.League;
using LeagueVersionDomain = ProjectFootballSim.Leagues.Domain.Catalog.Entities.LeagueVersion;

namespace ProjectFootballSim.Leagues.Infrastructure.Repositories;

internal sealed class LeaguesRepository(LeaguesDbContext dbContext) : ILeaguesRepository
{
    public async Task<IReadOnlyDictionary<int, LeagueDto>> GetAllActiveAsync(
        CancellationToken cancellationToken)
    {
        var activeLeagues = await dbContext.Leagues
            .AsNoTracking()
            .Join(
                dbContext.LeagueActivations.AsNoTracking(),
                league => league.Id,
                activation => activation.LeagueId,
                (league, activation) => new { league, activation })
            .Join(
                dbContext.LeagueVersions.AsNoTracking(),
                item => item.activation.LeagueVersionId,
                version => version.LeagueVersionId,
                (item, version) => new { item.league, item.activation, version })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return activeLeagues
            .Select(item => CreateLeagueDto(item.league, item.version, item.activation.LeagueVersionId))
            .ToDictionary(league => league.LeagueVersionId);
    }

    public async Task<LeagueDto?> GetAsync(
        int leagueVersionId,
        CancellationToken cancellationToken)
    {
        var version = await dbContext.LeagueVersions
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.LeagueVersionId == leagueVersionId, cancellationToken)
            .ConfigureAwait(false);
        if (version is null)
            return null;

        var league = await dbContext.Leagues
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == version.LeagueId, cancellationToken)
            .ConfigureAwait(false);
        if (league is null)
            return null;

        var activeLeagueVersionId = await dbContext.LeagueActivations
            .AsNoTracking()
            .Where(item => item.LeagueId == version.LeagueId)
            .Select(item => (int?)item.LeagueVersionId)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        return CreateLeagueDto(league, version, activeLeagueVersionId);
    }

    private static LeagueDto CreateLeagueDto(
        LeagueDomain league,
        LeagueVersionDomain version,
        int? activeLeagueVersionId) =>
        new(
            league.Id,
            version.LeagueVersionId,
            version.Version,
            IsActive: activeLeagueVersionId == version.LeagueVersionId,
            league.Name,
            league.Order,
            league.CountryId,
            version.TeamsCount,
            version.Format
                .OrderBy(stage => stage.Order)
                .Select(stage => new LeagueFormatDto(stage.Order, (LeagueStageTypeDto)stage.Type, stage.Legs))
                .ToList(),
            version.PromotionPositions,
            version.PromotionPlayOffPositions,
            version.RelegationPositions,
            version.RelegationPlayOffPositions,
            version.UefaChampionsLeaguePositions,
            version.UefaEuropaLeaguePositions,
            version.UefaConferenceLeaguePositions);
}
