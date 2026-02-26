<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const isRegisterMode = ref<boolean>(false)
const isLoading = ref<boolean>(false)
const errorMessage = ref<string>('')
const successMessage = ref<string>('')

const email = ref<string>('')
const password = ref<string>('')
const username = ref<string>('')
const confirmPassword = ref<string>('')

const formTitle = computed<string>(() => (isRegisterMode.value ? 'Criar conta' : 'Entrar'))
const submitLabel = computed<string>(() => (isRegisterMode.value ? 'Registrar' : 'Entrar'))

const emailError = computed<string>(() => {
  if (!email.value) return ''
  const emailRegex: RegExp = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  if (!emailRegex.test(email.value)) return 'E-mail inválido'
  return ''
})

const passwordError = computed<string>(() => {
  if (!password.value) return ''
  if (password.value.length < 6) return 'Mínimo de 6 caracteres'
  return ''
})

const confirmPasswordError = computed<string>(() => {
  if (!isRegisterMode.value || !confirmPassword.value) return ''
  if (confirmPassword.value !== password.value) return 'As senhas não coincidem'
  return ''
})

const usernameError = computed<string>(() => {
  if (!isRegisterMode.value || !username.value) return ''
  if (username.value.length < 3) return 'Mínimo de 3 caracteres'
  return ''
})

const isFormValid = computed<boolean>(() => {
  if (!email.value || !password.value) return false
  if (emailError.value || passwordError.value) return false

  if (isRegisterMode.value) {
    if (!username.value || !confirmPassword.value) return false
    if (usernameError.value || confirmPasswordError.value) return false
  }

  return true
})

function toggleMode(): void {
  isRegisterMode.value = !isRegisterMode.value
  errorMessage.value = ''
  successMessage.value = ''
  username.value = ''
  confirmPassword.value = ''
}

function clearMessages(): void {
  errorMessage.value = ''
  successMessage.value = ''
}

async function handleSubmit(): Promise<void> {
  if (!isFormValid.value || isLoading.value) return

  clearMessages()
  isLoading.value = true

  try {
    if (isRegisterMode.value) {
      const message: string = await authStore.register({
        username: username.value,
        email: email.value,
        password: password.value,
      })
      successMessage.value = message
      isRegisterMode.value = false
      username.value = ''
      confirmPassword.value = ''
    } else {
      await authStore.login({
        email: email.value,
        password: password.value,
      })
      const redirect: string = typeof route.query.redirect === 'string' ? route.query.redirect : '/'
      await router.push(redirect)
    }
  } catch (error: unknown) {
    errorMessage.value = authStore.extractErrorMessage(error)
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <div class="login-view">
    <div class="card card--elevated">
      <div class="page-header text-center">
        <h1>{{ formTitle }}</h1>
        <p>{{ isRegisterMode ? 'Preencha os dados para criar sua conta' : 'Faça login para registrar dados climáticos' }}</p>
      </div>

      <div v-if="errorMessage" class="alert alert--error mb-md">
        {{ errorMessage }}
      </div>

      <div v-if="successMessage" class="alert alert--success mb-md">
        {{ successMessage }}
      </div>

      <form @submit.prevent="handleSubmit">
        <div v-if="isRegisterMode" class="form-group mb-md">
          <label for="username">Nome de usuário</label>
          <input
            id="username"
            v-model="username"
            type="text"
            class="input"
            placeholder="Seu nome de usuário"
            autocomplete="username"
            @input="clearMessages"
          >
          <span v-if="usernameError" class="error-text">{{ usernameError }}</span>
        </div>

        <div class="form-group mb-md">
          <label for="email">E-mail</label>
          <input
            id="email"
            v-model="email"
            type="email"
            class="input"
            placeholder="seu@email.com"
            autocomplete="email"
            @input="clearMessages"
          >
          <span v-if="emailError" class="error-text">{{ emailError }}</span>
        </div>

        <div class="form-group mb-md">
          <label for="password">Senha</label>
          <input
            id="password"
            v-model="password"
            type="password"
            class="input"
            placeholder="Sua senha"
            :autocomplete="isRegisterMode ? 'new-password' : 'current-password'"
            @input="clearMessages"
          >
          <span v-if="passwordError" class="error-text">{{ passwordError }}</span>
        </div>

        <div v-if="isRegisterMode" class="form-group mb-md">
          <label for="confirm-password">Confirmar senha</label>
          <input
            id="confirm-password"
            v-model="confirmPassword"
            type="password"
            class="input"
            placeholder="Repita a senha"
            autocomplete="new-password"
            @input="clearMessages"
          >
          <span v-if="confirmPasswordError" class="error-text">{{ confirmPasswordError }}</span>
        </div>

        <button
          type="submit"
          class="btn btn--primary btn--block mt-md"
          :disabled="!isFormValid || isLoading"
        >
          {{ isLoading ? 'Aguarde...' : submitLabel }}
        </button>
      </form>

      <p class="toggle-text text-center mt-lg">
        {{ isRegisterMode ? 'Já tem uma conta?' : 'Não tem uma conta?' }}
        <button class="toggle-link" @click="toggleMode">
          {{ isRegisterMode ? 'Entrar' : 'Criar conta' }}
        </button>
      </p>
    </div>
  </div>
</template>

<style scoped>
.login-view {
  max-width: 24rem;
  margin: var(--spacing-2xl) auto;
}

.toggle-text {
  font-size: 0.875rem;
  color: var(--color-text-soft);
}

.toggle-link {
  background: none;
  border: none;
  color: var(--color-primary);
  font-weight: 500;
  cursor: pointer;
  font-size: inherit;
  padding: 0;
  text-decoration: underline;
}

.toggle-link:hover {
  color: var(--color-primary-hover);
}
</style>
