import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import { createSeason, getCurrentSeason, type CurrentSeasonResponse } from '../api/footballApi';
import { useAuth } from './AuthContext';

interface SeasonContextValue {
  currentSeason: CurrentSeasonResponse | null;
  isLoading: boolean;
  error: string | null;
  refreshSeason: () => Promise<void>;
  startNewSeason: () => Promise<void>;
}

const SeasonContext = createContext<SeasonContextValue | null>(null);

export function SeasonProvider({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, logout } = useAuth();
  const [currentSeason, setCurrentSeason] = useState<CurrentSeasonResponse | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const refreshSeason = useCallback(async () => {
    if (!isAuthenticated) {
      setCurrentSeason(null);
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const season = await getCurrentSeason();
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

  const startNewSeason = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      await createSeason();
      await refreshSeason();
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
    }
  }, [isAuthenticated]);

  const value = useMemo<SeasonContextValue>(
    () => ({ currentSeason, isLoading, error, refreshSeason, startNewSeason }),
    [currentSeason, isLoading, error, refreshSeason, startNewSeason],
  );

  return <SeasonContext value={value}>{children}</SeasonContext>;
}

export function useSeason(): SeasonContextValue {
  const ctx = useContext(SeasonContext);
  if (!ctx) throw new Error('useSeason must be used inside <SeasonProvider>');
  return ctx;
}
