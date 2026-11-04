import { createContext, useContext } from 'react'
import type { Role, User } from '../api/types'

// A "context" shares one value with every component, without passing it down by hand.
// Here it shares: who is logged in, plus the login and logout functions.
export interface AuthValue {
  user: User | null
  login: (email: string, password: string) => Promise<User>
  logout: () => void
}

export const AuthContext = createContext<AuthValue | null>(null)

export function useAuth(): AuthValue {
  const value = useContext(AuthContext)
  if (!value) throw new Error('useAuth must be used inside <AuthProvider>')
  return value
}

// Workers use the phone screens; supervisors and admins use the web dashboard.
export function homePathFor(role: Role): string {
  return role === 'Worker' ? '/home' : '/dashboard'
}
