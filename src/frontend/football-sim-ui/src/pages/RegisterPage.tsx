import { type FormEvent, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { registerUser } from '../api/authApi';

export function RegisterPage() {
  const navigate = useNavigate();

  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [confirm, setConfirm] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);

    if (password !== confirm) {
      setError('Passwords do not match.');
      return;
    }

    setLoading(true);
    try {
      await registerUser(username, password);
      navigate('/login', { state: { registered: true } });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Registration failed.');
    } finally {
      setLoading(false);
    }
  }

  return (
    <main className="auth-page">
      <div className="auth-card">
        <div className="auth-card-header">
          <span className="auth-icon">⚽</span>
          <h2 className="auth-title">Create account</h2>
          <p className="auth-subtitle">Join Football Simulator</p>
        </div>

        <form className="auth-form" onSubmit={handleSubmit} noValidate>
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
              autoComplete="new-password"
              required
              value={password}
              onChange={e => setPassword(e.target.value)}
            />
          </div>

          <div className="auth-field">
            <label className="selector-label" htmlFor="confirm">Confirm password</label>
            <input
              id="confirm"
              className="auth-input"
              type="password"
              autoComplete="new-password"
              required
              value={confirm}
              onChange={e => setConfirm(e.target.value)}
            />
          </div>

          <button className="simulate-btn auth-submit-btn" type="submit" disabled={loading}>
            {loading ? 'Creating account…' : 'Create account'}
          </button>
        </form>

        <p className="auth-switch">
          Already have an account? <Link to="/login">Sign in</Link>
        </p>
      </div>
    </main>
  );
}
