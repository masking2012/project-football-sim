using ProjectFootballSim.Matches.Application.Common.Dtos;
using ProjectFootballSim.Matches.Application.Common.Services;
using ProjectFootballSim.Matches.Domain.Services;
using ProjectFootballSim.Matches.Domain.ValueObjects;

namespace ProjectFootballSim.Matches.Application.Features.RegularTime;

public sealed class RegularTimeSimulator
    (IPossessionCalculator possessionCalculator,
    IChancesCalculator chancesCalculator,
    IGoalsCalculator goalsCalculator)
{
    public ScoreResultDto Play(MatchTeamDto homeDto, MatchTeamDto awayDto, MatchSettingsDto matchSettings)
    {
        ArgumentNullException.ThrowIfNull(homeDto);
        ArgumentNullException.ThrowIfNull(awayDto);
        ArgumentNullException.ThrowIfNull(matchSettings);

        MatchTeam home = TeamMapper.Map(homeDto);
        MatchTeam away = TeamMapper.Map(awayDto);

        // Calculate possession based on midfield strength
        Possession homePossession = possessionCalculator.Calculate(home.Midfield, away.Midfield);
        AdvantageRatio attackingAdvantageRatio = matchSettings.HasHomeAdvantage ? new AdvantageRatio(1.1) : AdvantageRatio.Neutral;

        // Calculate number of attacking chances based on possession and attack/defense matchup
        int homeChances = chancesCalculator.Calculate(home, away, homePossession, attackingAdvantageRatio, GoalChancesSettings.RegularTime);
        int awayChances = chancesCalculator.Calculate(away, home, homePossession.OpponentPossession, AdvantageRatio.Neutral, GoalChancesSettings.RegularTime);

        // Convert chances to goals
        int homeGoals = goalsCalculator.Calculate(home.Attack, away.Defence, homeChances);
        int awayGoals = goalsCalculator.Calculate(away.Attack, home.Defence, awayChances);

        return new ScoreResultDto(homeGoals, awayGoals);
    }
}
