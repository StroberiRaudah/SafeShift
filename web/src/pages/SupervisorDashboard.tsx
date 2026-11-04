import { useAuth } from '../auth/AuthContext'
import { firstName, greeting } from '../utils/greeting'

const NAV = ['Dashboard', 'Live map', 'Incidents', 'Corrective actions', 'Users']

const SUMMARY = [
  { label: 'Open incidents', dot: 'dot-blue' },
  { label: 'Needs review', dot: 'dot-amber' },
  { label: 'Overdue actions', dot: 'dot-red' },
  { label: 'Days without LTI', dot: 'dot-green' },
]

// Web 1 (HSE Dashboard). Placeholder for Week 1: real numbers arrive in Week 3.
export function SupervisorDashboard() {
  const { user, logout } = useAuth()
  if (!user) return null

  return (
    <div className="dashboard">
      <aside className="card sidebar">
        <div className="brand">
          <img src="/favicon.svg" alt="" width={32} height={32} />
          <strong>SafeShift</strong>
        </div>
        <nav aria-label="Main">
          {NAV.map((item) =>
            item === 'Dashboard' ? (
              <a key={item} href="/dashboard" className="active" aria-current="page">
                {item}
              </a>
            ) : (
              <span key={item} className="nav-soon" title="Coming soon">
                {item}
              </span>
            ),
          )}
        </nav>
        <button type="button" className="btn-ghost" onClick={logout}>
          Log out
        </button>
      </aside>

      <main className="dashboard-main">
        <header>
          <h1>
            {greeting()}, {firstName(user.fullName)}
          </h1>
          <p className="muted">
            {user.role === 'HseAdmin' ? 'HSE Admin' : 'Supervisor'} · {user.siteName ?? 'All sites'}
          </p>
        </header>

        <div className="summary-grid">
          {SUMMARY.map((s) => (
            <section key={s.label} className="card summary">
              <p className="muted small">
                <span className={`dot ${s.dot}`} aria-hidden="true" /> {s.label}
              </p>
              <p className="summary-number">—</p>
            </section>
          ))}
        </div>

        <section className="card">
          <h2>Needs review</h2>
          <p className="muted small">New reports from your site will appear here (Week 3).</p>
        </section>
      </main>
    </div>
  )
}
