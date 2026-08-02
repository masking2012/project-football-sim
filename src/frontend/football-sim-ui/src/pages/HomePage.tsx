import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useSeason } from '../context/SeasonContext';
import { Loader } from '../components/Loader';

export function HomePage() {
  const { currentSeason, isLoading, startNewSeason } = useSeason();
  const [creating, setCreating] = useState(false);
  const navigate = useNavigate();

  const handleStartNewGame = async () => {
    setCreating(true);
    try {
      await startNewSeason();
      navigate('/friendly');
    } catch (err) {
      console.error('Failed to start new season:', err);
    } finally {
      setCreating(false);
    }
  };

  if (isLoading) {
    return (
      <main className="app-main">
        <Loader />
      </main>
    );
  }

  if (currentSeason) {
    return (
      <main className="app-main">
        <div className="home-welcome">
          <h2 className="home-title">Welcome back!</h2>
          <p className="home-message">
            Your season is in progress. Continue with friendly matches or explore other features.
          </p>
          <button 
            type="button" 
            className="home-action-btn"
            onClick={() => navigate('/friendly')}
          >
            ⚽ Play Friendly Match
          </button>
        </div>
      </main>
    );
  }

  return (
    <main className="app-main">
      <div className="home-welcome">
        <h2 className="home-title">Welcome to Football Simulator</h2>
        <p className="home-message">
          Start a new season to begin your football management journey!
        </p>
        <button 
          type="button" 
          className="home-start-btn"
          onClick={handleStartNewGame}
          disabled={creating}
        >
          {creating ? '⏳ Starting...' : '🎮 Start New Game'}
        </button>
      </div>
    </main>
  );
}
