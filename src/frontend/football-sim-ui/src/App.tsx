import { useEffect, useState } from 'react';
import './App.css';
import { fetchCountries, fetchTeams, simulateMatch } from './api/footballApi';
import type { CountryDto, MatchResultResponse, TeamDto } from './api/footballApi';
import { CountrySelector } from './components/CountrySelector';
import { TeamSelector } from './components/TeamSelector';
import { MatchResult } from './components/MatchResult';

function App() {
  const [countries, setCountries] = useState<CountryDto[]>([]);
  const [homeCountryId, setHomeCountryId] = useState<string>('');
  const [awayCountryId, setAwayCountryId] = useState<string>('');
  const [homeTeams, setHomeTeams] = useState<TeamDto[]>([]);
  const [awayTeams, setAwayTeams] = useState<TeamDto[]>([]);
  const [homeId, setHomeId] = useState<string>('');
  const [awayId, setAwayId] = useState<string>('');
  const [homeAdvantage, setHomeAdvantage] = useState(true);
  const [result, setResult] = useState<MatchResultResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchCountries()
      .then(setCountries)
      .catch(() => setError('Could not load countries. Is the API running?'));
  }, []);

  useEffect(() => {
    if (!homeCountryId) {
      setHomeTeams([]);
      setHomeId('');
      return;
    }
    fetchTeams(homeCountryId)
      .then(setHomeTeams)
      .catch(() => setError('Could not load home teams. Is the API running?'));
  }, [homeCountryId]);

  useEffect(() => {
    if (!awayCountryId) {
      setAwayTeams([]);
      setAwayId('');
      return;
    }
    fetchTeams(awayCountryId)
      .then(setAwayTeams)
      .catch(() => setError('Could not load away teams. Is the API running?'));
  }, [awayCountryId]);

  async function handleSimulate() {
    if (homeId == null || awayId == null) return;
    setLoading(true);
    setError(null);
    setResult(null);
    try {
      const res = await simulateMatch({ homeTeamId: homeId, awayTeamId: awayId, hasHomeAdvantage: homeAdvantage });
      setResult(res);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Simulation failed.');
    } finally {
      setLoading(false);
    }
  }

  const canSimulate = homeId != null && awayId != null && homeId !== awayId && !loading;

  return (
    <div className="app">
      <header className="app-header">
        <span className="app-header-icon">⚽</span>
        <h1 className="app-title">Football Simulator</h1>
        <p className="app-subtitle">Pick two teams and simulate a friendly match</p>
      </header>

      <main className="app-main">
        <div className="selectors">
          <div className="team-section">
            <CountrySelector
              label="🌍 Home Country"
              countries={countries}
              selectedId={homeCountryId}
              onChange={setHomeCountryId}
            />
            {homeCountryId && (
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
            {awayCountryId && (
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
              {loading ? 'Simulating…' : '▶ Simulate Match'}
            </button>
          </>
        )}

        {error && <div className="error-banner">{error}</div>}

        {result && <MatchResult result={result} />}
      </main>
    </div>
  );
}

export default App;

