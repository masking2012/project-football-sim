using ProjectFootballSim.Leagues.Domain.ValueObjects;

namespace ProjectFootballSim.Leagues.Application.Common.Models;

public sealed record LeagueFormatDto(int Order, LeagueFormatType Type, int Legs);
