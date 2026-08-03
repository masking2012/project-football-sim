namespace ProjectFootballSim.Matches.Domain.Services;

public interface IGoalsCalculator
{
    int Calculate(int attack, int opponentDefence, int chances);
}
