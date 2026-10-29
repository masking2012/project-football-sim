import { TOKEN_KEY } from './authApi';

export const SESSION_EXPIRED = 'SESSION_EXPIRED';

export function isSessionExpiredError(error: unknown): boolean {
  return error instanceof Error && error.message === SESSION_EXPIRED;
}

export function authHeaders(): Record<string, string> {
  const token = localStorage.getItem(TOKEN_KEY);
  return token ? { Authorization: `Bearer ${token}` } : {};
}

export async function toHttpError(res: Response): Promise<Error> {
  const text = await res.text();
  return new Error(text || `HTTP ${res.status}`);
}

export async function ensureSuccess(res: Response): Promise<void> {
  if (res.status === 401) {
    throw new Error(SESSION_EXPIRED);
  }
  if (!res.ok) {
    throw await toHttpError(res);
  }
}

export async function handleResponse<T>(res: Response): Promise<T> {
  await ensureSuccess(res);
  return res.json() as Promise<T>;
}
