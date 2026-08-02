import { TOKEN_KEY } from './authApi';

export interface CountryDto {
  id: string;
  name: string;
}

export interface GameSave {
  gameId: string;
  slotId: number;
  name: string;
  createdAtUtc: string;
}

export interface TeamDto {
  id: string;
  name: string;
  attack: number;
  defence: number;
  midfield: number;
}

export interface ScoreDto {
  homeScore: number;
  awayScore: number;
}

export interface MatchResultResponse {
  homeTeam: TeamDto;
  awayTeam: TeamDto;
  regularTime: ScoreDto;
  extraTime: ScoreDto | null;
  penalties: ScoreDto | null;
  finalScore: ScoreDto;
  winner: string;
}

export interface SimulateMatchRequest {
  homeTeamId: string;
  awayTeamId: string;
  hasHomeAdvantage: boolean;
}

export interface CurrentSeasonResponse {
  id: string;
  startDate: string;
  endDate: string;
}

const BASE = '/api';

function authHeaders(): Record<string, string> {
  const token = localStorage.getItem(TOKEN_KEY);
  return token ? { Authorization: `Bearer ${token}` } : {};
}

async function handleResponse<T>(res: Response): Promise<T> {
  if (res.status === 401) {
    throw new Error('SESSION_EXPIRED');
  }
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || `HTTP ${res.status}`);
  }
  return res.json() as Promise<T>;
}

export async function fetchCountries(): Promise<CountryDto[]> {
  const res = await fetch(`${BASE}/countries`, { headers: authHeaders() });
  return handleResponse<CountryDto[]>(res);
}

export async function fetchTeams(countryId?: string): Promise<TeamDto[]> {
  const url = countryId ? `${BASE}/teams?countryId=${countryId}` : `${BASE}/teams`;
  const res = await fetch(url, { headers: authHeaders() });
  return handleResponse<TeamDto[]>(res);
}

export async function simulateMatch(req: SimulateMatchRequest): Promise<MatchResultResponse> {
  const res = await fetch(`${BASE}/matches/simulate`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json', ...authHeaders() },
    body: JSON.stringify(req),
  });
  return handleResponse<MatchResultResponse>(res);
}

export async function getCurrentSeason(): Promise<CurrentSeasonResponse | null> {
  try {
    const res = await fetch(`${BASE}/seasons/current`, { headers: authHeaders() });
    if (res.status === 404) {
      return null;
    }
    return handleResponse<CurrentSeasonResponse>(res);
  } catch (error) {
    if (error instanceof Error && error.message === 'SESSION_EXPIRED') {
      throw error;
    }
    return null;
  }
}

export async function createSeason(gameId = crypto.randomUUID()): Promise<void> {
  const res = await fetch(`${BASE}/seasons`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json', ...authHeaders() },
    body: JSON.stringify({ gameId }),
  });
  if (res.status === 401) {
    throw new Error('SESSION_EXPIRED');
  }
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || `HTTP ${res.status}`);
  }
}

export async function fetchGameSaves(): Promise<GameSave[]> {
  const res = await fetch(`${BASE}/games`, { headers: authHeaders() });
  return handleResponse<GameSave[]>(res);
}

export async function saveGame(gameId: string, slotId: number, name: string): Promise<void> {
  const res = await fetch(`${BASE}/games`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json', ...authHeaders() },
    body: JSON.stringify({ gameId, slotId, name }),
  });
  await handleResponse<unknown>(res);
}

