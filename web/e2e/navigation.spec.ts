import { test, expect } from './fixtures/auth.fixture'

test.describe('Navigation – unauthenticated', () => {
  test('should show "Entrar" button in navbar', async ({ page }) => {
    await page.goto('/')

    await expect(page.locator('.navbar').getByRole('link', { name: 'Entrar' })).toBeVisible()
    await expect(page.locator('.navbar').getByRole('button', { name: 'Sair' })).toBeHidden()
  })

  test('should navigate to login page from navbar', async ({ page }) => {
    await page.goto('/')

    await page.locator('.navbar').getByRole('link', { name: 'Entrar' }).click()

    await page.waitForURL('/login')
    await expect(page.getByRole('heading', { name: 'Entrar' })).toBeVisible()
  })

  test('should navigate to dashboard from navbar brand link', async ({ page }) => {
    await page.goto('/login')

    await page.locator('.navbar__brand').click()

    await page.waitForURL('/')
    await expect(page.getByRole('heading', { name: 'Dashboard' })).toBeVisible()
  })

  test('should navigate to dashboard from Dashboard link', async ({ page }) => {
    await page.goto('/login')

    await page.locator('.navbar').getByRole('link', { name: 'Dashboard' }).click()

    await page.waitForURL('/')
    await expect(page.getByRole('heading', { name: 'Dashboard' })).toBeVisible()
  })
})

test.describe('Navigation – authenticated', () => {
  test('should show "Sair" button instead of "Entrar"', async ({ authenticatedPage }) => {
    await expect(authenticatedPage.locator('.navbar').getByRole('button', { name: 'Sair' })).toBeVisible()
    await expect(authenticatedPage.locator('.navbar').getByRole('link', { name: 'Entrar' })).toBeHidden()
  })
})

test.describe('Theme toggle', () => {
  test('should cycle theme when clicking theme button', async ({ page }) => {
    await page.goto('/')

    const themeButton = page.locator('.navbar').getByTitle(/Tema:/)

    await expect(themeButton).toBeVisible()

    const initialTitle = await themeButton.getAttribute('title')
    await themeButton.click()

    await expect(themeButton).not.toHaveAttribute('title', initialTitle ?? '')
  })
})
