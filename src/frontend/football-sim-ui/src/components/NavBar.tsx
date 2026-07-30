import { NavLink } from 'react-router-dom';

export function NavBar() {
  return (
    <nav className="app-nav">
      <NavLink to="/" end className={({ isActive }) => 'nav-link' + (isActive ? ' nav-link--active' : '')}>
        ⚽ Simulator
      </NavLink>
      <NavLink to="/friendly" className={({ isActive }) => 'nav-link' + (isActive ? ' nav-link--active' : '')}>
        🤝 Friendly
      </NavLink>
    </nav>
  );
}
