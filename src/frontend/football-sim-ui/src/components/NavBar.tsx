import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export function NavBar() {
  const { isAuthenticated, username, logout } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate('/login');
  }

  return (
    <nav className="app-nav">
      {isAuthenticated && (
        <NavLink to="/friendly" className={({ isActive }) => 'nav-link' + (isActive ? ' nav-link--active' : '')}>
          🤝 Friendly
        </NavLink>
      )}

      <div className="nav-auth">
        {isAuthenticated ? (
          <>
            <span className="nav-user-chip">👤 {username}</span>
            <button className="nav-logout-btn" type="button" onClick={handleLogout}>
              Sign out
            </button>
          </>
        ) : (
          <>
            <NavLink to="/login" className={({ isActive }) => 'nav-link' + (isActive ? ' nav-link--active' : '')}>
              Sign in
            </NavLink>
            <NavLink to="/register" className={({ isActive }) => 'nav-link' + (isActive ? ' nav-link--active' : '')}>
              Register
            </NavLink>
          </>
        )}
      </div>
    </nav>
  );
}
