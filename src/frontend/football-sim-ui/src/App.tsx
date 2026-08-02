import { BrowserRouter, Routes, Route } from 'react-router-dom';
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
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { SeasonInfo } from './components/SeasonInfo';

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <SeasonProvider>
          <GameProvider>
            <div className="app">
            <header className="app-header">
              <span className="app-header-icon">⚽</span>
              <h1 className="app-title">Football Simulator</h1>
              <SeasonInfo />
              <NavBar />
            </header>

            <Routes>
              <Route path="/" element={<HomeRedirect />} />
              <Route path="/login" element={<LoginPage />} />
              <Route path="/register" element={<RegisterPage />} />

              <Route element={<ProtectedRoute />}>
                <Route path="/home" element={<HomePage />} />
                <Route path="/friendly" element={<SimulatorPage subtitle="Pick two teams and simulate a friendly match" />} />
                <Route path="/save-game" element={<SaveGamePage />} />
              </Route>

              <Route path="*" element={<HomeRedirect />} />
            </Routes>
            </div>
          </GameProvider>
        </SeasonProvider>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;

