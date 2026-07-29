import { useId } from 'react';
import type { TeamDto } from '../api/footballApi';

interface Props {
  label: string;
  teams: TeamDto[];
  selectedId: number | null;
  disabledId: number | null;
  onChange: (id: number | null) => void;
}

export function TeamSelector({ label, teams, selectedId, disabledId, onChange }: Props) {
  const selectId = useId();
  return (
    <div className="team-selector">
      <label className="selector-label" htmlFor={selectId}>{label}</label>
      <select
        id={selectId}
        className="selector-select"
        value={selectedId ?? ''}
        onChange={(e) => onChange(Number(e.target.value))}
      >
        <option value="">— Pick a team —</option>
        {teams.map((t) => (
          <option key={t.id} value={t.id} disabled={t.id === disabledId}>
            {t.name}
          </option>
        ))}
      </select>
      {selectedId && (
        <div className="team-stats">
          {teams
            .filter((t) => t.id === selectedId)
            .map((t) => (
              <div key={t.id} className="stats-row">
                <Stat label="ATK" value={t.attack} />
                <Stat label="MID" value={t.midfield} />
                <Stat label="DEF" value={t.defence} />
              </div>
            ))}
        </div>
      )}
    </div>
  );
}

function Stat({ label, value }: { label: string; value: number }) {
  return (
    <div className="stat">
      <span className="stat-label">{label}</span>
      <span className="stat-value">{value}</span>
      <div className="stat-bar">
        <div className="stat-fill" style={{ width: `${value}%` }} />
      </div>
    </div>
  );
}
