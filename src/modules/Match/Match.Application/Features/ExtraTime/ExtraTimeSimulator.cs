using ProjectFootballSim.Match.Application.Common.Dtos;
using ProjectFootballSim.Match.Application.Common.Services;
using ProjectFootballSim.Match.Domain.ValueObjects;

namespace ProjectFootballSim.Match.Application.Features.ExtraTime;

public sealed class ExtraTimeSimulator
    (IPossessionCalculator possessionCalculator,
    IChancesCalculator chancesCalculator,
    IGoalsCalculator goalsCalculator)
{
    public ScoreResult Play(MatchTeamDto homeDto, MatchTeamDto awayDto, MatchSettings matchSettings)
    {
        ArgumentNullException.ThrowIfNull(homeDto);
        ArgumentNullException.ThrowIfNull(awayDto);
        ArgumentNullException.ThrowIfNull(matchSettings);

        Team home = TeamMapper.Map(homeDto);
        Team away = TeamMapper.Map(awayDto);

        // Calculate possession based on midfield strength
        Possession homePossession = possessionCalculator.Calculate(home.Midfield, away.Midfield);
        AdvantageRatio attackingAdvantageRatio = matchSettings.HasHomeAdvantage ? new AdvantageRatio(1.1) : AdvantageRatio.Neutral;

        // Calculate number of attacking chances based on possession and attack/defense matchup
        int homeChances = chancesCalculator.Calculate(home, away, homePossession, attackingAdvantageRatio, GoalChancesSettings.ExtraTime);
        int awayChances = chancesCalculator.Calculate(away, home, homePossession.OpponentPossession, AdvantageRatio.Neutral, GoalChancesSettings.ExtraTime);

        // Convert chances to goals
        int homeGoals = goalsCalculator.Calculate(home.Attack, away.Defence, homeChances);
        int awayGoals = goalsCalculator.Calculate(away.Attack, home.Defence, awayChances);

        return new ScoreResult(homeGoals, awayGoals);
    }
}
