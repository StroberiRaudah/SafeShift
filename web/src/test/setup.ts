import '@testing-library/jest-dom/vitest' // adds checks like toBeInTheDocument()
import { cleanup } from '@testing-library/react'
import { afterEach, vi } from 'vitest'

// Start every test with a clean page, no saved login and no fake fetch.
afterEach(() => {
  cleanup()
  localStorage.clear()
  vi.unstubAllGlobals()
})
