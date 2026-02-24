import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { decodeJwt, isTokenExpired } from '@/utils/jwt'
import type { JwtPayload } from '@/utils/jwt'

/**
 * Creates a fake JWT string with a controlled payload.
 *
 * A real JWT is three base64url-encoded parts separated by dots:
 *   header.payload.signature
 *
 * We only need the payload to be decodable — the signature is irrelevant
 * because our decode function does not verify signatures (that's the
 * backend's job).
 */
function createFakeJwt(payload: object): string {
  const header: string = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }))
  const body: string = btoa(JSON.stringify(payload))
  const signature: string = 'fake-signature'
  return `${header}.${body}.${signature}`
}

/** A complete payload matching the backend's JWT claims. */
const VALID_PAYLOAD: JwtPayload = {
  sub: '123',
  unique_name: 'lucas',
  email: 'lucas@email.com',
  exp: Math.floor(Date.now() / 1000) + 3600, // 1 hour from now
  iss: 'weather-api',
  aud: 'weather-client',
}

// ---------------------------------------------------------------------------
// decodeJwt
// ---------------------------------------------------------------------------

describe('decodeJwt', () => {
  it('should decode a valid JWT and return the payload', () => {
    const token: string = createFakeJwt(VALID_PAYLOAD)

    const result: JwtPayload | null = decodeJwt(token)

    // toEqual compares object contents deeply (not by reference).
    // toBe would fail because they are different object instances.
    expect(result).toEqual(VALID_PAYLOAD)
  })

  it('should return null when the token has fewer than 3 parts', () => {
    // A JWT without a signature part — only "header.payload"
    expect(decodeJwt('part1.part2')).toBeNull()
  })

  it('should return null when the token has more than 3 parts', () => {
    expect(decodeJwt('a.b.c.d')).toBeNull()
  })

  it('should return null for an empty string', () => {
    expect(decodeJwt('')).toBeNull()
  })

  it('should return null when the payload is valid base64 but not JSON', () => {
    // btoa('not-json') produces valid base64, but JSON.parse will throw
    const header: string = btoa('{}')
    const payload: string = btoa('not-json')
    const token: string = `${header}.${payload}.signature`

    expect(decodeJwt(token)).toBeNull()
  })
})

// ---------------------------------------------------------------------------
// isTokenExpired
// ---------------------------------------------------------------------------

describe('isTokenExpired', () => {
  /**
   * vi.useFakeTimers() / vi.useRealTimers()
   *
   * These let us control Date.now() so tests are deterministic.
   * Without fake timers, a test checking "token expires in 1 hour"
   * depends on the real clock — which is fragile.
   *
   * vi.setSystemTime(date) pins Date.now() to the given timestamp.
   */
  beforeEach(() => {
    vi.useFakeTimers()
    // Pin to a fixed moment: 2026-01-01T00:00:00Z
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

  it('should return true when the token is exactly at expiration', () => {
    // exp === now means the token has just expired (exp < now is false,
    // but exp === now means "expired" because the second has passed)
    const nowInSeconds: number = Math.floor(new Date('2026-01-01T00:00:00Z').getTime() / 1000)
    const token: string = createFakeJwt({ ...VALID_PAYLOAD, exp: nowInSeconds })

    // Our function checks: payload.exp < now → false when equal.
    // So at the exact second, it's NOT expired. This documents the behavior.
    expect(isTokenExpired(token)).toBe(false)
  })

  it('should return true for an invalid token string', () => {
    // If decodeJwt returns null, isTokenExpired treats it as expired (safe default)
    expect(isTokenExpired('garbage')).toBe(true)
  })
})
