import { test, expect } from './fixtures/auth.fixture'
import type { Route } from '@playwright/test'

const MOCK_WEATHER_RECORD = {
  id: '00000000-0000-0000-0000-000000000001',
  city: 'São Paulo',
  state: 'São Paulo',
  country: 'BR',
  temperature: 25.3,
  latitude: -23.5505,
  longitude: -46.6333,
  recordedAt: new Date().toISOString(),
}

const MOCK_HISTORY_RECORDS = [
  { ...MOCK_WEATHER_RECORD, id: '00000000-0000-0000-0000-000000000001', temperature: 25.3 },
  { ...MOCK_WEATHER_RECORD, id: '00000000-0000-0000-0000-000000000002', temperature: 22.1 },
  { ...MOCK_WEATHER_RECORD, id: '00000000-0000-0000-0000-000000000003', temperature: 27.8 },
]

test.describe('Dashboard – public view', () => {
  test('should show CTA login card instead of register section', async ({ page }) => {
    await page.goto('/')

    await expect(page.getByText('Faça login para registrar leituras')).toBeVisible()
    await expect(page.locator('.cta-card').getByRole('link', { name: 'Entrar' })).toBeVisible()
    await expect(page.getByRole('heading', { name: 'Registrar Temperatura' })).toBeHidden()
  })

  test('should show history section without authentication', async ({ page }) => {
    await page.goto('/')

    await expect(page.getByRole('heading', { name: 'Consultar Histórico' })).toBeVisible()
    await expect(page.getByLabel('Cidade')).toBeVisible()
  })

  test('should query history by city and display results', async ({ page }) => {
    await page.route('**/api/v1/weather/history/by-city/**', async (route: Route) => {
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(MOCK_HISTORY_RECORDS),
      })
    })

    await page.goto('/')
    await page.getByLabel('Cidade').fill('São Paulo')
    await page.getByRole('button', { name: 'Consultar' }).click()

    await expect(page.locator('.history-table tbody tr')).toHaveCount(3)
    await expect(page.locator('.history-table')).toContainText('25.3')
    await expect(page.locator('canvas')).toBeVisible()
  })

  test('should query history by coordinates and display results', async ({ page }) => {
    await page.route('**/api/v1/weather/history/by-coordinates**', async (route: Route) => {
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(MOCK_HISTORY_RECORDS),
      })
    })

    await page.goto('/')

    const coordsTab = page.getByRole('button', { name: 'Por Coordenadas' })
    await coordsTab.last().click()

    await page.getByLabel('Latitude').fill('-23.5505')
    await page.getByLabel('Longitude').fill('-46.6333')
    await page.getByRole('button', { name: 'Consultar' }).click()

    await expect(page.locator('.history-table tbody tr')).toHaveCount(3)
    await expect(page.locator('canvas')).toBeVisible()
  })

  test('should show empty message when no history found', async ({ page }) => {
    await page.route('**/api/v1/weather/history/by-city/**', async (route: Route) => {
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify([]),
      })
    })

    await page.goto('/')
    await page.getByLabel('Cidade').fill('CidadeInexistente')
    await page.getByRole('button', { name: 'Consultar' }).click()

    await expect(page.getByText('Nenhum registro encontrado')).toBeVisible()
  })
})

test.describe('Dashboard – authenticated view', () => {
  test('should show register temperature section when authenticated', async ({ authenticatedPage }) => {
    await expect(authenticatedPage.getByRole('heading', { name: 'Registrar Temperatura' })).toBeVisible()
    await expect(authenticatedPage.getByText('Faça login para registrar leituras')).toBeHidden()
  })

  test('should register temperature by city', async ({ authenticatedPage }) => {
    await authenticatedPage.route('**/api/v1/weather/register/by-city', async (route: Route) => {
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(MOCK_WEATHER_RECORD),
      })
    })

    await authenticatedPage.getByLabel('Cidade *').fill('São Paulo')
    await authenticatedPage.getByRole('button', { name: 'Registrar' }).click()

    await expect(authenticatedPage.getByText('Leitura registrada')).toBeVisible()
    await expect(authenticatedPage.getByText('25.3 °C')).toBeVisible()
  })

  test('should register temperature by coordinates', async ({ authenticatedPage }) => {
    await authenticatedPage.route('**/api/v1/weather/register/by-coordinates', async (route: Route) => {
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(MOCK_WEATHER_RECORD),
      })
    })

    await authenticatedPage.getByRole('button', { name: 'Por Coordenadas' }).first().click()
    await authenticatedPage.getByLabel('Latitude *').fill('-23.5505')
    await authenticatedPage.getByLabel('Longitude *').fill('-46.6333')
    await authenticatedPage.getByRole('button', { name: 'Registrar' }).click()

    await expect(authenticatedPage.getByText('Leitura registrada')).toBeVisible()
    await expect(authenticatedPage.getByText('25.3 °C')).toBeVisible()
  })

  test('should switch tabs between city and coordinates in register section', async ({ authenticatedPage }) => {
    await expect(authenticatedPage.getByLabel('Cidade *')).toBeVisible()

    await authenticatedPage.getByRole('button', { name: 'Por Coordenadas' }).first().click()

    await expect(authenticatedPage.getByLabel('Latitude *')).toBeVisible()
    await expect(authenticatedPage.getByLabel('Longitude *')).toBeVisible()

    await authenticatedPage.getByRole('button', { name: 'Por Cidade' }).first().click()

    await expect(authenticatedPage.getByLabel('Cidade *')).toBeVisible()
  })
})
