import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { useGame } from '../context/GameContext';

interface ProtectedRouteProps {
  gameRequired?: boolean;
  children?: React.ReactNode;
}

export function ProtectedRoute({ gameRequired = false, children }: ProtectedRouteProps) {
  const { isAuthenticated } = useAuth();
  const { gameId } = useGame();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (gameRequired && !gameId) {
    return <Navigate to="/system" replace />;
  }

  return children ?? <Outlet />;
}
