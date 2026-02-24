import { describe, it, expect, beforeEach, vi } from 'vitest'
import type { Mock } from 'vitest'
import { mount } from '@vue/test-utils'
import type { VueWrapper } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import type { Pinia } from 'pinia'
import { createRouter, createMemoryHistory } from 'vue-router'
import type { Router } from 'vue-router'
import LoginView from '@/views/LoginView.vue'

vi.mock('@/services/authService', () => ({
  authService: {
    login: vi.fn(),
    register: vi.fn(),
  },
}))

vi.mock('@/api/client', () => ({
  default: {},
  AUTH_TOKEN_KEY: 'auth_token',
}))

import { authService } from '@/services/authService'

const loginMock: Mock = authService.login as Mock
const registerMock: Mock = authService.register as Mock

function createFakeJwt(payload: object): string {
  const header: string = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }))
  const body: string = btoa(JSON.stringify(payload))
  return `${header}.${body}.fake-signature`
}

const VALID_TOKEN: string = createFakeJwt({
  sub: '1',
  unique_name: 'aliare',
  email: 'aliare@test.com',
  exp: Math.floor(Date.now() / 1000) + 3600,
  iss: 'api',
  aud: 'client',
})

function createTestRouter(): Router {
  return createRouter({
    history: createMemoryHistory(),
    routes: [
      { path: '/', name: 'dashboard', component: { template: '<div>Dashboard</div>' } },
      { path: '/login', name: 'login', component: LoginView },
    ],
  })
}

function mountLoginView(pinia: Pinia, router: Router): VueWrapper {
  return mount(LoginView, {
    global: {
      plugins: [pinia, router],
    },
  })
}

const matchMediaMock = vi.fn().mockReturnValue({
  matches: false,
  addEventListener: vi.fn(),
})

Object.defineProperty(globalThis, 'matchMedia', {
  writable: true,
  value: matchMediaMock,
})

describe('LoginView', () => {
  let pinia: Pinia
  let router: Router

  beforeEach(async () => {
    localStorage.clear()
    vi.clearAllMocks()

    pinia = createPinia()
    setActivePinia(pinia)

    router = createTestRouter()
    router.push('/login')
    await router.isReady()
  })

  it('should render the login form by default', () => {
    const wrapper: VueWrapper = mountLoginView(pinia, router)

    expect(wrapper.find('h1').text()).toBe('Entrar')
    expect(wrapper.find('input#email').exists()).toBe(true)
    expect(wrapper.find('input#password').exists()).toBe(true)
    expect(wrapper.find('input#username').exists()).toBe(false)
    expect(wrapper.find('input#confirm-password').exists()).toBe(false)
  })

  it('should switch to register mode when toggle is clicked', async () => {
    const wrapper: VueWrapper = mountLoginView(pinia, router)

    await wrapper.find('button.toggle-link').trigger('click')

    expect(wrapper.find('h1').text()).toBe('Criar conta')
    expect(wrapper.find('input#username').exists()).toBe(true)
    expect(wrapper.find('input#confirm-password').exists()).toBe(true)
  })

  it('should show email validation error for invalid email', async () => {
    const wrapper: VueWrapper = mountLoginView(pinia, router)

    await wrapper.find('input#email').setValue('not-an-email')

    expect(wrapper.find('.error-text').text()).toBe('E-mail inválido')
  })

  it('should show password validation error when too short', async () => {
    const wrapper: VueWrapper = mountLoginView(pinia, router)

    await wrapper.find('input#password').setValue('123')

    expect(wrapper.find('.error-text').text()).toBe('Mínimo de 6 caracteres')
  })

  it('should show confirm password mismatch error in register mode', async () => {
    const wrapper: VueWrapper = mountLoginView(pinia, router)
    await wrapper.find('button.toggle-link').trigger('click')

    await wrapper.find('input#password').setValue('123456')
    await wrapper.find('input#confirm-password').setValue('different')

    const errorTexts: string[] = wrapper
      .findAll('.error-text')
      .map((el) => el.text())

    expect(errorTexts).toContain('As senhas não coincidem')
  })

  it('should disable submit button when form is incomplete', () => {
    const wrapper: VueWrapper = mountLoginView(pinia, router)

    const button = wrapper.find('button[type="submit"]')
    expect(button.attributes('disabled')).toBeDefined()
  })

  it('should enable submit button when login form is valid', async () => {
    const wrapper: VueWrapper = mountLoginView(pinia, router)

    await wrapper.find('input#email').setValue('aliare@test.com')
    await wrapper.find('input#password').setValue('123456')

    const button = wrapper.find('button[type="submit"]')
    expect(button.attributes('disabled')).toBeUndefined()
  })

  it('should call login and redirect on successful login', async () => {
    loginMock.mockResolvedValue({ token: VALID_TOKEN })

    const wrapper: VueWrapper = mountLoginView(pinia, router)

    await wrapper.find('input#email').setValue('aliare@test.com')
    await wrapper.find('input#password').setValue('123456')
    await wrapper.find('form').trigger('submit')

    await vi.waitFor(() => {
      expect(loginMock).toHaveBeenCalledWith({
        email: 'aliare@test.com',
        password: '123456',
      })
    })

    await vi.waitFor(() => {
      expect(router.currentRoute.value.path).toBe('/')
    })
  })

  it('should show error message on failed login', async () => {
    loginMock.mockRejectedValue({
      response: { data: { error: 'Credenciais inválidas.' } },
    })

    const wrapper: VueWrapper = mountLoginView(pinia, router)

    await wrapper.find('input#email').setValue('aliare@test.com')
    await wrapper.find('input#password').setValue('wrong-password')
    await wrapper.find('form').trigger('submit')

    await vi.waitFor(() => {
      expect(wrapper.find('.alert--error').text()).toBe('Credenciais inválidas.')
    })
  })

  it('should show success message and switch to login after register', async () => {
    registerMock.mockResolvedValue({ message: 'Usuário criado com sucesso!' })

    const wrapper: VueWrapper = mountLoginView(pinia, router)
    await wrapper.find('button.toggle-link').trigger('click')

    await wrapper.find('input#username').setValue('aliare')
    await wrapper.find('input#email').setValue('aliare@test.com')
    await wrapper.find('input#password').setValue('123456')
    await wrapper.find('input#confirm-password').setValue('123456')
    await wrapper.find('form').trigger('submit')

    await vi.waitFor(() => {
      expect(wrapper.find('.alert--success').text()).toBe('Usuário criado com sucesso!')
    })

    expect(wrapper.find('h1').text()).toBe('Entrar')
  })
})
