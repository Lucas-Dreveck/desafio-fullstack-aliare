export interface RegisterByCityRequest {
  city: string
  state?: string
  country?: string
}

export interface RegisterByCoordinatesRequest {
  latitude: number
  longitude: number
}

export interface WeatherRecordResponse {
  id: number
  city: string
  country: string | null
  state: string | null
  temperature: number
  latitude: number
  longitude: number
  recordedAt: string
}
