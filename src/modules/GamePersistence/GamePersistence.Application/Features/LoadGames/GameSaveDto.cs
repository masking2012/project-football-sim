namespace ProjectFootballSim.GamePersistence.Application.Features.LoadGames;

public sealed record GameSaveDto(
    Guid GameId,
    int SlotId,
    string Name,
    DateTime CreatedAtUtc);
