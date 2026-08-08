import { useSeason } from '../context/SeasonContext';
import { useAuth } from '../context/AuthContext';

export function SeasonInfo() {
  const { isAuthenticated } = useAuth();
  const { currentSeason, currentDay } = useSeason();

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
  const currentDayState = currentDay?.dayState === 'NotStarted' && currentDay.matchEvents.length === 0
    ? 'Completed'
    : currentDay?.dayState;
  const currentDayDate = currentDay
    ? new Date(currentDay.date).toLocaleDateString('en-US', {
      weekday: 'long',
      month: 'long',
      day: 'numeric',
      year: 'numeric',
    })
    : 'Loading current day...';

  return (
    <div className="season-info">
      <span className="season-info-label">Current day:</span>
      <span className="season-info-current-day">{currentDayDate}</span>
      {currentDayState && <span className={`day-state day-state--${currentDayState.toLowerCase()}`}>{currentDayState}</span>}
      <span className="season-info-season-range">({startDate} - {endDate})</span>
    </div>
  );
}
