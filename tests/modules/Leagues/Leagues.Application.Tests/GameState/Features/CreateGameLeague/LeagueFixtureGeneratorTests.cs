using Moq;
using ProjectFootballSim.Leagues.Application.Catalog.Interfaces;
using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Application.GameState.Features.CreateGameLeague;

namespace ProjectFootballSim.Leagues.Application.Tests.GameState.Features.CreateGameLeague;

internal sealed class LeagueFixtureGeneratorTests
{
    [Test]
    public async Task ShouldGenerateConfiguredNumberOfLegs()
    {
        var generator = CreateGenerator(legs: 1, rounds: 3);

        var fixtures = await generator.GenerateAsync(
            leagueVersionId: 101001,
            gameLeagueId: Guid.NewGuid(),
            teamIds: [1, 2, 3, 4],
            seasonStartDate: new DateTime(2026, 1, 1),
            CancellationToken.None).ConfigureAwait(false);

        await Assert.That(fixtures).Count().IsEqualTo(6);
        await Assert.That(fixtures.Select(match => match.Round).Distinct()).Count().IsEqualTo(3);
    }

    [Test]
    public async Task ShouldScheduleEachTeamOnlyOncePerRound()
    {
        var generator = CreateGenerator(legs: 1, rounds: 3);

        var fixtures = await generator.GenerateAsync(
            leagueVersionId: 101001,
            gameLeagueId: Guid.NewGuid(),
            teamIds: [1, 2, 3, 4],
            seasonStartDate: new DateTime(2026, 1, 1),
            CancellationToken.None).ConfigureAwait(false);

        foreach (var round in fixtures.GroupBy(match => match.Round))
        {
            var participatingTeams = round
                .SelectMany(match => new[] { match.HomeTeamId, match.AwayTeamId })
                .ToList();

            await Assert.That(participatingTeams).Count().IsEqualTo(4);
            await Assert.That(participatingTeams.Distinct()).Count().IsEqualTo(4);
        }
    }

    [Test]
    public async Task ShouldScheduleFutureRoundsAfterPreviousRounds()
    {
        var generator = CreateGenerator(legs: 1, rounds: 3);

        var fixtures = await generator.GenerateAsync(
            leagueVersionId: 101001,
            gameLeagueId: Guid.NewGuid(),
            teamIds: [1, 2, 3, 4],
            seasonStartDate: new DateTime(2026, 1, 1),
            CancellationToken.None).ConfigureAwait(false);

        var datesByRound = fixtures
            .GroupBy(match => match.Round)
            .ToDictionary(round => round.Key, round => round.Select(match => match.Date).ToList());

        for (var round = 1; round < datesByRound.Count; round++)
        {
            await Assert.That(datesByRound[round].Max()).IsLessThan(datesByRound[round + 1].Min());
        }
    }

    [Test]
    public async Task ShouldRejectCalendarRoundCountMismatch()
    {
        var generator = CreateGenerator(legs: 2, rounds: 3);

        await Assert.That(async () => await generator.GenerateAsync(
            leagueVersionId: 101001,
            gameLeagueId: Guid.NewGuid(),
            teamIds: [1, 2, 3, 4],
            seasonStartDate: new DateTime(2026, 1, 1),
            CancellationToken.None).ConfigureAwait(false)).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldMirrorFirstLapRoundsWithSwappedVenuesInSecondLap()
    {
        var generator = CreateGenerator(legs: 2, rounds: 6);

        var fixtures = await generator.GenerateAsync(
            leagueVersionId: 101001,
            gameLeagueId: Guid.NewGuid(),
            teamIds: [1, 2, 3, 4],
            seasonStartDate: new DateTime(2026, 1, 1),
            CancellationToken.None).ConfigureAwait(false);

        var pairingsByRound = fixtures
            .GroupBy(match => match.Round)
            .ToDictionary(round => round.Key, round => round.Select(match => (match.HomeTeamId, match.AwayTeamId)).ToHashSet());

        for (var round = 1; round <= 3; round++)
        {
            var mirroredPairings = pairingsByRound[round].Select(pairing => (pairing.AwayTeamId, pairing.HomeTeamId)).ToHashSet();

            await Assert.That(pairingsByRound[round + 3].SetEquals(mirroredPairings)).IsTrue();
        }
    }

    private static LeagueFixtureGenerator CreateGenerator(int legs, int rounds)
    {
        var league = new LeagueDto(
            Id: 1,
            LeagueVersionId: 101001,
            Version: 1,
            IsActive: true,
            Name: "Test League",
            Order: 1,
            CountryId: 1,
            TeamsCount: 4,
            Format: [new LeagueFormatDto(1, LeagueStageTypeDto.RoundRobin, legs)],
            PromotionPositions: null,
            PromotionPlayOffPositions: null,
            RelegationPositions: null,
            RelegationPlayOffPositions: null,
            UefaChampionsLeaguePositions: null,
            UefaEuropaLeaguePositions: null,
            UefaConferenceLeaguePositions: null);

        var leaguesCatalog = new Mock<ICachedLeaguesRetriever>();
        leaguesCatalog.Setup(catalog => catalog.GetLeagueAsync(
                101001,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(league);

        var leagueRounds = Enumerable.Range(1, rounds)
            .Select(round => new LeagueRoundDto(1, 101001, round, round, false))
            .ToList();
        leaguesCatalog.Setup(catalog => catalog.GetLeagueRoundsAsync(
                101001,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(leagueRounds);

        return new LeagueFixtureGenerator(leaguesCatalog.Object);
    }
}
