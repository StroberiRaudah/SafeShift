import type { ReactNode } from 'react'
import { Navigate, useLocation } from 'react-router-dom'
import type { Role } from '../api/types'
import { homePathFor, useAuth } from './AuthContext'

// A guard around a page: not logged in → login page; wrong role → your own home page.
// This is only for a nice experience. The real security is [Authorize] on the API.
export function RequireRole({ roles, children }: { roles: Role[]; children: ReactNode }) {
  const { user } = useAuth()
  const location = useLocation()

  if (!user) return <Navigate to="/login" replace state={{ from: location.pathname }} />
  if (!roles.includes(user.role)) return <Navigate to={homePathFor(user.role)} replace />
  return <>{children}</>
}
