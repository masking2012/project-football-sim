using Moq;
using ProjectFootballSim.Calendar.Application.Common.Interfaces;
using ProjectFootballSim.Calendar.Application.Common.Models;
using ProjectFootballSim.Calendar.Application.Features.GetCalendarDay;
using ProjectFootballSim.Calendar.Contracts.Features.GetCalendarDay;
using ProjectFootballSim.Calendar.Domain.Entities;
using ProjectFootballSim.Calendar.Domain.Enums;
using ProjectFootballSim.Leagues.Contracts.GameStateFeatures.GetGameLeagueMatchesByIds;

namespace ProjectFootballSim.Calendar.Application.Tests.Features.GetCalendarDay;

internal sealed class GetCalendarDayQueryHandlerTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid GameId = Guid.NewGuid();
    private static readonly DateTime CurrentDate = new(2026, 8, 1);

    private readonly Mock<IGameCalendarRepository> _gameCalendarRepository = new();
    private readonly Mock<IGetCalendarDayFixturesRepository> _fixturesRepository = new();
    private readonly Mock<IGetGameLeagueMatchesByIdsQueryHandler> _leagueMatchesHandler = new();

    private GetCalendarDayQueryHandler CreateSut() => new(
        _gameCalendarRepository.Object,
        _fixturesRepository.Object,
        _leagueMatchesHandler.Object);

    [Test]
    public async Task ShouldThrowWhenCalendarNotFound()
    {
        _gameCalendarRepository
            .Setup(value => value.GetCurrentCalendarAsync(UserId, GameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GameCalendarDto?)null);

        Func<Task> act = () => CreateSut().HandleAsync(new GetCalendarDayQuery(UserId, GameId, null), CancellationToken.None);

        await Assert.That(act).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldReturnCurrentDayStatusWhenNoDateProvided()
    {
        SetupCalendar(new GameCalendarDto(CurrentDate, GameDayStatus.InProgress));
        SetupFixtures([]);

        var result = await CreateSut().HandleAsync(new GetCalendarDayQuery(UserId, GameId, null), CancellationToken.None).ConfigureAwait(false);

        using (Assert.Multiple())
        {
            await Assert.That(result.Date).IsEqualTo(CurrentDate);
            await Assert.That(result.DayStatus).IsEqualTo(GameDayStatus.InProgress.ToString());
            await Assert.That(result.LeagueMatches).IsEmpty();
        }
    }

    [Test]
    public async Task ShouldReportCompletedStatusForPastDate()
    {
        SetupCalendar(new GameCalendarDto(CurrentDate, GameDayStatus.NotStarted));
        SetupFixtures([]);

        var result = await CreateSut().HandleAsync(new GetCalendarDayQuery(UserId, GameId, CurrentDate.AddDays(-1)), CancellationToken.None).ConfigureAwait(false);

        await Assert.That(result.DayStatus).IsEqualTo(GameDayStatus.Completed.ToString());
    }

    [Test]
    public async Task ShouldReportNotStartedStatusForFutureDate()
    {
        SetupCalendar(new GameCalendarDto(CurrentDate, GameDayStatus.Completed));
        SetupFixtures([]);

        var result = await CreateSut().HandleAsync(new GetCalendarDayQuery(UserId, GameId, CurrentDate.AddDays(1)), CancellationToken.None).ConfigureAwait(false);

        await Assert.That(result.DayStatus).IsEqualTo(GameDayStatus.NotStarted.ToString());
    }

    [Test]
    public async Task ShouldMapLeagueMatchesForFixturesOnDate()
    {
        var matchId = Guid.NewGuid();
        var fixture = new GameFixture(
            matchId: matchId,
            gameCompetitionId: Guid.NewGuid(),
            competitionVersionId: 1,
            competitionType: CompetitionType.League,
            homeTeamId: 1,
            awayTeamId: 2,
            scheduledAt: CurrentDate,
            status: FixtureStatus.Scheduled,
            roundId: 1,
            userId: UserId,
            gameId: GameId);
        var match = new GetGameLeagueMatchDto(
            matchId, 1, 2, "Home", "Away", 2, 1, 10, "League", 3, 4, 5, "Country");

        SetupCalendar(new GameCalendarDto(CurrentDate, GameDayStatus.InProgress));
        SetupFixtures([fixture]);
        _leagueMatchesHandler
            .Setup(value => value.HandleAsync(
                It.Is<GetGameLeagueMatchesByIdsQuery>(query => query.Ids.Single() == matchId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([match]);

        var result = await CreateSut().HandleAsync(new GetCalendarDayQuery(UserId, GameId, null), CancellationToken.None).ConfigureAwait(false);

        var mapped = result.LeagueMatches.Single();
        using (Assert.Multiple())
        {
            await Assert.That(mapped.Id).IsEqualTo(matchId);
            await Assert.That(mapped.HomeTeamName).IsEqualTo("Home");
            await Assert.That(mapped.AwayTeamScore).IsEqualTo(1);
            await Assert.That(mapped.LeagueName).IsEqualTo("League");
            await Assert.That(mapped.CountryName).IsEqualTo("Country");
        }
    }

    private void SetupCalendar(GameCalendarDto calendar) =>
        _gameCalendarRepository
            .Setup(value => value.GetCurrentCalendarAsync(UserId, GameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(calendar);

    private void SetupFixtures(IReadOnlyList<GameFixture> fixtures) =>
        _fixturesRepository
            .Setup(value => value.GetFixturesAsync(UserId, GameId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixtures);
}
