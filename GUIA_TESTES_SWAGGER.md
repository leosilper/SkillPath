# Guia de Testes CRUD no Swagger - SkillShift Hub API

Este guia detalha como testar todas as operações CRUD (Create, Read, Update, Delete) da SkillShift Hub API usando o Swagger UI.

## 📋 Índice

1. [Acessando o Swagger](#1-acessando-o-swagger)
2. [Configurando Autenticação](#2-configurando-autenticação)
3. [Testando CRUD de Skills](#3-testando-crud-de-skills)
4. [Testando CRUD de Courses](#4-testando-crud-de-courses)
5. [Testando Operações de Plans](#5-testando-operações-de-plans)
6. [Fluxo Completo de Teste](#6-fluxo-completo-de-teste)
7. [Troubleshooting](#7-troubleshooting)

---

## 1. Acessando o Swagger

### 1.1. Executar a API

```bash
# Restaurar dependências
dotnet restore

# Compilar o projeto
dotnet build

# Executar a API
dotnet run --project src/SkillShiftHub.Api/SkillShiftHub.Api.csproj
```

### 1.2. Abrir o Swagger UI

1. Abra seu navegador
2. Acesse: `https://localhost:5001/swagger`
3. Se aparecer um aviso de certificado SSL, aceite a exceção (ambiente de desenvolvimento)
4. No topo da página, selecione a versão da API:
   - **v1.0** - Regras originais de trilha
   - **v2.0** - Versão refinada com novas regras

---

## 2. Configurando Autenticação

A maioria das operações de escrita (CREATE, UPDATE, DELETE) requer autenticação via JWT.

### 2.1. Obter Token JWT

#### Opção A: Registrar Novo Usuário

1. Localize o endpoint `POST /api/v1/auth/register`
2. Clique em **"Try it out"**
3. Preencha o body com os dados do usuário:

```json
{
  "name": "João Silva",
  "email": "joao.silva@example.com",
  "password": "senha123456",
  "currentJob": "Desenvolvedor",
  "targetArea": "Data Science",
  "educationLevel": "Superior Completo"
}
```

4. Clique em **"Execute"**
5. Na resposta, copie o valor do campo `token`:

```json
{
  "userId": "123e4567-e89b-12d3-a456-426614174000",
  "name": "João Silva",
  "email": "joao.silva@example.com",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

#### Opção B: Fazer Login

1. Localize o endpoint `POST /api/v1/auth/login`
2. Clique em **"Try it out"**
3. Preencha o body:

```json
{
  "email": "joao.silva@example.com",
  "password": "senha123456"
}
```

4. Clique em **"Execute"**
5. Copie o `token` da resposta

### 2.2. Configurar Token no Swagger

1. No topo da página do Swagger, clique no botão **"Authorize"** (ícone de cadeado 🔒)
2. No campo **"Value"**, cole **APENAS** o token (sem a palavra "Bearer")
   - ✅ **Correto**: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
   - ❌ **Errado**: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
3. Clique em **"Authorize"**
4. Clique em **"Close"**

> **Nota**: O token é válido por 4 horas. Se receber erro 401, faça login novamente.

---

## 3. Testando CRUD de Skills

### 3.1. READ - Listar Skills (Não requer autenticação)

**Endpoint**: `GET /api/v1/skills`

1. Clique em **"Try it out"**
2. Configure os parâmetros opcionais:
   - `search`: Buscar por nome (ex: "python")
   - `page`: Número da página (padrão: 1)
   - `pageSize`: Itens por página (padrão: 10)
3. Clique em **"Execute"**
4. Verifique a resposta com lista paginada e links HATEOAS

**Exemplo de Resposta**:
```json
{
  "data": [
    {
      "id": 1,
      "name": "Python Programming",
      "description": "Fundamentos de programação em Python"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalItems": 25,
    "totalPages": 3
  },
  "links": {
    "self": "/api/v1/skills?page=1&pageSize=10",
    "next": "/api/v1/skills?page=2&pageSize=10",
    "prev": null
  }
}
```

### 3.2. READ - Buscar Skill por ID (Não requer autenticação)

**Endpoint**: `GET /api/v1/skills/{id}`

1. Clique em **"Try it out"**
2. Preencha o `id` (ex: 1)
3. Clique em **"Execute"**

**Exemplo de Resposta**:
```json
{
  "id": 1,
  "name": "Python Programming",
  "description": "Fundamentos de programação em Python"
}
```

### 3.3. CREATE - Criar Skill (Requer autenticação)

**Endpoint**: `POST /api/v1/skills`

1. **Certifique-se de ter configurado o token** (seção 2.2)
2. Clique em **"Try it out"**
3. Preencha o body:

```json
{
  "name": "Python Programming",
  "description": "Fundamentos de programação em Python"
}
```

4. Clique em **"Execute"**
5. Verifique a resposta **201 Created** com o skill criado

**Validações**:
- `name`: Obrigatório, máximo 200 caracteres
- `description`: Obrigatório, máximo 400 caracteres

### 3.4. UPDATE - Atualizar Skill (Requer autenticação)

**Endpoint**: `PUT /api/v1/skills/{id}`

1. **Certifique-se de ter configurado o token**
2. Clique em **"Try it out"**
3. Preencha o `id` do skill a ser atualizado
4. Preencha o body:

```json
{
  "name": "Python Avançado",
  "description": "Programação avançada em Python com frameworks e bibliotecas modernas"
}
```

5. Clique em **"Execute"**
6. Verifique a resposta **200 OK** com o skill atualizado

### 3.5. DELETE - Deletar Skill (Requer autenticação)

**Endpoint**: `DELETE /api/v1/skills/{id}`

1. **Certifique-se de ter configurado o token**
2. Clique em **"Try it out"**
3. Preencha o `id` do skill a ser deletado
4. Clique em **"Execute"**
5. Verifique a resposta **204 No Content**

---

## 4. Testando CRUD de Courses

### 4.1. READ - Listar Courses (Não requer autenticação)

**Endpoint**: `GET /api/v1/courses`

1. Clique em **"Try it out"**
2. Configure os parâmetros opcionais:
   - `skillId`: Filtrar por skill (ex: 1)
   - `search`: Buscar por nome (ex: "python")
   - `page`: Número da página (padrão: 1)
   - `pageSize`: Itens por página (padrão: 10)
3. Clique em **"Execute"**

**Exemplo de Resposta**:
```json
{
  "data": [
    {
      "id": 1,
      "skillId": 1,
      "skillName": "Python Programming",
      "name": "Curso de Python Básico",
      "provider": "Udemy",
      "url": "https://www.udemy.com/course/python-basico"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalItems": 15,
    "totalPages": 2
  },
  "links": {
    "self": "/api/v1/courses?page=1&pageSize=10",
    "next": "/api/v1/courses?page=2&pageSize=10",
    "prev": null
  }
}
```

### 4.2. READ - Buscar Course por ID (Não requer autenticação)

**Endpoint**: `GET /api/v1/courses/{id}`

1. Clique em **"Try it out"**
2. Preencha o `id` (ex: 1)
3. Clique em **"Execute"**

### 4.3. CREATE - Criar Course (Requer autenticação)

**Endpoint**: `POST /api/v1/courses`

1. **Certifique-se de ter configurado o token**
2. Clique em **"Try it out"**
3. Preencha o body:

```json
{
  "skillId": 1,
  "name": "Curso de Python Básico",
  "provider": "Udemy",
  "url": "https://www.udemy.com/course/python-basico"
}
```

4. Clique em **"Execute"**
5. Verifique a resposta **201 Created**

**Validações**:
- `skillId`: Obrigatório, deve existir na base de dados
- `name`: Obrigatório, máximo 220 caracteres
- `provider`: Obrigatório, máximo 160 caracteres
- `url`: Obrigatório, máximo 400 caracteres, deve ser uma URL válida

### 4.4. UPDATE - Atualizar Course (Requer autenticação)

**Endpoint**: `PUT /api/v1/courses/{id}`

1. **Certifique-se de ter configurado o token**
2. Clique em **"Try it out"**
3. Preencha o `id` do course a ser atualizado
4. Preencha o body:

```json
{
  "skillId": 1,
  "name": "Curso de Python Avançado",
  "provider": "Coursera",
  "url": "https://www.coursera.org/course/python-avancado"
}
```

5. Clique em **"Execute"**
6. Verifique a resposta **200 OK**

### 4.5. DELETE - Deletar Course (Requer autenticação)

**Endpoint**: `DELETE /api/v1/courses/{id}`

1. **Certifique-se de ter configurado o token**
2. Clique em **"Try it out"**
3. Preencha o `id` do course a ser deletado
4. Clique em **"Execute"**
5. Verifique a resposta **204 No Content**

---

## 5. Testando Operações de Plans

Os Plans não seguem um CRUD tradicional, mas possuem operações importantes para gerenciar trilhas de requalificação.

### 5.1. CREATE/GENERATE - Gerar ou Buscar Plano (Requer autenticação)

**Endpoint**: `POST /api/v1/plans` ou `POST /api/v2/plans`

1. **Certifique-se de ter configurado o token**
2. Clique em **"Try it out"**
3. **Não é necessário enviar body** (o userId vem do token JWT)
4. Clique em **"Execute"**

**Comportamento**:
- Se o usuário **não tem plano**: cria um novo → **201 Created**
- Se o usuário **já tem plano**: retorna o existente → **200 OK**

**Exemplo de Resposta**:
```json
{
  "planId": "123e4567-e89b-12d3-a456-426614174000",
  "title": "Trilha de Requalificação - Data Science",
  "createdAt": "2024-11-12T10:00:00Z",
  "totalItems": 5,
  "completedItems": 0,
  "progressPercent": 0,
  "items": [
    {
      "skillId": 1,
      "order": 1,
      "skill": "Python Programming",
      "description": "Fundamentos de programação em Python",
      "isCompleted": false,
      "completedAt": null
    },
    {
      "skillId": 2,
      "order": 2,
      "skill": "Data Analysis",
      "description": "Análise de dados com Pandas",
      "isCompleted": false,
      "completedAt": null
    }
  ],
  "links": {
    "self": "/api/v1/plans",
    "toggleItemTemplate": "/api/v1/plans/123e4567-e89b-12d3-a456-426614174000/items/{order}",
    "coursesTemplate": "/api/v1/courses?skillId={skillId}"
  }
}
```

> **Importante**: Anote o `planId` e os valores de `order` dos itens para usar nas próximas operações.

### 5.2. READ - Buscar Plano Atual (Requer autenticação)

**Endpoint**: `GET /api/v1/plans` ou `GET /api/v2/plans`

1. **Certifique-se de ter configurado o token**
2. Clique em **"Try it out"**
3. Clique em **"Execute"**
4. Verifique a resposta com o plano atual do usuário autenticado

### 5.3. UPDATE - Alternar Conclusão de Item (Requer autenticação)

**Endpoint**: `PUT /api/v1/plans/{planId}/items/{order}` ou `PUT /api/v2/plans/{planId}/items/{order}`

Esta operação alterna o status de conclusão de um item do plano:
- Se estiver **incompleto** → marca como **completo** (`IsCompleted: true`, `CompletedAt` preenchido)
- Se estiver **completo** → marca como **incompleto** (`IsCompleted: false`, `CompletedAt: null`)

1. **Certifique-se de ter configurado o token**
2. Clique em **"Try it out"**
3. Preencha os parâmetros:
   - `planId`: GUID do plano (obtido na resposta do GET/POST)
   - `order`: Número da ordem do item (1, 2, 3, etc.)
4. Clique em **"Execute"**
5. Verifique a resposta **200 OK** com o plano atualizado

**Exemplo**:
```
PUT /api/v1/plans/123e4567-e89b-12d3-a456-426614174000/items/1
```

### 5.4. DELETE - Deletar Plano (Requer autenticação)

**Endpoint**: `DELETE /api/v1/plans/{planId}` ou `DELETE /api/v2/plans/{planId}`

1. **Certifique-se de ter configurado o token**
2. Clique em **"Try it out"**
3. Preencha o `planId` (GUID do plano)
4. Clique em **"Execute"**
5. Verifique a resposta **204 No Content**

### 5.5. Diferenças entre v1 e v2

- **v1** (`/api/v1/plans`): Regras originais, 3-6 itens fixos por área
- **v2** (`/api/v2/plans`): Versão refinada que considera:
  - Nível de escolaridade do usuário
  - Equilíbrio entre hard skills e soft skills
  - Mesmos contratos de resposta

---

## 6. Fluxo Completo de Teste

Seguindo este fluxo, você testará todas as operações CRUD de forma integrada:

### Passo 1: Autenticação
```
POST /api/v1/auth/register
→ Copiar token
→ Configurar no "Authorize"
```

### Passo 2: Criar Skill
```
POST /api/v1/skills
Body: { "name": "Python Programming", "description": "Fundamentos de Python" }
→ Anotar o ID retornado (ex: skillId = 1)
```

### Passo 3: Listar Skills
```
GET /api/v1/skills
→ Verificar se o skill criado aparece na lista
```

### Passo 4: Buscar Skill Específico
```
GET /api/v1/skills/1
→ Verificar detalhes do skill
```

### Passo 5: Atualizar Skill
```
PUT /api/v1/skills/1
Body: { "name": "Python Avançado", "description": "Python avançado com frameworks" }
→ Verificar atualização
```

### Passo 6: Criar Course Vinculado
```
POST /api/v1/courses
Body: {
  "skillId": 1,
  "name": "Curso de Python Básico",
  "provider": "Udemy",
  "url": "https://www.udemy.com/course/python-basico"
}
→ Anotar o ID retornado (ex: courseId = 1)
```

### Passo 7: Listar Courses
```
GET /api/v1/courses?skillId=1
→ Verificar se o course criado aparece na lista
```

### Passo 8: Atualizar Course
```
PUT /api/v1/courses/1
Body: {
  "skillId": 1,
  "name": "Curso de Python Avançado",
  "provider": "Coursera",
  "url": "https://www.coursera.org/course/python-avancado"
}
→ Verificar atualização
```

### Passo 9: Gerar Plano
```
POST /api/v1/plans
→ Anotar planId e order dos itens
```

### Passo 10: Ver Plano Atual
```
GET /api/v1/plans
→ Verificar estrutura e progresso
```

### Passo 11: Marcar Item como Completo
```
PUT /api/v1/plans/{planId}/items/1
→ Verificar IsCompleted: true e CompletedAt preenchido
```

### Passo 12: Verificar Progresso
```
GET /api/v1/plans
→ Verificar progressPercent e completedItems atualizados
```

### Passo 13: Desmarcar Item
```
PUT /api/v1/plans/{planId}/items/1
→ Verificar IsCompleted: false e CompletedAt: null
```

### Passo 14: Deletar Course
```
DELETE /api/v1/courses/1
→ Verificar 204 No Content
```

### Passo 15: Deletar Skill
```
DELETE /api/v1/skills/1
→ Verificar 204 No Content
```

### Passo 16: Deletar Plano
```
DELETE /api/v1/plans/{planId}
→ Verificar 204 No Content
```

---

## 7. Troubleshooting

### Erro 401 Unauthorized

**Possíveis causas**:
1. Token não foi configurado no Swagger
   - **Solução**: Siga a seção 2.2
2. Token expirado (válido por 4 horas)
   - **Solução**: Faça login novamente
3. Token inválido ou corrompido
   - **Solução**: Gere um novo token via login/register
4. Token foi copiado com "Bearer" incluído
   - **Solução**: Remova "Bearer" e cole apenas o token

### Erro 400 Bad Request

**Possíveis causas**:
1. Validação falhou (campos obrigatórios ausentes)
   - **Solução**: Verifique o body da requisição
2. Formato de dados inválido (ex: URL inválida)
   - **Solução**: Verifique os formatos esperados
3. Tamanho de campo excedido
   - **Solução**: Verifique os limites máximos

### Erro 404 Not Found

**Possíveis causas**:
1. Recurso não existe (ID inválido)
   - **Solução**: Verifique se o ID existe antes de atualizar/deletar
2. URL incorreta
   - **Solução**: Verifique a rota do endpoint

### Erro 500 Internal Server Error

**Possíveis causas**:
1. Erro no servidor
   - **Solução**: Verifique os logs da aplicação
2. Problema de conexão com banco de dados
   - **Solução**: Verifique a configuração do banco

### Verificar se o Token está sendo enviado

1. Abra o console do navegador (F12)
2. Execute uma requisição
3. Vá para a aba **"Network"**
4. Clique na requisição
5. Verifique os **Headers**
6. Procure pelo header: `Authorization: Bearer {seu-token}`

### Token não persiste entre requisições

- O Swagger mantém o token enquanto a aba estiver aberta
- Se fechar e reabrir, será necessário configurar novamente
- O token expira após 4 horas

---

## 📝 Resumo dos Endpoints

| Recurso | CREATE | READ | UPDATE | DELETE | Autenticação |
|---------|--------|------|--------|--------|--------------|
| **Skills** | ✅ POST | ✅ GET | ✅ PUT | ✅ DELETE | POST/PUT/DELETE requerem |
| **Courses** | ✅ POST | ✅ GET | ✅ PUT | ✅ DELETE | POST/PUT/DELETE requerem |
| **Plans** | ✅ POST (Generate) | ✅ GET | ✅ PUT (Toggle) | ✅ DELETE | Todas requerem |
| **Auth** | ✅ POST (Register/Login) | ❌ | ❌ | ❌ | Nenhuma requer |

---

## 🔗 Links Úteis

- [Guia de Autenticação](./GUIA_AUTENTICACAO.md)
- [README Principal](./README.md)
- Swagger UI: `https://localhost:5001/swagger`

---

