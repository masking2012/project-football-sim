using Microsoft.Extensions.DependencyInjection;
using Moq;
using ProjectFootballSim.Teams.Application.Common.Interfaces;
using ProjectFootballSim.Teams.Application.Common.Models;

namespace ProjectFootballSim.Teams.Application.Tests.Common.Services;

internal sealed class CacheTeamsRetrieverTests
{
    [Test]
    public async Task ShouldRetrieveTeamByIdAndTeamsByCountry()
    {
        var repository = new Mock<IGetTeamsRepository>();
        repository
            .Setup(value => value.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                CreateTeam(7, "Home Team", 3, 80, 70, 60),
                CreateTeam(8, "Away Team", 3, 50, 40, 30),
                CreateTeam(9, "Other Team", 4, 20, 30, 40)]);

        var retriever = CreateRetriever(repository);

        var teams = await retriever.GetByIdsAsync([7], CancellationToken.None).ConfigureAwait(false);
        var countryTeams = await retriever.GetByCountryAsync(3, CancellationToken.None).ConfigureAwait(false);

        using (Assert.Multiple())
        {
            await Assert.That(teams).Count().IsEqualTo(1);
            await Assert.That(teams[0]).IsEqualTo(new TeamDto(7, "Home Team", 80, 70, 60, 3));
            await Assert.That(countryTeams).Count().IsEqualTo(2);
            await Assert.That(countryTeams[0].Id).IsEqualTo(7);
            await Assert.That(countryTeams[1].Id).IsEqualTo(8);
        }
    }

    [Test]
    public async Task ShouldReturnEmptyForUnknownTeamId()
    {
        var repository = new Mock<IGetTeamsRepository>();
        repository
            .Setup(value => value.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateTeam(7, "Known Team", 3, 80, 70, 60)]);

        var retriever = CreateRetriever(repository);

        var result = await retriever.GetByIdsAsync([999], CancellationToken.None).ConfigureAwait(false);

        await Assert.That(result).Count().IsEqualTo(0);
    }

    [Test]
    public async Task ShouldReturnEmptyForUnknownCountry()
    {
        var repository = new Mock<IGetTeamsRepository>();
        repository
            .Setup(value => value.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateTeam(7, "Known Team", 3, 80, 70, 60)]);

        var retriever = CreateRetriever(repository);

        var result = await retriever.GetByCountryAsync(999, CancellationToken.None).ConfigureAwait(false);

        await Assert.That(result).Count().IsEqualTo(0);
    }

    [Test]
    public async Task ShouldLoadRepositoryOnlyOnce()
    {
        var repository = new Mock<IGetTeamsRepository>();
        repository
            .Setup(value => value.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateTeam(7, "Known Team", 3, 80, 70, 60)]);

        var retriever = CreateRetriever(repository);

        await retriever.GetByIdsAsync([7], CancellationToken.None).ConfigureAwait(false);
        await retriever.GetByCountryAsync(3, CancellationToken.None).ConfigureAwait(false);

        repository.Verify(value => value.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static ICacheTeamsRetriever CreateRetriever(Mock<IGetTeamsRepository> repository)
    {
        var services = new ServiceCollection();
        services.AddSingleton(repository.Object);
        services.AddHybridCache();
        services.AddTeamsApplication();

        return services
            .BuildServiceProvider()
            .GetRequiredService<ICacheTeamsRetriever>();
    }

    private static TeamDto CreateTeam(
        int id,
        string name,
        int countryId,
        int attack,
        int midfield,
        int defence) => new(
            id,
            name,
            attack,
            midfield,
            defence,
            countryId);
}
