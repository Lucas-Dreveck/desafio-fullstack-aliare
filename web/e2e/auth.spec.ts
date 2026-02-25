import { test, expect } from './fixtures/auth.fixture'

/**
 * Authentication e2e tests.
 *
 * Fixtures are lazy — `testUser` only registers a user when the test
 * destructures it, and `authenticatedPage` only logs in when requested.
 * Tests that only need `{ page }` run without any user setup.
 */

test.describe('Register', () => {
  test('should register a new user and show success message', async ({ page }) => {
    const id: string = `${Date.now()}-${Math.random().toString(36).slice(2, 7)}`

    await page.goto('/login')
    await page.getByRole('button', { name: 'Criar conta' }).click()

    await page.getByLabel('Nome de usuário').fill(`e2e-reg-${id}`)
    await page.getByLabel('E-mail').fill(`e2e-reg-${id}@test.com`)
    await page.getByLabel('Senha', { exact: true }).fill('Test@123456')
    await page.getByLabel('Confirmar senha').fill('Test@123456')

    await page.getByRole('button', { name: 'Registrar' }).click()

    await expect(page.locator('.alert--success')).toBeVisible()
    await expect(page.getByRole('heading', { name: 'Entrar' })).toBeVisible()
  })

  test('should show error when registering with duplicate email', async ({ page }) => {
    const id: string = `${Date.now()}-${Math.random().toString(36).slice(2, 7)}`
    const email: string = `e2e-dup-${id}@test.com`

    await page.request.post('/api/v1/auth/register', {
      data: { username: `e2e-dup-${id}`, email, password: 'Test@123456' },
    })

    await page.goto('/login')
    await page.getByRole('button', { name: 'Criar conta' }).click()

    await page.getByLabel('Nome de usuário').fill(`e2e-dup2-${id}`)
    await page.getByLabel('E-mail').fill(email)
    await page.getByLabel('Senha', { exact: true }).fill('Test@123456')
    await page.getByLabel('Confirmar senha').fill('Test@123456')

    await page.getByRole('button', { name: 'Registrar' }).click()

    await expect(page.locator('.alert--error')).toBeVisible()
  })
})

test.describe('Login', () => {
  test('should login with valid credentials and redirect to dashboard', async ({ page, testUser }) => {
    await page.goto('/login')

    await page.getByLabel('E-mail').fill(testUser.email)
    await page.getByLabel('Senha').fill(testUser.password)
    await page.getByRole('button', { name: 'Entrar' }).click()

    await page.waitForURL('/')
    await expect(page.getByRole('heading', { name: 'Dashboard' })).toBeVisible()
    await expect(page.getByRole('button', { name: 'Sair' })).toBeVisible()
  })

  test('should show error with wrong password', async ({ page, testUser }) => {
    await page.goto('/login')

    await page.getByLabel('E-mail').fill(testUser.email)
    await page.getByLabel('Senha').fill('WrongPassword123')
    await page.getByRole('button', { name: 'Entrar' }).click()

    await expect(page.locator('.alert--error')).toBeVisible()
  })

  test('should redirect to original route after login', async ({ page, testUser }) => {
    await page.goto('/login?redirect=%2F')

    await page.getByLabel('E-mail').fill(testUser.email)
    await page.getByLabel('Senha').fill(testUser.password)
    await page.getByRole('button', { name: 'Entrar' }).click()

    await page.waitForURL('/')
    await expect(page.getByRole('heading', { name: 'Dashboard' })).toBeVisible()
  })
})

test.describe('Logout', () => {
  test('should logout and redirect to login', async ({ authenticatedPage }) => {
    await expect(authenticatedPage.getByRole('button', { name: 'Sair' })).toBeVisible()

    await authenticatedPage.getByRole('button', { name: 'Sair' }).click()

    await authenticatedPage.waitForURL('/login')
    await expect(authenticatedPage.getByRole('heading', { name: 'Entrar' })).toBeVisible()
    await expect(authenticatedPage.getByRole('link', { name: 'Entrar' })).toBeVisible()
  })
})

test.describe('Session persistence', () => {
  test('should maintain session after page reload via refresh token cookie', async ({ authenticatedPage }) => {
    await expect(authenticatedPage.getByRole('button', { name: 'Sair' })).toBeVisible()

    await authenticatedPage.reload()

    await expect(authenticatedPage.getByRole('heading', { name: 'Dashboard' })).toBeVisible()
    await expect(authenticatedPage.getByRole('button', { name: 'Sair' })).toBeVisible()
  })
})
