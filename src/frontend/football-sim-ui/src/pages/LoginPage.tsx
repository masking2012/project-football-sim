import { type FormEvent, useEffect, useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { loginUser } from '../api/authApi';
import { useAuth } from '../context/AuthContext';

export function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const justRegistered = (location.state as { registered?: boolean } | null)?.registered === true;

  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (justRegistered) {
      window.history.replaceState({}, '');
    }
  }, []);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      const { token } = await loginUser(username, password);
      login(token);
      navigate('/');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Login failed.');
    } finally {
      setLoading(false);
    }
  }

  return (
    <main className="auth-page">
      <div className="auth-card">
        <div className="auth-card-header">
          <span className="auth-icon">🔐</span>
          <h2 className="auth-title">Sign in</h2>
          <p className="auth-subtitle">Welcome back</p>
        </div>

        <form className="auth-form" onSubmit={handleSubmit} noValidate>
          {justRegistered && (
            <div className="success-banner">
              Account created! You can now sign in.
            </div>
          )}
          {error && <div className="error-banner">{error}</div>}

          <div className="auth-field">
            <label className="selector-label" htmlFor="username">Username</label>
            <input
              id="username"
              className="auth-input"
              type="text"
              autoComplete="username"
              required
              value={username}
              onChange={e => setUsername(e.target.value)}
            />
          </div>

          <div className="auth-field">
            <label className="selector-label" htmlFor="password">Password</label>
            <input
              id="password"
              className="auth-input"
              type="password"
              autoComplete="current-password"
              required
              value={password}
              onChange={e => setPassword(e.target.value)}
            />
          </div>

          <button className="simulate-btn auth-submit-btn" type="submit" disabled={loading}>
            {loading ? 'Signing in…' : 'Sign in'}
          </button>
        </form>

        <p className="auth-switch">
          No account? <Link to="/register">Create one</Link>
        </p>
      </div>
    </main>
  );
}
