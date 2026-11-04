// "Good morning" before 12:00, "Good afternoon" before 18:00, then "Good evening".
export function greeting(now: Date = new Date()): string {
  const hour = now.getHours()
  if (hour < 12) return 'Good morning'
  if (hour < 18) return 'Good afternoon'
  return 'Good evening'
}

export function shiftName(now: Date = new Date()): string {
  const hour = now.getHours()
  if (hour >= 7 && hour < 19) return 'day shift'
  return 'night shift'
    
  }

// "Aiman Hakim" → "Aiman"
export function firstName(fullName: string): string {
  return fullName.trim().split(/\s+/)[0] ?? ''
}
