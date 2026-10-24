using ProjectFootballSim.Leagues.Domain.Catalog.ValueObjects;

namespace ProjectFootballSim.Leagues.Application.Catalog.Models;

public sealed record LeagueFormatDto(int Order, LeagueFormatType Type, int Legs);
