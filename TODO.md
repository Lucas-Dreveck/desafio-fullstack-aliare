# Plano de ação

## Em Andamento
- [ ] Organizar requisitos e começar testes (TDD) para o server;

## Conclúido
- [x] Definir a estrutura base do repositório monorepo (API .NET, Vue 3 + TS, MAUI);

## A fazer (Backlog)

### Backend
- [ ] Conectar DB (PostgreSQL) e organizar schemas;
- [ ] Criar service e conectar com *[OpenWeatherMaps](https://openweathermap.org)*;
- [ ] Criar endpoints de consulta (e health check) e validar tests unitários;
- [ ] Criar endpoint de histórico (30 dias) com modo lat/long e cidade e validar testes de integração;
- [ ] Adicionar autenticação JWT;
- [ ] Validar e toques finais ao Swagger;
- [ ] Adicionar feature flag para troca de provedor;

### Frontend
- [ ] Configurar estilo padrão e rotas;
- [ ] Configurar conexão API;
- [ ] Tela para Login e validar conexão;
- [ ] Telas para consulta;
- [ ] Telas para histórico com modo de lista e gráfico;

### Mobile (Opcional)
- [ ] Configurar Estilo e otas baseado no Frontend;
- [ ] Conexão API;
- [ ] Replicação das telas Frontend;

### DevOps
- [ ] Configurar `docker-compose.yml` e publicar imagem no Docker Hub;
- [ ] Documentar e guiar execução do projeto no `README.md`;
- [ ] Configurar GitHub Actions;
