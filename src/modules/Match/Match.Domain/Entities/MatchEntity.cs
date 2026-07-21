using ProjectFootballSim.Match.Domain.ValueObjects;
using ProjectFootballSim.ValueObjects;

namespace ProjectFootballSim.Match.Domain.Entities;

public sealed class MatchEntity
{
    public Guid Id { get; init; }
    public Team Home { get; init; }
    public Team Away { get; init; }
    public MatchSettings Settings { get; init; }
    public MatchResult? Result { get; private set; }
    public bool IsFinished => Result != null;

    public MatchEntity(Team home, Team away, MatchSettings settings)
    {
        Home = home;
        Away = away;
        Settings = settings;
    }

    public void SetResult(MatchResult result)
    {
        if (IsFinished)
            throw new InvalidOperationException("Match is already finished.");
        Result = result;
    }
}
