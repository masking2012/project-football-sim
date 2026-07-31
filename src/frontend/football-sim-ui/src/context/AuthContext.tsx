import { createContext, useCallback, useContext, useMemo, useState } from 'react';
import { TOKEN_KEY } from '../api/authApi';

function decodeUsername(token: string): string {
  try {
    const payload = JSON.parse(atob(token.split('.')[1])) as Record<string, unknown>;
    return (payload['unique_name'] ?? payload['sub'] ?? 'User') as string;
  } catch {
    return 'User';
  }
}

interface AuthContextValue {
  token: string | null;
  username: string | null;
  isAuthenticated: boolean;
  login: (token: string) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [token, setToken] = useState<string | null>(() => localStorage.getItem(TOKEN_KEY));
  const [username, setUsername] = useState<string | null>(() => {
    const stored = localStorage.getItem(TOKEN_KEY);
    return stored ? decodeUsername(stored) : null;
  });

  const login = useCallback((newToken: string) => {
    localStorage.setItem(TOKEN_KEY, newToken);
    setToken(newToken);
    setUsername(decodeUsername(newToken));
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem(TOKEN_KEY);
    setToken(null);
    setUsername(null);
  }, []);

  const value = useMemo<AuthContextValue>(
    () => ({ token, username, isAuthenticated: token !== null, login, logout }),
    [token, username, login, logout],
  );

  return <AuthContext value={value}>{children}</AuthContext>;
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used inside <AuthProvider>');
  return ctx;
}
