import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { fetchCountries, fetchTeams, simulateMatch } from '../api/footballApi';
import type { CountryDto, MatchResultResponse, TeamDto } from '../api/footballApi';
import { CountrySelector } from '../components/CountrySelector';
import { TeamSelector } from '../components/TeamSelector';
import { MatchResult } from '../components/MatchResult';
import { Loader } from '../components/Loader';
import { useAuth } from '../context/AuthContext';

interface SimulatorPageProps {
  subtitle?: string;
}

export function SimulatorPage({ subtitle = 'Pick two teams and simulate a match' }: SimulatorPageProps) {
  const { logout } = useAuth();
  const navigate = useNavigate();

  function handleApiError(err: unknown, fallback: string) {
    if (err instanceof Error && err.message === 'SESSION_EXPIRED') {
      logout();
      navigate('/login');
      return;
    }
    setError(err instanceof Error ? err.message : fallback);
  }

  const [countries, setCountries] = useState<CountryDto[]>([]);
  const [homeCountryId, setHomeCountryId] = useState<string>('');
  const [awayCountryId, setAwayCountryId] = useState<string>('');
  const [homeTeams, setHomeTeams] = useState<TeamDto[]>([]);
  const [awayTeams, setAwayTeams] = useState<TeamDto[]>([]);
  const [homeId, setHomeId] = useState<string>('');
  const [awayId, setAwayId] = useState<string>('');
  const [homeAdvantage, setHomeAdvantage] = useState(true);
  const [result, setResult] = useState<MatchResultResponse | null>(null);
  const [loadingCountries, setLoadingCountries] = useState(false);
  const [loadingHomeTeams, setLoadingHomeTeams] = useState(false);
  const [loadingAwayTeams, setLoadingAwayTeams] = useState(false);
  const [loadingSimulation, setLoadingSimulation] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setLoadingCountries(true);
    fetchCountries()
      .then(setCountries)
      .catch((err) => handleApiError(err, 'Could not load countries. Is the API running?'))
      .finally(() => setLoadingCountries(false));
  }, []);

  useEffect(() => {
    if (!homeCountryId) {
      setHomeTeams([]);
      setHomeId('');
      return;
    }
    setLoadingHomeTeams(true);
    fetchTeams(homeCountryId)
      .then(setHomeTeams)
      .catch((err) => handleApiError(err, 'Could not load home teams. Is the API running?'))
      .finally(() => setLoadingHomeTeams(false));
  }, [homeCountryId]);

  useEffect(() => {
    if (!awayCountryId) {
      setAwayTeams([]);
      setAwayId('');
      return;
    }
    setLoadingAwayTeams(true);
    fetchTeams(awayCountryId)
      .then(setAwayTeams)
      .catch((err) => handleApiError(err, 'Could not load away teams. Is the API running?'))
      .finally(() => setLoadingAwayTeams(false));
  }, [awayCountryId]);

  async function handleSimulate() {
    if (homeId == null || awayId == null) return;
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

  const canSimulate = homeId != null && awayId != null && homeId !== awayId && !loadingSimulation;

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
                {loadingHomeTeams && <Loader size="small" text="Loading teams..." />}
                {homeCountryId && !loadingHomeTeams && (
                  <TeamSelector
                    label="🏠 Home Team"
                    teams={homeTeams}
                    selectedId={homeId}
                    disabledId=""
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
                {loadingAwayTeams && <Loader size="small" text="Loading teams..." />}
                {awayCountryId && !loadingAwayTeams && (
                  <TeamSelector
                    label="✈️ Away Team"
                    teams={awayTeams}
                    selectedId={awayId}
                    disabledId=""
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
