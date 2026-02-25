import { test as base, type Page } from '@playwright/test'

export interface TestUser {
  username: string
  email: string
  password: string
}

interface AuthFixtures {
  testUser: TestUser
  authenticatedPage: Page
}

/**
 * Extends Playwright's base `test` with authentication helpers.
 *
 * - `testUser`  — registers a unique user via the API (no UI interaction).
 * - `authenticatedPage` — logs that user in via the login form so the
 *    browser context has the HttpOnly refresh-token cookie and the
 *    Pinia auth store holds the access token in memory.
 */
export const test = base.extend<AuthFixtures>({
  testUser: async ({ page }, use) => {
    const id: string = `${Date.now()}-${Math.random().toString(36).slice(2, 7)}`

    const user: TestUser = {
      username: `e2e-${id}`,
      email: `e2e-${id}@test.com`,
      password: 'Test@123456',
    }

    const response = await page.request.post('/api/v1/auth/register', {
      data: user,
    })

    if (!response.ok()) {
      throw new Error(`Failed to register test user: ${response.status()} ${await response.text()}`)
    }

    await use(user)
  },

  authenticatedPage: async ({ page, testUser }, use) => {
    await page.goto('/login')

    await page.getByLabel('E-mail').fill(testUser.email)
    await page.getByLabel('Senha').fill(testUser.password)
    await page.getByRole('button', { name: 'Entrar' }).click()

    await page.waitForURL('/')

    await use(page)
  },
})

export { expect } from '@playwright/test'
