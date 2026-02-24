import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest'
import { nextTick } from 'vue'
import { createPinia, setActivePinia } from 'pinia'
import { useThemeStore } from '@/stores/theme'

const matchMediaMock = vi.fn().mockReturnValue({
  matches: false,
  addEventListener: vi.fn(),
})

Object.defineProperty(globalThis, 'matchMedia', {
  writable: true,
  value: matchMediaMock,
})

describe('useThemeStore', () => {
  beforeEach(() => {
    localStorage.clear()
    setActivePinia(createPinia())
  })

  afterEach(() => {
    vi.restoreAllMocks()
  })

  it('should initialize with mode "auto" when no preference is stored', () => {
    const store = useThemeStore()

    expect(store.mode).toBe('auto')
  })

  it('should initialize with the stored preference from localStorage', () => {
    localStorage.setItem('theme_preference', 'dark')
    setActivePinia(createPinia())

    const store = useThemeStore()

    expect(store.mode).toBe('dark')
  })

  it('should update mode and persist to localStorage via setMode', () => {
    const store = useThemeStore()

    store.setMode('dark')

    expect(store.mode).toBe('dark')
    expect(localStorage.getItem('theme_preference')).toBe('dark')
  })

  it('should return the mode directly as resolvedTheme when not "auto"', () => {
    const store = useThemeStore()

    store.setMode('dark')
    expect(store.resolvedTheme).toBe('dark')

    store.setMode('light')
    expect(store.resolvedTheme).toBe('light')
  })

  it('should resolve "auto" to system theme via resolvedTheme', () => {
    const store = useThemeStore()

    expect(store.mode).toBe('auto')
    expect(store.resolvedTheme).toBe('light')
  })

  it('should toggle from light to dark', () => {
    const store = useThemeStore()
    store.setMode('light')

    store.toggle()

    expect(store.mode).toBe('dark')
  })

  it('should toggle from dark to light', () => {
    const store = useThemeStore()
    store.setMode('dark')

    store.toggle()

    expect(store.mode).toBe('light')
  })

  it('should set data-theme attribute on document element', async () => {
    const store = useThemeStore()

    store.setMode('dark')
    await nextTick()

    expect(document.documentElement.dataset.theme).toBe('dark')
  })
})
