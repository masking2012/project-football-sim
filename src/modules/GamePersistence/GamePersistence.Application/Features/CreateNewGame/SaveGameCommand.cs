namespace ProjectFootballSim.GamePersistence.Application.Features.CreateNewGame;

public sealed record SaveGameCommand(Guid UserId, Guid GameId, int SlotId, string Name);
