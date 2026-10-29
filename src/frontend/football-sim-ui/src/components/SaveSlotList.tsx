import type { GameSave } from '../api/footballApi';

const SAVE_SLOT_IDS = [1, 2, 3];

function formatCreatedDate(value: string): string {
  return new Date(value).toLocaleString('en-US', {
    dateStyle: 'medium',
    timeStyle: 'short',
  });
}

interface SaveSlotListProps {
  saves: GameSave[];
  selectedSlotId: number;
  onSelect: (slotId: number) => void;
}

export function SaveSlotList({ saves, selectedSlotId, onSelect }: SaveSlotListProps) {
  const savesBySlot = new Map(saves.map((save) => [save.slotId, save]));

  return (
    <div className="save-slots">
      {SAVE_SLOT_IDS.map((slotId) => {
        const save = savesBySlot.get(slotId);
        return (
          <button
            key={slotId}
            type="button"
            className={`save-slot${selectedSlotId === slotId ? ' save-slot--selected' : ''}`}
            onClick={() => onSelect(slotId)}
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
  );
}
