export interface RegisterByCityRequest {
  cityName: string
  stateCode?: string
  countryCode?: string
}

export interface RegisterByCoordinatesRequest {
  latitude: number
  longitude: number
}

export interface WeatherRecordResponse {
  id: number
  cityName: string
  temperature: number
  latitude: number
  longitude: number
  recordedAt: string
}
