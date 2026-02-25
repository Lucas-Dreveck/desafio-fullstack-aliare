<script setup lang="ts">
import { RouterView } from 'vue-router'
import { onMounted, ref } from 'vue'
import { useAuthStore } from '@/stores/auth'
import AppNavbar from '@/components/layout/AppNavbar.vue'

const authStore = useAuthStore()
const loading = ref<boolean>(true)

onMounted(async () => {
  await authStore.init()
  loading.value = false
})
</script>

<template>
  <AppNavbar />
  <main class="container">
    <div v-if="loading" class="loading-screen">
      <p>Carregando...</p>
    </div>
    <RouterView v-else />
  </main>
</template>

<style scoped>
.loading-screen {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 60vh;
  font-size: 1.125rem;
  color: var(--color-text-muted);
}
</style>
