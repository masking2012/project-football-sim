namespace ProjectFootballSim.Teams.Contracts.Features.GetTeamsByCountry;

public interface IGetTeamsByCountryQueryHandler
{
    Task<IReadOnlyDictionary<int, TeamByCountryDto>> HandleAsync(
        int countryId,
        CancellationToken cancellationToken);
}
