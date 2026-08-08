import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { useGame } from '../context/GameContext';

export function HomeRedirect() {
  const { isAuthenticated } = useAuth();
  const { gameId } = useGame();
  return <Navigate to={!isAuthenticated ? '/login' : gameId ? '/home' : '/system'} replace />;
}
