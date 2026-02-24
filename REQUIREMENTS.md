# Checklist de requisitos

Validação baseada nos requisitos definidos em [CHALLENGE.md](CHALLENGE.md).

## Requisitos funcionais

- [x] **Registrar temperatura por cidade** — `POST /api/v1/weather/register/by-city` recebe nome da cidade, consulta o provedor, persiste no banco e retorna a temperatura.
- [x] **Registrar temperatura por coordenadas** — `POST /api/v1/weather/register/by-coordinates` recebe latitude/longitude, consulta o provedor, persiste e retorna a temperatura.
- [x] **Histórico por cidade** — `GET /api/v1/weather/history/by-city/{cityName}` retorna registros dos últimos 30 dias, ordenados do mais recente ao mais antigo.
- [x] **Histórico por coordenadas** — `GET /api/v1/weather/history/by-coordinates?latitude=&longitude=` com a mesma lógica.
- [x] **Interface Web: registrar leitura** — Dashboard permite informar cidade e registrar temperatura.
- [x] **Interface Web: histórico em lista** — Dashboard exibe o histórico retornado pela API.
- [x] **Interface Web: histórico em gráfico** — Componente `HistoryChart` (Chart.js + vue-chartjs) renderiza o histórico em gráfico de linha.

## Requisitos não funcionais

- [x] **Backend em .NET 8 (C#)** — `Aliare.Weather.Api` com target `net8.0`.
- [x] **Frontend em Vue 3 + TypeScript** — Vue 3.5, TypeScript 5.9, Vite 7.
- [x] **Banco relacional PostgreSQL** — EF Core com Npgsql, migrations versionadas.
- [x] **Swagger** — Disponível em `/swagger` com definição de segurança Bearer.
- [x] **Health check em `/health`** — Configurado com `MapHealthChecks("/health")`.
- [x] **Testes unitários** — 43 testes backend (xUnit + Moq), 38 testes frontend (Vitest + Vue Test Utils).
- [x] **Teste de integração** — `WebApplicationFactory` com banco in-memory: endpoints de auth, weather e health.
- [x] **Docker Compose** — `docker-compose.yml` orquestra `db` (PostgreSQL), `api` (.NET) e `web` (nginx).
- [x] **README com instruções** — Guia de execução via Docker Compose e local.

## Pontos extras

- [x] **Autenticação JWT** — Endpoints de escrita protegidos com `[Authorize]`. Login retorna token, interceptor Axios no frontend.
- [x] **Feature flag para provedor de clima** — Configuração `WeatherProvider` alterna entre `OpenWeather` (real) e `Fake` (simulado).
- [ ] **Aplicativo .NET MAUI** — Projeto criado (`mobile/`), não implementado.
- [x] **Pipeline CI/CD (GitHub Actions)** — `server-ci.yml` (build + test .NET) e `web-ci.yml` (lint + type-check + test + build Vue).

## Entrega

- [ ] Resolução enviada via pull request.
- [ ] Imagem Docker publicada em host público (Docker Hub).
