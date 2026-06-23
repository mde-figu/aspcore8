# EcoTracker API — Governança e Compliance Ambiental (ESG)

API RESTful desenvolvida em **.NET Core 8** para o **Tema 4 — Governança e Compliance Ambiental**. A solução monitora emissões de carbono, controla licenças ambientais, registra auditorias de compliance e gerencia alertas ambientais automatizados.

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

### Docker
```bash
docker build -t ecotracker-api .
docker run -p 8080:8080 ecotracker-api
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

## Coleção Insomnia

O arquivo `insomnia/EcoTracker_Insomnia.json` contém todas as requisições organizadas por controller, pronto para importar no Insomnia.
