using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Shared.Data.Entities.Leagues;
using ProjectFootballSim.Shared.Data.Entities.Locations;
using ProjectFootballSim.Leagues.Domain.Catalog.Entities;
using ProjectFootballSim.Leagues.Domain.Catalog.ValueObjects;
using ProjectFootballSim.Leagues.Infrastructure.Database;
using DataLeagueFormatType = ProjectFootballSim.Shared.Data.Entities.Leagues.LeagueFormatType;
using DomainLeagueFormatType = ProjectFootballSim.Leagues.Domain.Catalog.Enums.LeagueStageType;

namespace ProjectFootballSim.Leagues.Infrastructure;

public static class DependencyRegistrator
{
    public static IServiceCollection AddLeaguesInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringSectionName)
    {
        services.AddDbContext<LeaguesDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString(connectionStringSectionName),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null))
            .UseSeeding(SeedWithPredefinedValues));

        return services;
    }

    private static void SeedWithPredefinedValues(DbContext context, bool storeManagementOpetationWasPerformed)
    {
        var countries = LocationsDataProvider.GetCountries()
            .Where(c => c.Id == 1); //TODO: temporary filter for testing purposes, remove this line when ready to seed all countries

        foreach (var countryData in countries)
        {
            foreach (var leagueData in LeaguesDataProvider.GetLeaguesByCountryId(countryData.Id))
            {
                var league = context.Set<League>().SingleOrDefault(c => c.Id == leagueData.Id && c.Version == leagueData.Version);
                if (league is null)
                    context.Set<League>().Add(
                        new League(
                            id: leagueData.Id,
                            version: leagueData.Version,
                            name: leagueData.Name,
                            order: leagueData.Order,
                            countryId: leagueData.CountryId,
                            teamsCount: leagueData.TeamsCount,
                            format: MapFormat(leagueData.Format),
                            promotionPositions: leagueData.PromotionPositions,
                            promotionPlayOffPositions: leagueData.PromotionPlayOffPositions,
                            relegationPositions: leagueData.RelegationPositions,
                            relegationPlayOffPositions: leagueData.RelegationPlayOffPositions,
                            uefaChampionsLeaguePositions: leagueData.UefaChampionsLeaguePositions,
                            uefaEuropaLeaguePositions: leagueData.UefaEuropaLeaguePositions,
                            uefaConferenceLeaguePositions: leagueData.UefaConferenceLeaguePositions));
            }

            foreach (var leagueTeamData in LeaguesDataProvider.GetLeagueTeamsByCountryId(countryData.Id))
            {
                var leagueTeam = context.Set<LeagueTeam>()
                    .SingleOrDefault(x => x.LeagueId == leagueTeamData.LeagueId && x.Version == leagueTeamData.Version && x.TeamId == leagueTeamData.TeamId);
                if (leagueTeam is null)
                    context.Set<LeagueTeam>().Add(
                        new LeagueTeam(
                            LeagueId: leagueTeamData.LeagueId,
                            Version: leagueTeamData.Version,
                            TeamId: leagueTeamData.TeamId));
            }

            foreach (var leagueRoundData in LeaguesDataProvider.GetLeagueRoundsByCountryId(countryData.Id))
            {
                var leagueRound = context.Set<LeagueRound>()
                    .SingleOrDefault(x => x.LeagueId == leagueRoundData.LeagueId && x.Version == leagueRoundData.Version && x.Round == leagueRoundData.Round);
                if (leagueRound is null)
                    context.Set<LeagueRound>()
                        .Add(new LeagueRound(leagueId: leagueRoundData.LeagueId, version: leagueRoundData.Version, round: leagueRoundData.Round, week: leagueRoundData.Week, isMidweek: leagueRoundData.IsMidweek));
            }

            foreach (var activationData in LeaguesDataProvider.GetLeagueActivationsByCountryId(countryData.Id))
            {
                var activation = context.Set<LeagueActivation>()
                    .SingleOrDefault(x => x.LeagueId == activationData.LeagueId);
                if (activation is null)
                    context.Set<LeagueActivation>().Add(
                        new LeagueActivation(activationData.LeagueId, activationData.CurrentVersion));
                else
                    activation.ChangeVersion(activationData.CurrentVersion);
            }
        }

        context.SaveChanges();
    }

    private static List<LeagueStage> MapFormat(IReadOnlyList<LeagueFormatData> format) =>
        format
            .Select(stage => new LeagueStage(
                order: stage.Order,
                type: stage.Type switch
                {
                    DataLeagueFormatType.RoundRobin => DomainLeagueFormatType.RoundRobin,
                    _ => throw new InvalidDataException($"Unsupported league format type '{stage.Type}'.")
                },
                legs: stage.Legs))
            .ToList();
}
