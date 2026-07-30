import { useId } from 'react';
import type { CountryDto } from '../api/footballApi';

interface Props {
  label: string;
  countries: CountryDto[];
  selectedId: string;
  onChange: (id: string) => void;
}

export function CountrySelector({ label, countries, selectedId, onChange }: Props) {
  const selectId = useId();
  return (
    <div className="country-selector">
      <label className="selector-label" htmlFor={selectId}>{label}</label>
      <select
        id={selectId}
        className="selector-select"
        value={selectedId ?? ''}
        onChange={(e) => onChange(e.target.value)}
      >
        <option value="">— Pick a country —</option>
        {countries.map((c) => (
          <option key={c.id} value={c.id}>
            {c.name}
          </option>
        ))}
      </select>
    </div>
  );
}
