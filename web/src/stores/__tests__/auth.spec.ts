import { describe, it, expect, beforeEach, vi } from 'vitest'
import type { Mock } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'
import { useAuthStore } from '@/stores/auth'

let mockAccessToken: string | null = null

vi.mock('@/services/authService', () => ({
  authService: {
    login: vi.fn(),
    register: vi.fn(),
    refresh: vi.fn(),
    logout: vi.fn(),
  },
}))

vi.mock('@/api/client', () => ({
  default: {},
  setAccessToken: vi.fn((token: string | null) => {
    mockAccessToken = token
  }),
  getAccessToken: vi.fn(() => mockAccessToken),
}))

import { authService } from '@/services/authService'

const loginMock: Mock = authService.login as Mock
const refreshMock: Mock = authService.refresh as Mock
const logoutMock: Mock = authService.logout as Mock

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

describe('useAuthStore', () => {
  beforeEach(() => {
    mockAccessToken = null
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  it('should start unauthenticated when no token exists in memory', () => {
    const store = useAuthStore()

    expect(store.isAuthenticated).toBe(false)
    expect(store.token).toBeNull()
    expect(store.username).toBe('')
  })

  it('should restore session via silent refresh on init', async () => {
    refreshMock.mockResolvedValue({ token: VALID_TOKEN })

    const store = useAuthStore()
    await store.init()

    expect(refreshMock).toHaveBeenCalledOnce()
    expect(store.isAuthenticated).toBe(true)
    expect(store.token).toBe(VALID_TOKEN)
  })

  it('should remain unauthenticated when silent refresh fails on init', async () => {
    refreshMock.mockRejectedValue(new Error('No refresh token'))

    const store = useAuthStore()
    await store.init()

    expect(store.isAuthenticated).toBe(false)
    expect(store.token).toBeNull()
  })

  it('should only call refresh once even if init is called multiple times', async () => {
    refreshMock.mockResolvedValue({ token: VALID_TOKEN })

    const store = useAuthStore()
    await store.init()
    await store.init()

    expect(refreshMock).toHaveBeenCalledOnce()
  })

  it('should mark initialized after init completes', async () => {
    refreshMock.mockResolvedValue({ token: VALID_TOKEN })

    const store = useAuthStore()
    expect(store.initialized).toBe(false)

    await store.init()

    expect(store.initialized).toBe(true)
  })

  it('should save token and become authenticated after login', async () => {
    loginMock.mockResolvedValue({ token: VALID_TOKEN })

    const store = useAuthStore()
    await store.login({ email: 'aliare@test.com', password: '123456' })

    expect(store.isAuthenticated).toBe(true)
    expect(store.token).toBe(VALID_TOKEN)
    expect(mockAccessToken).toBe(VALID_TOKEN)
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

  it('should call authService.logout and clear token on logout', async () => {
    logoutMock.mockResolvedValue(undefined)
    loginMock.mockResolvedValue({ token: VALID_TOKEN })

    const store = useAuthStore()
    await store.login({ email: 'aliare@test.com', password: '123456' })

    await store.logout()

    expect(logoutMock).toHaveBeenCalledOnce()
    expect(store.isAuthenticated).toBe(false)
    expect(store.token).toBeNull()
    expect(mockAccessToken).toBeNull()
  })

  it('should clear token even when server-side logout fails', async () => {
    logoutMock.mockRejectedValue(new Error('Server error'))
    loginMock.mockResolvedValue({ token: VALID_TOKEN })

    const store = useAuthStore()
    await store.login({ email: 'aliare@test.com', password: '123456' })

    await store.logout()

    expect(store.isAuthenticated).toBe(false)
    expect(store.token).toBeNull()
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
