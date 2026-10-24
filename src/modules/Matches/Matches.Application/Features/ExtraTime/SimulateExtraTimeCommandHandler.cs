using Microsoft.Extensions.Logging;
using ProjectFootballSim.Matches.Application.Common.Models;
using ProjectFootballSim.Matches.Application.Common.Services;
using ProjectFootballSim.Matches.Domain.Services;
using ProjectFootballSim.Matches.Domain.ValueObjects;

namespace ProjectFootballSim.Matches.Application.Features.ExtraTime;

public sealed class SimulateExtraTimeCommandHandler
    (ILogger<SimulateExtraTimeCommandHandler> logger,
    IPossessionCalculator possessionCalculator,
    IChancesCalculator chancesCalculator,
    IGoalsCalculator goalsCalculator)
{
    public ScoreResultDto Handle(SimulateExtraTimeCommand command)
    {
        MatchTeam home = TeamMapper.Map(command.HomeDto);
        MatchTeam away = TeamMapper.Map(command.AwayDto);

        // Calculate possession based on midfield strength
        Possession homePossession = possessionCalculator.Calculate(home.Midfield, away.Midfield);
        AdvantageRatio attackingAdvantageRatio = command.MatchSettings.HasHomeAdvantage ? new AdvantageRatio(1.1) : AdvantageRatio.Neutral;

        // Calculate number of attacking chances based on possession and attack/defence matchup
        int homeChances = chancesCalculator.Calculate(home, away, homePossession, attackingAdvantageRatio, GoalChancesSettings.ExtraTime);
        int awayChances = chancesCalculator.Calculate(away, home, homePossession.OpponentPossession, AdvantageRatio.Neutral, GoalChancesSettings.ExtraTime);

        // Convert chances to goals
        int homeGoals = goalsCalculator.Calculate(home.Attack, away.Defence, homeChances);
        int awayGoals = goalsCalculator.Calculate(away.Attack, home.Defence, awayChances);

        Log.ExtraTimeStatistics(logger, homeTeamId: home.Id, homePossession: homePossession.Value, homeChances: homeChances, awayTeamId: away.Id, awayPossession: homePossession.OpponentPossession.Value, awayChances: awayChances);
        Log.ExtraTimeResult(logger, home.Id, homeGoals, away.Id, awayGoals);

        return new ScoreResultDto(homeGoals, awayGoals);
    }
}
