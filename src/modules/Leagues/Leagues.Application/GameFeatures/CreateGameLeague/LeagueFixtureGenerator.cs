using ProjectFootballSim.Common.Features;
using ProjectFootballSim.Leagues.Domain.Entities;
using ProjectFootballSim.Leagues.Infrastructure.Database;

namespace ProjectFootball.Core.Simulation.ChampionshipSimulation;

public sealed class LeagueFixtureGenerator(LeaguesDbContext dbContext)
{
    public IReadOnlyList<GameLeagueMatch> Generate(
        int leagueId,
        Guid gameLeagueId,
        IEnumerable<int> teamsIds,
        DateTime seasonStartDate)
    {
        var result = new List<GameLeagueMatch>();

        var leagueRounds = dbContext.LeagueRounds.Where(x => x.LeagueId == leagueId).ToList();        
        var firstSundayOfSeason = DateTimeUtils.GetNextDayOfWeek(seasonStartDate, DayOfWeek.Sunday);
        Dictionary<int, DateTime> roundDates = new();
        foreach (LeagueRound leagueRound in leagueRounds)
        {
            roundDates[leagueRound.Round] = firstSundayOfSeason.AddDays((leagueRound.Week - 1) * 7);
        }

        int stubTeamId = -1;
        List<int> tempTeams = teamsIds.Shuffle().ToList();

        int numTeams = tempTeams.Count;
        if (numTeams % 2 != 0)
        {
            tempTeams.Add(stubTeamId); // if odd, add stub
            numTeams++;
        }

        int numRounds = numTeams - 1;
        int matchesPerRound = numTeams / 2;

        int fixedTeam = tempTeams[0];
        var rotatingTeams = tempTeams.Skip(1).ToList();

        SortedDictionary<int, List<dynamic>> fixturesByRound = new();

        for (int round = 0; round < numRounds; round++)
        {
            for (int i = 0; i < matchesPerRound; i++)
            {
                int home, away;

                if (i == 0)
                {
                    home = fixedTeam;
                    away = rotatingTeams[round % rotatingTeams.Count];
                }
                else
                {
                    int firstIndex = (round + i) % rotatingTeams.Count;
                    int secondIndex = (round + rotatingTeams.Count - i) % rotatingTeams.Count;
                    home = rotatingTeams[firstIndex];
                    away = rotatingTeams[secondIndex];
                }

                if (home != stubTeamId && away != stubTeamId)
                {
                    if (!fixturesByRound.ContainsKey(round + 1))
                        fixturesByRound.Add(round + 1, new List<dynamic>());

                    result.Add(new GameLeagueMatch(
                        date: roundDates[round + 1],
                        homeTeamId: home,
                        awayTeamId: away,
                        round: round + 1,
                        gameLeagueId: gameLeagueId));

                    result.Add(new GameLeagueMatch(
                        date: roundDates[round + numRounds + 1],
                        homeTeamId: away,
                        awayTeamId: home,
                        round: round + numRounds + 1,
                        gameLeagueId: gameLeagueId));
                }
            }
        }

        return result;
    }
}
