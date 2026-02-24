import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import type { AxiosError } from 'axios'
import { AUTH_TOKEN_KEY } from '@/api/client'
import { authService } from '@/services/authService'
import type { LoginRequest, RegisterUserRequest } from '@/types/auth'
import type { ApiErrorResponse } from '@/types/api'

function extractErrorMessage(error: unknown): string {
  const axiosError = error as AxiosError<ApiErrorResponse>

  if (axiosError.response?.data?.error) {
    return axiosError.response.data.error
  }

  return 'Ocorreu um erro inesperado. Tente novamente.'
}

async function loginRequest(request: LoginRequest): Promise<string> {
  const response = await authService.login(request)
  return response.token
}

async function registerRequest(request: RegisterUserRequest): Promise<string> {
  const response = await authService.register(request)
  return response.message
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem(AUTH_TOKEN_KEY))

  const isAuthenticated = computed<boolean>(() => token.value !== null)

  function setToken(newToken: string): void {
    token.value = newToken
    localStorage.setItem(AUTH_TOKEN_KEY, newToken)
  }

  function clearToken(): void {
    token.value = null
    localStorage.removeItem(AUTH_TOKEN_KEY)
  }

  async function login(request: LoginRequest): Promise<void> {
    const tokenValue: string = await loginRequest(request)
    setToken(tokenValue)
  }

  function logout(): void {
    clearToken()
  }

  return {
    token,
    isAuthenticated,
    login,
    register: registerRequest,
    logout,
    extractErrorMessage,
  }
})
