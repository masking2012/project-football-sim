namespace ProjectFootballSim.GamePersistence.Contracts.Events;

public sealed record GameCreatedEvent(
    Guid UserId,
    Guid GameId);
