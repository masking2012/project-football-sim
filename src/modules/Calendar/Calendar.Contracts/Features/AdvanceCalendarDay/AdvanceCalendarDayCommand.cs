namespace ProjectFootballSim.Calendar.Contracts.Features.AdvanceCalendarDay;

public sealed record AdvanceCalendarDayCommand(
    Guid UserId,
    Guid GameId);
