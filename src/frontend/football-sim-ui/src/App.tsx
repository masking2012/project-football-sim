import { useEffect, useState } from 'react';
import './App.css';
import { fetchTeams, simulateMatch } from './api/footballApi';
import type { MatchResultResponse, TeamDto } from './api/footballApi';
import { TeamSelector } from './components/TeamSelector';
import { MatchResult } from './components/MatchResult';

function App() {
  const [teams, setTeams] = useState<TeamDto[]>([]);
  const [homeId, setHomeId] = useState<number | null>(null);
  const [awayId, setAwayId] = useState<number | null>(null);
  const [homeAdvantage, setHomeAdvantage] = useState(true);
  const [result, setResult] = useState<MatchResultResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchTeams()
      .then(setTeams)
      .catch(() => setError('Could not load teams. Is the API running?'));
  }, []);

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
          <TeamSelector
            label="🏠 Home Team"
            teams={teams}
            selectedId={homeId}
            disabledId={awayId}
            onChange={setHomeId}
          />

          <div className="vs-badge">VS</div>

          <TeamSelector
            label="✈️ Away Team"
            teams={teams}
            selectedId={awayId}
            disabledId={homeId}
            onChange={setAwayId}
          />
        </div>

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

        {error && <div className="error-banner">{error}</div>}

        {result && <MatchResult result={result} />}
      </main>
    </div>
  );
}

export default App;

