using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectFootballSim.Common.Data.Entities.Leagues;
using ProjectFootballSim.Common.Data.Entities.Locations;
using ProjectFootballSim.Leagues.Domain.Entities;
using ProjectFootballSim.Leagues.Domain.ValueObjects;
using ProjectFootballSim.Leagues.Infrastructure.Database;

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
                var league = context.Set<League>().SingleOrDefault(c => c.Id == leagueData.Id);
                if (league is null)
                    context.Set<League>().Add(
                        new League(
                            id: leagueData.Id,
                            name: leagueData.Name,
                            order: leagueData.Order,
                            countryId: leagueData.CountryId,
                            teamsCount: leagueData.TeamsCount,
                            promotionPositions: leagueData.PromotionPositions,
                            promotionPlayOffPositions: leagueData.PromotionPlayOffPositions,
                            relegationPositions: leagueData.RelegationPositions,
                            relegationPlayOffPositions: leagueData.RelegationPlayOffPositions,
                            uefaChampionsLeaguePositions: leagueData.UefaChampionsLeaguePositions,
                            uefaEuropaLeaguePositions: leagueData.UefaEuropaLeaguePositions,
                            uefaConferenceLeaguePositions: leagueData.UefaConferenceLeaguePositions));
                else
                    league.Update(
                        name: leagueData.Name,
                        order: leagueData.Order,
                        countryId: leagueData.CountryId,
                        teamsCount: leagueData.TeamsCount,
                        promotionPositions: leagueData.PromotionPositions,
                        promotionPlayOffPositions: leagueData.PromotionPlayOffPositions,
                        relegationPositions: leagueData.RelegationPositions,
                        relegationPlayOffPositions: leagueData.RelegationPlayOffPositions,
                        uefaChampionsLeaguePositions: leagueData.UefaChampionsLeaguePositions,
                        uefaEuropaLeaguePositions: leagueData.UefaEuropaLeaguePositions,
                        uefaConferenceLeaguePositions: leagueData.UefaConferenceLeaguePositions);

                foreach(var leagueTeamData in LeaguesDataProvider.GetLeagueTeamsByCountryId(countryData.Id))
                {
                    var leagueTeam = context.Set<LeagueTeam>()
                        .SingleOrDefault(x => x.LeagueId == leagueTeamData.LeagueId && x.TeamId == leagueTeamData.TeamId);
                    if (league is null)
                        context.Set<LeagueTeam>().Add(
                            new LeagueTeam(
                                LeagueId: leagueTeamData.LeagueId,
                                TeamId: leagueTeamData.TeamId));
                }

                foreach (var leagueRoundData in LeaguesDataProvider.GetLeagueRoundsByCountryId(countryData.Id))
                {
                    var leagueRound = context.Set<LeagueRound>()
                        .SingleOrDefault(x => x.LeagueId == leagueRoundData.LeagueId && leagueRoundData.Round == leagueRoundData.Round);
                    if (leagueRound is null)
                        context.Set<LeagueRound>()
                            .Add(new LeagueRound(leagueId: leagueRoundData.LeagueId, round: leagueRoundData.Round, week: leagueRoundData.Week, isMidweek: leagueRoundData.IsMidweek));
                    else
                        leagueRound.Update(leagueRoundData.Week, leagueRoundData.IsMidweek);
                }
            }
        }

        context.SaveChanges();
    }
}
