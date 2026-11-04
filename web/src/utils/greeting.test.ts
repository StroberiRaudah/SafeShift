import { describe, expect, it } from 'vitest'
import { firstName, greeting } from './greeting'

describe('greeting', () => {
  it.each([
    [7, 'Good morning'],
    [11, 'Good morning'],
    [12, 'Good afternoon'],
    [17, 'Good afternoon'],
    [18, 'Good evening'],
    [2, 'Good morning'], // night shift at 2am: still "morning"
  ])('at %i:00 says "%s"', (hour, expected) => {
    expect(greeting(new Date(2026, 10, 2, hour))).toBe(expected)
  })

  it('takes the first name only', () => {
    expect(firstName('Aiman Hakim')).toBe('Aiman')
    expect(firstName('  Nurul   Aina ')).toBe('Nurul')
  })
})
