namespace ProjectFootballSim.Calendar.Application.Features.AdvanceCalendarDay;

public sealed record AdvanceCalendarDayCommand(
    Guid UserId,
    Guid GameId);
