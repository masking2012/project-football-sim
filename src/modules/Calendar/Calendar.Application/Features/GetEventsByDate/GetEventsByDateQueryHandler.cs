using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Calendar.Infrastructure.Database;
using ProjectFootballSim.Leagues.Application.GameFeatures.GetGameLeagueMatchesByDate;

namespace ProjectFootballSim.Calendar.Application.Features.GetEventsByDate;

public sealed class GetEventsByDateQueryHandler(
    CalendarDbContext dbContext,
    GetGameLeagueMatchesByDateQueryHandler getGameLeagueMatchesByDateQueryHandler)
{
    public async Task<IEnumerable<MatchEventDto>> HandleAsync(GetEventsByDateQuery query, CancellationToken cancellationToken)
    {
        DateTime? date = query.Date;
        if (date is null)
        {
            var gameCalendar = await dbContext.GameCalendars.SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            if (gameCalendar is null)
                throw new InvalidOperationException("Game calendar not found.");

            date = gameCalendar.CurrentDate;
        }

        var getGameLeagueMatchesByDateQuery = new GetGameLeagueMatchesByDateQuery(GameId: query.GameId, Date: date.Value);
        var matches = await getGameLeagueMatchesByDateQueryHandler
            .HandleAsync(getGameLeagueMatchesByDateQuery, cancellationToken)
            .ConfigureAwait(false);

        return matches.Select(x => new MatchEventDto(
            Id: x.Id,
            HomeTeamId: x.HomeTeamId,
            AwayTeamId: x.AwayTeamId,
            HomeTeamScore: x.HomeTeamScore,
            AwayTeamScore: x.AwayTeamScore,
            Round: x.Round,
            LeagueName: x.LeagueName,
            LeagueId: x.LeagueId,
            CountryId: x.CountryId));
    }
}

public sealed record GameLeagueMatchDto(
    Guid Id,
    DateTime Date,
    int HomeTeamId,
    int AwayTeamId,
    int? HomeTeamScore,
    int? AwayTeamScore,
    int Round);
