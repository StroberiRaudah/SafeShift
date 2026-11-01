/// <reference types="vitest/config" />
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    // In development, send /api calls to the .NET API, so the browser sees one address.
    proxy: {
      '/api': 'http://localhost:5080',
      '/health': 'http://localhost:5080',
    },
  },
  test: {
    environment: 'jsdom',          // a fake browser for tests
    setupFiles: './src/test/setup.ts',
  },
})
