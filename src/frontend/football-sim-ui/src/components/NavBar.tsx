import { useEffect, useMemo, useState } from 'react';
import { NavLink, useLocation, useNavigate } from 'react-router-dom';
import { fetchCountries, fetchLeagues, type CountryDto, type LeagueDto } from '../api/footballApi';
import { useAuth } from '../context/AuthContext';

export function NavBar() {
  const { isAuthenticated, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [countries, setCountries] = useState<CountryDto[]>([]);
  const [leagues, setLeagues] = useState<LeagueDto[]>([]);
  const [isLeaguesOpen, setIsLeaguesOpen] = useState(false);
  const [openCountryId, setOpenCountryId] = useState<string | null>(null);
  const [leaguesError, setLeaguesError] = useState<string | null>(null);

  useEffect(() => {
    if (!isAuthenticated) {
      setCountries([]);
      setLeagues([]);
      setIsLeaguesOpen(false);
      setOpenCountryId(null);
      setLeaguesError(null);
      return;
    }

    let isCurrent = true;
    void Promise.all([fetchCountries(), fetchLeagues()])
      .then(([loadedCountries, loadedLeagues]) => {
        if (!isCurrent) return;
        setCountries(loadedCountries);
        setLeagues(loadedLeagues);
        setLeaguesError(null);
      })
      .catch((error: unknown) => {
        if (!isCurrent) return;
        if (error instanceof Error && error.message === 'SESSION_EXPIRED') {
          logout();
        } else {
          setLeaguesError('Could not load leagues.');
        }
      });

    return () => {
      isCurrent = false;
    };
  }, [isAuthenticated, logout]);

  const leaguesByCountry = useMemo(() => {
    const grouped = new Map<string, LeagueDto[]>();
    for (const league of leagues) {
      const countryId = String(league.countryId);
      const countryLeagues = grouped.get(countryId) ?? [];
      countryLeagues.push(league);
      grouped.set(countryId, countryLeagues);
    }
    for (const countryLeagues of grouped.values()) {
      countryLeagues.sort((left, right) => left.order - right.order || left.name.localeCompare(right.name));
    }
    return grouped;
  }, [leagues]);

  function handleLeagueClick(leagueId: number) {
    setIsLeaguesOpen(false);
    setOpenCountryId(null);
    navigate(`/leagues/${leagueId}`);
  }

  return (
    <nav className="app-nav">
      {isAuthenticated && (
        <>
          <NavLink to="/home" className={({ isActive }) => 'nav-link' + (isActive ? ' nav-link--active' : '')}>
            🏠 Home
          </NavLink>
          <NavLink to="/friendly" className={({ isActive }) => 'nav-link' + (isActive ? ' nav-link--active' : '')}>
            🤝 Friendly
          </NavLink>
          <div
            className="nav-leagues"
            onMouseEnter={() => setIsLeaguesOpen(true)}
            onMouseLeave={() => setIsLeaguesOpen(false)}
          >
            <button
              type="button"
              className={'nav-link nav-leagues-toggle' + (location.pathname.startsWith('/leagues') ? ' nav-link--active' : '')}
              aria-expanded={isLeaguesOpen}
              onClick={() => setIsLeaguesOpen((isOpen) => !isOpen)}
            >
              🏆 Leagues <span className="nav-chevron">⌄</span>
            </button>
            {isLeaguesOpen && (
              <div className="league-menu" role="menu">
                {leaguesError && <p className="league-menu-message">{leaguesError}</p>}
                {!leaguesError && countries.length === 0 && <p className="league-menu-message">No leagues available.</p>}
                {countries.map((country) => {
                  const countryId = String(country.id);
                  const countryLeagues = leaguesByCountry.get(countryId) ?? [];
                  if (countryLeagues.length === 0) return null;

                  return (
                    <div
                      className="league-country"
                      key={countryId}
                      onMouseEnter={() => setOpenCountryId(countryId)}
                    >
                      <button
                        type="button"
                        className="league-country-button"
                        aria-expanded={openCountryId === countryId}
                        onClick={() => setOpenCountryId((openId) => (openId === countryId ? null : countryId))}
                      >
                        <span>{country.name}</span>
                        <span className="league-country-chevron">›</span>
                      </button>
                      {openCountryId === countryId && (
                        <div className="league-list" role="menu">
                          {countryLeagues.map((league) => (
                            <button
                              type="button"
                              role="menuitem"
                              className="league-item"
                              key={league.id}
                              onClick={() => handleLeagueClick(league.id)}
                            >
                              {league.name}
                            </button>
                          ))}
                        </div>
                      )}
                    </div>
                  );
                })}
              </div>
            )}
          </div>
          <NavLink to="/system" className={({ isActive }) => 'nav-link' + (isActive ? ' nav-link--active' : '')}>
            ⚙️ System
          </NavLink>
        </>
      )}

      {!isAuthenticated && (
        <div className="nav-auth">
          <>
            <NavLink to="/login" className={({ isActive }) => 'nav-link' + (isActive ? ' nav-link--active' : '')}>
              Sign in
            </NavLink>
            <NavLink to="/register" className={({ isActive }) => 'nav-link' + (isActive ? ' nav-link--active' : '')}>
              Register
            </NavLink>
          </>
        </div>
      )}
    </nav>
  );
}
