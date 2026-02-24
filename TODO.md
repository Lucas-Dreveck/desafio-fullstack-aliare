# Plano de ação

## Em Andamento
- [ ] Configurar conexão API e interceptor JWT (Frontend);

## Concluído

### Backend
- [x] Organizar domínio: entidade, interface do provider e testes unitários (TDD);
- [x] Criar service com registro e histórico de temperaturas (TDD);
- [x] Conectar DB (PostgreSQL) e organizar schemas;
- [x] Implementar provider com *[OpenWeatherMaps](https://openweathermap.org)*;
- [x] Criar endpoints de registro e histórico por cidade e por lat/long;
- [x] Configurar health check em /health;
- [x] Adicionar autenticação JWT;
- [x] Validar testes unitários e de integração;
- [x] Adicionar feature flag para troca de provedor;

### Frontend
- [x] Configurar estilo padrão e rotas;

### Mobile (Opcional)
- Nenhuma tarefa no momento

### DevOps
- [x] Definir a estrutura base do repositório monorepo (API .NET, Vue 3 + TS, MAUI);
- [x] GitHub Actions parcial (server);

## A fazer (Backlog)

### Backend
- Nenhuma tarefa no momento

### Frontend
- [ ] Tela de Login e registro de usuário;
- [ ] Tela de Dashboard para consulta de temperatura;
- [ ] Tela de Histórico com lista e gráfico;

### Mobile (Opcional)
- [ ] Configurar Estilo e rotas baseado no Frontend;
- [ ] Conexão API;
- [ ] Replicação das telas Frontend;

### DevOps
- [ ] Configurar `docker-compose.yml` e publicar imagem no Docker Hub;
- [ ] Documentar e guiar execução do projeto no `README.md`;
- [ ] Configurar GitHub Actions (completamente);
