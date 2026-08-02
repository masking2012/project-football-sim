import { useSeason } from '../context/SeasonContext';
import { useAuth } from '../context/AuthContext';

export function SeasonInfo() {
  const { isAuthenticated } = useAuth();
  const { currentSeason } = useSeason();

  if (!isAuthenticated || !currentSeason) {
    return null;
  }

  const startDate = new Date(currentSeason.startDate).toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
  });

  const endDate = new Date(currentSeason.endDate).toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
  });

  return (
    <div className="season-info">
      <span className="season-info-label">Current Season:</span>
      <span className="season-info-dates">
        {startDate} - {endDate}
      </span>
    </div>
  );
}
