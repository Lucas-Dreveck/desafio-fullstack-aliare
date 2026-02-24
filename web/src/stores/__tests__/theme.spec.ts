import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest'
import { nextTick } from 'vue'
import { createPinia, setActivePinia } from 'pinia'
import { useThemeStore } from '@/stores/theme'

// ---------------------------------------------------------------------------
// Mock matchMedia
// ---------------------------------------------------------------------------

/**
 * jsdom does not implement window.matchMedia, so we stub it.
 *
 * vi.fn() creates a "spy" function — it does nothing but records every call
 * so we can inspect it later. .mockReturnValue() makes it always return the
 * provided object.
 *
 * The returned object simulates a MediaQueryList with:
 *   - matches: false → system theme is "light"
 *   - addEventListener: vi.fn() → no-op stub so the store's listener setup
 *     doesn't throw
 */
const matchMediaMock = vi.fn().mockReturnValue({
  matches: false,
  addEventListener: vi.fn(),
})

Object.defineProperty(globalThis, 'matchMedia', {
  writable: true,
  value: matchMediaMock,
})

// ---------------------------------------------------------------------------
// Tests
// ---------------------------------------------------------------------------

describe('useThemeStore', () => {
  /**
   * beforeEach runs before EVERY it() block.
   * We create a fresh Pinia so each test starts with a clean store,
   * and clear localStorage so no preference leaks between tests.
   */
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

    // Re-create pinia so the store reads the new localStorage value
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
    // matchMedia returns { matches: false } → getSystemTheme() returns 'light'
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

    // Vue watchers are async by default — they batch updates and flush
    // on the next microtask. nextTick() waits for that flush to complete,
    // so applyTheme() has run and dataset.theme is updated.
    await nextTick()

    expect(document.documentElement.dataset.theme).toBe('dark')
  })
})
