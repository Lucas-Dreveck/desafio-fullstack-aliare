<script setup lang="ts">
import { ref, computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { weatherService } from '@/services/weatherService'
import HistoryChart from '@/components/HistoryChart.vue'
import type { WeatherRecordResponse } from '@/types/weather'
import type { AxiosError } from 'axios'
import type { ApiErrorResponse } from '@/types/api'

const authStore = useAuthStore()

type TabMode = 'city' | 'coordinates'

// --- Register section ---
const registerTab = ref<TabMode>('city')
const registerLoading = ref<boolean>(false)
const registerError = ref<string>('')
const registerResult = ref<WeatherRecordResponse | null>(null)

const registerCity = ref<string>('')
const registerState = ref<string>('')
const registerCountry = ref<string>('')
const registerLatitude = ref<string>('')
const registerLongitude = ref<string>('')

const latitudeError = computed<string>(() => {
  if (!registerLatitude.value) return ''
  const value: number = Number(registerLatitude.value)
  if (Number.isNaN(value)) return 'Deve ser um número'
  if (value < -90 || value > 90) return 'Deve estar entre -90 e 90'
  return ''
})

const longitudeError = computed<string>(() => {
  if (!registerLongitude.value) return ''
  const value: number = Number(registerLongitude.value)
  if (Number.isNaN(value)) return 'Deve ser um número'
  if (value < -180 || value > 180) return 'Deve estar entre -180 e 180'
  return ''
})

const isRegisterValid = computed<boolean>(() => {
  if (registerTab.value === 'city') {
    return registerCity.value.trim().length > 0
  }
  return (
    registerLatitude.value !== '' &&
    registerLongitude.value !== '' &&
    !latitudeError.value &&
    !longitudeError.value
  )
})

function clearRegister(): void {
  registerError.value = ''
  registerResult.value = null
}

async function handleRegister(): Promise<void> {
  if (!isRegisterValid.value || registerLoading.value) return

  clearRegister()
  registerLoading.value = true

  try {
    if (registerTab.value === 'city') {
      registerResult.value = await weatherService.registerByCity({
        city: registerCity.value.trim(),
        state: registerState.value.trim() || undefined,
        country: registerCountry.value.trim() || undefined,
      })
    } else {
      registerResult.value = await weatherService.registerByCoordinates({
        latitude: Number(registerLatitude.value),
        longitude: Number(registerLongitude.value),
      })
    }
  } catch (error: unknown) {
    registerError.value = extractError(error)
  } finally {
    registerLoading.value = false
  }
}

// --- History section ---
const historyTab = ref<TabMode>('city')
const historyLoading = ref<boolean>(false)
const historyError = ref<string>('')
const historyRecords = ref<WeatherRecordResponse[]>([])
const historySearched = ref<boolean>(false)

const historyCity = ref<string>('')
const historyState = ref<string>('')
const historyCountry = ref<string>('')
const historyLatitude = ref<string>('')
const historyLongitude = ref<string>('')

const historyLatitudeError = computed<string>(() => {
  if (!historyLatitude.value) return ''
  const value: number = Number(historyLatitude.value)
  if (Number.isNaN(value)) return 'Deve ser um número'
  if (value < -90 || value > 90) return 'Deve estar entre -90 e 90'
  return ''
})

const historyLongitudeError = computed<string>(() => {
  if (!historyLongitude.value) return ''
  const value: number = Number(historyLongitude.value)
  if (Number.isNaN(value)) return 'Deve ser um número'
  if (value < -180 || value > 180) return 'Deve estar entre -180 e 180'
  return ''
})

const isHistoryValid = computed<boolean>(() => {
  if (historyTab.value === 'city') {
    return historyCity.value.trim().length > 0
  }
  return (
    historyLatitude.value !== '' &&
    historyLongitude.value !== '' &&
    !historyLatitudeError.value &&
    !historyLongitudeError.value
  )
})

function clearHistory(): void {
  historyError.value = ''
  historyRecords.value = []
  historySearched.value = false
}

async function handleHistory(): Promise<void> {
  if (!isHistoryValid.value || historyLoading.value) return

  historyError.value = ''
  historyLoading.value = true
  historySearched.value = false

  try {
    if (historyTab.value === 'city') {
      historyRecords.value = await weatherService.getHistoryByCity(
        historyCity.value.trim(),
        historyState.value.trim() || undefined,
        historyCountry.value.trim() || undefined,
      )
    } else {
      historyRecords.value = await weatherService.getHistoryByCoordinates(
        Number(historyLatitude.value),
        Number(historyLongitude.value),
      )
    }
    historySearched.value = true
  } catch (error: unknown) {
    historyError.value = extractError(error)
  } finally {
    historyLoading.value = false
  }
}

// --- Shared ---
function extractError(error: unknown): string {
  const axiosError = error as AxiosError<ApiErrorResponse>
  if (axiosError.response?.data?.error) return axiosError.response.data.error
  return 'Ocorreu um erro inesperado. Tente novamente.'
}

function formatDate(isoDate: string): string {
  const date: Date = new Date(isoDate)
  return new Intl.DateTimeFormat('pt-BR', {
    dateStyle: 'short',
    timeStyle: 'short',
  }).format(date)
}

function formatTemperature(value: number): string {
  return `${value.toFixed(1)} °C`
}

function formatCoordinates(lat: number, lon: number): string {
  return `${lat.toFixed(4)}, ${lon.toFixed(4)}`
}
</script>

<template>
  <div class="dashboard-view">
    <div class="page-header">
      <h1>Dashboard</h1>
      <p>Consulte e registre temperaturas por cidade ou coordenadas</p>
    </div>

    <!-- Register Section -->
    <section v-if="authStore.isAuthenticated" class="dashboard-section">
      <h2 class="section-title">Registrar Temperatura</h2>

      <div class="card">
        <div class="tabs">
          <button
            class="tab"
            :class="{ 'tab--active': registerTab === 'city' }"
            @click="registerTab = 'city'; clearRegister()"
          >
            Por Cidade
          </button>
          <button
            class="tab"
            :class="{ 'tab--active': registerTab === 'coordinates' }"
            @click="registerTab = 'coordinates'; clearRegister()"
          >
            Por Coordenadas
          </button>
        </div>

        <form @submit.prevent="handleRegister" class="section-form">
          <template v-if="registerTab === 'city'">
            <div class="form-group">
              <label for="reg-city">Cidade *</label>
              <input
                id="reg-city"
                v-model="registerCity"
                type="text"
                class="input"
                placeholder="Ex: São Paulo"
                @input="clearRegister"
              >
            </div>
            <div class="form-row">
              <div class="form-group">
                <label for="reg-state">Estado</label>
                <input
                  id="reg-state"
                  v-model="registerState"
                  type="text"
                  class="input"
                  placeholder="Ex: Paraná ou PR "
                >
              </div>
              <div class="form-group">
                <label for="reg-country">País</label>
                <input
                  id="reg-country"
                  v-model="registerCountry"
                  type="text"
                  class="input"
                  placeholder="Ex: BR ou Brazil"
                >
              </div>
            </div>
          </template>

          <template v-else>
            <div class="form-row">
              <div class="form-group">
                <label for="reg-lat">Latitude *</label>
                <input
                  id="reg-lat"
                  v-model="registerLatitude"
                  type="text"
                  inputmode="decimal"
                  class="input"
                  placeholder="Ex: -23.5505"
                  @input="clearRegister"
                >
                <span v-if="latitudeError" class="error-text">{{ latitudeError }}</span>
              </div>
              <div class="form-group">
                <label for="reg-lon">Longitude *</label>
                <input
                  id="reg-lon"
                  v-model="registerLongitude"
                  type="text"
                  inputmode="decimal"
                  class="input"
                  placeholder="Ex: -46.6333"
                  @input="clearRegister"
                >
                <span v-if="longitudeError" class="error-text">{{ longitudeError }}</span>
              </div>
            </div>
          </template>

          <button
            type="submit"
            class="btn btn--primary"
            :disabled="!isRegisterValid || registerLoading"
          >
            {{ registerLoading ? 'Registrando...' : 'Registrar' }}
          </button>
        </form>

        <div v-if="registerError" class="alert alert--error mt-md">
          {{ registerError }}
        </div>

        <div v-if="registerResult" class="result-card mt-md">
          <h3 class="result-card__title">Leitura registrada</h3>
          <div class="result-card__grid">
            <div class="result-card__item">
              <span class="result-card__label">Cidade</span>
              <span class="result-card__value">{{ registerResult.city }}</span>
            </div>
            <div class="result-card__item">
              <span class="result-card__label">Estado</span>
              <span class="result-card__value">{{ registerResult.state ?? '—' }}</span>
            </div>
            <div class="result-card__item">
              <span class="result-card__label">País</span>
              <span class="result-card__value">{{ registerResult.country ?? '—' }}</span>
            </div>
            <div class="result-card__item">
              <span class="result-card__label">Temperatura</span>
              <span class="result-card__value result-card__value--highlight">
                {{ formatTemperature(registerResult.temperature) }}
              </span>
            </div>
            <div class="result-card__item">
              <span class="result-card__label">Coordenadas</span>
              <span class="result-card__value">
                {{ formatCoordinates(registerResult.latitude, registerResult.longitude) }}
              </span>
            </div>
            <div class="result-card__item">
              <span class="result-card__label">Data</span>
              <span class="result-card__value">{{ formatDate(registerResult.recordedAt) }}</span>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Login CTA -->
    <section v-else class="dashboard-section">
      <div class="card cta-card text-center">
        <p class="cta-card__text">Faça login para registrar leituras de temperatura</p>
        <RouterLink to="/login" class="btn btn--primary">
          Entrar
        </RouterLink>
      </div>
    </section>

    <!-- History Section -->
    <section class="dashboard-section">
      <h2 class="section-title">Consultar Histórico</h2>

      <div class="card">
        <div class="tabs">
          <button
            class="tab"
            :class="{ 'tab--active': historyTab === 'city' }"
            @click="historyTab = 'city'; clearHistory()"
          >
            Por Cidade
          </button>
          <button
            class="tab"
            :class="{ 'tab--active': historyTab === 'coordinates' }"
            @click="historyTab = 'coordinates'; clearHistory()"
          >
            Por Coordenadas
          </button>
        </div>

        <form @submit.prevent="handleHistory" class="section-form">
          <template v-if="historyTab === 'city'">
            <div class="form-group">
              <label for="hist-city">Cidade</label>
              <input
                id="hist-city"
                v-model="historyCity"
                type="text"
                class="input"
                placeholder="Ex: São Paulo"
                @input="historySearched = false"
              >
            </div>
            <div class="form-row">
              <div class="form-group">
                <label for="hist-state">Estado</label>
                <input
                  id="hist-state"
                  v-model="historyState"
                  type="text"
                  class="input"
                  placeholder="Ex: Paraná ou PR"
                >
              </div>
              <div class="form-group">
                <label for="hist-country">País</label>
                <input
                  id="hist-country"
                  v-model="historyCountry"
                  type="text"
                  class="input"
                  placeholder="Ex: BR"
                  maxlength="2"
                >
              </div>
            </div>
          </template>

          <template v-else>
            <div class="form-row">
              <div class="form-group">
                <label for="hist-lat">Latitude</label>
                <input
                  id="hist-lat"
                  v-model="historyLatitude"
                  type="text"
                  inputmode="decimal"
                  class="input"
                  placeholder="Ex: -23.5505"
                  @input="historySearched = false"
                >
                <span v-if="historyLatitudeError" class="error-text">{{ historyLatitudeError }}</span>
              </div>
              <div class="form-group">
                <label for="hist-lon">Longitude</label>
                <input
                  id="hist-lon"
                  v-model="historyLongitude"
                  type="text"
                  inputmode="decimal"
                  class="input"
                  placeholder="Ex: -46.6333"
                  @input="historySearched = false"
                >
                <span v-if="historyLongitudeError" class="error-text">{{ historyLongitudeError }}</span>
              </div>
            </div>
          </template>

          <button
            type="submit"
            class="btn btn--primary"
            :disabled="!isHistoryValid || historyLoading"
          >
            {{ historyLoading ? 'Consultando...' : 'Consultar' }}
          </button>
        </form>

        <div v-if="historyError" class="alert alert--error mt-md">
          {{ historyError }}
        </div>

        <div v-if="historySearched && historyRecords.length === 0" class="empty-state mt-md">
          <p class="text-soft">Nenhum registro encontrado nos últimos 30 dias.</p>
        </div>

        <div v-if="historyRecords.length > 0" class="history-list mt-md">
          <table class="history-table">
            <thead>
              <tr>
                <th>Cidade</th>
                <th>Estado</th>
                <th>País</th>
                <th>Temperatura</th>
                <th>Coordenadas</th>
                <th>Data</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="record in historyRecords" :key="record.id">
                <td>{{ record.city }}</td>
                <td>{{ record.state ?? '—' }}</td>
                <td>{{ record.country ?? '—' }}</td>
                <td>{{ formatTemperature(record.temperature) }}</td>
                <td>{{ formatCoordinates(record.latitude, record.longitude) }}</td>
                <td>{{ formatDate(record.recordedAt) }}</td>
              </tr>
            </tbody>
          </table>

          <HistoryChart :records="historyRecords" />
        </div>
      </div>
    </section>
  </div>
</template>

<style scoped>
.dashboard-section {
  margin-bottom: var(--spacing-xl);
}

.section-title {
  font-size: 1.25rem;
  margin-bottom: var(--spacing-md);
}

.section-form {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
}

.tabs {
  display: flex;
  gap: var(--spacing-xs);
  margin-bottom: var(--spacing-lg);
  border-bottom: 1px solid var(--color-border);
  padding-bottom: var(--spacing-xs);
}

.tab {
  padding: var(--spacing-sm) var(--spacing-md);
  font-size: 0.875rem;
  font-weight: 500;
  color: var(--color-text-soft);
  background: none;
  border: none;
  border-bottom: 2px solid transparent;
  cursor: pointer;
  transition:
    color var(--transition-fast),
    border-color var(--transition-fast);
}

.tab:hover {
  color: var(--color-text);
}

.tab--active {
  color: var(--color-primary);
  border-bottom-color: var(--color-primary);
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--spacing-md);
}

.cta-card {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--spacing-md);
  padding: var(--spacing-xl);
}

.cta-card__text {
  color: var(--color-text-soft);
  font-size: 0.9375rem;
}

/* Result card */
.result-card {
  background-color: var(--color-background-soft);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  padding: var(--spacing-md);
}

.result-card__title {
  font-size: 0.9375rem;
  font-weight: 600;
  margin-bottom: var(--spacing-sm);
  color: var(--color-heading);
}

.result-card__grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--spacing-sm);
}

.result-card__item {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.result-card__label {
  font-size: 0.75rem;
  color: var(--color-text-soft);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.result-card__value {
  font-size: 0.9375rem;
  color: var(--color-text);
}

.result-card__value--highlight {
  font-size: 1.25rem;
  font-weight: 700;
  color: var(--color-primary);
}

/* History table */
.history-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.875rem;
}

.history-table th {
  text-align: left;
  padding: var(--spacing-sm) var(--spacing-md);
  font-weight: 600;
  color: var(--color-heading);
  border-bottom: 2px solid var(--color-border);
}

.history-table td {
  padding: var(--spacing-sm) var(--spacing-md);
  color: var(--color-text);
  border-bottom: 1px solid var(--color-border);
}

.history-table tbody tr:hover {
  background-color: var(--color-background-soft);
}

.empty-state {
  padding: var(--spacing-lg);
  text-align: center;
}

@media (max-width: 640px) {
  .form-row {
    grid-template-columns: 1fr;
  }

  .result-card__grid {
    grid-template-columns: 1fr;
  }

  .history-table {
    font-size: 0.8125rem;
  }

  .history-table th,
  .history-table td {
    padding: var(--spacing-xs) var(--spacing-sm);
  }
}
</style>
