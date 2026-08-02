import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Loader } from '../components/Loader';
import { useGame } from '../context/GameContext';

export function HomePage() {
  const { gameId, saves, isLoading, error, startNewGame, loadGames, loadGame } = useGame();
  const [showSaves, setShowSaves] = useState(false);
  const [actionError, setActionError] = useState<string | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    if (showSaves) loadGames().catch(() => undefined);
  }, [showSaves, loadGames]);

  async function handleStartNewGame() {
    setActionError(null);
    try {
      await startNewGame();
      navigate('/friendly');
    } catch (err) {
      setActionError(err instanceof Error ? err.message : 'Could not start a new game.');
    }
  }

  function handleLoadGame(id: string) {
    loadGame(id);
    navigate('/friendly');
  }

  return (
    <main className="app-main">
      <div className="home-welcome">
        <h2 className="home-title">Football Simulator</h2>
        <p className="home-message">Start a new game or load one of your saved games.</p>
        <div className="home-actions">
          <button type="button" className="home-start-btn" onClick={handleStartNewGame} disabled={isLoading}>
            {isLoading ? '⏳ Starting...' : '🎮 New Game'}
          </button>
          <button type="button" className="home-action-btn home-action-btn--secondary" onClick={() => setShowSaves((value) => !value)}>
            📂 Load Games
          </button>
        </div>

        {showSaves && (
          <div className="save-list">
            {isLoading && <Loader size="small" text="Loading saved games..." />}
            {!isLoading && saves.length === 0 && <p className="home-message">No saved games found.</p>}
            {saves.map((save) => (
              <button key={`${save.gameId}-${save.slotId}`} type="button" className="save-list-item" onClick={() => handleLoadGame(save.gameId)}>
                <span>{save.name}</span>
                <small>{save.gameId}</small>
              </button>
            ))}
          </div>
        )}

        {(actionError || error) && <div className="error-banner">{actionError || error}</div>}
        {gameId && <p className="game-id-footer">Current game: {gameId}</p>}
      </div>
    </main>
  );
}