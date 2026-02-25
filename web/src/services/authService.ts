import type { AxiosResponse } from 'axios'
import apiClient from '@/api/client'
import type { LoginRequest, RegisterUserRequest, AuthResponse } from '@/types/auth'
import type { ApiMessageResponse } from '@/types/api'

async function login(request: LoginRequest): Promise<AuthResponse> {
  const response: AxiosResponse<AuthResponse> = await apiClient.post('/auth/login', request)
  return response.data
}

async function register(request: RegisterUserRequest): Promise<ApiMessageResponse> {
  const response: AxiosResponse<ApiMessageResponse> = await apiClient.post('/auth/register', request)
  return response.data
}

async function refresh(): Promise<AuthResponse> {
  const response: AxiosResponse<AuthResponse> = await apiClient.post('/auth/refresh')
  return response.data
}

async function logout(): Promise<void> {
  await apiClient.post('/auth/logout')
}

export const authService = {
  login,
  register,
  refresh,
  logout,
}
