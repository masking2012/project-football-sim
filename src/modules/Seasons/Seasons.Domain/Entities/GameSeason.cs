namespace ProjectFootballSim.Seasons.Domain.Entities;

public sealed class GameSeason
{
    public Guid Id { get; }
    public Guid GameId { get; }
    public Guid UserId { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public int Order { get; }
    public bool IsCurrent { get; private set; }

    public GameSeason(Guid gameId, Guid userId, DateTime startDate, DateTime endDate, int order)
    {
        if (endDate <= startDate)
            throw new ArgumentException("End date cannot be earlier than start date.");

        if (order < 1)
            throw new ArgumentException("Order must be a positive integer.");

        GameId = gameId;
        UserId = userId;
        StartDate = startDate;
        EndDate = endDate;
        Order = order;
        IsCurrent = true;
    }

    public void CloseSeason()
    {
        if (!IsCurrent)
            throw new InvalidOperationException("Cannot end a season that is not current.");

        IsCurrent = false;
    }
}
