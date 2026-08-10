import { useCallback, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { fetchGameEvents, proceedCalendar, simulateGameDay } from '../api/footballApi';
import type { DayDetailsResponse, MatchEventDto } from '../api/footballApi';
import { Loader } from '../components/Loader';
import { useAuth } from '../context/AuthContext';
import { useGame } from '../context/GameContext';
import { useSeason } from '../context/SeasonContext';

function MatchEvent({ event }: { event: MatchEventDto }) {
  return (
    <div className="fixture today-fixture">
      <div className="fixture-teams">
        <div className="fixture-team-home">{event.homeTeamName}</div>
        <div className="fixture-score" aria-label="Fixture score">
          {event.homeTeamScore === null || event.awayTeamScore === null
            ? '- : -'
            : `${event.homeTeamScore} : ${event.awayTeamScore}`}
        </div>
        <div className="fixture-team-away">{event.awayTeamName}</div>
      </div>
    </div>
  );
}

export function HomePage() {
  const { gameId } = useGame();
  const { logout } = useAuth();
  const navigate = useNavigate();
  const { setCurrentDay } = useSeason();
  const [day, setDay] = useState<DayDetailsResponse | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isActing, setIsActing] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const effectiveDayState = day?.dayState === 'NotStarted' && day.matchEvents.length === 0
    ? 'Completed'
    : day?.dayState;
  const matchesByLeague = new Map<string, { countryName: string; leagueName: string; rounds: Map<number, MatchEventDto[]> }>();
  for (const event of [...(day?.matchEvents ?? [])].sort((left, right) =>
    left.leagueName.localeCompare(right.leagueName) || left.round - right.round || left.id.localeCompare(right.id))) {
    const leagueKey = `${event.countryId}:${event.leagueId}`;
    const league = matchesByLeague.get(leagueKey) ?? {
      countryName: event.countryName,
      leagueName: event.leagueName,
      rounds: new Map<number, MatchEventDto[]>(),
    };
    const roundEvents = league.rounds.get(event.round) ?? [];
    roundEvents.push(event);
    league.rounds.set(event.round, roundEvents);
    matchesByLeague.set(leagueKey, league);
  }

  const handleError = useCallback((err: unknown) => {
    if (err instanceof Error && err.message === 'SESSION_EXPIRED') {
      logout();
      navigate('/login');
      return;
    }
    setError(err instanceof Error ? err.message : 'Could not load the current day.');
  }, [logout, navigate]);

  const loadDay = useCallback(async () => {
    if (!gameId) {
      setDay(null);
      setCurrentDay(null);
      setIsLoading(false);
      return;
    }

    setIsLoading(true);
    setError(null);
    try {
      const loadedDay = await fetchGameEvents(gameId);
      setDay(loadedDay);
      setCurrentDay(loadedDay);
    } catch (err) {
      handleError(err);
    } finally {
      setIsLoading(false);
    }
  }, [gameId, handleError, setCurrentDay]);

  useEffect(() => {
    void loadDay();
  }, [loadDay]);

  async function handleDayAction() {
    if (!gameId || !day || isActing) return;

    setIsActing(true);
    setError(null);
    try {
      if (effectiveDayState === 'NotStarted') {
        await simulateGameDay(gameId);
      } else if (effectiveDayState === 'Completed') {
        await proceedCalendar(gameId);
      }
      await loadDay();
    } catch (err) {
      handleError(err);
    } finally {
      setIsActing(false);
    }
  }

  if (!gameId) {
    return (
      <main className="app-main">
        <div className="home-welcome">
          <h2 className="home-title">Welcome to Football Simulator</h2>
          <p className="home-message">Start a new game or load a saved game to begin.</p>
        </div>
      </main>
    );
  }

  return (
    <main className="app-main">
      {isLoading && <Loader size="large" text="Loading today&apos;s events..." />}
      {!isLoading && day && (
        <div className="home-calendar">
          {day.matchEvents.length > 0 ? (
            <div className="today-fixtures fixtures-section">
              <div className="fixtures-heading">
                <h3>Today&apos;s matches</h3>
                <span className="fixtures-count">{day.matchEvents.length} matches</span>
              </div>
              <div className="today-leagues">
                {[...matchesByLeague].map(([leagueKey, league]) => (
                  <section className="today-league" key={leagueKey}>
                    <h3 className="today-league-title">{league.countryName} - {league.leagueName}</h3>
                    <div className="fixtures-rounds">
                      {[...league.rounds].map(([round, roundEvents]) => (
                        <section className="fixtures-round" key={round}>
                          <h4>Round {round}</h4>
                          <div className="fixtures-list">
                            {[...roundEvents]
                              .sort((left, right) => left.id.localeCompare(right.id))
                              .map((event) => <MatchEvent key={event.id} event={event} />)}
                          </div>
                        </section>
                      ))}
                    </div>
                  </section>
                ))}
              </div>
            </div>
          ) : (
            <p className="home-message">There are no matches scheduled for today.</p>
          )}

          {effectiveDayState !== 'InProgress' && (
            <button type="button" className="start-btn day-action-btn" onClick={handleDayAction} disabled={isActing}>
              {isActing ? '⏳ Updating day...' : effectiveDayState === 'NotStarted' ? '▶ Simulate day' : '▶ Proceed to next day'}
            </button>
          )}
        </div>
      )}
      {error && <div className="error-banner">{error}</div>}
    </main>
  );
}
