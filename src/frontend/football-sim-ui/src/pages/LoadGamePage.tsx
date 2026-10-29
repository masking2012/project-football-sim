import { useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Loader } from '../components/Loader';
import { useGame } from '../context/GameContext';
import { ConfirmationModal } from '../components/ConfirmationModal';
import { SaveSlotList } from '../components/SaveSlotList';

export function LoadGamePage() {
  const { gameId, saves, isLoading, error, loadGame } = useGame();
  const [selectedSlotId, setSelectedSlotId] = useState(1);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [showLoadConfirmation, setShowLoadConfirmation] = useState(false);
  const navigate = useNavigate();

  const savesBySlot = useMemo(() => {
    return new Map(saves.map((save) => [save.slotId, save]));
  }, [saves]);

  function requestLoad() {
    const selectedSave = savesBySlot.get(selectedSlotId);
    if (!selectedSave) {
      setLoadError(`Slot ${selectedSlotId} is empty.`);
      return;
    }

    if (gameId) {
      setShowLoadConfirmation(true);
      return;
    }

    void loadSelectedGame();
  }

  async function loadSelectedGame() {
    const selectedSave = savesBySlot.get(selectedSlotId);
    if (!selectedSave) return;
    setShowLoadConfirmation(false);
    setLoadError(null);
    await loadGame(selectedSave.gameId);
    navigate('/home');
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

      <SaveSlotList
        saves={saves}
        selectedSlotId={selectedSlotId}
        onSelect={(slotId) => { setLoadError(null); setSelectedSlotId(slotId); }}
      />

      <div className="save-game-form">
        <label htmlFor="selected-game-name">Selected slot</label>
        <input
          id="selected-game-name"
          type="text"
          value={savesBySlot.get(selectedSlotId)?.name ?? 'Empty slot'}
          readOnly
        />
        <button type="button" className="start-btn" onClick={requestLoad} disabled={isLoading || !savesBySlot.has(selectedSlotId)}>
          📂 Load Game
        </button>
      </div>

      {(loadError || error) && <div className="error-banner">{loadError || error}</div>}
      {showLoadConfirmation && (
        <ConfirmationModal
          title="Load this game?"
          message="Do you really want to load this game? Your current game will be replaced."
          confirmLabel="Load Game"
          onConfirm={loadSelectedGame}
          onCancel={() => setShowLoadConfirmation(false)}
        />
      )}
    </main>
  );
}
