using System;
using System.Collections.Generic;
namespace ProjectFootballSim.Calendar.Contracts.Features.GetCalendarDay;

public sealed record LeagueMatchDto(
    Guid Id,
    int HomeTeamId,
    int AwayTeamId,
    string HomeTeamName,
    string AwayTeamName,
    int? HomeTeamScore,
    int? AwayTeamScore,
    int LeagueVersionId,
    string LeagueName,
    int LeagueRound,
    int LeagueOrder,
    int CountryId,
    string CountryName);
