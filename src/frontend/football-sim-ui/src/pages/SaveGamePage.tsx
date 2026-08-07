import { useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Loader } from '../components/Loader';
import { useGame } from '../context/GameContext';
import { ConfirmationModal } from '../components/ConfirmationModal';

const SLOT_IDS = [1, 2, 3];

function formatCreatedDate(value: string): string {
  return new Date(value).toLocaleString('en-US', {
    dateStyle: 'medium',
    timeStyle: 'short',
  });
}

export function SaveGamePage() {
  const { gameId, saves, isLoading, error, loadGames, saveCurrentGame } = useGame();
  const [selectedSlotId, setSelectedSlotId] = useState(1);
  const [slotName, setSlotName] = useState('');
  const [saveError, setSaveError] = useState<string | null>(null);
  const [saveSuccess, setSaveSuccess] = useState<string | null>(null);
  const [isSaving, setIsSaving] = useState(false);
  const [showOverwriteConfirmation, setShowOverwriteConfirmation] = useState(false);
  const navigate = useNavigate();

  const savesBySlot = useMemo(() => {
    return new Map(saves.map((save) => [save.slotId, save]));
  }, [saves]);

  useEffect(() => {
    const selectedSave = savesBySlot.get(selectedSlotId);
    setSlotName(selectedSave?.name ?? '');
  }, [selectedSlotId, savesBySlot]);

  function handleSave() {
    const name = slotName.trim();
    if (!name || !gameId) return;

    if (savesBySlot.has(selectedSlotId)) {
      setShowOverwriteConfirmation(true);
      return;
    }

    void saveSelectedGame(name);
  }

  async function saveSelectedGame(name: string) {
    setShowOverwriteConfirmation(false);
    setIsSaving(true);
    setSaveError(null);
    setSaveSuccess(null);
    try {
      await saveCurrentGame(selectedSlotId, name);
      await loadGames();
      setSaveSuccess(`Game was saved successfully to slot ${selectedSlotId}.`);
    } catch (saveRequestError) {
      setSaveError(saveRequestError instanceof Error ? saveRequestError.message : 'Could not save game.');
    } finally {
      setIsSaving(false);
    }
  }

  return (
    <main className="app-main save-game-page">
      <div className="save-game-header">
        <div>
          <h2 className="home-title">Save Game</h2>
          <p className="home-message">Choose a slot and enter a name for your save.</p>
        </div>
        <button type="button" className="game-menu-btn" onClick={() => { setSaveSuccess(null); navigate('/friendly'); }}>
          Back to Friendly
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
              onClick={() => { setSaveSuccess(null); setSelectedSlotId(slotId); }}
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
        <label htmlFor="save-game-name">Slot {selectedSlotId} name</label>
        <input
          id="save-game-name"
          type="text"
          value={slotName}
          maxLength={100}
          placeholder="Enter save game name"
          onChange={(event) => setSlotName(event.target.value)}
          disabled={isSaving || !gameId}
        />
        <button type="button" className="start-btn" onClick={handleSave} disabled={isSaving || !gameId || !slotName.trim()}>
          {isSaving ? '⏳ Saving...' : '💾 Save Game'}
        </button>
      </div>

      {saveSuccess && <div className="success-banner" role="status">{saveSuccess}</div>}
      {(saveError || error) && <div className="error-banner">{saveError || error}</div>}
      {showOverwriteConfirmation && (
        <ConfirmationModal
          title="Overwrite save?"
          message="Are you sure you want to override this save?"
          confirmLabel="Override Save"
          onConfirm={() => { void saveSelectedGame(slotName.trim()); }}
          onCancel={() => setShowOverwriteConfirmation(false)}
        />
      )}
    </main>
  );
}
