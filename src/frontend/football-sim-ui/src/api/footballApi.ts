import { TOKEN_KEY } from './authApi';

export interface CountryDto {
  id: string;
  name: string;
}

export interface LeagueDto {
  id: number;
  name: string;
  order: number;
  countryId: number;
  teamsCount: number;
  promotionPositions: number[] | null;
  promotionPlayOffPositions: number[] | null;
  relegationPositions: number[] | null;
  relegationPlayOffPositions: number[] | null;
  uefaChampionsLeaguePositions: number[] | null;
  uefaEuropaLeaguePositions: number[] | null;
  uefaConferenceLeaguePositions: number[] | null;
}

export interface TeamStandingDto {
  teamId: number;
  name: string;
  position: number;
  wins: number;
  draws: number;
  losses: number;
  goalsFor: number;
  goalsAgainst: number;
  points: number;
}

export interface LeagueFixtureDto {
  id: string;
  date: string;
  homeTeamId: number;
  awayTeamId: number;
  homeTeamScore: number | null;
  awayTeamScore: number | null;
  round: number;
  homeTeamName: string;
  awayTeamName: string;
}

export interface GameSave {
  gameId: string;
  slotId: number;
  name: string;
  createdAtUtc: string;
}

export interface CreateGameResponse {
  gameId: string;
  firstSeasonId: string;
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

export interface LeagueMatchDto {
  id: string;
  homeTeamId: number;
  homeTeamName: string;
  awayTeamId: number;
  awayTeamName: string;
  homeTeamScore: number | null;
  awayTeamScore: number | null;
  round: number;
  leagueName: string;
  leagueId: number;
  countryId: number;
  countryName: string;
  order: number;
}

export interface CalendarDayResponse {
  date: string;
  dayStatus: 'NotStarted' | 'InProgress' | 'Completed';
  leagueMatches: LeagueMatchDto[];
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
  isCurrent: boolean;
}

export interface CompleteSeasonResponse {
  id: string;
}

export interface GameSave {
  gameId: string;
  slotId: number;
  name: string;
  createdAtUtc: string;
}

const BASE = '/api';
let countriesRequest: Promise<CountryDto[]> | null = null;
let countriesCache: CountryDto[] | null = null;
let leaguesRequest: Promise<LeagueDto[]> | null = null;
let leaguesCache: LeagueDto[] | null = null;
const teamsRequests = new Map<string, Promise<TeamDto[]>>();
const teamsCache = new Map<string, TeamDto[]>();
const standingsRequests = new Map<string, Promise<TeamStandingDto[]>>();
const fixturesRequests = new Map<string, Promise<LeagueFixtureDto[]>>();
const seasonsRequests = new Map<string, Promise<CurrentSeasonResponse[]>>();
const calendarDayRequests = new Map<string, Promise<CalendarDayResponse>>();
const seasonsCache = new Map<string, CurrentSeasonResponse[]>();
let cacheGeneration = 0;

export function resetGameSessionCache(): void {
  cacheGeneration += 1;
  countriesRequest = null;
  countriesCache = null;
  leaguesRequest = null;
  leaguesCache = null;
  teamsRequests.clear();
  teamsCache.clear();
  standingsRequests.clear();
  fixturesRequests.clear();
  seasonsRequests.clear();
  calendarDayRequests.clear();
  seasonsCache.clear();
}

export async function fetchLeagues(): Promise<LeagueDto[]> {
  if (leaguesCache) {
    return leaguesCache;
  }

  if (leaguesRequest) {
    return leaguesRequest;
  }

  const request = (async () => {
    const res = await fetch(`${BASE}/leagues`, { headers: authHeaders() });
    return handleResponse<LeagueDto[]>(res);
  })();
  leaguesRequest = request;
  request.then(
    (leagues) => {
      leaguesCache = leagues;
      clearLeaguesRequest(request);
    },
    () => clearLeaguesRequest(request),
  );
  return request;
}

function clearLeaguesRequest(request: Promise<LeagueDto[]>) {
  if (leaguesRequest === request) {
    leaguesRequest = null;
  }
}

export async function fetchCalendarDay(gameId: string): Promise<CalendarDayResponse> {
  const existingRequest = calendarDayRequests.get(gameId);
  if (existingRequest) {
    return existingRequest;
  }

  const request = (async () => {
    const res = await fetch(`${BASE}/games/${encodeURIComponent(gameId)}/calendar/day`, { headers: authHeaders() });
    return handleResponse<CalendarDayResponse>(res);
  })();
  calendarDayRequests.set(gameId, request);
  request.then(
    () => clearCalendarDayRequest(gameId, request),
    () => clearCalendarDayRequest(gameId, request),
  );
  return request;
}

function clearCalendarDayRequest(gameId: string, request: Promise<CalendarDayResponse>) {
  if (calendarDayRequests.get(gameId) === request) {
    calendarDayRequests.delete(gameId);
  }
}

export async function simulateCalendarDay(gameId: string): Promise<void> {
  const res = await fetch(`${BASE}/games/${encodeURIComponent(gameId)}/calendar/simulate`, {
    method: 'POST',
    headers: authHeaders(),
  });
  await handleVoidResponse(res);
}

export async function advanceCalendarDay(gameId: string): Promise<void> {
  const res = await fetch(`${BASE}/games/${encodeURIComponent(gameId)}/calendar/proceed`, {
    method: 'POST',
    headers: authHeaders(),
  });
  await handleVoidResponse(res);
}

export async function completeSeason(gameId: string, seasonId: string): Promise<CompleteSeasonResponse> {
  const res = await fetch(
    `${BASE}/games/${encodeURIComponent(gameId)}/seasons/${encodeURIComponent(seasonId)}/complete`,
    {
      method: 'POST',
      headers: authHeaders(),
    },
  );
  return handleResponse<CompleteSeasonResponse>(res);
}

async function handleVoidResponse(res: Response): Promise<void> {
  if (res.status === 401) {
    throw new Error('SESSION_EXPIRED');
  }
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || `HTTP ${res.status}`);
  }
}

export async function fetchLeagueStandings(gameId: string, seasonId: string, leagueId: number): Promise<TeamStandingDto[]> {
  const requestKey = `${gameId}:${seasonId}:${leagueId}`;
  const existingRequest = standingsRequests.get(requestKey);
  if (existingRequest) {
    return existingRequest;
  }

  const request = (async () => {
    const res = await fetch(
      `${BASE}/games/${encodeURIComponent(gameId)}/seasons/${encodeURIComponent(seasonId)}/leagues/${leagueId}/standings`,
      {
        headers: authHeaders(),
      },
    );
    return handleResponse<TeamStandingDto[]>(res);
  })();
  standingsRequests.set(requestKey, request);
  request.then(
    () => clearStandingsRequest(requestKey, request),
    () => clearStandingsRequest(requestKey, request),
  );
  return request;
}

function clearStandingsRequest(requestKey: string, request: Promise<TeamStandingDto[]>) {
  if (standingsRequests.get(requestKey) === request) {
    standingsRequests.delete(requestKey);
  }
}

export async function fetchLeagueFixtures(gameId: string, seasonId: string, leagueId: number): Promise<LeagueFixtureDto[]> {
  const requestKey = `${gameId}:${seasonId}:${leagueId}`;
  const existingRequest = fixturesRequests.get(requestKey);
  if (existingRequest) {
    return existingRequest;
  }

  const request = (async () => {
    const res = await fetch(
      `${BASE}/games/${encodeURIComponent(gameId)}/seasons/${encodeURIComponent(seasonId)}/leagues/${leagueId}/fixtures`,
      {
        headers: authHeaders(),
      },
    );
    return handleResponse<LeagueFixtureDto[]>(res);
  })();
  fixturesRequests.set(requestKey, request);
  request.then(
    () => clearFixturesRequest(requestKey, request),
    () => clearFixturesRequest(requestKey, request),
  );
  return request;
}

function clearFixturesRequest(requestKey: string, request: Promise<LeagueFixtureDto[]>) {
  if (fixturesRequests.get(requestKey) === request) {
    fixturesRequests.delete(requestKey);
  }
}

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
  if (countriesCache) {
    return countriesCache;
  }

  if (countriesRequest) {
    return countriesRequest;
  }

  const requestGeneration = cacheGeneration;
  const request = (async () => {
    const res = await fetch(`${BASE}/countries`, { headers: authHeaders() });
    return handleResponse<CountryDto[]>(res);
  })();
  countriesRequest = request;
  request.then(
    (countries) => {
      if (requestGeneration === cacheGeneration) {
        countriesCache = countries;
      }
      clearCountriesRequest(request);
    },
    () => clearCountriesRequest(request),
  );
  return request;
}

function clearCountriesRequest(request: Promise<CountryDto[]>) {
  if (countriesRequest === request) {
    countriesRequest = null;
  }
}

export async function fetchTeams(countryId?: string): Promise<TeamDto[]> {
  const cacheKey = countryId ?? '';
  const cachedTeams = teamsCache.get(cacheKey);
  if (cachedTeams) {
    return cachedTeams;
  }

  const existingRequest = teamsRequests.get(cacheKey);
  if (existingRequest) {
    return existingRequest;
  }

  const requestGeneration = cacheGeneration;
  const request = (async () => {
    const url = countryId ? `${BASE}/teams?countryId=${encodeURIComponent(countryId)}` : `${BASE}/teams`;
    const res = await fetch(url, { headers: authHeaders() });
    return handleResponse<TeamDto[]>(res);
  })();
  teamsRequests.set(cacheKey, request);
  request.then(
    (teams) => {
      if (requestGeneration === cacheGeneration) {
        teamsCache.set(cacheKey, teams);
      }
      clearTeamsRequest(cacheKey, request);
    },
    () => clearTeamsRequest(cacheKey, request),
  );
  return request;
}

function clearTeamsRequest(cacheKey: string, request: Promise<TeamDto[]>) {
  if (teamsRequests.get(cacheKey) === request) {
    teamsRequests.delete(cacheKey);
  }
}

export async function simulateMatch(req: SimulateMatchRequest): Promise<MatchResultResponse> {
  const res = await fetch(`${BASE}/matches/simulate`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json', ...authHeaders() },
    body: JSON.stringify(req),
  });
  return handleResponse<MatchResultResponse>(res);
}

export async function fetchSeasons(gameId: string): Promise<CurrentSeasonResponse[]> {
  const cachedSeasons = seasonsCache.get(gameId);
  if (cachedSeasons) {
    return cachedSeasons;
  }

  const existingRequest = seasonsRequests.get(gameId);
  if (existingRequest) {
    return existingRequest;
  }

  const requestGeneration = cacheGeneration;
  const request = (async () => {
    const res = await fetch(`${BASE}/games/${encodeURIComponent(gameId)}/seasons`, { headers: authHeaders() });
    return handleResponse<CurrentSeasonResponse[]>(res);
  })();
  seasonsRequests.set(gameId, request);
  request.then(
    (seasons) => {
      if (requestGeneration === cacheGeneration) {
        seasonsCache.set(gameId, seasons);
      }
      clearSeasonsRequest(gameId, request);
    },
    () => clearSeasonsRequest(gameId, request),
  );
  return request;
}

function clearSeasonsRequest(gameId: string, request: Promise<CurrentSeasonResponse[]>) {
  if (seasonsRequests.get(gameId) === request) {
    seasonsRequests.delete(gameId);
  }
}

export async function createGame(): Promise<string> {
  const res = await fetch(`${BASE}/games`, {
    method: 'POST',
    headers: authHeaders(),
  });
  if (res.status === 401) {
    throw new Error('SESSION_EXPIRED');
  }
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || `HTTP ${res.status}`);
  }

  const response = await handleResponse<CreateGameResponse>(res);
  return response.gameId;
}

export async function fetchGameSaves(): Promise<GameSave[]> {
  const res = await fetch(`${BASE}/games`, { headers: authHeaders() });
  return handleResponse<GameSave[]>(res);
}

export async function saveGame(gameId: string, slotId: number, name: string): Promise<void> {
  const res = await fetch(`${BASE}/games/${encodeURIComponent(gameId)}/save`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json', ...authHeaders() },
    body: JSON.stringify({ slotId, name }),
  });
  await handleVoidResponse(res);
}

