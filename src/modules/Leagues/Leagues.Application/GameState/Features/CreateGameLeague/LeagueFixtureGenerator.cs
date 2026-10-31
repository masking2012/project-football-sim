using ProjectFootballSim.Shared.Features;
using ProjectFootballSim.Leagues.Domain.GameState.Entities;
using ProjectFootballSim.Leagues.Application.Catalog.Models;
using ProjectFootballSim.Leagues.Application.Catalog.Interfaces;

namespace ProjectFootballSim.Leagues.Application.GameState.Features.CreateGameLeague;

public sealed class LeagueFixtureGenerator(
    ICachedLeaguesRetriever leaguesRetriever)
{
    private const int StubTeamId = -1;
    private const int MaxVenueStreak = 3;

    public async Task<IReadOnlyList<GameLeagueMatch>> GenerateAsync(
        int leagueVersionId,
        Guid gameLeagueId,
        IEnumerable<int> teamIds,
        DateTime seasonStartDate,
        CancellationToken cancellationToken)
    {
        LeagueDto? league = await leaguesRetriever.GetLeagueAsync(leagueVersionId, cancellationToken).ConfigureAwait(false);
        if (league is null)
            throw new InvalidOperationException($"League with version ID '{leagueVersionId}' not found.");

        var roundDates = await LoadRoundDatesAsync(leagueVersionId, seasonStartDate, cancellationToken).ConfigureAwait(false);
        var teams = PrepareTeams(teamIds);
        var pairings = GenerateRoundRobinPairings(teams);
        var allFixtures = new List<GameLeagueMatch>();
        var round = 1;

        foreach (var formatStage in league.Format.OrderBy(stage => stage.Order))
        {
            if (formatStage.Type != LeagueStageTypeDto.RoundRobin)
                throw new InvalidOperationException($"League format type '{formatStage.Type}' is not supported.");

            foreach (var stageRound in GenerateRoundRobinStage(pairings, teams.Where(t => t != StubTeamId), formatStage.Legs))
            {
                if (!roundDates.TryGetValue(round, out var date))
                    throw new InvalidOperationException($"League with version ID '{leagueVersionId}' has no calendar date for generated round {round}.");

                foreach (var (home, away) in stageRound)
                {
                    allFixtures.Add(new GameLeagueMatch(date, home, away, round, gameLeagueId));
                }

                round++;
            }
        }

        if (round - 1 != roundDates.Count)
            throw new InvalidOperationException($"League with ID '{leagueVersionId}' format generates {round - 1} rounds, but its calendar contains {roundDates.Count} rounds.");

        return allFixtures;
    }

    // ------------------------------------------------------------
    // ROUND DATES
    // ------------------------------------------------------------

    private async Task<Dictionary<int, DateTime>> LoadRoundDatesAsync(int leagueVersionId, DateTime seasonStartDate, CancellationToken cancellationToken)
    {
        var leagueRounds = await leaguesRetriever.GetLeagueRoundsAsync(leagueVersionId, cancellationToken)
            .ConfigureAwait(false);

        var firstSunday = DateTimeUtils.GetNextDayOfWeek(seasonStartDate, DayOfWeek.Sunday);

        return leagueRounds.ToDictionary(
            lr => lr.Round,
            lr => firstSunday.AddDays((lr.Week - 1) * 7));
    }

    // ------------------------------------------------------------
    // TEAM PREPARATION
    // ------------------------------------------------------------

    private static List<int> PrepareTeams(IEnumerable<int> teamIds)
    {
        var teams = teamIds.Shuffle().ToList();

        if (teams.Count % 2 != 0)
            teams.Add(StubTeamId);

        return teams;
    }

    // ------------------------------------------------------------
    // ROUND ROBIN PAIRINGS (FIRST HALF)
    // ------------------------------------------------------------

    private static List<List<(int First, int Second)>> GenerateRoundRobinPairings(List<int> teams)
    {
        int numTeams = teams.Count;
        int numRounds = numTeams - 1;
        int matchesPerRound = numTeams / 2;

        int fixedTeam = teams[0];
        var rotating = teams.Skip(1).ToList();

        var rounds = new List<List<(int First, int Second)>>();

        for (int round = 0; round < numRounds; round++)
        {
            var roundPairings = new List<(int, int)>();

            for (int i = 0; i < matchesPerRound; i++)
            {
                int home, away;

                if (i == 0)
                {
                    home = fixedTeam;
                    away = rotating[round % rotating.Count];
                }
                else
                {
                    int a = (round + i) % rotating.Count;
                    int b = (round + rotating.Count - i) % rotating.Count;
                    home = rotating[a];
                    away = rotating[b];
                }

                if (home != StubTeamId && away != StubTeamId)
                    roundPairings.Add((home, away));
            }

            rounds.Add(roundPairings);
        }

        return rounds;
    }

    // ------------------------------------------------------------
    // FIXTURE GENERATION WITH VENUE STREAK CONSTRAINTS
    // ------------------------------------------------------------

    private static bool TryAssignFixtures(
        List<List<(int First, int Second)>> pairings,
        int legs,
        int round,
        Dictionary<int, VenueStreak> streaks,
        List<List<(int Home, int Away)>> fixtures)
    {
        if (round == pairings.Count * legs)
            return true;

        var lap = round / pairings.Count;
        var index = round % pairings.Count;

        IEnumerable<List<(int Home, int Away)>> options = lap == 0
            ? CreateRoundOptions(pairings[index], streaks)
            : [MirrorRound(fixtures[index], lap)];

        foreach (var option in options)
        {
            var nextStreaks = ApplyStreaks(streaks, option);

            if (option.Any(f => nextStreaks[f.Home].Length > MaxVenueStreak || nextStreaks[f.Away].Length > MaxVenueStreak))
                continue;

            fixtures.Add(option);

            if (TryAssignFixtures(pairings, legs, round + 1, nextStreaks, fixtures))
                return true;

            fixtures.RemoveAt(fixtures.Count - 1);
        }

        return false;
    }

    private static List<(int Home, int Away)> MirrorRound(List<(int Home, int Away)> round, int lap) =>
        lap % 2 == 1
            ? round.Select(fixture => (fixture.Away, fixture.Home)).ToList()
            : round.ToList();

    private static Dictionary<int, VenueStreak> ApplyStreaks(
        Dictionary<int, VenueStreak> streaks,
        List<(int Home, int Away)> fixtures)
    {
        var next = new Dictionary<int, VenueStreak>(streaks);

        foreach (var (home, away) in fixtures)
        {
            next[home] = next[home].AddHome();
            next[away] = next[away].AddAway();
        }

        return next;
    }

    private static IEnumerable<List<(int Home, int Away)>> CreateRoundOptions(
        List<(int First, int Second)> pairings,
        Dictionary<int, VenueStreak> streaks)
    {
        var options = new List<List<(int Home, int Away)>>();

        void Recurse(int index, List<(int Home, int Away)> current, Dictionary<int, VenueStreak> currentStreaks)
        {
            if (index == pairings.Count)
            {
                options.Add(current.ToList());
                return;
            }

            var (a, b) = pairings[index];

            TryAdd(a, b);
            TryAdd(b, a);

            void TryAdd(int home, int away)
            {
                var homeStreak = currentStreaks[home].AddHome();
                var awayStreak = currentStreaks[away].AddAway();

                if (homeStreak.Length > MaxVenueStreak || awayStreak.Length > MaxVenueStreak)
                    return;

                var nextStreaks = new Dictionary<int, VenueStreak>(currentStreaks)
                {
                    [home] = homeStreak,
                    [away] = awayStreak
                };

                current.Add((home, away));
                Recurse(index + 1, current, nextStreaks);
                current.RemoveAt(current.Count - 1);
            }
        }

        Recurse(0, new List<(int, int)>(), streaks);

        return options.OrderBy(option =>
            option.Sum(f => streaks[f.Home].AddHome().Length + streaks[f.Away].AddAway().Length));
    }

    // ------------------------------------------------------------
    // FULL SEASON (FIRST + SECOND HALF)
    // ------------------------------------------------------------

    private static List<List<(int Home, int Away)>> GenerateRoundRobinStage(
        List<List<(int First, int Second)>> pairings,
        IEnumerable<int> teamIds,
        int legs)
    {
        var streaks = teamIds.ToDictionary(id => id, _ => new VenueStreak());
        var fixtures = new List<List<(int Home, int Away)>>();

        if (!TryAssignFixtures(pairings, legs, 0, streaks, fixtures))
            throw new InvalidOperationException("Unable to create fixtures with at most three consecutive home or away games.");

        return fixtures;
    }

    // ------------------------------------------------------------
    // VENUE STREAK
    // ------------------------------------------------------------

    private readonly record struct VenueStreak(int Venue = 0, int Length = 0)
    {
        public VenueStreak AddHome() => Add(1);
        public VenueStreak AddAway() => Add(-1);

        private VenueStreak Add(int venue) =>
            venue == Venue ? new VenueStreak(venue, Length + 1) : new VenueStreak(venue, 1);
    }
}
