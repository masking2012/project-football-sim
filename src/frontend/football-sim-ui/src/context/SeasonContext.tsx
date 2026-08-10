import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import { createSeason, fetchSeasons, type CurrentSeasonResponse, type DayDetailsResponse } from '../api/footballApi';
import { useAuth } from './AuthContext';

interface SeasonContextValue {
  currentSeason: CurrentSeasonResponse | null;
  currentDay: DayDetailsResponse | null;
  setCurrentDay: (day: DayDetailsResponse | null) => void;
  isLoading: boolean;
  error: string | null;
  refreshSeason: (gameId: string | null) => Promise<void>;
  startNewSeason: (gameId: string) => Promise<void>;
}

const SeasonContext = createContext<SeasonContextValue | null>(null);

export function SeasonProvider({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, logout } = useAuth();
  const [currentSeason, setCurrentSeason] = useState<CurrentSeasonResponse | null>(null);
  const [currentDay, setCurrentDay] = useState<DayDetailsResponse | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const refreshSeason = useCallback(async (gameId: string | null) => {
    if (!isAuthenticated || !gameId) {
      setCurrentSeason(null);
      setCurrentDay(null);
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const seasons = await fetchSeasons(gameId);
      const season = seasons.find((item) => item.isCurrent) ?? null;
      setCurrentSeason(season);
    } catch (err) {
      if (err instanceof Error && err.message === 'SESSION_EXPIRED') {
        logout();
      } else {
        setError(err instanceof Error ? err.message : 'Failed to fetch season');
      }
    } finally {
      setIsLoading(false);
    }
  }, [isAuthenticated, logout]);

  const startNewSeason = useCallback(async (gameId: string) => {
    setIsLoading(true);
    setError(null);

    try {
      await createSeason(gameId);
      await refreshSeason(gameId);
    } catch (err) {
      if (err instanceof Error && err.message === 'SESSION_EXPIRED') {
        logout();
      } else {
        setError(err instanceof Error ? err.message : 'Failed to create season');
        throw err;
      }
    } finally {
      setIsLoading(false);
    }
  }, [refreshSeason, logout]);

  useEffect(() => {
    if (!isAuthenticated) {
      setCurrentSeason(null);
      setCurrentDay(null);
    }
  }, [isAuthenticated]);

  const value = useMemo<SeasonContextValue>(
    () => ({ currentSeason, currentDay, setCurrentDay, isLoading, error, refreshSeason, startNewSeason }),
    [currentSeason, currentDay, isLoading, error, refreshSeason, startNewSeason],
  );

  return <SeasonContext value={value}>{children}</SeasonContext>;
}

export function useSeason(): SeasonContextValue {
  const ctx = useContext(SeasonContext);
  if (!ctx) throw new Error('useSeason must be used inside <SeasonProvider>');
  return ctx;
}
