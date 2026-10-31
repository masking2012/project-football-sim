using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Leagues.Application.Catalog.Interfaces;
using ProjectFootballSim.Leagues.Application.GameState.Features.CheckGameLeagueMatchesByDate;
using ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueFixtures;
using ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueMatchesByDate;
using ProjectFootballSim.Leagues.Application.GameState.Features.GetGameLeagueStandings;
using ProjectFootballSim.Leagues.Application.GameState.Features.SimulateLeagueGameDay;
using ProjectFootballSim.Leagues.Application.GameState.Interfaces;
using ProjectFootballSim.Leagues.Application.GameState.Services;
using ProjectFootballSim.Leagues.Domain.Catalog.Entities;
using ProjectFootballSim.Leagues.Domain.Catalog.ValueObjects;
using ProjectFootballSim.Leagues.Infrastructure.Database;
using ProjectFootballSim.Leagues.Infrastructure.Repositories;
using ProjectFootballSim.Shared.Data.Entities.Leagues;
using ProjectFootballSim.Shared.Data.Entities.Locations;
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

        services.AddScoped<ILeaguesRepository, LeaguesRepository>();
        services.AddScoped<ILeaguesRoundsRepository, LeaguesRoundsRepository>();
        services.AddScoped<ILeaguesTeamsRepository, LeaguesTeamsRepository>();
        services.AddScoped<IGameLeaguesRepository, GameLeagueRepository>();
        services.AddScoped<IGameLeagueWriteRepository, GameLeagueWriteRepository>();

        services.AddScoped<IGetGameLeagueStandingsRepository, GetGameLeagueStandingsRepository>();
        services.AddScoped<IGetGameLeagueMatchesRepository, GetGameLeagueMatchesRepository>();
        services.AddScoped<IGetGameLeagueMatchesByDateRepository, GetGameLeagueMatchesByDateRepository>();
        services.AddScoped<ICheckGameLeagueMatchesRepository, CheckGameLeagueMatchesRepository>();
        services.AddScoped<ISimulateLeagueDayRepository, SimulateLeagueDayRepository>();

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
                var league = context.Set<League>().SingleOrDefault(c => c.Id == leagueData.Id);
                if (league is null)
                    context.Set<League>().Add(
                        new League(
                            id: leagueData.Id,
                            name: leagueData.Name,
                            order: leagueData.Order,
                            countryId: leagueData.CountryId));
            }

            foreach (var versionData in LeaguesDataProvider.GetLeagueVersionsByCountryId(countryData.Id))
            {
                var version = context.Set<LeagueVersion>().SingleOrDefault(x => x.LeagueVersionId == versionData.LeagueVersionId);
                if (version is null)
                    context.Set<LeagueVersion>().Add(
                        new LeagueVersion(
                            leagueVersionId: versionData.LeagueVersionId,
                            leagueId: versionData.LeagueId,
                            version: versionData.Version,
                            teamsCount: versionData.TeamsCount,
                            format: MapFormat(versionData.Format),
                            promotionPositions: versionData.PromotionPositions,
                            promotionPlayOffPositions: versionData.PromotionPlayOffPositions,
                            relegationPositions: versionData.RelegationPositions,
                            relegationPlayOffPositions: versionData.RelegationPlayOffPositions,
                            uefaChampionsLeaguePositions: versionData.UefaChampionsLeaguePositions,
                            uefaEuropaLeaguePositions: versionData.UefaEuropaLeaguePositions,
                            uefaConferenceLeaguePositions: versionData.UefaConferenceLeaguePositions));
            }

            foreach (var leagueTeamData in LeaguesDataProvider.GetLeagueTeamsByCountryId(countryData.Id))
            {
                var leagueTeam = context.Set<LeagueTeam>()
                    .SingleOrDefault(x => x.LeagueVersionId == leagueTeamData.LeagueVersionId && x.TeamId == leagueTeamData.TeamId);
                if (leagueTeam is null)
                    context.Set<LeagueTeam>().Add(
                        new LeagueTeam(
                            leagueVersionId: leagueTeamData.LeagueVersionId,
                            teamId: leagueTeamData.TeamId));
            }

            foreach (var leagueRoundData in LeaguesDataProvider.GetLeagueRoundsByCountryId(countryData.Id))
            {
                var leagueRound = context.Set<LeagueRound>()
                    .SingleOrDefault(x => x.LeagueVersionId == leagueRoundData.LeagueVersionId && x.Round == leagueRoundData.Round);
                if (leagueRound is null)
                    context.Set<LeagueRound>()
                        .Add(new LeagueRound(leagueVersionId: leagueRoundData.LeagueVersionId, round: leagueRoundData.Round, week: leagueRoundData.Week, isMidweek: leagueRoundData.IsMidweek));
            }

            foreach (var activationData in LeaguesDataProvider.GetLeagueActivationsByCountryId(countryData.Id))
            {
                var activation = context.Set<LeagueActivation>()
                    .SingleOrDefault(x => x.LeagueId == activationData.LeagueId);
                if (activation is null)
                    context.Set<LeagueActivation>().Add(
                        new LeagueActivation(activationData.LeagueId, activationData.LeagueVersionId));
                else
                    activation.ChangeVersion(activationData.LeagueVersionId);
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
