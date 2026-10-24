using Microsoft.Extensions.Logging;
using ProjectFootballSim.Matches.Application.Common.Models;
using ProjectFootballSim.Matches.Application.Common.Services;
using ProjectFootballSim.Matches.Domain.Services;
using ProjectFootballSim.Matches.Domain.ValueObjects;

namespace ProjectFootballSim.Matches.Application.Features.RegularTime;

public sealed class SimulateRegularTimeCommandHandler
    (ILogger<SimulateRegularTimeCommandHandler> logger,
    IPossessionCalculator possessionCalculator,
    IChancesCalculator chancesCalculator,
    IGoalsCalculator goalsCalculator)
{
    public ScoreResultDto Handle(SimulateRegularTimeCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        MatchTeam home = TeamMapper.Map(command.Home);
        MatchTeam away = TeamMapper.Map(command.Away);

        // Calculate possession based on midfield strength
        Possession homePossession = possessionCalculator.Calculate(home.Midfield, away.Midfield);
        AdvantageRatio attackingAdvantageRatio = command.MatchSettings.HasHomeAdvantage ? new AdvantageRatio(1.1) : AdvantageRatio.Neutral;

        // Calculate number of attacking chances based on possession and attack/defence matchup
        int homeChances = chancesCalculator.Calculate(home, away, homePossession, attackingAdvantageRatio, GoalChancesSettings.RegularTime);
        int awayChances = chancesCalculator.Calculate(away, home, homePossession.OpponentPossession, AdvantageRatio.Neutral, GoalChancesSettings.RegularTime);

        // Convert chances to goals
        int homeGoals = goalsCalculator.Calculate(home.Attack, away.Defence, homeChances);
        int awayGoals = goalsCalculator.Calculate(away.Attack, home.Defence, awayChances);

        Log.RegularTimeStatistics(logger, homeTeamId: home.Id, homePossession: homePossession.Value, homeChances: homeChances, awayTeamId: away.Id, awayPossession: homePossession.OpponentPossession.Value, awayChances: awayChances);
        Log.RegularTimeResult(logger, home.Id, homeGoals, away.Id, awayGoals);

        return new ScoreResultDto(homeGoals, awayGoals);
    }
}
