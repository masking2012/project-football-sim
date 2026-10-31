namespace ProjectFootballSim.GamePersistence.Contracts.Features.CreateGame;

public sealed record GameCreatedEvent(
    Guid UserId,
    Guid GameId);
