<script setup lang="ts">
import { RouterView } from 'vue-router'
import AppNavbar from '@/components/layout/AppNavbar.vue'
</script>

<template>
  <AppNavbar />
  <main class="container">
    <RouterView />
  </main>
</template>
