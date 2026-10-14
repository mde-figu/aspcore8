# EcoTracker — Projeto Cidades ESG Inteligentes

**Fase 6 — DevOps** · Grupo 102 · Markus de Figueiredo Ferraz Wirz (markusfwirz@gmail.com)

API RESTful desenvolvida em **.NET Core 8** para o **Tema 4 — Governança e Compliance Ambiental**. A solução monitora emissões de carbono, controla licenças ambientais, registra auditorias de compliance e gerencia alertas ambientais automatizados.

Esta entrega acrescenta ao projeto a camada de **DevOps**: containerização, orquestração com Docker Compose (API + SQL Server, com volumes, redes e variáveis de ambiente) e um pipeline de **CI/CD no GitHub Actions** com build, testes, publicação de imagem e deploy automatizado em **staging** e **produção**.

## Arquitetura

- **Padrão MVVM**: Models, ViewModels (DTOs de request/response) e Controllers
- **Entity Framework Core 8** com SQL Server (fallback InMemory)
- **JWT Authentication** para endpoints protegidos
- **Paginação** em todos os endpoints de listagem
- **Validação** via Data Annotations nos ViewModels
- **Tratamento global de exceções** via middleware
- **Swagger/OpenAPI** para documentação interativa

## Controllers (4 endpoints RESTful)

| Controller | Rota Base | Descrição |
|---|---|---|
| `CarbonEmissionsController` | `/api/CarbonEmissions` | Monitoramento de emissão de carbono e compensação ambiental |
| `EnvironmentalLicensesController` | `/api/EnvironmentalLicenses` | Controle de licenças ambientais e alertas de renovação |
| `ComplianceAuditsController` | `/api/ComplianceAudits` | Registro de conformidade com normas ambientais e auditorias |
| `EnvironmentalAlertsController` | `/api/EnvironmentalAlerts` | Alertas automáticos (expiração, limites, etc.) |
| `AuthController` | `/api/Auth` | Autenticação JWT |

## Como Executar

### Requisitos
- .NET SDK 8.0+
- Docker (opcional)

### Localmente
```bash
cd src/EcoTracker
dotnet run
```
A API estará disponível em `http://localhost:5000`. Acesse `http://localhost:5000/swagger` para a documentação interativa.

### Docker (imagem isolada)
```bash
docker build -t ecotracker-api:local .
docker run -p 8080:8080 -e DisableHttpsRedirection=true ecotracker-api:local
```

### Docker Compose (API + SQL Server) — forma recomendada
```bash
cp .env.example .env          # ajuste senhas e portas
docker compose up -d --build  # sobe api + db
curl http://localhost:8080/health
# Swagger: http://localhost:8080/swagger
docker compose down           # (use -v para apagar os volumes)
```

Ambientes:
```bash
# staging  (Swagger habilitado, porta 8080)
docker compose -f docker-compose.yml -f docker-compose.staging.yml up -d

# produção (Swagger desabilitado, porta 9090, banco não exposto)
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

## Autenticação

Faça login via `POST /api/Auth/login` para obter o token JWT:

| Usuário | Senha | Role |
|---|---|---|
| `admin` | `Admin@123` | Admin |
| `auditor` | `Auditor@123` | Auditor |
| `viewer` | `Viewer@123` | Viewer |

Use o token no header: `Authorization: Bearer <token>`

## Testes

```bash
dotnet test
```

17 testes de integração xUnit validando status code 200 para todos os controllers.

## Banco de Dados

- **Desenvolvimento**: InMemory Database (dados seed incluídos)
- **Produção**: SQL Server (configure a connection string em `appsettings.json`)
- **Migrações**: `src/EcoTracker/Data/Migrations/`

Para aplicar migrações no SQL Server:
```bash
dotnet ef database update --project src/EcoTracker
```

---

# DevOps — Fase 6

## Tecnologias utilizadas

| Camada | Tecnologia |
|---|---|
| Aplicação | .NET 8 / ASP.NET Core, EF Core 8, JWT, Swagger |
| Testes | xUnit (17 testes de integração com `WebApplicationFactory`) |
| Container | Docker (multi-stage build, imagem final `mcr.microsoft.com/dotnet/aspnet:8.0`, usuário não-root, HEALTHCHECK) |
| Orquestração | Docker Compose (serviços `api` + `db`, volumes nomeados, rede bridge dedicada, variáveis de ambiente) |
| Banco | SQL Server 2022 (container), InMemory como fallback quando não há connection string |
| CI/CD | GitHub Actions (`.github/workflows/ci-cd.yml`) + composite action `.github/actions/deploy-compose` |
| Registry | GitHub Container Registry (ghcr.io) |

## Pipeline de CI/CD

Arquivo: `.github/workflows/ci-cd.yml`. Dispara em `push` em qualquer branch, em `pull_request` e manualmente (`workflow_dispatch`).

```
build ──► test ──► docker ──► deploy-staging ──► deploy-production
```

| Job | O que faz |
|---|---|
| `build` | `dotnet restore` + `dotnet build -c Release` + `dotnet publish`; publica o artefato `ecotracker-publish` |
| `test` | `dotnet test -c Release` com relatório `.trx`; publica o artefato `test-results` (falha do teste quebra o pipeline) |
| `docker` | Build multi-stage com cache do Buildx e push para `ghcr.io/<repo>/ecotracker-api` nas tags `:<sha>` e `:latest` |
| `deploy-staging` | Ambiente `staging`: gera o `.env`, faz `docker compose -f docker-compose.yml -f docker-compose.staging.yml up -d` com a imagem recém-publicada, aguarda o `/health` e roda smoke test (login JWT + consulta autenticada) |
| `deploy-production` | Ambiente `production`, só após o staging passar: mesmo fluxo com `docker-compose.prod.yml` (Swagger desabilitado, banco sem porta publicada, limites de CPU/memória) na porta 9090 |

A lógica de deploy está isolada na composite action `.github/actions/deploy-compose`, reutilizada pelos dois ambientes apenas trocando o override do Compose, a porta e os segredos. Senhas (`MSSQL_SA_PASSWORD`, `JWT_KEY`) vêm de *secrets* do repositório; nenhum segredo real está versionado — apenas placeholders em `.env.example`.

## Containerização

`Dockerfile` multi-stage:

1. **build** (`sdk:8.0`) — restore em camada separada (cache de NuGet) e `dotnet build -c Release`;
2. **test** — estágio opcional que executa `dotnet test` dentro da imagem;
3. **publish** — `dotnet publish` sem apphost;
4. **final** (`aspnet:8.0`) — apenas o runtime + artefatos publicados, usuário `ecotracker` (uid 1001), `EXPOSE 8080` e `HEALTHCHECK` chamando `/health`. Imagem final de ~243 MB.

A aplicação ganhou um endpoint anônimo `GET /health` (ASP.NET Core Health Checks) e três chaves de configuração para operar em containers: `EnableSwagger`, `DisableHttpsRedirection` e `Database:StartupRetries` (retentativas até o SQL Server ficar disponível).

## Orquestração (Docker Compose)

`docker-compose.yml` define a stack base e os overrides de ambiente:

- **Serviços**: `api` (porta `${API_PORT}` → 8080) e `db` (SQL Server 2022);
- **Volumes**: `mssql-data` (dados do banco, `mssql-data-staging` / `mssql-data-prod` nos overrides) e `api-logs`;
- **Rede**: `ecotracker-net` (bridge) — a API acessa o banco pelo hostname `db`;
- **Variáveis de ambiente**: lidas do `.env` (`cp .env.example .env`) e injetadas como configuração do ASP.NET (`ConnectionStrings__DefaultConnection`, `Jwt__Key`, `EnableSwagger`, ...);
- **Dependência e saúde**: a API só inicia com `db` `service_healthy`; ambos têm `healthcheck` e `restart: unless-stopped`.

## Evidências

Build e testes automatizados:

![Build](docs/devops/shot_build.png)
![Testes](docs/devops/shot_test.png)

Subida da stack em staging, imagem/volumes/rede criados:

![Compose staging](docs/devops/shot_compose_staging.png)
![Infra](docs/devops/shot_infra.png)

Aplicação funcionando em **staging** (health, login JWT, consulta autenticada e Swagger):

![Smoke test](docs/devops/shot_smoke.png)
![API autenticada](docs/devops/shot_api.png)
![Swagger staging](docs/devops/shot_swagger_staging.png)

Aplicação funcionando em **produção** (porta 9090, Swagger retornando 404, banco sem porta publicada):

![Produção](docs/devops/shot_prod.png)

Validação dos arquivos de Compose e logs da API:

![Config e logs](docs/devops/shot_config.png)

Execução real do pipeline no GitHub Actions (build → testes → imagem → staging → produção, todos verdes):

![Pipeline](docs/devops/shot_pipeline.png)
![Deploy staging no CI](docs/devops/shot_ci_staging.png)
![Deploy produção no CI](docs/devops/shot_ci_prod.png)

As execuções ficam em **Actions** do repositório: https://github.com/mde-figu/aspcore8/actions

## Checklist da entrega

- [x] Projeto compactado em `.zip` organizado
- [x] Dockerfile funcional (multi-stage, usuário não-root, healthcheck)
- [x] `docker-compose.yml` com volumes, variáveis de ambiente e rede (+ overrides de staging e produção)
- [x] Pipeline com build, testes e deploy automatizados (GitHub Actions)
- [x] README com instruções de execução e prints
- [x] Documentação técnica em PDF com evidências
- [x] Deploy realizado em staging e em produção
