namespace ProjectFootballSim.GamePersistence.Domain.Entities;

public sealed record GameSave
{
    public Guid UserId { get; }
    public int SlotId { get; }
    public string Name { get; }
    public Guid GameId { get; }
    public DateTime CreatedAtUtc { get; }

    public GameSave(Guid userId, int slotId, string name, Guid gameId, DateTime createdAtUtc)
    {
        if (slotId < 1 || slotId > 3)
            throw new ArgumentOutOfRangeException(nameof(slotId), "SlotId must be between 1 and 3.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));

        UserId = userId;
        SlotId = slotId;
        Name = name;
        GameId = gameId;
        CreatedAtUtc = createdAtUtc;
    }
}
