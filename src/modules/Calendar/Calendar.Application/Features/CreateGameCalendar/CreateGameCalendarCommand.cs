namespace ProjectFootballSim.Calendar.Application.Features.CreateGameCalendar
{
    public sealed record CreateGameCalendarCommand(Guid GameId, DateTime NewDate);
}
