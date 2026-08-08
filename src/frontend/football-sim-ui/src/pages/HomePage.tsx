import { useCallback, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { fetchGameEvents, proceedCalendar, simulateGameDay } from '../api/footballApi';
import type { DayDetailsResponse, MatchEventDto } from '../api/footballApi';
import { Loader } from '../components/Loader';
import { useAuth } from '../context/AuthContext';
import { useGame } from '../context/GameContext';

function formatDate(value: string): string {
  return new Date(value).toLocaleDateString('en-US', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  });
}

function MatchEvent({ event }: { event: MatchEventDto }) {
  const score = event.homeTeamScore !== null && event.awayTeamScore !== null
    ? `${event.homeTeamScore} - ${event.awayTeamScore}`
    : '—';

  return (
    <article className="calendar-match">
      <div className="calendar-match-info">
        <strong>{event.leagueName}</strong>
        <span>{event.countryName} · Round {event.round}</span>
      </div>
      <div className="calendar-match-teams">
        <span>Team {event.homeTeamId}</span>
        <b>{score}</b>
        <span>Team {event.awayTeamId}</span>
      </div>
    </article>
  );
}

export function HomePage() {
  const { gameId } = useGame();
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [day, setDay] = useState<DayDetailsResponse | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isActing, setIsActing] = useState(false);
  const [error, setError] = useState<string | null>(null);

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
      setIsLoading(false);
      return;
    }

    setIsLoading(true);
    setError(null);
    try {
      setDay(await fetchGameEvents(gameId));
    } catch (err) {
      handleError(err);
    } finally {
      setIsLoading(false);
    }
  }, [gameId, handleError]);

  useEffect(() => {
    void loadDay();
  }, [loadDay]);

  async function handleDayAction() {
    if (!gameId || !day || isActing) return;

    setIsActing(true);
    setError(null);
    try {
      if (day.dayState === 'NotStarted') {
        await simulateGameDay(gameId);
      } else if (day.dayState === 'Completed') {
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
          <div className="home-calendar-header">
            <div>
              <p className="home-eyebrow">Current day</p>
              <h2 className="home-title">{formatDate(day.date)}</h2>
            </div>
            <span className={`day-state day-state--${day.dayState.toLowerCase()}`}>{day.dayState}</span>
          </div>

          {day.matchEvents.length > 0 ? (
            <div className="calendar-matches">
              {day.matchEvents.map((event) => <MatchEvent key={event.id} event={event} />)}
            </div>
          ) : (
            <p className="home-message">There are no matches scheduled for today.</p>
          )}

          {day.dayState !== 'InProgress' && (
            <button type="button" className="start-btn day-action-btn" onClick={handleDayAction} disabled={isActing}>
              {isActing ? '⏳ Updating day...' : day.dayState === 'NotStarted' ? '▶ Simulate day' : '▶ Proceed to next day'}
            </button>
          )}
        </div>
      )}
      {error && <div className="error-banner">{error}</div>}
    </main>
  );
}
