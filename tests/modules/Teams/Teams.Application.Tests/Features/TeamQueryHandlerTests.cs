using Moq;
using ProjectFootballSim.Teams.Application.Common.Interfaces;
using ProjectFootballSim.Teams.Application.Common.Models;
using ProjectFootballSim.Teams.Application.Features.GetTeamById;
using ProjectFootballSim.Teams.Application.Features.GetTeamsByCountry;
using ProjectFootballSim.Teams.Application.Features.GetTeamsByIds;

namespace ProjectFootballSim.Teams.Application.Tests.Features;

internal sealed class TeamQueryHandlerTests
{
    [Test]
    public async Task GetTeamByIdShouldReturnCatalogTeam()
    {
        var expected = CreateTeamDto(7, 3);
        var catalog = new Mock<ICacheTeamsRetriever>();
        catalog
            .Setup(value => value.GetByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new GetTeamByIdQueryHandler(catalog.Object);
        var result = await handler.HandleAsync(7, CancellationToken.None).ConfigureAwait(false);

        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task GetTeamByIdShouldReturnNullWhenTeamDoesNotExist()
    {
        var catalog = new Mock<ICacheTeamsRetriever>();
        catalog
            .Setup(value => value.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TeamDto?)null);

        var handler = new GetTeamByIdQueryHandler(catalog.Object);
        var result = await handler.HandleAsync(999, CancellationToken.None).ConfigureAwait(false);

        await Assert.That(result).IsNull();
    }

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
            await Assert.That(result[0]).IsEqualTo(expected[0]);
            await Assert.That(result[1]).IsEqualTo(expected[1]);
        }
    }

    [Test]
    public async Task GetTeamsByIdsShouldReturnDistinctExistingTeams()
    {
        var first = CreateTeamDto(7, 3);
        var second = CreateTeamDto(8, 3);
        var catalog = new Mock<ICacheTeamsRetriever>();
        catalog
            .Setup(value => value.GetByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(first);
        catalog
            .Setup(value => value.GetByIdAsync(8, It.IsAny<CancellationToken>()))
            .ReturnsAsync(second);
        catalog
            .Setup(value => value.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TeamDto?)null);

        var handler = new GetTeamsByIdsQueryHandler(catalog.Object);
        var result = await handler.HandleAsync([7, 7, 8, 999], CancellationToken.None).ConfigureAwait(false);

        using (Assert.Multiple())
        {
            await Assert.That(result).Count().IsEqualTo(2);
            await Assert.That(result[7]).IsEqualTo(first);
            await Assert.That(result[8]).IsEqualTo(second);
        }

        catalog.Verify(value => value.GetByIdAsync(7, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static TeamDto CreateTeamDto(int id, int countryId) => new(
        Id: id,
        Name: $"Team {id}",
        Attack: 80,
        Midfield: 70,
        Defence: 60,
        CountryId: countryId);
}
