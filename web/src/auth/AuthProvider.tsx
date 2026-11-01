import { useState, type ReactNode } from 'react'
import { api, loadSession, saveSession, type Session } from '../api/client'
import type { LoginResponse } from '../api/types'
import { AuthContext } from './AuthContext'

export function AuthProvider({ children }: { children: ReactNode }) {
  // Start from the saved session, so refreshing the page keeps you logged in.
  const [session, setSession] = useState<Session | null>(() => loadSession())

  async function login(email: string, password: string) {
    const response = await api<LoginResponse>('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    })
    const newSession = { token: response.token, expiresAt: response.expiresAt, user: response.user }
    saveSession(newSession)
    setSession(newSession)
    return response.user
  }

  function logout() {
    saveSession(null)
    setSession(null)
  }

  return (
    <AuthContext.Provider value={{ user: session?.user ?? null, login, logout }}>
      {children}
    </AuthContext.Provider>
  )
}
