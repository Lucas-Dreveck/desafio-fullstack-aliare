import { describe, it, expect, beforeEach, vi } from 'vitest'
import type { Mock } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'
import { useAuthStore } from '@/stores/auth'

vi.mock('@/services/authService', () => ({
  authService: {
    login: vi.fn(),
    register: vi.fn(),
  },
}))

vi.mock('@/api/client', () => ({
  default: {},
  AUTH_TOKEN_KEY: 'auth_token',
}))

import { authService } from '@/services/authService'

const loginMock: Mock = authService.login as Mock

function createFakeJwt(payload: object): string {
  const header: string = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }))
  const body: string = btoa(JSON.stringify(payload))
  return `${header}.${body}.fake-signature`
}

const FUTURE_EXP: number = Math.floor(Date.now() / 1000) + 3600

const VALID_TOKEN: string = createFakeJwt({
  sub: '1',
  unique_name: 'aliare',
  email: 'aliare@test.com',
  exp: FUTURE_EXP,
  iss: 'api',
  aud: 'client',
})

const EXPIRED_TOKEN: string = createFakeJwt({
  sub: '1',
  unique_name: 'aliare',
  email: 'aliare@test.com',
  exp: 0,
  iss: 'api',
  aud: 'client',
})

describe('useAuthStore', () => {
  beforeEach(() => {
    localStorage.clear()
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

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

  it('should save token and become authenticated after login', async () => {
    loginMock.mockResolvedValue({ token: VALID_TOKEN })

    const store = useAuthStore()
    await store.login({ email: 'aliare@test.com', password: '123456' })

    expect(store.isAuthenticated).toBe(true)
    expect(store.token).toBe(VALID_TOKEN)
    expect(localStorage.getItem('auth_token')).toBe(VALID_TOKEN)
  })

  it('should propagate login errors to the caller', async () => {
    const error: Error = new Error('Network error')
    loginMock.mockRejectedValue(error)

    const store = useAuthStore()

    await expect(
      store.login({ email: 'aliare@test.com', password: 'wrong' }),
    ).rejects.toThrow('Network error')

    expect(store.isAuthenticated).toBe(false)
  })

  it('should clear token and become unauthenticated after logout', async () => {
    loginMock.mockResolvedValue({ token: VALID_TOKEN })

    const store = useAuthStore()
    await store.login({ email: 'aliare@test.com', password: '123456' })

    store.logout()

    expect(store.isAuthenticated).toBe(false)
    expect(store.token).toBeNull()
    expect(localStorage.getItem('auth_token')).toBeNull()
  })

  it('should extract username from JWT payload', async () => {
    loginMock.mockResolvedValue({ token: VALID_TOKEN })

    const store = useAuthStore()
    await store.login({ email: 'aliare@test.com', password: '123456' })

    expect(store.username).toBe('aliare')
  })

  it('should extract error message from API error response', () => {
    const store = useAuthStore()

    const axiosError = {
      response: {
        data: { error: 'Email já cadastrado.' },
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
