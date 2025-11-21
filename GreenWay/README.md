## GreenWay API

### App de Mobilidade Sustentável Corporativa

API RESTful para gerenciar mobilidade sustentável corporativa, permitindo:
- Organização de caronas corporativas entre colaboradores
- Sugestão de rotas sustentáveis (transporte público ou bicicleta)
- Cálculo de impacto ambiental evitado (CO₂ poupado, km ecológicos)
- Cadastro corporativo com email da empresa
- Perfil de transporte (endereço, meio de transporte, horários)

### Requisitos implementados

#### 1. Boas Práticas REST (30 pts) ✅
- ✅ **Paginação**: Implementada em todos os endpoints GET com parâmetros `pageNumber` e `pageSize`
- ✅ **HATEOAS**: Links de navegação (`self`, `update`, `delete`) em todas as respostas
- ✅ **Status Codes**: Uso correto de 200, 201, 204, 400, 404
- ✅ **Verbos HTTP**: GET, POST, PUT, DELETE implementados corretamente

#### 2. Monitoramento e Observabilidade (15 pts) ✅
- ✅ **Health Check**: Endpoint `/health` com verificação do banco de dados
- ✅ **Logging**: ILogger implementado em todos os controllers com logs estruturados
- ✅ **Tracing**: ActivitySource configurado para rastreamento distribuído

#### 3. Versionamento da API (10 pts) ✅
- ✅ **Versionamento**: Rotas estruturadas como `/api/v{version}/...`
- ✅ **Controle de versões**: Configurado com `Microsoft.AspNetCore.Mvc.Versioning`
- ✅ **Documentação**: Explicado no README

#### 4. Integração e Persistência (30 pts) ✅
- ✅ **Banco de dados**: Oracle configurado via Entity Framework Core
- ✅ **Entity Framework Core**: Configurado com Oracle.EntityFrameworkCore
- ✅ **Migrations**: Migrations criadas e disponíveis na pasta `Migrations/`

#### 5. Testes Integrados (15 pts) ✅
- ✅ **xUnit**: Projeto de testes separado (`GreenWay.Tests`)
- ✅ **Testes de integração**: Implementados com `WebApplicationFactory`
- ✅ **Testes unitários**: Testes para controllers e endpoints principais

#### Outros recursos
- Segurança via API Key (cabeçalho `X-Api-Key`)
- Swagger/OpenAPI com documentação completa

### Modelos

#### Colaborador
- Cadastro com email corporativo
- Endereço aproximado
- Meio de transporte preferido
- Horários de trabalho (entrada/saída)
- Disponibilidade para caronas

#### Carona
- Organização de caronas entre colaboradores
- Motorista e passageiro
- Data, horário, origem e destino
- Status da carona
- Distância percorrida

#### RotaSustentavel
- Sugestão de rotas com transporte público ou bicicleta
- Origem e destino
- Tipo de rota
- Distância e tempo estimado
- CO₂ poupado

#### ImpactoAmbiental
- Registro de impacto ambiental
- CO₂ poupado em kg
- Quilômetros ecológicos
- Tipo de transporte utilizado
- Relacionamento com colaborador e/ou carona

### Como executar
1. Configure a `ApiKey` em `appsettings.json` (valor padrão: `dev-secret-key-change-me`).
2. Opcional: ajuste a string de conexão Oracle em `appsettings.json`.
3. Execute a API:
```bash
dotnet run --project GreenWay.csproj
```
4. Acesse Swagger em `/swagger`.

### Autenticação por API Key
- Envie o cabeçalho `X-Api-Key: <sua-chave>` em todas as requisições (exceto `/health` e `/swagger`).

Exemplo (PowerShell):
```bash
curl -H "X-Api-Key: dev-secret-key-change-me" https://localhost:5001/api/v1/colaborador
```

### Endpoints Principais

#### Colaborador
- `GET /api/v1/colaborador` - Lista todos os colaboradores
- `GET /api/v1/colaborador/{id}` - Busca colaborador por ID
- `GET /api/v1/colaborador/email/{email}` - Busca colaborador por email
- `GET /api/v1/colaborador/disponiveis-caronas` - Lista colaboradores disponíveis para caronas
- `POST /api/v1/colaborador` - Cria novo colaborador
- `PUT /api/v1/colaborador/{id}` - Atualiza colaborador
- `DELETE /api/v1/colaborador/{id}` - Remove colaborador

#### Carona
- `GET /api/v1/carona` - Lista todas as caronas
- `GET /api/v1/carona/{id}` - Busca carona por ID
- `GET /api/v1/carona/colaborador/{colaboradorId}` - Busca caronas por colaborador
- `POST /api/v1/carona` - Cria nova carona
- `PUT /api/v1/carona/{id}` - Atualiza carona
- `DELETE /api/v1/carona/{id}` - Remove carona

#### ImpactoAmbiental
- `GET /api/v1/impactoambiental` - Lista todos os impactos
- `GET /api/v1/impactoambiental/{id}` - Busca impacto por ID
- `GET /api/v1/impactoambiental/colaborador/{colaboradorId}/total-co2` - Total de CO₂ poupado por colaborador
- `GET /api/v1/impactoambiental/total-geral` - Total geral de impacto ambiental
- `POST /api/v1/impactoambiental` - Cria novo registro de impacto
- `PUT /api/v1/impactoambiental/{id}` - Atualiza impacto
- `DELETE /api/v1/impactoambiental/{id}` - Remove impacto

#### RotaSustentavel
- `GET /api/v1/rotasustentavel` - Lista todas as rotas
- `GET /api/v1/rotasustentavel/{id}` - Busca rota por ID
- `GET /api/v1/rotasustentavel/colaborador/{colaboradorId}` - Busca rotas por colaborador
- `POST /api/v1/rotasustentavel` - Cria nova rota
- `PUT /api/v1/rotasustentavel/{id}` - Atualiza rota
- `DELETE /api/v1/rotasustentavel/{id}` - Remove rota

### Aplicar Migrations
Para criar/atualizar o banco de dados Oracle:
```bash
dotnet ef database update --project GreenWay.csproj
```

### Executar testes
1. Restaure dependências e execute os testes:
```bash
dotnet test
```

Os testes incluem:
- Testes de integração para endpoints da API
- Testes de controllers com banco em memória
- Testes de Health Check

### Estrutura do Projeto
```
GreenWay/
├── Controllers/
│   ├── ColaboradorController.cs
│   ├── CaronaController.cs
│   ├── ImpactoAmbientalController.cs
│   └── RotaSustentavelController.cs
├── Models/
│   ├── Colaborador.cs
│   ├── Carona.cs
│   ├── ImpactoAmbiental.cs
│   └── RotaSustentavel.cs
├── Data/
│   └── AppDbContext.cs
├── Migrations/
│   ├── 20241201000000_InitialCreate.cs
│   ├── 20241201000000_InitialCreate.Designer.cs
│   └── AppDbContextModelSnapshot.cs
├── Middleware/
│   └── ApiKeyMiddleware.cs
├── Swagger/
│   └── ConfigureSwaggerOptions.cs
├── Program.cs
├── appsettings.json
└── GreenWay.Tests/
    ├── Controllers/
    │   ├── ColaboradorControllerTests.cs
    │   └── CaronaControllerTests.cs
    └── Integration/
        └── HealthCheckTests.cs
```

### Logging e Tracing
- **Logging**: Todos os controllers utilizam `ILogger<T>` para registrar operações
- **Tracing**: `ActivitySource` configurado para rastreamento distribuído
- Logs incluem informações sobre requisições, erros e operações do banco de dados
