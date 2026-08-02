import { createContext, useCallback, useContext, useMemo, useState } from 'react';
import { createSeason, fetchGameSaves, saveGame, type GameSave } from '../api/footballApi';
import { useAuth } from './AuthContext';

interface GameContextValue {
  gameId: string | null;
  saves: GameSave[];
  isLoading: boolean;
  error: string | null;
  startNewGame: () => Promise<string>;
  loadGames: () => Promise<void>;
  loadGame: (gameId: string) => void;
  saveCurrentGame: () => Promise<void>;
}

const GAME_ID_KEY = 'football-sim.game-id';
const GameContext = createContext<GameContextValue | null>(null);

export function GameProvider({ children }: { children: React.ReactNode }) {
  const { logout } = useAuth();
  const [gameId, setGameId] = useState<string | null>(() => localStorage.getItem(GAME_ID_KEY));
  const [saves, setSaves] = useState<GameSave[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleError = useCallback((error: unknown) => {
    if (error instanceof Error && error.message === 'SESSION_EXPIRED') {
      logout();
      return;
    }
    setError(error instanceof Error ? error.message : 'Game operation failed.');
    throw error;
  }, [logout]);

  const selectGame = useCallback((id: string) => {
    setGameId(id);
    localStorage.setItem(GAME_ID_KEY, id);
  }, []);

  const startNewGame = useCallback(async () => {
    const newGameId = crypto.randomUUID();
    setIsLoading(true);
    setError(null);
    try {
      await createSeason(newGameId);
      await saveGame(newGameId, 1, 'New Game');
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

  const loadGame = useCallback((id: string) => selectGame(id), [selectGame]);

  const saveCurrentGame = useCallback(async () => {
    if (!gameId) return;
    setIsLoading(true);
    setError(null);
    try {
      await saveGame(gameId, 1, 'Saved Game');
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
