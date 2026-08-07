import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useGame } from '../context/GameContext';
import { ConfirmationModal } from '../components/ConfirmationModal';

export function SystemPage() {
  const { gameId, isLoading, error, startNewGame, loadGames } = useGame();
  const [actionError, setActionError] = useState<string | null>(null);
  const [showNewGameConfirmation, setShowNewGameConfirmation] = useState(false);
  const navigate = useNavigate();

  async function handleStartNewGame() {
    setShowNewGameConfirmation(false);
    setActionError(null);
    try {
      await startNewGame();
      navigate('/friendly');
    } catch (err) {
      setActionError(err instanceof Error ? err.message : 'Could not start a new game.');
    }
  }

  async function handleOpenSaveGame() {
    setActionError(null);
    try {
      await loadGames();
      navigate('/save-game');
    } catch (err) {
      setActionError(err instanceof Error ? err.message : 'Could not load saved games.');
    }
  }

  async function handleOpenLoadGame() {
    setActionError(null);
    try {
      await loadGames();
      navigate('/load-game');
    } catch (err) {
      setActionError(err instanceof Error ? err.message : 'Could not load saved games.');
    }
  }

  return (
    <main className="app-main">
      <div className="system-welcome">
        <p className="system-message">Start a new game or load one of your saved games.</p>
        <div className="system-actions">
          <button
            type="button"
            className="start-btn"
            onClick={() => gameId ? setShowNewGameConfirmation(true) : handleStartNewGame()}
            disabled={isLoading}
          >
            {isLoading ? '⏳ Starting...' : '🎮 New Game'}
          </button>
          {gameId && (
            <button type="button" className="action-btn system-action-btn--secondary" onClick={handleOpenSaveGame} disabled={isLoading}>
              {isLoading ? '⏳ Loading slots...' : '💾 Save Game'}
            </button>
          )}
          <button type="button" className="action-btn system-action-btn--secondary" onClick={handleOpenLoadGame} disabled={isLoading}>
            {isLoading ? '⏳ Loading slots...' : '📂 Load Game'}
          </button>
        </div>

        {(actionError || error) && <div className="error-banner">{actionError || error}</div>}
      </div>
      {showNewGameConfirmation && (
        <ConfirmationModal
          title="Start a new game?"
          message="Are you sure you want to start a new game? Your current progress will be lost."
          confirmLabel="Start New Game"
          onConfirm={handleStartNewGame}
          onCancel={() => setShowNewGameConfirmation(false)}
        />
      )}
    </main>
  );
}
