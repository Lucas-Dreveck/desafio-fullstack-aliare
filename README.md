# Aliare Weather

Aplicação fullstack para consulta e registro de dados climáticos por cidade ou coordenadas, com histórico dos últimos 30 dias e visualização em gráfico.

## Tecnologias

| Camada | Stack |
|--------|-------|
| Backend | .NET 8 (C#), Entity Framework Core, PostgreSQL, JWT |
| Frontend | Vue 3, TypeScript, Pinia, Vue Router, Axios, Chart.js |
| Infra | Docker, Docker Compose, nginx, GitHub Actions |

## Execução rápida

Requisitos: [Docker](https://docs.docker.com/get-docker/) e uma chave de API do [OpenWeatherMap](https://openweathermap.org/api) (plano gratuito).

```bash
git clone https://github.com/Lucas-Dreveck/desafio-fullstack-aliare.git
cd desafio-fullstack-aliare
echo "OPENWEATHER_API_KEY=sua_chave_aqui" > .env
docker compose up --build -d
```

| Serviço | URL |
|---------|-----|
| Frontend | http://localhost:3000 |
| Swagger | http://localhost:3000/swagger |
| Health Check | http://localhost:3000/health |
| API (direto) | http://localhost:5251 |

> O nginx do frontend faz proxy reverso para a API — todas as rotas `/api`, `/swagger` e `/health` são acessíveis via `localhost:3000`.

Para parar: `docker compose down` (adicione `-v` para limpar o volume do banco).

## Docker Hub

As imagens estão publicadas no Docker Hub e podem ser usadas sem clonar o repositório.

```bash
docker pull lucasdreveck/aliare-weather-api
docker pull lucasdreveck/aliare-weather-web
```

Para rodar a stack completa usando as imagens publicadas, utilize o `docker-compose.hub.yml`:

```bash
echo "OPENWEATHER_API_KEY=sua_chave_aqui" > .env
docker compose -f docker-compose.hub.yml up -d
```

> O `docker-compose.hub.yml` é idêntico ao `docker-compose.yml`, mas usa `image` ao invés de `build` — sem necessidade de compilar nada localmente.

Para rodar apenas a API (necessário PostgreSQL acessível):

```bash
docker run -d --name aliare-weather-api \
  -p 5251:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=aliare_weather;Username=postgres;Password=postgres" \
  -e Jwt__SecretKey="SuaChaveSecretaComPeloMenos32Caracteres!" \
  -e OpenWeather__ApiKey="SUA_CHAVE_OPENWEATHER" \
  -e RunMigrations="true" \
  lucasdreveck/aliare-weather-api
```

> A variável `OpenWeather__ApiKey` é obrigatória para consultar dados reais. Sem ela, configure `WeatherProvider=Fake` para usar o provedor simulado.

## Execução local (desenvolvimento)

### Backend

Requisitos: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0), PostgreSQL rodando na porta 5432.

```bash
cd server/src/Aliare.Weather.Api
dotnet run
```

A API estará em `https://localhost:5251`. Configure connection string e API key via `appsettings.Development.json` ou `dotnet user-secrets`.

### Frontend

Requisitos: [Node.js](https://nodejs.org/) 20.19+ ou 22.12+.

```bash
cd web
npm install
npm run dev
```

Disponível em `http://localhost:5173` com proxy automático para a API.

## Testes

O projeto conta com **121 testes automatizados** distribuídos em três camadas.

### Backend — 59 testes (xUnit)

```bash
cd server
dotnet test
```

| Categoria | O que cobre |
|-----------|-------------|
| Domínio | Validação de entidades (`WeatherRecord`, `User`) |
| Serviços | `WeatherService` e `AuthService` com Moq (unitários) |
| Integração | Endpoints HTTP via `WebApplicationFactory` com banco in-memory |

### Frontend — 40 testes unitários (Vitest)

```bash
cd web
npm run test:unit
```

| Categoria | O que cobre |
|-----------|-------------|
| Stores | `auth` (login, logout, refresh token, estado) |
| Componentes | `LoginView` (formulário, validação, toggle login/registro) |
| Utilitários | Decode de JWT, interceptor Axios |

### Frontend — 22 testes e2e (Playwright)

```bash
cd web
npm run test:e2e -- --project=chromium
```

Necessita da API e do banco rodando (Docker Compose).

| Spec | O que cobre |
|------|-------------|
| `auth.spec.ts` | Registro, login, logout, persistência de sessão via refresh token |
| `dashboard.spec.ts` | CTA público, consulta de histórico, registro de temperatura |
| `navigation.spec.ts` | Links da navbar, navegação autenticada/não-autenticada, tema |

## Endpoints da API

| Método | Rota | Auth | Descrição |
|--------|------|:----:|-----------|
| `POST` | `/api/v1/auth/register` | — | Cadastro de usuário |
| `POST` | `/api/v1/auth/login` | — | Login (retorna JWT + refresh token via cookie) |
| `POST` | `/api/v1/auth/refresh` | — | Renova o access token usando refresh token |
| `POST` | `/api/v1/auth/logout` | JWT | Revoga refresh token e encerra sessão |
| `POST` | `/api/v1/weather/register/by-city` | JWT | Registra temperatura por cidade |
| `POST` | `/api/v1/weather/register/by-coordinates` | JWT | Registra temperatura por coordenadas |
| `GET`  | `/api/v1/weather/history/by-city/{cityName}` | — | Histórico por cidade (30 dias) |
| `GET`  | `/api/v1/weather/history/by-coordinates` | — | Histórico por coordenadas (30 dias) |
| `GET`  | `/health` | — | Health check |
| `GET`  | `/swagger` | — | Documentação interativa |

## Estrutura do projeto

```
├── server/                             # Backend .NET 8
│   ├── src/Aliare.Weather.Api/
│   │   ├── Api/Controllers/            # Auth, Weather
│   │   ├── Domain/                     # Entidades, interfaces
│   │   ├── Infrastructure/             # EF Core, repositórios, providers
│   │   └── Services/                   # WeatherService, AuthService
│   └── tests/Aliare.Weather.Api.Tests/
│       ├── Domain/                     # Testes de entidade
│       ├── Services/                   # Testes unitários (Moq)
│       └── Integration/               # Testes de integração
├── web/                                # Frontend Vue 3
│   ├── src/
│   │   ├── views/                      # LoginView, DashboardView
│   │   ├── components/                 # HistoryChart, AppNavbar
│   │   ├── stores/                     # auth (Pinia)
│   │   └── services/                   # weatherService, authService
│   └── e2e/                            # Testes Playwright
│       ├── fixtures/                   # auth.fixture.ts
│       └── *.spec.ts
├── mobile/                             # .NET MAUI (não implementado)
├── docker-compose.yml
└── .github/workflows/                  # CI: server, web, e2e
```

## Feature flag: provedor de clima

A configuração `WeatherProvider` alterna o provedor sem alterar código:

| Valor | Comportamento |
|-------|--------------|
| `OpenWeather` | Consulta a API real do OpenWeatherMap |
| `Fake` | Retorna dados simulados (desenvolvimento/testes) |

Configurável via `appsettings.json`, variável de ambiente ou Docker Compose.

## CI/CD

Três workflows no GitHub Actions, acionados em push/PR para `development` e `main`:

| Workflow | O que faz |
|----------|-----------|
| `server-ci.yml` | Restore, build e testes do backend (.NET 8) |
| `web-ci.yml` | Lint (oxlint + ESLint), type-check, testes unitários e build (Vue) |
| `e2e-ci.yml` | Docker Compose (db + api) + Playwright no Chromium |
