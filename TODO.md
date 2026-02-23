# Plano de ação

## Em Andamento
- [ ] Implementar provider com *[OpenWeatherMaps](https://openweathermap.org)*;

## Concluído
- [x] Definir a estrutura base do repositório monorepo (API .NET, Vue 3 + TS, MAUI);
- [x] Organizar domínio: entidade, interface do provider e testes unitários (TDD);
- [x] Criar service com registro e histórico de temperaturas (TDD);
- [x] Conectar DB (PostgreSQL) e organizar schemas;

## A fazer (Backlog)

### Backend
- [ ] Implementar provider com *[OpenWeatherMaps](https://openweathermap.org)*;
- [ ] Criar endpoints de registro por cidade e por lat/long;
- [ ] Criar endpoint de histórico (30 dias) com modo lat/long e cidade;
- [ ] Validar testes unitários e de integração;
- [ ] Configurar health check em /health;
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
- [ ] Configurar Estilo e rotas baseado no Frontend;
- [ ] Conexão API;
- [ ] Replicação das telas Frontend;

### DevOps
- [ ] Configurar `docker-compose.yml` e publicar imagem no Docker Hub;
- [ ] Documentar e guiar execução do projeto no `README.md`;
- [ ] Configurar GitHub Actions;
