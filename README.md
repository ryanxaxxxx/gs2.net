## GreenWay API

### App de Mobilidade Sustentável Corporativa

API RESTful para gerenciar mobilidade sustentável corporativa, permitindo:
- Organização de caronas corporativas entre colaboradores
- Sugestão de rotas sustentáveis (transporte público ou bicicleta)
- Cálculo de impacto ambiental evitado (CO₂ poupado, km ecológicos)
- Cadastro corporativo com email da empresa
- Perfil de transporte (endereço, meio de transporte, horários)


#### Outros recursos
- Segurança via API Key (cabeçalho `dev-secret-key-change-me`)
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

#### Carona

#### ImpactoAmbiental

#### RotaSustentavel

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

Links dos Deploys da API
Informações de Acesso
Banco de Dados Oracle:
Host: oracle.fiap.com.br:1521/orcl
User ID: rm555924
Password: 030905
API Key (Autenticação):
Header: X-Api-Key
Valor: dev-secret-key-change-me
URLs Locais (Desenvolvimento):
HTTPS: https://localhost:7228
HTTP: http://localhost:5185
Swagger: https://localhost:7228/swagger ou http://localhost:5185/swagger
Como Testar
Exemplo de requisição com cURL (PowerShell):



### Logging e Tracing
- **Logging**: Todos os controllers utilizam `ILogger<T>` para registrar operações
- **Tracing**: `ActivitySource` configurado para rastreamento distribuído
- Logs incluem informações sobre requisições, erros e operações do banco de dados
