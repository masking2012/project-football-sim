using Microsoft.EntityFrameworkCore;
using ProjectFootballSim.Leagues.Application.Common.Models;
using ProjectFootballSim.Leagues.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectFootballSim.Leagues.Application.Features.GetLeagueById;

public sealed class GetLeagueByIdQueryHandler(LeaguesDbContext dbContext)
{
    public async Task<LeagueDto?> HandleAsync(int leagueId, CancellationToken cancellationToken)
    {
        var league = await dbContext.Leagues
            .FirstOrDefaultAsync(l => l.Id == leagueId, cancellationToken)
            .ConfigureAwait(false);

        if (league is null)
            return null;
        return new LeagueDto(league.Id, league.Name, league.Order, league.CountryId);
    }
}
