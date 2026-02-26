import type { AxiosResponse } from 'axios'
import apiClient from '@/api/client'
import type { RegisterByCityRequest, RegisterByCoordinatesRequest, WeatherRecordResponse } from '@/types/weather'

async function registerByCity(request: RegisterByCityRequest): Promise<WeatherRecordResponse> {
  const response: AxiosResponse<WeatherRecordResponse> = await apiClient.post(
    '/weather/register/by-city',
    request,
  )
  return response.data
}

async function registerByCoordinates(request: RegisterByCoordinatesRequest): Promise<WeatherRecordResponse> {
  const response: AxiosResponse<WeatherRecordResponse> = await apiClient.post(
    '/weather/register/by-coordinates',
    request,
  )
  return response.data
}

async function getHistoryByCity(
  city: string,
  state?: string,
  country?: string,
): Promise<WeatherRecordResponse[]> {
  const response: AxiosResponse<WeatherRecordResponse[]> = await apiClient.get(
    `/weather/history/by-city/${encodeURIComponent(city)}`,
    { params: { state, country } },
  )
  return response.data
}

async function getHistoryByCoordinates(latitude: number, longitude: number): Promise<WeatherRecordResponse[]> {
  const response: AxiosResponse<WeatherRecordResponse[]> = await apiClient.get(
    '/weather/history/by-coordinates',
    { params: { latitude, longitude } },
  )
  return response.data
}

export const weatherService = {
  registerByCity,
  registerByCoordinates,
  getHistoryByCity,
  getHistoryByCoordinates,
}
