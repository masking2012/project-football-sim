import { BrowserRouter, Routes, Route } from 'react-router-dom';
import './App.css';
import { AuthProvider } from './context/AuthContext';
import { NavBar } from './components/NavBar';
import { SimulatorPage } from './pages/SimulatorPage';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <div className="app">
          <header className="app-header">
            <span className="app-header-icon">⚽</span>
            <h1 className="app-title">Football Simulator</h1>
            <NavBar />
          </header>

          <Routes>
            <Route path="/" element={<SimulatorPage subtitle="Pick two teams and simulate a match" />} />
            <Route path="/friendly" element={<SimulatorPage subtitle="Pick two teams and simulate a friendly match" />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
          </Routes>
        </div>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;

