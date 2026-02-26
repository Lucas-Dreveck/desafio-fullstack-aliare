import { ref, computed, watch } from 'vue'
import { defineStore } from 'pinia'

export type ThemeMode = 'light' | 'dark' | 'auto'

const STORAGE_KEY = 'theme_preference'

function getSystemTheme(): 'light' | 'dark' {
  return globalThis.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'
}

function applyTheme(mode: ThemeMode): void {
  const resolved: 'light' | 'dark' = mode === 'auto' ? getSystemTheme() : mode
  document.documentElement.dataset.theme = resolved
}

export const useThemeStore = defineStore('theme', () => {
  const stored: string | null = localStorage.getItem(STORAGE_KEY)
  const mode = ref<ThemeMode>((stored as ThemeMode) || 'auto')
  const systemTheme = ref<'light' | 'dark'>(getSystemTheme())

  const resolvedTheme = computed<'light' | 'dark'>(() =>
    mode.value === 'auto' ? systemTheme.value : mode.value,
  )

  function setMode(newMode: ThemeMode): void {
    mode.value = newMode
    localStorage.setItem(STORAGE_KEY, newMode)
  }

  function toggle(): void {
    const resolved: 'light' | 'dark' = mode.value === 'auto' ? getSystemTheme() : mode.value
    setMode(resolved === 'dark' ? 'light' : 'dark')
  }

  watch(mode, (value: ThemeMode) => applyTheme(value), { immediate: true })

  // Sync with OS changes when mode is 'auto'
  globalThis.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', () => {
    systemTheme.value = getSystemTheme()
    if (mode.value === 'auto') {
      applyTheme('auto')
    }
  })

  return { mode, resolvedTheme, setMode, toggle }
})
