import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export function HomeRedirect() {
  const { isAuthenticated } = useAuth();
  return <Navigate to={isAuthenticated ? '/friendly' : '/login'} replace />;
}
