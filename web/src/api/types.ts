// The shapes of the JSON the API sends back. They match the C# records in AuthController.cs.

export type Role = 'Worker' | 'Supervisor' | 'HseAdmin'

export interface User {
  id: string
  fullName: string
  email: string
  role: Role
  siteId: number | null
  siteName: string | null
}

export interface LoginResponse {
  token: string
  expiresAt: string // ISO date, e.g. "2026-11-02T14:00:00Z"
  user: User
}
