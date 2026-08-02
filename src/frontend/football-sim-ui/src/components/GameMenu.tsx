import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useGame } from '../context/GameContext';

export function GameMenu() {
  const { gameId, saveCurrentGame, isLoading } = useGame();
  const [message, setMessage] = useState<string | null>(null);
  const navigate = useNavigate();

  async function handleSave() {
    setMessage(null);
    try {
      await saveCurrentGame();
      setMessage('Game saved.');
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Could not save game.');
    }
  }

  return (
    <>
      <div className="game-menu">
        <button type="button" className="game-menu-btn" onClick={handleSave} disabled={!gameId || isLoading}>
          {isLoading ? '⏳ Saving...' : '💾 Save Game'}
        </button>
        <button type="button" className="game-menu-btn" onClick={() => navigate('/home')}>
          📂 Load Game
        </button>
      </div>
      {message && <div className="game-menu-message">{message}</div>}
      {gameId && <div className="game-id-footer">Game ID: {gameId}</div>}
    </>
  );
}
