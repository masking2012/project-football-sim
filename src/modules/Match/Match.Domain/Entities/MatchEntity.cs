using ProjectFootballSim.Match.Domain.ValueObjects;

namespace ProjectFootballSim.Match.Domain.Entities;

public sealed class MatchEntity
{
    public Guid Id { get; init; }
    public Team Home { get; init; }
    public Team Away { get; init; }
    public MatchSettings Settings { get; init; }
    public ScoreResult? Result { get; private set; }
    public bool IsFinished => Result != null;

    public MatchEntity(Team home, Team away, MatchSettings settings)
    {
        Home = home;
        Away = away;
        Settings = settings;
    }

    public void SetResult(ScoreResult result)
    {
        if (IsFinished)
            throw new InvalidOperationException("Match is already finished.");
        Result = result;
    }
}
