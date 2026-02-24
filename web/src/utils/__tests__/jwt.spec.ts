import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { decodeJwt, isTokenExpired } from '@/utils/jwt'
import type { JwtPayload } from '@/utils/jwt'

function createFakeJwt(payload: object): string {
  const header: string = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }))
  const body: string = btoa(JSON.stringify(payload))
  const signature: string = 'fake-signature'
  return `${header}.${body}.${signature}`
}

const VALID_PAYLOAD: JwtPayload = {
  sub: '123',
  unique_name: 'aliare',
  email: 'aliare@email.com',
  exp: Math.floor(Date.now() / 1000) + 3600,
  iss: 'weather-api',
  aud: 'weather-client',
}

describe('decodeJwt', () => {
  it('should decode a valid JWT and return the payload', () => {
    const token: string = createFakeJwt(VALID_PAYLOAD)
    const result: JwtPayload | null = decodeJwt(token)

    expect(result).toEqual(VALID_PAYLOAD)
  })

  it('should return null when the token has fewer than 3 parts', () => {
    expect(decodeJwt('part1.part2')).toBeNull()
  })

  it('should return null when the token has more than 3 parts', () => {
    expect(decodeJwt('a.b.c.d')).toBeNull()
  })

  it('should return null for an empty string', () => {
    expect(decodeJwt('')).toBeNull()
  })

  it('should return null when the payload is valid base64 but not JSON', () => {
    const header: string = btoa('{}')
    const payload: string = btoa('not-json')
    const token: string = `${header}.${payload}.signature`

    expect(decodeJwt(token)).toBeNull()
  })
})

describe('isTokenExpired', () => {
  beforeEach(() => {
    vi.useFakeTimers()
    vi.setSystemTime(new Date('2026-01-01T00:00:00Z'))
  })

  afterEach(() => {
    vi.useRealTimers()
  })

  it('should return false when the token expires in the future', () => {
    const futureExp: number = Math.floor(new Date('2026-01-01T01:00:00Z').getTime() / 1000)
    const token: string = createFakeJwt({ ...VALID_PAYLOAD, exp: futureExp })

    expect(isTokenExpired(token)).toBe(false)
  })

  it('should return true when the token is already expired', () => {
    const pastExp: number = Math.floor(new Date('2025-12-31T23:00:00Z').getTime() / 1000)
    const token: string = createFakeJwt({ ...VALID_PAYLOAD, exp: pastExp })

    expect(isTokenExpired(token)).toBe(true)
  })

  it('should not consider the token expired at the exact expiration second', () => {
    const nowInSeconds: number = Math.floor(new Date('2026-01-01T00:00:00Z').getTime() / 1000)
    const token: string = createFakeJwt({ ...VALID_PAYLOAD, exp: nowInSeconds })

    expect(isTokenExpired(token)).toBe(false)
  })

  it('should return true for an invalid token string', () => {
    expect(isTokenExpired('garbage')).toBe(true)
  })
})
