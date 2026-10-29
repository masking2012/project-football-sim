import { useEffect, useState } from 'react';
import { fetchTeams, type TeamDto } from '../api/footballApi';

export function useTeamsByCountry(
  countryId: string,
  errorMessage: string,
  onError: (error: unknown, fallback: string) => void,
) {
  const [teams, setTeams] = useState<TeamDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    if (!countryId) {
      setTeams([]);
      setIsLoading(false);
      return;
    }

    let ignore = false;
    setIsLoading(true);
    fetchTeams(countryId)
      .then((loadedTeams) => {
        if (!ignore) setTeams(loadedTeams);
      })
      .catch((error: unknown) => {
        if (!ignore) onError(error, errorMessage);
      })
      .finally(() => {
        if (!ignore) setIsLoading(false);
      });

    return () => {
      ignore = true;
    };
  }, [countryId, errorMessage, onError]);

  return { teams, isLoading };
}
