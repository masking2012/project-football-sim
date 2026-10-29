const BASE = '/api/auth';

export const TOKEN_KEY = 'auth_token';

export interface AuthResponse {
  token: string;
}

async function ensureSuccess(res: Response): Promise<void> {
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || `HTTP ${res.status}`);
  }
}

async function handleResponse<T>(res: Response): Promise<T> {
  await ensureSuccess(res);
  return res.json() as Promise<T>;
}

export async function registerUser(username: string, password: string): Promise<void> {
  const res = await fetch(`${BASE}/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username, password }),
  });
  await ensureSuccess(res);
}

export async function loginUser(username: string, password: string): Promise<AuthResponse> {
  const res = await fetch(`${BASE}/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username, password }),
  });
  if (res.status === 401) {
    throw new Error('Invalid username or password.');
  }
  return handleResponse<AuthResponse>(res);
}
