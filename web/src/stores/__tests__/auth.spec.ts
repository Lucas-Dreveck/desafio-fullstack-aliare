import { describe, it, expect, beforeEach, vi } from 'vitest'
import type { Mock } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'
import { useAuthStore } from '@/stores/auth'

// ---------------------------------------------------------------------------
// Mocks
// ---------------------------------------------------------------------------

/**
 * vi.mock() replaces an entire module with a fake implementation.
 *
 * When the auth store calls authService.login(), it will call our mock
 * instead of the real HTTP function. This keeps tests fast and isolated
 * — no server needed.
 *
 * The factory function returns the module's "shape" with vi.fn() stubs.
 */
vi.mock('@/services/authService', () => ({
  authService: {
    login: vi.fn(),
    register: vi.fn(),
  },
}))

/**
 * The auth store imports from '@/api/client' which imports '@/router'.
 * The router import triggers side effects (createRouter, createWebHistory)
 * that fail in jsdom. We mock the entire client module to avoid this.
 */
vi.mock('@/api/client', () => ({
  default: {},
  AUTH_TOKEN_KEY: 'auth_token',
}))

// Import AFTER vi.mock() — Vitest hoists mocks to the top of the file
// automatically, so the mock is applied before any import.
import { authService } from '@/services/authService'

const loginMock: Mock = authService.login as Mock

/**
 * Helper: creates a fake JWT with the given payload.
 * Same technique from jwt.spec.ts — we control exp and claims so tests
 * are deterministic.
 */
function createFakeJwt(payload: object): string {
  const header: string = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }))
  const body: string = btoa(JSON.stringify(payload))
  return `${header}.${body}.fake-signature`
}

const FUTURE_EXP: number = Math.floor(Date.now() / 1000) + 3600

const VALID_TOKEN: string = createFakeJwt({
  sub: '1',
  unique_name: 'lucas',
  email: 'lucas@test.com',
  exp: FUTURE_EXP,
  iss: 'api',
  aud: 'client',
})

const EXPIRED_TOKEN: string = createFakeJwt({
  sub: '1',
  unique_name: 'lucas',
  email: 'lucas@test.com',
  exp: 0, // Unix epoch → long expired
  iss: 'api',
  aud: 'client',
})

// ---------------------------------------------------------------------------
// Tests
// ---------------------------------------------------------------------------

describe('useAuthStore', () => {
  beforeEach(() => {
    localStorage.clear()
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  // --- Initialization ---

  it('should start unauthenticated when localStorage is empty', () => {
    const store = useAuthStore()

    expect(store.isAuthenticated).toBe(false)
    expect(store.token).toBeNull()
    expect(store.username).toBe('')
  })

  it('should start unauthenticated when stored token is expired', () => {
    localStorage.setItem('auth_token', EXPIRED_TOKEN)
    setActivePinia(createPinia())

    const store = useAuthStore()

    // The store's loadValidToken() detects expiration and removes it
    expect(store.isAuthenticated).toBe(false)
    expect(localStorage.getItem('auth_token')).toBeNull()
  })

  it('should load a valid token from localStorage on init', () => {
    localStorage.setItem('auth_token', VALID_TOKEN)
    setActivePinia(createPinia())

    const store = useAuthStore()

    expect(store.isAuthenticated).toBe(true)
    expect(store.token).toBe(VALID_TOKEN)
  })

  // --- Login ---

  it('should save token and become authenticated after login', async () => {
    /**
     * mockResolvedValue(x) makes the mock return Promise.resolve(x)
     * when called. Since authService.login is async, we use this
     * instead of mockReturnValue.
     */
    loginMock.mockResolvedValue({ token: VALID_TOKEN })

    const store = useAuthStore()
    await store.login({ email: 'lucas@test.com', password: '123456' })

    expect(store.isAuthenticated).toBe(true)
    expect(store.token).toBe(VALID_TOKEN)
    expect(localStorage.getItem('auth_token')).toBe(VALID_TOKEN)
  })

  it('should propagate login errors to the caller', async () => {
    const error: Error = new Error('Network error')
    loginMock.mockRejectedValue(error)

    const store = useAuthStore()

    // We expect the promise to reject, so we assert with rejects.toThrow()
    await expect(
      store.login({ email: 'lucas@test.com', password: 'wrong' }),
    ).rejects.toThrow('Network error')

    expect(store.isAuthenticated).toBe(false)
  })

  // --- Logout ---

  it('should clear token and become unauthenticated after logout', async () => {
    loginMock.mockResolvedValue({ token: VALID_TOKEN })

    const store = useAuthStore()
    await store.login({ email: 'lucas@test.com', password: '123456' })

    store.logout()

    expect(store.isAuthenticated).toBe(false)
    expect(store.token).toBeNull()
    expect(localStorage.getItem('auth_token')).toBeNull()
  })

  // --- Computed: username ---

  it('should extract username from JWT payload', async () => {
    loginMock.mockResolvedValue({ token: VALID_TOKEN })

    const store = useAuthStore()
    await store.login({ email: 'lucas@test.com', password: '123456' })

    expect(store.username).toBe('lucas')
  })

  // --- extractErrorMessage ---

  it('should extract error message from API error response', () => {
    const store = useAuthStore()

    // Simulate an AxiosError with response.data.error
    const axiosError = {
      response: {
        data: {
          error: 'Email já cadastrado.',
        },
      },
    }

    expect(store.extractErrorMessage(axiosError)).toBe('Email já cadastrado.')
  })

  it('should return fallback message for unknown error shapes', () => {
    const store = useAuthStore()

    expect(store.extractErrorMessage(new Error('something'))).toBe(
      'Ocorreu um erro inesperado. Tente novamente.',
    )
  })

  it('should return fallback message for null/undefined errors', () => {
    const store = useAuthStore()

    expect(store.extractErrorMessage(null)).toBe(
      'Ocorreu um erro inesperado. Tente novamente.',
    )
  })
})
