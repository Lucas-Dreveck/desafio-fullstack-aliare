<script setup lang="ts">
import { computed } from 'vue'
import { Line } from 'vue-chartjs'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler,
} from 'chart.js'
import type { ChartData, ChartOptions } from 'chart.js'
import type { WeatherRecordResponse } from '@/types/weather'
import { useThemeStore } from '@/stores/theme'

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend, Filler)

const themeStore = useThemeStore()

const props = defineProps<{
  records: WeatherRecordResponse[]
}>()

function formatLabel(isoDate: string): string {
  const date: Date = new Date(isoDate)
  return new Intl.DateTimeFormat('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
  }).format(date)
}

const lineColor = computed<string>(() =>
  themeStore.resolvedTheme === 'dark' ? '#60a5fa' : '#3b82f6',
)

const fillColor = computed<string>(() =>
  themeStore.resolvedTheme === 'dark' ? 'rgba(96, 165, 250, 0.12)' : 'rgba(59, 130, 246, 0.12)',
)

const gridColor = computed<string>(() =>
  themeStore.resolvedTheme === 'dark' ? 'rgba(148, 163, 184, 0.12)' : 'rgba(100, 116, 139, 0.12)',
)

const tickColor = computed<string>(() =>
  themeStore.resolvedTheme === 'dark' ? '#94a3b8' : '#64748b',
)

const chartData = computed<ChartData<'line'>>(() => {
  const labels: string[] = props.records.map(
    (record: WeatherRecordResponse) => formatLabel(record.recordedAt),
  )
  const temperatures: number[] = props.records.map(
    (record: WeatherRecordResponse) => record.temperature,
  )

  return {
    labels,
    datasets: [
      {
        label: 'Temperatura (°C)',
        data: temperatures,
        borderColor: lineColor.value,
        backgroundColor: fillColor.value,
        borderWidth: 2,
        pointRadius: 4,
        pointHoverRadius: 6,
        tension: 0.3,
        fill: true,
      },
    ],
  }
})

const chartOptions = computed<ChartOptions<'line'>>(() => ({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: false,
    },
    tooltip: {
      callbacks: {
        label: (context) => `${(context.raw as number).toFixed(1)} °C`,
      },
    },
  },
  scales: {
    x: {
      grid: {
        display: false,
      },
      ticks: {
        maxRotation: 45,
        font: { size: 11 },
        color: tickColor.value,
      },
    },
    y: {
      ticks: {
        callback: (value) => `${value} °C`,
        font: { size: 11 },
        color: tickColor.value,
      },
      grid: {
        color: gridColor.value,
      },
    },
  },
}))
</script>

<template>
  <div class="history-chart">
    <h3 class="history-chart__title">Evolução da Temperatura</h3>
    <div class="history-chart__container">
      <Line :data="chartData" :options="chartOptions" />
    </div>
  </div>
</template>

<style scoped>
.history-chart {
  margin-top: var(--spacing-lg);
}

.history-chart__title {
  font-size: 0.9375rem;
  font-weight: 600;
  color: var(--color-heading);
  margin-bottom: var(--spacing-sm);
}

.history-chart__container {
  position: relative;
  height: 300px;
}
</style>
