namespace ProjectFootballSim.Calendar.Application.Features.CreateGameCalendar
{
    public sealed record CreateGameCalendarCommand(
        Guid UserId,
        Guid GameId);
}
