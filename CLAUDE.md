# CLAUDE.md — Solo Consultoria de Imóveis · Contas Correntes Web

Este arquivo orienta o Claude Code sobre o contexto, arquitetura, convenções e
restrições deste projeto. Leia-o integralmente antes de qualquer tarefa.

---

## Visão Geral do Projeto

Migração de um sistema desktop de gerenciamento de contas correntes (VB.Net +
SQL Server) para uma aplicação web. O banco de dados SQL Server é mantido; apenas
o frontend e a camada de acesso a dados são reescritos.

**Stack:**
- **Frontend:** React (Vite), servido como SPA
- **Backend:** Python 3.12 · FastAPI · SQLAlchemy
- **Banco de dados:** SQL Server (schema existente — não alterar sem autorização)
- **Infraestrutura:** Docker Compose · VPS Linux

---

## Estrutura do Monorepo

```
/
├── backend/
│   ├── app/
│   │   ├── main.py              # Entrypoint FastAPI
│   │   ├── database.py          # Engine e SessionLocal (SQLAlchemy)
│   │   ├── models/              # Modelos ORM (um arquivo por tabela)
│   │   ├── routers/             # Um arquivo por domínio (auth, contas, clientes…)
│   │   ├── schemas/             # Schemas Pydantic (request/response)
│   │   ├── services/            # Lógica de negócio desacoplada dos routers
│   │   └── dependencies.py      # get_db, get_current_user etc.
│   ├── tests/
│   ├── Dockerfile
│   └── pyproject.toml
│
├── frontend/
│   ├── src/
│   │   ├── pages/               # Uma pasta por tela (ex: Login/, Principal/)
│   │   ├── components/          # Componentes reutilizáveis
│   │   ├── hooks/               # Custom hooks
│   │   ├── services/            # Chamadas à API (fetch/axios)
│   │   └── main.jsx
│   ├── Dockerfile
│   └── package.json
│
├── legacy/
│   └── forms/                   # Código-fonte original VB.Net (somente leitura)
│       ├── frmLogin.vb
│       ├── frmLogin.Designer.vb
│       ├── frmLogin.resx
│       └── …
├── docker-compose.yml
├── docker-compose.prod.yml
└── CLAUDE.md
```

---

## Contexto de Migração (CRÍTICO)

Este projeto migra um sistema legado VB.Net. Cada tela nova deve ser rastreável
ao form original de origem.

**Regras inegociáveis:**
- Toda regra de negócio identificada nos arquivos `.vb` originais **deve ser
  preservada** na migração, salvo decisão explícita documentada.
- Cada router FastAPI deve referenciar nos docstrings quais RNs (regras de
  negócio) do form original implementa (ex: `# RN03 — Valida credenciais`).
- O schema do banco SQL Server **não deve ser alterado** sem aprovação do
  responsável pelo sistema. Migrations DDL estão fora do escopo desta fase.
- Senhas atualmente em texto plano na tabela `Usuários` **devem ser migradas
  para bcrypt** antes do go-live. Nunca implementar comparação em texto plano
  em código novo.

---

## Backend (FastAPI)

### Comandos

```bash
cd backend
pip install -e ".[dev]"      # instala dependências incluindo as de dev
ruff check .                 # lint
ruff format .                # formatação
pytest                       # roda todos os testes
uvicorn app.main:app --reload  # dev local
```

### Convenções

- Um arquivo por domínio em `routers/` (ex: `auth.py`, `contas.py`).
- A lógica de negócio fica em `services/`, **não** nos routers.
- Routers apenas validam entrada, chamam services e retornam respostas.
- Todos os endpoints devem ter `summary` e `description` no decorator.
- Dependências compartilhadas (sessão DB, usuário autenticado) ficam em
  `dependencies.py` e são injetadas via `Depends()`.
- Nunca usar `SELECT *` — sempre listar as colunas explicitamente.
- Queries ao SQL Server via SQLAlchemy Core (`text()`) ou ORM — nunca
  concatenação de strings SQL.

### Autenticação

- JWT via `python-jose`. Token inclui claims `sub` (usuário) e `perfil`.
- O perfil controla permissões nos demais módulos — consultar tabela `Perfis`
  para os valores válidos antes de implementar guards.
- Endpoints públicos: `GET /auth/usuarios`, `POST /auth/login`.
- Todos os demais endpoints exigem `Depends(get_current_user)`.

### Variáveis de Ambiente

Nunca hardcodar credenciais. Usar `.env` local (não commitado) e `os.getenv()`:

```
DATABASE_URL=mssql+pyodbc://user:pass@host/db?driver=ODBC+Driver+17+for+SQL+Server
JWT_SECRET_KEY=...
ACCESS_TOKEN_EXPIRE_MINUTES=480
```

---

## Frontend (React)

### Comandos

```bash
cd frontend
npm install
npm run dev        # dev local
npm run lint       # ESLint
npm run build      # build de produção
npm run test       # Vitest
```

### Convenções

- Uma pasta por página em `src/pages/` contendo `index.jsx` e estilos locais.
- Chamadas à API centralizadas em `src/services/` — componentes nunca fazem
  `fetch` diretamente.
- Token JWT armazenado em `localStorage` com as chaves `access_token`, `perfil`
  e `usuario`.
- Nomes de componentes em PascalCase; hooks em camelCase com prefixo `use`.
- Sem estilos inline em componentes finais — usar CSS Modules ou Tailwind
  (definir design system antes de iniciar as telas).
- Todo campo de formulário deve replicar o comportamento de foco e validação
  do form VB.Net de origem (documentado nos arquivos `*_documentacao.md`).

### Variáveis de Ambiente

```
VITE_API_URL=http://localhost:8000
```

---

## Testes

### Backend

- Framework: `pytest` + `httpx` (cliente async para FastAPI).
- Todo router novo deve ter ao menos: teste do caminho feliz, teste de
  validação obrigatória e teste de autenticação negada.
- Fixtures de banco usam um banco de teste isolado — nunca o banco de produção.

### Frontend

- Framework: Vitest + React Testing Library.
- Todo componente de formulário deve ter teste de: renderização, submissão
  válida e exibição de erros.

**PRs sem testes para código novo serão recusados.**

---

## Git

### Branches

```
main          # produção — push direto bloqueado
develop       # integração
feature/nome  # novas funcionalidades
fix/nome      # correções
```

### Commits (Conventional Commits)

```
feat(auth): adiciona endpoint POST /auth/login
fix(login): corrige foco após senha incorreta
refactor(contas): extrai lógica de saldo para service
test(auth): adiciona testes de autenticação negada
docs(frmLogin): adiciona documentação funcional de migração
chore: atualiza dependências do backend
```

O escopo entre parênteses deve corresponder ao domínio ou form de origem.

---

## Docker

```bash
# Desenvolvimento
docker compose up --build

# Produção
docker compose -f docker-compose.prod.yml up -d
```

- O container do backend expõe a porta `8000`.
- O container do frontend serve o build via Nginx na porta `80`.
- O SQL Server roda fora dos containers (instância existente na VPS) — a
  connection string é injetada via variável de ambiente.

---

## Código Legado (Referência)

A pasta `legacy/forms/` contém os arquivos originais do sistema desktop VB.Net.
Ela existe exclusivamente como fonte da verdade para auditar regras de negócio
durante a migração.

**Regras:**
- ❌ Nunca modificar ou deletar arquivos dentro de `legacy/`.
- ❌ Nunca executar esses arquivos — são apenas referência de leitura.
- ✅ Ao implementar uma tela, consultar os três arquivos do form correspondente
  (`.vb`, `.Designer.vb`, `.resx`) para garantir fidelidade às regras originais.
- ✅ Novos forms a migrar devem ser adicionados a `legacy/forms/` antes de
  iniciar a análise.

---

## O Que Não Fazer

- ❌ Não alterar o schema do banco SQL Server sem aprovação.
- ❌ Não armazenar senhas em texto plano em código novo.
- ❌ Não fazer chamadas `fetch` dentro de componentes React — usar `services/`.
- ❌ Não commitar arquivos `.env` ou credenciais.
- ❌ Não criar endpoints sem autenticação fora de `auth.py`.
- ❌ Não remover ou alterar regras de negócio RNxx sem registrar a decisão
  em comentário no código e na documentação do form correspondente.
- ❌ Não usar `SELECT *` em queries SQL.

---

## Rastreabilidade Form → Web

Ao criar ou modificar uma tela, referenciar sempre o form de origem:

| Tela Web      | Form Original   | Documentação                        |
|---------------|-----------------|-------------------------------------|
| `/login`      | `frmLogin`      | `docs/frmLogin_documentacao.md`     |
| `/principal`  | `FrmPrincipal`  | `docs/FrmPrincipal_documentacao.md` |
|               |                 |                                     |

---

## Dúvidas Frequentes

**Posso criar uma nova tabela no banco?**
Não nesta fase. O schema é legado e compartilhado. Solicitar aprovação antes.

**Como sei qual perfil tem acesso a qual tela?**
Consultar a tabela `Perfis` no banco e o form `frmLogin.vb` — o campo `Perfil`
retornado no login controla o acesso.

**Onde fica a lógica de negócio?**
No backend, em `services/`. No frontend, apenas lógica de apresentação.
