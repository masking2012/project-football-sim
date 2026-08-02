namespace ProjectFootballSim.Api.Endpoints.GamePersistence;

internal sealed record GameSaveItemResponse
    (Guid GameId,
    int SlotId,
    string Name,
    DateTime CreatedAtUtc);
