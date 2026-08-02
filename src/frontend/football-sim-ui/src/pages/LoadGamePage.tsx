import { useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Loader } from '../components/Loader';
import { useGame } from '../context/GameContext';

const SLOT_IDS = [1, 2, 3];

function formatCreatedDate(value: string): string {
  return new Date(value).toLocaleString('en-US', {
    dateStyle: 'medium',
    timeStyle: 'short',
  });
}

export function LoadGamePage() {
  const { gameId, saves, isLoading, error, loadGame } = useGame();
  const [selectedSlotId, setSelectedSlotId] = useState(1);
  const [loadError, setLoadError] = useState<string | null>(null);
  const navigate = useNavigate();

  const savesBySlot = useMemo(() => {
    return new Map(saves.map((save) => [save.slotId, save]));
  }, [saves]);

  function handleLoad() {
    const selectedSave = savesBySlot.get(selectedSlotId);
    if (!selectedSave) {
      setLoadError(`Slot ${selectedSlotId} is empty.`);
      return;
    }

    if (gameId && !window.confirm('Do you really want to load this game?')) {
      return;
    }

    setLoadError(null);
    loadGame(selectedSave.gameId);
    navigate('/friendly');
  }

  return (
    <main className="app-main save-game-page">
      <div className="save-game-header">
        <div>
          <h2 className="home-title">Load Game</h2>
          <p className="home-message">Choose a saved game slot to continue.</p>
        </div>
        <button type="button" className="game-menu-btn" onClick={() => navigate('/home')}>
          Back to Home
        </button>
      </div>

      {isLoading && <Loader size="small" text="Loading saved games..." />}

      <div className="save-slots">
        {SLOT_IDS.map((slotId) => {
          const save = savesBySlot.get(slotId);
          return (
            <button
              key={slotId}
              type="button"
              className={`save-slot${selectedSlotId === slotId ? ' save-slot--selected' : ''}`}
              onClick={() => { setLoadError(null); setSelectedSlotId(slotId); }}
            >
              <span className="save-slot-title">Slot {slotId}</span>
              {save ? (
                <>
                  <strong>{save.name}</strong>
                  <small>Game ID: {save.gameId}</small>
                  <small>Saved: {formatCreatedDate(save.createdAtUtc)}</small>
                </>
              ) : (
                <small>Empty slot</small>
              )}
            </button>
          );
        })}
      </div>

      <div className="save-game-form">
        <label htmlFor="selected-game-name">Selected slot</label>
        <input
          id="selected-game-name"
          type="text"
          value={savesBySlot.get(selectedSlotId)?.name ?? 'Empty slot'}
          readOnly
        />
        <button type="button" className="home-start-btn" onClick={handleLoad} disabled={isLoading || !savesBySlot.has(selectedSlotId)}>
          📂 Load Game
        </button>
      </div>

      {(loadError || error) && <div className="error-banner">{loadError || error}</div>}
    </main>
  );
}
