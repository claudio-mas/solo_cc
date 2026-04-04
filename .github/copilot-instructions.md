# Diretrizes do Projeto

## Contexto
- Este monorepo migra um sistema legado VB.Net para web (FastAPI + React + SQL Server).
- Antes de implementar qualquer tela ou regra de negócio, consulte [CLAUDE.md](../CLAUDE.md) e a documentação do form correspondente em [docs/](../docs/).
- Arquivos em [legacy/forms/](../legacy/forms/) são somente leitura e servem como fonte de verdade funcional.

## Arquitetura
- Backend: `routers` validam e orquestram, `services` concentram regra de negócio, `schemas` definem contratos.
- Frontend: `pages` compõem interface, `hooks` orquestram fluxo, `services` fazem chamadas HTTP.
- Componentes React não devem chamar `fetch` diretamente; use sempre a camada `frontend/src/services/`.

## Comandos de Trabalho
- Backend:
  - `cd backend && pip install -e ".[dev]"`
  - `cd backend && uvicorn app.main:app --reload`
  - `cd backend && ruff check . && ruff format .`
  - `cd backend && pytest`
- Frontend:
  - `cd frontend && npm install`
  - `cd frontend && npm run dev`
  - `cd frontend && npm run lint`
  - `cd frontend && npm run test`
  - `cd frontend && npm run build`

## Convenções Críticas
- Não alterar schema do SQL Server sem aprovação explícita.
- Não usar `SELECT *`; sempre listar colunas.
- Não implementar senha em texto plano em código novo; usar bcrypt.
- Exceto endpoints públicos de auth, os demais endpoints devem exigir autenticação.
- Ao migrar comportamento de form legado, preserve RNxx e documente qualquer decisão de ADAPTAR/DESCARTAR.

## Testes
- Mudanças de backend devem incluir testes de caminho feliz, validação e autenticação negada.
- Mudanças de frontend em formulários devem cobrir renderização, submissão válida e erros.

## Referências (Link, não duplicar)
- Regras gerais e restrições: [CLAUDE.md](../CLAUDE.md)
- Regras específicas de frontend: [frontend/AGENTS.MD](../frontend/AGENTS.MD)
- Escopo e contexto da migração: [docs/escopo_migracao.md](../docs/escopo_migracao.md)
- Documentação funcional por form: [docs/](../docs/)