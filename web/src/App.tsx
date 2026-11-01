import { Navigate, Route, Routes } from 'react-router-dom'
import { homePathFor, useAuth } from './auth/AuthContext'
import { RequireRole } from './auth/RequireRole'
import { LoginPage } from './pages/LoginPage'
import { SupervisorDashboard } from './pages/SupervisorDashboard'
import { WorkerHome } from './pages/WorkerHome'

// Which page shows for which address.
export function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route
        path="/home"
        element={
          <RequireRole roles={['Worker']}>
            <WorkerHome />
          </RequireRole>
        }
      />
      <Route
        path="/dashboard"
        element={
          <RequireRole roles={['Supervisor', 'HseAdmin']}>
            <SupervisorDashboard />
          </RequireRole>
        }
      />
      <Route path="*" element={<GoHome />} />
    </Routes>
  )
}

// Any other address: send you to your own home page, or to login.
function GoHome() {
  const { user } = useAuth()
  return <Navigate to={user ? homePathFor(user.role) : '/login'} replace />
}
