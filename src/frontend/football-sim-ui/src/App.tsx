import { BrowserRouter, Routes, Route } from 'react-router-dom';
import './App.css';
import { NavBar } from './components/NavBar';
import { SimulatorPage } from './pages/SimulatorPage';

function App() {
  return (
    <BrowserRouter>
      <div className="app">
        <header className="app-header">
          <span className="app-header-icon">⚽</span>
          <h1 className="app-title">Football Simulator</h1>
          <NavBar />
        </header>

        <Routes>
          <Route path="/" element={<SimulatorPage subtitle="Pick two teams and simulate a match" />} />
          <Route path="/friendly" element={<SimulatorPage subtitle="Pick two teams and simulate a friendly match" />} />
        </Routes>
      </div>
    </BrowserRouter>
  );
}

export default App;

