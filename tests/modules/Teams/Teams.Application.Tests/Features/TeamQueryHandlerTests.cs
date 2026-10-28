using Moq;
using ProjectFootballSim.Teams.Application.Common.Interfaces;
using ProjectFootballSim.Teams.Application.Common.Models;
using ProjectFootballSim.Teams.Application.Features.GetTeamsByCountry;
using ProjectFootballSim.Teams.Application.Features.GetTeamsByIds;
using ProjectFootballSim.Teams.Contracts.Features.GetTeamsByCountry;
using ProjectFootballSim.Teams.Contracts.Features.GetTeamsByIds;

namespace ProjectFootballSim.Teams.Application.Tests.Features;

internal sealed class TeamQueryHandlerTests
{
    [Test]
    public async Task GetTeamsByCountryShouldReturnCatalogTeams()
    {
        IReadOnlyList<TeamDto> expected = [CreateTeamDto(7, 3), CreateTeamDto(8, 3)];
        var catalog = new Mock<ICacheTeamsRetriever>();
        catalog
            .Setup(value => value.GetByCountryAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new GetTeamsByCountryQueryHandler(catalog.Object);
        var result = await handler.HandleAsync(3, CancellationToken.None).ConfigureAwait(false);

        using (Assert.Multiple())
        {
            await Assert.That(result).Count().IsEqualTo(2);
            await Assert.That(result[7]).IsEqualTo(new TeamByCountryDto(7, "Team 7", 80, 70, 60));
            await Assert.That(result[8]).IsEqualTo(new TeamByCountryDto(8, "Team 8", 80, 70, 60));
        }
    }

    [Test]
    public async Task GetTeamsByIdsShouldReturnTeamsKeyedById()
    {
        IReadOnlyList<TeamDto> teams = [CreateTeamDto(7, 3), CreateTeamDto(8, 3)];
        var catalog = new Mock<ICacheTeamsRetriever>();
        catalog
            .Setup(value => value.GetByIdsAsync(It.IsAny<IReadOnlyList<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);

        var handler = new GetTeamsByIdsQueryHandler(catalog.Object);
        var result = await handler.HandleAsync([7, 8], CancellationToken.None).ConfigureAwait(false);

        using (Assert.Multiple())
        {
            await Assert.That(result).Count().IsEqualTo(2);
            await Assert.That(result[7]).IsEqualTo(new TeamByIdsDto(7, "Team 7", 80, 70, 60, 3));
            await Assert.That(result[8]).IsEqualTo(new TeamByIdsDto(8, "Team 8", 80, 70, 60, 3));
        }

        catalog.Verify(value => value.GetByIdsAsync(It.IsAny<IReadOnlyList<int>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetTeamsByIdsShouldThrowWhenTeamIsMissing()
    {
        IReadOnlyList<TeamDto> teams = [CreateTeamDto(7, 3)];
        var catalog = new Mock<ICacheTeamsRetriever>();
        catalog
            .Setup(value => value.GetByIdsAsync(It.IsAny<IReadOnlyList<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);

        var handler = new GetTeamsByIdsQueryHandler(catalog.Object);

        Func<Task> act = () => handler.HandleAsync([7, 999], CancellationToken.None);

        await Assert.That(act).Throws<InvalidOperationException>();
    }

    private static TeamDto CreateTeamDto(int id, int countryId) => new(
        Id: id,
        Name: $"Team {id}",
        Attack: 80,
        Midfield: 70,
        Defence: 60,
        CountryId: countryId);
}
