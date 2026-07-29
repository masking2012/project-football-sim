export interface CountryDto {
  id: string;
  name: string;
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

const BASE = '/api';

async function handleResponse<T>(res: Response): Promise<T> {
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || `HTTP ${res.status}`);
  }
  return res.json() as Promise<T>;
}

export async function fetchCountries(): Promise<CountryDto[]> {
  const res = await fetch(`${BASE}/countries`);
  return handleResponse<CountryDto[]>(res);
}

export async function fetchTeams(countryId?: string): Promise<TeamDto[]> {
  const url = countryId ? `${BASE}/teams?countryId=${countryId}` : `${BASE}/teams`;
  const res = await fetch(url);
  return handleResponse<TeamDto[]>(res);
}

export async function simulateMatch(req: SimulateMatchRequest): Promise<MatchResultResponse> {
  const res = await fetch(`${BASE}/matches/simulate`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(req),
  });
  return handleResponse<MatchResultResponse>(res);
}
