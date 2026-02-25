# Plano de ação

## Em Andamento
- [ ] Implementar testes e2e com Playwright;

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
- [x] Substituir `var` por tipos explícitos em todo o backend;
- [x] Usar Guid como chave primária do User em vez de int sequencial;
- [x] Tratar cidades homônimas e tolerância de coordenadas no histórico (backend);
- [x] Implementar refresh token (backend);

### Frontend
- [x] Configurar estilo padrão e rotas;
- [x] Configurar conexão API e interceptor JWT (Frontend);
- [x] Tela de Login e registro de usuário;
- [x] Dashboard com registro de temperatura e consulta de histórico;
- [x] Gráfico de histórico (Chart.js + vue-chartjs);
- [x] Implementar testes unitários e de componente;
- [x] Tratar cidades homônimas e tolerância de coordenadas no histórico (frontend);
- [x] Implementar refresh token (frontend);

### Mobile (Opcional)
- Nenhuma tarefa no momento

### DevOps
- [x] Definir a estrutura base do repositório monorepo (API .NET, Vue 3 + TS, MAUI);
- [x] GitHub Actions completo (server + web);
- [x] Configurar Docker Compose (API + PostgreSQL + Frontend via nginx);
- [x] Documentar e guiar execução do projeto no `README.md`;

## A fazer (Backlog)

### Backend
- Nenhuma tarefa no momento

### Frontend
- Nenhuma tarefa no momento

### DevOps
- [ ] Publicar imagens no Docker Hub;

### Mobile (Opcional)
- [ ] Configurar Estilo e rotas baseado no Frontend;
- [ ] Conexão API;
- [ ] Replicação das telas Frontend;
