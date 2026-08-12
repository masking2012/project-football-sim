namespace ProjectFootballSim.Calendar.Domain.Entities;

public sealed class GameSeason
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid GameId { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public int Order { get; }
    public bool IsCurrent { get; private set; }

    public GameSeason(Guid userId, Guid gameId, DateTime startDate, DateTime endDate, int order)
    {
        if (endDate <= startDate)
            throw new ArgumentException("End date cannot be earlier than start date.");
        if (order < 1)
            throw new ArgumentException("Order must be a positive integer.");
        ValidateDate(startDate);
        ValidateDate(endDate);

        UserId = userId;
        GameId = gameId;
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

    private static void ValidateDate(DateTime date)
    {
        if (date.Hour != 0 || date.Minute != 0 || date.Second != 0 || date.Millisecond != 0)
            throw new ArgumentException("Date must be at midnight (00:00:00).");
    }
}
