import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import { createSeason, fetchGameSaves, resetGameSessionCache, saveGame, type GameSave } from '../api/footballApi';
import { useAuth } from './AuthContext';
import { useSeason } from './SeasonContext';

interface GameContextValue {
  gameId: string | null;
  saves: GameSave[];
  isLoading: boolean;
  error: string | null;
  startNewGame: () => Promise<string>;
  loadGames: () => Promise<void>;
  loadGame: (gameId: string) => Promise<void>;
  saveCurrentGame: (slotId: number, name: string) => Promise<void>;
}

const GameContext = createContext<GameContextValue | null>(null);
const GAME_ID_KEY = 'football-sim.game-id';

export function GameProvider({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, logout } = useAuth();
  const { refreshSeason } = useSeason();
  const [gameId, setGameId] = useState<string | null>(() => localStorage.getItem(GAME_ID_KEY));
  const [saves, setSaves] = useState<GameSave[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!isAuthenticated) {
      setGameId(null);
      setSaves([]);
      setError(null);
      localStorage.removeItem(GAME_ID_KEY);
    }
  }, [isAuthenticated]);

  useEffect(() => {
    void refreshSeason(gameId);
  }, [gameId, refreshSeason]);

  const handleError = useCallback((error: unknown) => {
    if (error instanceof Error && error.message === 'SESSION_EXPIRED') {
      logout();
      return;
    }
    setError(error instanceof Error ? error.message : 'Game operation failed.');
    throw error;
  }, [logout]);

  const selectGame = useCallback((id: string) => {
    resetGameSessionCache();
    setGameId(id);
    localStorage.setItem(GAME_ID_KEY, id);
  }, []);

  const startNewGame = useCallback(async () => {
    const newGameId = crypto.randomUUID();
    setIsLoading(true);
    setError(null);
    try {
      await createSeason(newGameId);
      selectGame(newGameId);
      return newGameId;
    } catch (error) {
      handleError(error);
      throw error;
    } finally {
      setIsLoading(false);
    }
  }, [handleError, selectGame]);

  const loadGames = useCallback(async () => {
    setIsLoading(true);
    setError(null);
    try {
      setSaves(await fetchGameSaves());
    } catch (error) {
      handleError(error);
    } finally {
      setIsLoading(false);
    }
  }, [handleError]);

  const loadGame = useCallback(async (id: string) => {
    selectGame(id);
    await refreshSeason(id);
  }, [refreshSeason, selectGame]);

  const saveCurrentGame = useCallback(async (slotId: number, name: string) => {
    if (!gameId) return;
    setIsLoading(true);
    setError(null);
    try {
      await saveGame(gameId, slotId, name);
    } catch (error) {
      handleError(error);
    } finally {
      setIsLoading(false);
    }
  }, [gameId, handleError]);

  const value = useMemo(() => ({
    gameId,
    saves,
    isLoading,
    error,
    startNewGame,
    loadGames,
    loadGame,
    saveCurrentGame,
  }), [gameId, saves, isLoading, error, startNewGame, loadGames, loadGame, saveCurrentGame]);

  return <GameContext value={value}>{children}</GameContext>;
}

export function useGame(): GameContextValue {
  const context = useContext(GameContext);
  if (!context) throw new Error('useGame must be used inside <GameProvider>');
  return context;
}
