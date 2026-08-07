import { BrowserRouter, Routes, Route, useNavigate } from 'react-router-dom';
import './App.css';
import { AuthProvider } from './context/AuthContext';
import { SeasonProvider } from './context/SeasonContext';
import { GameProvider } from './context/GameContext';
import { NavBar } from './components/NavBar';
import { HomeRedirect } from './components/HomeRedirect';
import { ProtectedRoute } from './components/ProtectedRoute';
import { HomePage } from './pages/HomePage';
import { SimulatorPage } from './pages/SimulatorPage';
import { SaveGamePage } from './pages/SaveGamePage';
import { LoadGamePage } from './pages/LoadGamePage';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { LeagueStandingsPage } from './pages/LeagueStandingsPage';
import { SeasonInfo } from './components/SeasonInfo';
import { useGame } from './context/GameContext';
import { useAuth } from './context/AuthContext';

function GameIdFooter() {
  const { gameId } = useGame();

  if (!gameId) {
    return null;
  }

  return <footer className="app-game-id-footer">Game ID: {gameId}</footer>;
}

function AuthenticatedHeader() {
  const { username, logout } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate('/login');
  }

  return (
    <header className="app-header app-header--authenticated">
      <div className="app-header-row">
        <h1 className="app-title">Football Simulator ⚽</h1>
        <div className="nav-auth app-header-user">
          <span className="nav-user-chip">👤 {username}</span>
          <button className="nav-logout-btn" type="button" onClick={handleLogout}>
            Sign out
          </button>
        </div>
      </div>
      <SeasonInfo />
      <NavBar />
    </header>
  );
}

function AppContent() {
  const { isAuthenticated } = useAuth();

  return (
    <div className="app">
            {isAuthenticated ? (
              <AuthenticatedHeader />
            ) : (
              <header className="app-header">
                <h1 className="app-title">Football Simulator ⚽</h1>
                <SeasonInfo />
                <NavBar />
              </header>
            )}

            <Routes>
              <Route path="/" element={<HomeRedirect />} />
              <Route path="/login" element={<LoginPage />} />
              <Route path="/register" element={<RegisterPage />} />

              <Route element={<ProtectedRoute />}>
                <Route path="/home" element={<HomePage />} />
                <Route path="/friendly" element={<SimulatorPage subtitle="Pick two teams and simulate a friendly match" />} />
                <Route path="/save-game" element={<SaveGamePage />} />
                <Route path="/load-game" element={<LoadGamePage />} />
                 <Route path="/leagues/:leagueId" element={<LeagueStandingsPage />} />
              </Route>

              <Route path="*" element={<HomeRedirect />} />
            </Routes>
            <GameIdFooter />
    </div>
  );
}

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <SeasonProvider>
          <GameProvider>
            <AppContent />
          </GameProvider>
        </SeasonProvider>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;

