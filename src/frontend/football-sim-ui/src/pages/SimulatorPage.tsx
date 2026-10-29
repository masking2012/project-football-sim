import { useCallback, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { fetchCountries, simulateMatch } from '../api/footballApi';
import type { CountryDto, MatchResultResponse } from '../api/footballApi';
import { CountrySelector } from '../components/CountrySelector';
import { TeamSelector } from '../components/TeamSelector';
import { MatchResult } from '../components/MatchResult';
import { Loader } from '../components/Loader';
import { useAuth } from '../context/AuthContext';
import { useTeamsByCountry } from '../hooks/useTeamsByCountry';
import { isSessionExpiredError } from '../api/httpClient';

interface SimulatorPageProps {
  subtitle?: string;
}

export function SimulatorPage({ subtitle = 'Pick two teams and simulate a match' }: SimulatorPageProps) {
  const { logout } = useAuth();
  const navigate = useNavigate();

  const [countries, setCountries] = useState<CountryDto[]>([]);
  const [homeCountryId, setHomeCountryId] = useState<string>('');
  const [awayCountryId, setAwayCountryId] = useState<string>('');
  const [homeId, setHomeId] = useState<string>('');
  const [awayId, setAwayId] = useState<string>('');
  const [homeAdvantage, setHomeAdvantage] = useState(true);
  const [result, setResult] = useState<MatchResultResponse | null>(null);
  const [loadingCountries, setLoadingCountries] = useState(false);
  const [loadingSimulation, setLoadingSimulation] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleApiError = useCallback((err: unknown, fallback: string) => {
    if (isSessionExpiredError(err)) {
      logout();
      navigate('/login');
      return;
    }
    setError(err instanceof Error ? err.message : fallback);
  }, [logout, navigate]);

  const home = useTeamsByCountry(homeCountryId, 'Could not load home teams. Is the API running?', handleApiError);
  const away = useTeamsByCountry(awayCountryId, 'Could not load away teams. Is the API running?', handleApiError);

  useEffect(() => {
    setHomeId('');
  }, [homeCountryId]);

  useEffect(() => {
    setAwayId('');
  }, [awayCountryId]);

  useEffect(() => {
    setLoadingCountries(true);
    fetchCountries()
      .then(setCountries)
      .catch((err) => handleApiError(err, 'Could not load countries. Is the API running?'))
      .finally(() => setLoadingCountries(false));
  }, [handleApiError]);

  async function handleSimulate() {
    if (!homeId || !awayId) return;
    setLoadingSimulation(true);
    setError(null);
    setResult(null);
    try {
      const res = await simulateMatch({ homeTeamId: homeId, awayTeamId: awayId, hasHomeAdvantage: homeAdvantage });
      setResult(res);
    } catch (e) {
      handleApiError(e, 'Simulation failed.');
    } finally {
      setLoadingSimulation(false);
    }
  }

  const canSimulate = !!homeId && !!awayId && homeId !== awayId && !loadingSimulation;

  return (
    <>
      <p className="app-subtitle">{subtitle}</p>

      <main className="app-main">
        {loadingCountries ? (
          <Loader size="large" text="Loading countries..." />
        ) : (
          <>
            <div className="selectors">
              <div className="team-section">
                <CountrySelector
                  label="🌍 Home Country"
                  countries={countries}
                  selectedId={homeCountryId}
                  onChange={setHomeCountryId}
                />
                {home.isLoading && <Loader size="small" text="Loading teams..." />}
                {homeCountryId && !home.isLoading && (
                  <TeamSelector
                    label="🏠 Home Team"
                    teams={home.teams}
                    selectedId={homeId}
                    disabledId={awayId}
                    onChange={setHomeId}
                  />
                )}
              </div>

              <div className="vs-badge">VS</div>

              <div className="team-section">
                <CountrySelector
                  label="🌍 Away Country"
                  countries={countries}
                  selectedId={awayCountryId}
                  onChange={setAwayCountryId}
                />
                {away.isLoading && <Loader size="small" text="Loading teams..." />}
                {awayCountryId && !away.isLoading && (
                  <TeamSelector
                    label="✈️ Away Team"
                    teams={away.teams}
                    selectedId={awayId}
                    disabledId={homeId}
                    onChange={setAwayId}
                  />
                )}
              </div>
            </div>

            {homeId && awayId && (
              <>
                <div className="options">
                  <label className="checkbox-label">
                    <input
                      type="checkbox"
                      checked={homeAdvantage}
                      onChange={(e) => setHomeAdvantage(e.target.checked)}
                    />
                    Home advantage (+10% attack boost)
                  </label>
                </div>

                <button
                  type="button"
                  className="simulate-btn"
                  disabled={!canSimulate}
                  onClick={handleSimulate}
                >
                  {loadingSimulation ? 'Simulating…' : '▶ Simulate Match'}
                </button>
              </>
            )}

            {loadingSimulation && <Loader size="large" text="Simulating match..." />}
          </>
        )}

        {error && <div className="error-banner">{error}</div>}

        {result && <MatchResult result={result} />}
      </main>
    </>
  );
}
