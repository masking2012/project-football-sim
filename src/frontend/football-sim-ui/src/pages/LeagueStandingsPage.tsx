import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { fetchLeagueStandings, fetchLeagues, fetchSeasons, type TeamStandingDto } from '../api/footballApi';
import { useAuth } from '../context/AuthContext';
import { useGame } from '../context/GameContext';

export function LeagueStandingsPage() {
  const { leagueId } = useParams<{ leagueId: string }>();
  const { gameId, startNewGame } = useGame();
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [standings, setStandings] = useState<TeamStandingDto[]>([]);
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

      const loadedStandings = await fetchLeagueStandings(activeGameId, season.id, parsedLeagueId);

      if (!isCurrent) return;
      const seasonStartYear = new Date(season.startDate).getUTCFullYear();
      const seasonEndYear = new Date(season.endDate).getUTCFullYear();
      setLeagueTitle(`${league.name} ${seasonStartYear} ${seasonEndYear}`);
      setStandings(loadedStandings);
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
              <h3>{standings.length} teams</h3>
            </div>
            <span className="standings-live-dot">Live game data</span>
          </div>
          <div className="standings-table-wrap">
            <table className="standings-table">
              <thead>
                <tr>
                  <th scope="col">#</th>
                  <th scope="col" className="standings-team-column">Team</th>
                  <th scope="col" title="Played">P</th>
                  <th scope="col" title="Wins">W</th>
                  <th scope="col" title="Draws">D</th>
                  <th scope="col" title="Losses">L</th>
                  <th scope="col" title="Goal difference">GD</th>
                  <th scope="col" className="standings-points-column" title="Points">Pts</th>
                </tr>
              </thead>
              <tbody>
                {standings.map((team, index) => (
                  <tr key={team.teamId} className={index === 0 ? 'standings-row--leader' : undefined}>
                    <td><span className="standings-rank">{index + 1}</span></td>
                    <th scope="row" className="standings-team-name">{team.name}</th>
                    <td>{team.wins + team.draws + team.losses}</td>
                    <td>{team.wins}</td>
                    <td>{team.draws}</td>
                    <td>{team.losses}</td>
                    <td>{team.goalsFor - team.goalsAgainst > 0 ? '+' : ''}{team.goalsFor - team.goalsAgainst}</td>
                    <td className="standings-points">{team.points}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <p className="standings-legend">P Played · W Wins · D Draws · L Losses · GD Goal difference</p>
        </section>
      )}
    </main>
  );
}
