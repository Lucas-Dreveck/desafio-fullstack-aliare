import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import type { AxiosError } from 'axios'
import { setAccessToken, getAccessToken } from '@/api/client'
import { authService } from '@/services/authService'
import type { LoginRequest, RegisterUserRequest } from '@/types/auth'
import type { ApiErrorResponse } from '@/types/api'
import { decodeJwt } from '@/utils/jwt'
import type { JwtPayload } from '@/utils/jwt'

function extractErrorMessage(error: unknown): string {
  if (!error) {
    return 'Ocorreu um erro inesperado. Tente novamente.'
  }

  const axiosError = error as AxiosError<ApiErrorResponse>

  if (axiosError.response?.data?.error) {
    return axiosError.response.data.error
  }

  return 'Ocorreu um erro inesperado. Tente novamente.'
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(getAccessToken())
  const initialized = ref<boolean>(false)

  const isAuthenticated = computed<boolean>(() => token.value !== null)

  const userPayload = computed<JwtPayload | null>(() => {
    if (!token.value) return null
    return decodeJwt(token.value)
  })

  const username = computed<string>(() => userPayload.value?.unique_name ?? '')

  function updateToken(newToken: string | null): void {
    token.value = newToken
    setAccessToken(newToken)
  }

  async function init(): Promise<void> {
    if (initialized.value) return

    try {
      const response = await authService.refresh()
      updateToken(response.token)
    } catch {
      updateToken(null)
    } finally {
      initialized.value = true
    }
  }

  async function login(request: LoginRequest): Promise<void> {
    const response = await authService.login(request)
    updateToken(response.token)
  }

  async function register(request: RegisterUserRequest): Promise<string> {
    const response = await authService.register(request)
    return response.message
  }

  async function logout(): Promise<void> {
    try {
      await authService.logout()
    } catch {
      // Server-side revocation failed, clear locally anyway
    }
    updateToken(null)
  }

  return {
    token,
    initialized,
    isAuthenticated,
    username,
    init,
    login,
    register,
    logout,
    extractErrorMessage,
  }
})
