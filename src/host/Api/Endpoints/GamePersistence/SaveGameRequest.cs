namespace ProjectFootballSim.Api.Endpoints.GamePersistence;

internal sealed record SaveGameRequest(
    int SlotId,
    string Name);
