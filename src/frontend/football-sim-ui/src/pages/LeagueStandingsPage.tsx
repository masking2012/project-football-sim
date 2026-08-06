import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import {
  fetchLeagueFixtures,
  fetchLeagueStandings,
  fetchLeagues,
  fetchSeasons,
  type LeagueFixtureDto,
  type TeamStandingDto,
} from '../api/footballApi';
import { useAuth } from '../context/AuthContext';
import { useGame } from '../context/GameContext';

export function LeagueStandingsPage() {
  const { leagueId } = useParams<{ leagueId: string }>();
  const { gameId, startNewGame } = useGame();
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [standings, setStandings] = useState<TeamStandingDto[]>([]);
  const [fixtures, setFixtures] = useState<LeagueFixtureDto[]>([]);
  const [leagueTitle, setLeagueTitle] = useState('League standings');
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const parsedLeagueId = Number(leagueId);
    if (!Number.isInteger(parsedLeagueId) || parsedLeagueId < 1) {
      setError('That league could not be found.');
      setIsLoading(false);
      return;
    }

    let isCurrent = true;
    setIsLoading(true);
    setError(null);

    async function loadStandings() {
      const activeGameId = gameId ?? await startNewGame();
      const [leagues, seasons] = await Promise.all([
        fetchLeagues(),
        fetchSeasons(activeGameId),
      ]);
      const league = leagues.find((item) => item.id === parsedLeagueId);
      const season = seasons.find((item) => item.isCurrent) ?? seasons[0];
      if (!league || !season) {
        throw new Error('League or season not found.');
      }

      const [loadedStandings, loadedFixtures] = await Promise.all([
        fetchLeagueStandings(activeGameId, season.id, parsedLeagueId),
        fetchLeagueFixtures(activeGameId, season.id, parsedLeagueId),
      ]);

      if (!isCurrent) return;
      const seasonStartYear = new Date(season.startDate).getUTCFullYear();
      const seasonEndYear = new Date(season.endDate).getUTCFullYear();
      setLeagueTitle(`${league.name} ${seasonStartYear} ${seasonEndYear}`);
      setStandings(loadedStandings);
      setFixtures(loadedFixtures);
    }

    void loadStandings()
      .catch((requestError: unknown) => {
        if (!isCurrent) return;
        if (requestError instanceof Error && requestError.message === 'SESSION_EXPIRED') {
          logout();
          navigate('/login', { replace: true });
        } else {
          setError('Could not load the league standings. Please try again.');
        }
      })
      .finally(() => {
        if (isCurrent) setIsLoading(false);
      });

    return () => {
      isCurrent = false;
    };
  }, [gameId, leagueId, logout, navigate, startNewGame]);

  const sortedStandings = [...standings].sort((left, right) => left.position - right.position);
  const fixturesByRound = [...fixtures]
    .sort((left, right) => left.round - right.round || Date.parse(left.date) - Date.parse(right.date))
    .reduce<Map<number, LeagueFixtureDto[]>>((rounds, fixture) => {
      const roundFixtures = rounds.get(fixture.round) ?? [];
      roundFixtures.push(fixture);
      rounds.set(fixture.round, roundFixtures);
      return rounds;
    }, new Map());

  return (
    <main className="app-main standings-page">
      <div className="standings-heading">
        <div>
          <p className="standings-eyebrow">Competition centre</p>
          <h2>{leagueTitle}</h2>
        </div>
        <Link className="standings-back-link" to="/home">← Back to dashboard</Link>
      </div>

      {isLoading && (
        <section className="standings-state">
          <span className="standings-spinner" aria-hidden="true" />
          <p>{gameId ? 'Preparing the latest table...' : 'Creating your game and preparing the latest table...'}</p>
        </section>
      )}

      {!isLoading && error && (
        <section className="standings-state standings-state--error">
          <span className="standings-state-icon">⚠️</span>
          <h3>Standings unavailable</h3>
          <p>{error}</p>
        </section>
      )}

      {!isLoading && !error && standings.length === 0 && (
        <section className="standings-state standings-state--empty">
          <span className="standings-state-icon">🏟️</span>
          <h3>No table data yet</h3>
          <p>Play some matches to start building the league standings.</p>
        </section>
      )}

      {!isLoading && !error && standings.length > 0 && (
        <section className="standings-card">
          <div className="standings-card-header">
            <div>
              <span className="standings-card-label">Current table</span>
              <h3>{sortedStandings.length} teams</h3>
            </div>
            <span className="standings-live-dot">Live game data</span>
          </div>
          <div className="standings-table-wrap">
            <table className="standings-table">
              <thead>
                <tr>
                  <th scope="col">Pos</th>
                  <th scope="col" className="standings-team-column">Team</th>
                  <th scope="col" title="Played">P</th>
                  <th scope="col" title="Wins">W</th>
                  <th scope="col" title="Draws">D</th>
                  <th scope="col" title="Losses">L</th>
                  <th scope="col" title="Goals for">GF</th>
                  <th scope="col" title="Goals against">GA</th>
                  <th scope="col" title="Goal difference">GD</th>
                  <th scope="col" className="standings-points-column" title="Points">Pts</th>
                </tr>
              </thead>
              <tbody>
                {sortedStandings.map((team) => (
                  <tr key={team.teamId} className={team.position === 1 ? 'standings-row--leader' : undefined}>
                    <td><span className="standings-rank">{team.position}</span></td>
                    <th scope="row" className="standings-team-name">{team.name}</th>
                    <td>{team.wins + team.draws + team.losses}</td>
                    <td>{team.wins}</td>
                    <td>{team.draws}</td>
                    <td>{team.losses}</td>
                    <td>{team.goalsFor}</td>
                    <td>{team.goalsAgainst}</td>
                    <td>{team.goalsFor - team.goalsAgainst > 0 ? '+' : ''}{team.goalsFor - team.goalsAgainst}</td>
                    <td className="standings-points">{team.points}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <p className="standings-legend">P Played · W Wins · D Draws · L Losses · GF Goals for · GA Goals against · GD Goal difference · PTS Points</p>
           <div className="fixtures-section">
             <div className="fixtures-heading">
               <div>
                 <span className="standings-card-label">Season schedule</span>
                 <h3>Fixtures</h3>
               </div>
               <span className="fixtures-count">{fixtures.length} matches</span>
             </div>
             {fixturesByRound.size === 0 ? (
               <p className="fixtures-empty">No fixtures have been scheduled yet.</p>
             ) : (
               <div className="fixtures-rounds">
                 {[...fixturesByRound].map(([round, roundFixtures]) => (
                   <section className="fixtures-round" key={round}>
                     <h4>Round {round}</h4>
                     <div className="fixtures-list">
                       {roundFixtures.map((fixture) => (
                         <div className="fixture" key={fixture.id}>
                           <time className="fixture-date" dateTime={fixture.date}>
                             {formatFixtureDate(fixture.date)}
                           </time>
                           <div className="fixture-teams">
                             <div className="fixture-team-home">{fixture.homeTeamName}</div>
                             <div className="fixture-score" aria-label="Fixture score">
                                 {fixture.homeTeamScore === null || fixture.awayTeamScore === null
                                     ? '- : -'
                                     : `${fixture.homeTeamScore} : ${fixture.awayTeamScore}`}
                             </div>
                             <div className="fixture-team-away">{fixture.awayTeamName}</div>
                           </div>
                         </div>
                       ))}
                     </div>
                   </section>
                 ))}
               </div>
             )}
           </div>
        </section>
      )}
    </main>
  );
}

function formatFixtureDate(date: string): string {
    const d = new Date(date);
    const day = String(d.getDate()).padStart(2, '0');
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const year = String(d.getFullYear()).slice(-2);
    return `${day}.${month}.${year}`;
}
