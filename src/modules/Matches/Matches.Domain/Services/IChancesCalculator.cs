using ProjectFootballSim.Matches.Domain.ValueObjects;

namespace ProjectFootballSim.Matches.Domain.Services;

public interface IChancesCalculator
{
    int Calculate(MatchTeam attacking, MatchTeam defending,
        Possession attackingTeamPossession, AdvantageRatio attackingAdvantageRatio,
        GoalChancesSettings goalChancesSettings);
}
