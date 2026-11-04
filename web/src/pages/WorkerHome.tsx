import { useAuth } from '../auth/AuthContext'
import { firstName, greeting, shiftName } from '../utils/greeting'

// Phone screen 2 (Home). Placeholder for Week 1: the buttons come alive in Week 2.
export function WorkerHome() {
  const { user, logout } = useAuth()
  if (!user) return null

  return (
    <div className="phone">
      <header className="phone-header">
        <div>
          <p className="muted small">{greeting()},</p>
          <h1>{firstName(user.fullName)}</h1>
          <p className="muted small">
            <span className="dot dot-green" aria-hidden="true" /> {user.siteName ?? 'No site'} · {shiftName()}
          </p>
        </div>
        <button type="button" className="btn-ghost" onClick={logout}>
          Log out
        </button>
      </header>

      <section className="card report-card">
        <h2>Report an incident</h2>
        <p className="muted">Photo, location and severity in under a minute. Works offline.</p>
        <button type="button" className="btn-primary" disabled>
          New report · coming in Week 2
        </button>
      </section>

      <div className="tile-row">
        <button type="button" className="card tile" disabled>
          <span className="dot dot-amber" aria-hidden="true" /> Near miss
        </button>
        <button type="button" className="card tile" disabled>
          <span className="dot dot-red" aria-hidden="true" /> Hazard
        </button>
      </div>

      <section className="card lti">
        <p className="lti-number">—</p>
        <p className="muted small">days without a lost time injury</p>
      </section>

      <section className="card">
        <h2>Recent reports</h2>
        <p className="muted small">Your reports will show here.</p>
      </section>
    </div>
  )
}
