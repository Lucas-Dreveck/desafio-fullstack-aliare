<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { useThemeStore } from '@/stores/theme'
import { useAuthStore } from '@/stores/auth'

const themeStore = useThemeStore()
const authStore = useAuthStore()
const router = useRouter()

const themeIcon = computed<string>(() => {
  if (themeStore.mode === 'auto') return '🌗'
  return themeStore.mode === 'dark' ? '🌙' : '☀️'
})

const themeLabel = computed<string>(() => {
  if (themeStore.mode === 'auto') return 'Automático'
  return themeStore.mode === 'dark' ? 'Escuro' : 'Claro'
})

async function handleLogout(): Promise<void> {
  await authStore.logout()
  await router.push({ name: 'login' })
}
</script>

<template>
  <nav class="navbar">
    <div class="navbar__container">
      <RouterLink to="/" class="navbar__brand">
        ☁️ Aliare Weather
      </RouterLink>

      <div class="navbar__links">
        <RouterLink to="/" class="navbar__link">
          Dashboard
        </RouterLink>
      </div>

      <div class="navbar__actions">
        <button
          class="btn btn--ghost btn--sm"
          :title="`Tema: ${themeLabel}`"
          @click="themeStore.toggle()"
        >
          {{ themeIcon }}
        </button>
        <RouterLink to="/login" v-if="!authStore.isAuthenticated" class="btn btn--outline btn--sm">
          Entrar
        </RouterLink>
        <button v-else class="btn btn--outline btn--sm" @click="handleLogout">
          Sair
        </button>
      </div>
    </div>
  </nav>
</template>

<style scoped>
.navbar {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  height: var(--navbar-height);
  background-color: color-mix(in srgb, var(--color-background) 85%, transparent);
  backdrop-filter: blur(12px);
  border-bottom: 1px solid var(--color-border);
  z-index: 100;
}

.navbar__container {
  max-width: var(--content-max-width);
  margin: 0 auto;
  height: 100%;
  padding: 0 var(--spacing-md);
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.navbar__brand {
  font-size: 1.125rem;
  font-weight: 700;
  color: var(--color-heading);
  text-decoration: none;
}

.navbar__brand:hover {
  color: var(--color-primary);
}

.navbar__links {
  display: flex;
  gap: var(--spacing-xs);
}

.navbar__link {
  padding: var(--spacing-xs) var(--spacing-sm);
  font-size: 0.875rem;
  font-weight: 500;
  color: var(--color-text-soft);
  text-decoration: none;
  border-radius: var(--radius-md);
  transition:
    color var(--transition-fast),
    background-color var(--transition-fast);
}

.navbar__link:hover {
  color: var(--color-text);
  background-color: var(--color-background-mute);
}

.navbar__link.router-link-active {
  color: var(--color-primary);
  background-color: var(--color-primary-light);
}

.navbar__actions {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
}
</style>
