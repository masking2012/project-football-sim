using ProjectFootballSim.Common.Features;
using ProjectFootballSim.Leagues.Domain.Entities;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootballSim.Leagues.Application.GameFeatures.CreateGameLeague;

public sealed class LeagueFixtureGenerator(LeaguesDbContext dbContext)
{
    private const int StubTeamId = -1;
    private const int MaxVenueStreak = 3;

    public IReadOnlyList<GameLeagueMatch> Generate(
        int leagueId,
        Guid gameLeagueId,
        IEnumerable<int> teamIds,
        DateTime seasonStartDate)
    {
        var roundDates = LoadRoundDates(leagueId, seasonStartDate);
        var teams = PrepareTeams(teamIds);
        var pairings = GenerateRoundRobinPairings(teams);

        var firstHalfFixtures = CreateFirstHalfFixtures(pairings, teams.Where(t => t != StubTeamId));
        var allFixtures = GenerateFullSeasonFixtures(firstHalfFixtures, roundDates, gameLeagueId);

        return allFixtures;
    }

    // ------------------------------------------------------------
    // ROUND DATES
    // ------------------------------------------------------------

    private Dictionary<int, DateTime> LoadRoundDates(int leagueId, DateTime seasonStartDate)
    {
        var leagueRounds = dbContext.LeagueRounds
            .Where(x => x.LeagueId == leagueId)
            .ToList();

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

    private static List<List<(int Home, int Away)>> CreateFirstHalfFixtures(
        List<List<(int First, int Second)>> pairings,
        IEnumerable<int> teamIds)
    {
        var streaks = teamIds.ToDictionary(id => id, _ => new VenueStreak());
        var fixtures = new List<List<(int Home, int Away)>>();

        if (!TryAssignFixtures(pairings, 0, streaks, fixtures))
            throw new InvalidOperationException("Unable to create fixtures with at most three consecutive home or away games.");

        return fixtures;
    }

    private static bool TryAssignFixtures(
        List<List<(int First, int Second)>> pairings,
        int round,
        Dictionary<int, VenueStreak> streaks,
        List<List<(int Home, int Away)>> fixtures)
    {
        if (round == pairings.Count)
            return true;

        foreach (var option in CreateRoundOptions(pairings[round], streaks))
        {
            var nextStreaks = ApplyStreaks(streaks, option);

            fixtures.Add(option);

            if (TryAssignFixtures(pairings, round + 1, nextStreaks, fixtures))
                return true;

            fixtures.RemoveAt(fixtures.Count - 1);
        }

        return false;
    }

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

    private static List<GameLeagueMatch> GenerateFullSeasonFixtures(
        List<List<(int Home, int Away)>> firstHalf,
        Dictionary<int, DateTime> roundDates,
        Guid gameLeagueId)
    {
        int numRounds = firstHalf.Count;
        var result = new List<GameLeagueMatch>(numRounds * 2 * firstHalf[0].Count);

        for (int round = 0; round < numRounds; round++)
        {
            int firstRound = round + 1;
            int secondRound = numRounds * 2 - round;

            foreach (var (home, away) in firstHalf[round])
            {
                result.Add(new GameLeagueMatch(
                    roundDates[firstRound], home, away, firstRound, gameLeagueId));
            }

            foreach (var (home, away) in firstHalf[numRounds - round - 1])
            {
                result.Add(new GameLeagueMatch(
                    roundDates[secondRound], away, home, secondRound, gameLeagueId));
            }
        }

        return result;
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
