import type { User } from './types'

// Empty in development (Vite's proxy forwards /api). In Azure, set VITE_API_URL to the API's address.
const API_URL = import.meta.env.VITE_API_URL ?? ''
const SESSION_KEY = 'safeshift.session'

// What we keep after login, so a page refresh doesn't log you out.
export interface Session {
  token: string
  expiresAt: string
  user: User
}

export function loadSession(): Session | null {
  try {
    const raw = localStorage.getItem(SESSION_KEY)
    if (!raw) return null
    const session = JSON.parse(raw) as Session
    if (new Date(session.expiresAt) <= new Date()) {
      localStorage.removeItem(SESSION_KEY) // the shift is over: log in again
      return null
    }
    return session
  } catch {
    return null
  }
}

export function saveSession(session: Session | null) {
  if (session) localStorage.setItem(SESSION_KEY, JSON.stringify(session))
  else localStorage.removeItem(SESSION_KEY)
}

export class ApiError extends Error {
  status: number
  constructor(status: number, message: string) {
    super(message)
    this.status = status
  }
}

// Every API call goes through here, so the token is added in ONE place.
export async function api<T>(path: string, options: RequestInit = {}): Promise<T> {
  const headers = new Headers(options.headers)
  headers.set('Accept', 'application/json')
  if (options.body) headers.set('Content-Type', 'application/json')

  const token = loadSession()?.token
  if (token) headers.set('Authorization', `Bearer ${token}`)

  const response = await fetch(`${API_URL}${path}`, { ...options, headers })

  if (!response.ok) {
    let message = `Request failed (${response.status})`
    try {
      const body = await response.json()
      if (body?.message) message = body.message
    } catch {
      // the body wasn't JSON; keep the default message
    }
    throw new ApiError(response.status, message)
  }

  return response.status === 204 ? (undefined as T) : ((await response.json()) as T)
}
