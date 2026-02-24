# Aliare Weather

Aplicação fullstack para consulta e registro de dados climáticos por cidade ou coordenadas, com histórico dos últimos 30 dias e visualização em gráfico.

## Tecnologias

### Backend
- .NET 8 (C#) — API REST com versionamento (`/api/v1/`)
- Entity Framework Core + PostgreSQL
- Autenticação JWT (BCrypt para hash de senhas)
- Swagger (OpenAPI)
- Health Check em `/health`
- xUnit + Moq + WebApplicationFactory (43 testes)

### Frontend
- Vue 3 + TypeScript
- Pinia (gerenciamento de estado)
- Vue Router (navegação SPA)
- Axios (HTTP client com interceptor JWT)
- Chart.js + vue-chartjs (gráfico de histórico)
- Vitest + Vue Test Utils (38 testes)

### Infraestrutura
- Docker com multi-stage builds
- Docker Compose (PostgreSQL + API + Frontend via nginx)
- GitHub Actions (CI separado para server e web)

## Estrutura do projeto

```
├── server/                          # Backend .NET 8
│   ├── src/Aliare.Weather.Api/
│   │   ├── Api/Controllers/         # AuthController, WeatherController
│   │   ├── Domain/                  # Entidades, interfaces, modelos
│   │   ├── Infrastructure/          # EF Core, repositórios, providers
│   │   ├── Services/                # WeatherService, AuthService
│   │   └── Program.cs
│   └── tests/Aliare.Weather.Api.Tests/
│       ├── Domain/                  # Testes de entidade
│       ├── Services/                # Testes unitários (Moq)
│       └── Integration/             # Testes de integração (WebApplicationFactory)
├── web/                             # Frontend Vue 3
│   ├── src/
│   │   ├── views/                   # LoginView, DashboardView
│   │   ├── components/              # HistoryChart, AppNavbar
│   │   ├── stores/                  # auth (JWT), theme (dark mode)
│   │   └── router/
│   └── nginx.conf                   # Reverse proxy para produção
├── docker-compose.yml
└── .env.example
```

## Endpoints da API

| Método | Rota | Auth | Descrição |
|--------|------|:----:|-----------|
| `POST` | `/api/v1/auth/register` | — | Cadastro de usuário |
| `POST` | `/api/v1/auth/login` | — | Login (retorna JWT) |
| `POST` | `/api/v1/weather/register/by-city` | JWT | Registra clima por nome da cidade |
| `POST` | `/api/v1/weather/register/by-coordinates` | JWT | Registra clima por latitude/longitude |
| `GET` | `/api/v1/weather/history/by-city/{cityName}` | — | Histórico por cidade (últimos 30 dias) |
| `GET` | `/api/v1/weather/history/by-coordinates?latitude=&longitude=` | — | Histórico por coordenadas |
| `GET` | `/health` | — | Health check |
| `GET` | `/swagger` | — | Documentação interativa da API |

## Execução com Docker Compose

### Pré-requisitos
- [Docker](https://docs.docker.com/get-docker/) e [Docker Compose](https://docs.docker.com/compose/install/)
- Chave de API do [OpenWeatherMap](https://openweathermap.org/api) (plano gratuito)

### Passo a passo

1. Clone o repositório:
```bash
git clone https://github.com/Lucas-Dreveck/desafio-fullstack-aliare.git
cd desafio-fullstack-aliare
```

2. Configure a variável de ambiente:
```bash
cp .env.example .env
```
Edite o `.env` e insira sua chave da OpenWeatherMap:
```dotenv
OPENWEATHER_API_KEY=sua_chave_aqui
```

3. Inicie os serviços:
```bash
docker compose up --build -d
```

4. Acesse a aplicação:

| Serviço | URL |
|---------|-----|
| Frontend | http://localhost:3000 |
| API | http://localhost:5251 |
| Swagger | http://localhost:5251/swagger |
| Health Check | http://localhost:5251/health |

5. Para parar:
```bash
docker compose down
```

> O volume `pgdata` persiste os dados do PostgreSQL entre reinícios. Para limpar tudo: `docker compose down -v`

## Execução local (desenvolvimento)

### Backend

Requisitos: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0), PostgreSQL rodando na porta 5432.

```bash
cd server/src/Aliare.Weather.Api
```

Configure o `appsettings.Development.json` com sua connection string e API key, então:

```bash
dotnet run
```

A API estará disponível em `https://localhost:5251`.

### Frontend

Requisitos: [Node.js](https://nodejs.org/) 20.19+ ou 22.12+.

```bash
cd web
npm install
npm run dev
```

O frontend estará disponível em `http://localhost:5173` com proxy automático para a API via Vite.

## Testes

### Backend (43 testes — xUnit)
```bash
cd server/tests/Aliare.Weather.Api.Tests
dotnet test
```

Cobertura: entidades de domínio, serviços (unitário com Moq), integração (endpoints HTTP com banco in-memory).

### Frontend (38 testes — Vitest)
```bash
cd web
npm run test:unit
```

Cobertura: stores (auth, theme), utilitários (JWT decode), componentes (LoginView).

## Feature flag: provedor de clima

A configuração `WeatherProvider` permite trocar entre provedores sem alterar código:

| Valor | Comportamento |
|-------|--------------|
| `OpenWeather` | Consulta a API real do OpenWeatherMap |
| `Fake` | Retorna dados simulados (útil para desenvolvimento/testes) |

No `appsettings.json`:
```json
{
  "WeatherProvider": "OpenWeather"
}
```

No Docker Compose, é configurado via variável de ambiente `WeatherProvider`.
