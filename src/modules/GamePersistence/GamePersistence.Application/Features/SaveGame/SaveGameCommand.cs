namespace ProjectFootballSim.GamePersistence.Application.Features.SaveGame;

public sealed record SaveGameCommand(Guid UserId, Guid GameId, int SlotId, string Name);
