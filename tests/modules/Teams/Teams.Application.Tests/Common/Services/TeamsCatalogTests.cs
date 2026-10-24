using Microsoft.Extensions.DependencyInjection;
using Moq;
using ProjectFootballSim.Teams.Application;
using ProjectFootballSim.Teams.Application.Common.Models;
using ProjectFootballSim.Teams.Application.Common.Services;
using ProjectFootballSim.Teams.Domain.Entities;
using ProjectFootballSim.Teams.Domain.ValueObjects;

namespace ProjectFootballSim.Teams.Application.Tests.Common.Services;

internal sealed class TeamsCatalogTests
{
    [Test]
    public async Task ShouldMapTeamsByIdAndCountry()
    {
        var repository = new Mock<ITeamsRepository>();
        repository
            .Setup(value => value.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                CreateTeam(7, "Home Team", 3, 80, 70, 60),
                CreateTeam(8, "Away Team", 3, 50, 40, 30),
                CreateTeam(9, "Other Team", 4, 20, 30, 40)]);

        var catalog = CreateCatalog(repository);

        var team = await catalog.GetByIdAsync(7, CancellationToken.None).ConfigureAwait(false);
        var countryTeams = await catalog.GetByCountryAsync(3, CancellationToken.None).ConfigureAwait(false);

        using (Assert.Multiple())
        {
            await Assert.That(team).IsEqualTo(new TeamDto(7, "Home Team", 80, 70, 60, 3));
            await Assert.That(countryTeams).Count().IsEqualTo(2);
            await Assert.That(countryTeams[0].Id).IsEqualTo(7);
            await Assert.That(countryTeams[1].Id).IsEqualTo(8);
        }
    }

    [Test]
    public async Task ShouldReturnNullForUnknownTeam()
    {
        var repository = new Mock<ITeamsRepository>();
        repository
            .Setup(value => value.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateTeam(7, "Known Team", 3, 80, 70, 60)]);

        var catalog = CreateCatalog(repository);

        var result = await catalog.GetByIdAsync(999, CancellationToken.None).ConfigureAwait(false);

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task ShouldReturnEmptyListForUnknownCountry()
    {
        var repository = new Mock<ITeamsRepository>();
        repository
            .Setup(value => value.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateTeam(7, "Known Team", 3, 80, 70, 60)]);

        var catalog = CreateCatalog(repository);

        var result = await catalog.GetByCountryAsync(999, CancellationToken.None).ConfigureAwait(false);

        await Assert.That(result).Count().IsEqualTo(0);
    }

    [Test]
    public async Task ShouldLoadRepositoryOnlyOncePerCatalogCache()
    {
        var repository = new Mock<ITeamsRepository>();
        repository
            .Setup(value => value.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateTeam(7, "Known Team", 3, 80, 70, 60)]);

        var catalog = CreateCatalog(repository);

        await catalog.GetByIdAsync(7, CancellationToken.None).ConfigureAwait(false);
        await catalog.GetByCountryAsync(3, CancellationToken.None).ConfigureAwait(false);

        repository.Verify(value => value.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static ITeamsCatalog CreateCatalog(Mock<ITeamsRepository> repository)
    {
        var services = new ServiceCollection();
        services.AddSingleton(repository.Object);
        services.AddHybridCache();
        services.AddTeamsApplication();

        return services
            .BuildServiceProvider()
            .GetRequiredService<ITeamsCatalog>();
    }

    private static Team CreateTeam(
        int id,
        string name,
        int countryId,
        int attack,
        int midfield,
        int defence) => new(
            id,
            name,
            new TeamAttributeValue(attack),
            new TeamAttributeValue(midfield),
            new TeamAttributeValue(defence),
            countryId);
}
