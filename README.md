# SkillPath

**SkillPath é um planejador enxuto de requalificação para quem está em risco de automação. Sem IA "mágica" e sem microserviços: só o essencial, bem amarrado. O usuário informa a profissão atual, a área onde quer atuar (ex.: tecnologia, logística, atendimento, finanças) e o nível de escolaridade; a API aplica regras simples e devolve uma trilha curta (3–6 itens) de skills e cursos associados. Depois, o usuário acompanha o avanço marcando o que concluiu, vendo barra de progresso**.

---

## ✨ Recursos Implementados

✅ **Autenticação JWT** — registro e login, emissão de token e proteção de rotas  
✅ **Catálogo** — *skills* e *cursos* com seed (EF InMemory) e filtros básicos  
✅ **Trilhas (Plans)** — geração baseada em regras objetivas (profissão atual + área‑alvo + escolaridade na v2)  
✅ **Progresso** — alternância de conclusão por item, cálculo de progresso (%) e `CompletedAt`  
✅ **Swagger/OpenAPI** — UI com **Authorize (Bearer)** e exemplos de requests/responses  
✅ **Versionamento** — **v1** (regras básicas) e **v2** (regras refinadas)  
✅ **Health Checks** — `/health` e `/health/ready` (pronto p/ produção)  
✅ **Observabilidade** — Serilog + OpenTelemetry (ASP.NET, EF Core) com `X-Correlation-Id`  
✅ **Testes xUnit** — serviços e integração via `WebApplicationFactory`  
✅ **Camadas** — API / Application / Domain / Infrastructure, com DI e responsabilidades nítidas

---

## 👥 Integrantes do Grupo

| Nome | RM |
|------|----|
| Leonardo da Silva Pereira | 557598 |
| Bruno da Silva Souza | 94346 |
| Julio Samuel de Oliveira | 557453 |

---

## 🧭 Visão Geral do Domínio

O usuário informa **profissão atual**, **área onde quer atuar** e **escolaridade**. A API gera uma **trilha de requalificação** (3 a 6 itens) com *skills/cursos* e permite acompanhar o progresso:

- **v1**: regras fixas por área‑alvo (Tecnologia, Logística, Atendimento, Finanças; *fallback* padrão).  
- **v2**: mantém a base e **refina** com escolaridade e equilíbrio **hard/soft skills**.

Entidades principais:

- **User** (Id, Name, Email, PasswordHash, CurrentJob, TargetArea, EducationLevel)  
- **Plan** (Id, UserId, Title, CreatedAt, Items[], Progress)  
- **PlanItem** (Order, Skill, Description, IsCompleted, **CompletedAt**)  
- **Skill** e **Course** (catálogo, com associação `Course.SkillId`)

---

## 🏗️ Arquitetura em Camadas

```
SkillPath.sln
/src
  /SkillPath.Api                # ASP.NET Core 8: Controllers, Swagger, Versioning, Auth middleware
  /SkillPath.Application        # Use cases (Auth/Plan), DTOs, validações, contratos
  /SkillPath.Domain             # Entidades ricas e interfaces de repositório
  /SkillPath.Infrastructure     # EF Core (InMemory/SQL), repositórios, JWT provider, DI, migrations
/tests
  /SkillPath.Tests              # xUnit: serviços e integração HTTP
```

Boas práticas aplicadas: **controllers finos**, **aplicação desacoplada de infra**, **injeção de dependências**, **tratamento de erros** consistente e **observabilidade** de ponta‑a‑ponta (trace + correlação).

---

## ⚙️ Configuração de Banco de Dados

Suporte a **dois modos** (via `appsettings.*`):

1. **InMemory (padrão)** — ideal para desenvolvimento/avaliadores:
   - Usa um banco único nomeado (ex.: `SkillPathDb`) para evitar seeds “fantasmas”.
   - Catálogo de *skills/cursos* é **seedado** na inicialização.

2. **SQL (SqlServer/Oracle)** — produção ou POCs com persistência real:
```jsonc
{
  "Database": { "Provider": "SqlServer" }, // "Oracle" ou qualquer outro => InMemory
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=SkillPath;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": { "Key": "chave-super-secreta-com-no-minimo-32-caracteres!" }
}
```

### Migrations (quando SQL)
```bash
dotnet ef database update   -s src/SkillPath.Api/SkillPath.Api.csproj   -p src/SkillPath.Infrastructure/SkillPath.Infrastructure.csproj
```

---

## 🚀 Como Executar

```bash
dotnet restore
dotnet build
dotnet run --project src/SkillPath.Api/SkillPath.Api.csproj
```

- **Swagger**: `https://localhost:5001/swagger` (ou a porta exibida no console)
- Clique em **Authorize**, cole **apenas** o token (sem “Bearer ”) e confirme.

---

## 🔐 Autenticação JWT (exemplos)

### 1) Registrar
```http
POST /api/v1/auth/register
Content-Type: application/json
```
```json
{
  "name": "Felipe",
  "email": "felipe@example.com",
  "password": "123456",
  "currentJob": "Operador de Caixa",
  "targetArea": "Tecnologia",
  "educationLevel": "Médio"
}
```

**Resposta 200 OK (resumo):**
```json
{ "userId": "GUID", "name": "Felipe", "email": "felipe@example.com", "token": "eyJ..." }
```

### 2) Login
```http
POST /api/v1/auth/login
Content-Type: application/json
```
```json
{ "email": "felipe@example.com", "password": "123456" }
```

### 3) Usar o token
```
Authorization: Bearer eyJ...
```

---

## 📚 Endpoints (v1/v2)

> Use `/api/v1/...` ou `/api/v2/...` (ou `x-api-version: 2.0`).

### Catálogo
- `GET /api/v{version}/skills?search=&page=&pageSize=`  
- `GET /api/v{version}/courses?skillId=&search=&page=&pageSize=`  
  - **v2** pode aceitar `POST /courses` (opcional) com validação de `skillId` existente.

### Trilhas (autenticado)
- `POST /api/v{version}/plans` — gera **ou** retorna trilha atual do usuário  
- `GET /api/v{version}/plans` — recupera trilha atual com progresso  
- `PUT /api/v{version}/plans/{planId}/items/{order}` — alterna conclusão (atualiza `CompletedAt`)  
- `DELETE /api/v{version}/plans/{planId}` — remove trilha atual

### Health & Observabilidade
- `GET /health` — liveness  
- `GET /health/ready` — readiness (inclui checagem do banco quando SQL)  
- `X-Correlation-Id` — header aceito e devolvido em responses  
- Tracing distribuído com OpenTelemetry (exporter de console por padrão)

---

## 📘 Exemplos de Swagger (CRUD/Uso)

**Skills (leitura)**  
```http
GET /api/v1/skills
```

**Courses (listar por skill)**  
```http
GET /api/v1/courses?skillId=1
```

**Courses (criar – v2 opcional)**
```http
POST /api/v2/courses
Authorization: Bearer eyJ...
Content-Type: application/json
```
```json
{
  "name": "Matemática Financeira na Prática",
  "provider": "Plataforma Y",
  "url": "https://example.com/fin",
  "skillId": 1
}
```

**Plans (gerar/obter)**  
```http
POST /api/v1/plans
Authorization: Bearer eyJ...
```
> Não envia body; usa o **usuário do token**.

**Plans (ver atual)**  
```http
GET /api/v1/plans
Authorization: Bearer eyJ...
```

**Plans (toggle item)**  
```http
PUT /api/v1/plans/{planId}/items/{order}
Authorization: Bearer eyJ...
```

**Plans (remover)**  
```http
DELETE /api/v1/plans/{planId}
Authorization: Bearer eyJ...
```

---

## 🧪 Testes

### Executar tudo
```bash
dotnet test
```

### Modo watch (dev rápido)
```bash
dotnet watch test --project tests/SkillPath.Tests
```

### Listar nomes exatos de testes
```bash
dotnet test --list-tests -v n
```

### Cobertura (opcional — `coverlet.collector` no projeto de testes)
```bash
dotnet test tests/SkillPath.Tests   /p:CollectCoverage=true   /p:CoverletOutputFormat=lcov   /p:CoverletOutput=./TestResults/coverage
```

Cobertura alvo: geração de plano (v1/v2), alternância de item (incluindo `CompletedAt`), paginação do catálogo e fluxo feliz de autenticação.

---

## 🧠 Boas Práticas e Decisões de Arquitetura

- **Separation of Concerns**: controllers só orquestram; casos de uso ficam na **Application**.  
- **Application** não conhece EF/JWT; depende de **interfaces** (`IUserRepository`, `IPlanRepository`, `ITokenProvider`, `ICatalogRepository`).  
- **Infrastructure** implementa detalhes (EF InMemory/SQL, JWT, repos).  
- **Versionamento** sem quebrar contratos: v2 estende regra mantendo resposta compatível.  
- **HATEOAS** nos recursos de plano: links `self`, `toggleItem`, `courses`.  
- **Observabilidade** first‑class: logs com correlação e traces por operação.

---

## 🧾 Status Codes esperados

- `200 OK` — leitura/ações bem‑sucedidas (POST de plano retorna 200)  
- `201 Created` — criação de recursos (ex.: `POST /v2/courses`, se habilitado)  
- `204 No Content` — deleções/updates sem corpo  
- `400 Bad Request` — validação (ex.: área‑alvo inválida na v1/v2, JSON malformado)  
- `401 Unauthorized` — token ausente/inválido  
- `403 Forbidden` — falta de permissão  
- `404 Not Found` — recurso inexistente (ex.: `skillId` não existe)  

---

## 🛠️ Troubleshooting (o que costuma quebrar)

- **401 no /plans**: você não aplicou o token no Swagger (Authorize), ou token antigo/gerado com chave diferente. Gere login novamente e confira no bloco `curl` se aparece `Authorization: Bearer eyJ...`.
- **400 “TargetArea”**: área‑alvo fora das palavras‑chave aceitas. Use “Tecnologia”, “Logística”, “Atendimento” ou “Finanças” (v2 aplica normalização e sinônimos; há *fallback* seguro).
- **404 em /api/v2/...**: se a v2 não estiver habilitada no seu build, use **/api/v1**.  
- **Seed do catálogo não aparece**: banco InMemory com **nomes diferentes** em cada run/versão. Use **um único nome** (`SkillPathDb`) e reinicie a API.
- **Failed to fetch no Swagger**: use a **mesma origem/porta (HTTPS)** do Swagger e mantenha `SwaggerEndpoint` relativo.
- **SQL/Oracle**: configure `ConnectionStrings:Default` e rode `dotnet ef database update` com `-s`/`-p` conforme acima.

---

## 🔌 Comandos Rápidos

```bash
dotnet restore
dotnet build
dotnet run --project src/SkillPath.Api/SkillPath.Api.csproj
dotnet test
```
