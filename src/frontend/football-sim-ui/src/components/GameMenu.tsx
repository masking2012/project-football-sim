import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useGame } from '../context/GameContext';

export function GameMenu() {
  const { gameId, loadGames, isLoading } = useGame();
  const [message, setMessage] = useState<string | null>(null);
  const [openingSaveScreen, setOpeningSaveScreen] = useState(false);
  const navigate = useNavigate();

  async function handleSave() {
    setMessage(null);
    setOpeningSaveScreen(true);
    try {
      await loadGames();
      navigate('/save-game');
    } catch (error) {
      setMessage(error instanceof Error ? error.message : 'Could not load saved games.');
    } finally {
      setOpeningSaveScreen(false);
    }
  }

  return (
    <>
      <div className="game-menu">
        <button type="button" className="game-menu-btn" onClick={handleSave} disabled={!gameId || isLoading || openingSaveScreen}>
          {openingSaveScreen ? '⏳ Loading slots...' : '💾 Save Game'}
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
