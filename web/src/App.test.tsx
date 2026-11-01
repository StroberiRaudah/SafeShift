import { fireEvent, render, screen } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { describe, expect, it, vi } from 'vitest'
import { App } from './App'
import { AuthProvider } from './auth/AuthProvider'
import type { User } from './api/types'

const worker: User = {
  id: 'u1', fullName: 'Aiman Hakim', email: 'worker@safeshift.demo',
  role: 'Worker', siteId: 1, siteName: 'Platform A',
}
const supervisor: User = { ...worker, id: 'u2', fullName: 'Farid Rahman', role: 'Supervisor' }

// Render the whole app at a given address, like opening that URL in a browser.
function renderAt(path: string) {
  render(
    <MemoryRouter initialEntries={[path]} future={{ v7_startTransition: true, v7_relativeSplatPath: true }}>
      <AuthProvider>
        <App />
      </AuthProvider>
    </MemoryRouter>,
  )
}

// Replace the real network with a fake answer, so tests don't need the API running.
function fakeApi(status: number, body: unknown) {
  vi.stubGlobal('fetch', vi.fn().mockResolvedValue(new Response(JSON.stringify(body), { status })))
}

function logIn(email: string, password: string) {
  fireEvent.change(screen.getByLabelText('Email'), { target: { value: email } })
  fireEvent.change(screen.getByLabelText('Password'), { target: { value: password } })
  fireEvent.click(screen.getByRole('button', { name: 'Log in' }))
}

const in8Hours = new Date(Date.now() + 8 * 3600_000).toISOString()

describe('login and role routing', () => {
  it('sends a logged-out user to the login page', () => {
    renderAt('/dashboard')
    expect(screen.getByRole('button', { name: 'Log in' })).toBeInTheDocument()
  })

  it('takes a worker to the phone home screen', async () => {
    fakeApi(200, { token: 't', expiresAt: in8Hours, user: worker })
    renderAt('/login')
    logIn('worker@safeshift.demo', 'Demo@123')
    expect(await screen.findByRole('heading', { name: 'Aiman' })).toBeInTheDocument()
    expect(screen.getByText('Report an incident')).toBeInTheDocument()
  })

  it('takes a supervisor to the dashboard', async () => {
    fakeApi(200, { token: 't', expiresAt: in8Hours, user: supervisor })
    renderAt('/login')
    logIn('supervisor@safeshift.demo', 'Demo@123')
    expect(await screen.findByText('Needs review', { selector: 'h2' })).toBeInTheDocument()
  })

  it('shows the API message when the password is wrong', async () => {
    fakeApi(401, { message: 'Wrong email or password.' })
    renderAt('/login')
    logIn('worker@safeshift.demo', 'nope')
    expect(await screen.findByRole('alert')).toHaveTextContent('Wrong email or password.')
  })

  it('keeps a worker out of the supervisor dashboard', () => {
    localStorage.setItem('safeshift.session', JSON.stringify({ token: 't', expiresAt: in8Hours, user: worker }))
    renderAt('/dashboard')
    expect(screen.getByText('Report an incident')).toBeInTheDocument()
  })

  it('forgets a session whose token has expired', () => {
    const anHourAgo = new Date(Date.now() - 3600_000).toISOString()
    localStorage.setItem('safeshift.session', JSON.stringify({ token: 't', expiresAt: anHourAgo, user: worker }))
    renderAt('/home')
    expect(screen.getByRole('button', { name: 'Log in' })).toBeInTheDocument()
  })
})
