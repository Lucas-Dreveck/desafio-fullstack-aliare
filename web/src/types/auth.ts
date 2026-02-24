export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterUserRequest {
  username: string
  email: string
  password: string
}

export interface AuthResponse {
  token: string
}
