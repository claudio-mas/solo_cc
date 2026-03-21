# escopo_migracao.md — Solo Contas Correntes

Documento de visão geral do sistema legado para orientar a migração form a form.
Gerado a partir da leitura dos 14 arquivos `.vb` originais.
Deve ser lido pelo Claude Code no início de cada prompt de migração.

---

## Visão Geral do Sistema

Sistema de gestão de contas correntes para a Solo Consultoria de Imóveis.
Cada cliente possui uma conta corrente com lançamentos de débito e crédito,
organizados em "pastas". O sistema permite consulta de extrato, relatórios de
devedores/credores e totais consolidados.

---

## Tabelas do Banco de Dados (identificadas nos forms)

| Tabela     | Colunas identificadas                                                                 |
|------------|---------------------------------------------------------------------------------------|
| Clientes   | Id, Código, Cliente                                                                   |
| Contas     | Id, IdCliente, CodCliente, Conta, Dt, Ref, VValor, DC, ND, Saldo, Deb, Cred, IdLote, Selec |
| Usuários   | Id, Usuário, Psw, Perfil                                                              |
| Perfis     | (estrutura a confirmar no banco)                                                      |
| Chaves     | Id, Ref, Chave  ← senhas de operações críticas por contexto                          |

---

## Variáveis Globais do Sistema Legado

O sistema usa variáveis globais compartilhadas entre forms — padrão que **não
deve ser reproduzido na web**. Cada uma precisa ser substituída por mecanismo
adequado (JWT claims, estado de sessão, parâmetros de rota/query).

| Variável    | Uso no legado                                              | Substituto na web              |
|-------------|-------------------------------------------------------------|-------------------------------|
| `varUsu`    | Usuário logado                                              | JWT claim `sub`               |
| `varPerfil` | Perfil do usuário logado                                    | JWT claim `perfil`            |
| `varSenha`  | Contexto da operação que exige senha (1, 2, 3 ou 4)        | Endpoint específico por operação |

---

## Padrão de Navegação do Legado

No WinForms, o `FrmPrincipal` oculta a si mesmo (`Visible = False`) ao abrir
um form filho e restaura sua visibilidade quando o filho fecha. Esse padrão deve
ser DESCARTADO na web — o equivalente é roteamento SPA com React Router.
A tela principal permanece na URL `/principal` e as demais são sub-rotas ou
modais conforme o caso.

---

## Padrão frmSenha (CRÍTICO para redesign)

`frmSenha` é um modal de autenticação reutilizado por múltiplos contextos,
identificado pela variável global `varSenha`:

| varSenha | Contexto                          | Senha validada contra   |
|----------|-----------------------------------|-------------------------|
| "1"      | Acesso à gestão de usuários       | Tabela Chaves (Ref = 'Alteração de senhas') |
| "2"      | Exclusão de cliente               | Senha fixa "4321" ⚠️    |
| "3"      | Transferência de lançamentos      | Tabela Chaves (Ref = 'Transferência de lançamentos') |
| "4"      | Desbloqueio de lançamentos        | Tabela Chaves (Ref = 'Desbloquear lançamentos') |

⚠️ A senha "4321" hardcoded para exclusão de cliente deve ser migrada para a
tabela Chaves ou outro mecanismo antes do go-live. Nunca hardcodar senhas em
código novo.

Na web, cada operação crítica deve ter seu próprio endpoint protegido com
validação de senha via `POST` específico, sem variável global de contexto.

---

## Componente de Relatório Legado (DECISÃO NECESSÁRIA)

Três forms usam `C1Report` (ComponentOne) + `C1PrintPreviewControl`, um
componente proprietário WinForms. Não existe equivalente direto na web.

| Form            | Relatório gerado                        |
|-----------------|-----------------------------------------|
| FrmExtratoRpt   | Extrato do cliente (com filtros)        |
| frmRptDevedores1 | Clientes devedores por data/saldo      |
| frmCredores1    | Clientes credores por data/saldo        |

**Decisão necessária antes de migrar esses forms:** definir estratégia de
geração de relatórios/PDF na web (ex: geração server-side com Python/ReportLab,
React-to-PDF, ou tabela React com print CSS).

---

## Mapa de Forms e Dependências

```
frmLogin
  └── FrmPrincipal (hub central)
        ├── frmClienteNovo
        ├── frmAlterar
        │     └── frmSenha (varSenha=2, exclusão)
        ├── FrmExtrato
        │     ├── frmSenha (varSenha=3, transferência)
        │     ├── frmSenha (varSenha=4, desbloqueio)
        │     └── FrmExtratoRpt
        ├── FrmLancaData ──► FrmLanca
        ├── frmTotais2
        ├── frmReports
        │     ├── frmRptDevedores1
        │     └── frmCredores1
        └── frmSenha (varSenha=1, usuários)
              └── FrmUsuarios
```

---

## Catálogo de Forms — Complexidade e Notas de Migração

### frmLogin ✅ MIGRADO
Autenticação com usuário e senha. Abre FrmPrincipal após login.
Ver `docs/frmLogin_documentacao.md`.

---

### FrmPrincipal — PRÓXIMO
**Complexidade:** Média

**Função:** Hub de navegação. Lista todos os clientes em grid com busca
incremental por nome ou código. Exibe código do primeiro cliente no topo.
Executa UPDATE de integridade de dados no load (CTE que corrige CodCliente).

**Controles principais:**
- `RadGridView1` — grid de clientes (Id, Código, Cliente)
- `txtPesq` — campo de busca incremental (filtra por nome ou código)
- `RibbonGroup6` — exibe "CÓD. X" do cliente selecionado
- `lblUsuario` — exibe nome do usuário logado (preenchido pelo frmLogin)

**Ações disponíveis (botões):**
- Lançar → abre FrmLancaData (se sem data) ou FrmLanca diretamente
- Extrato → abre FrmExtrato
- Novo Cliente → abre frmClienteNovo
- Alterar → abre frmAlterar
- Totais → abre frmTotais2
- Relatórios → abre frmReports
- Usuários → abre frmSenha (varSenha=1) → FrmUsuarios
- Backup → executa BACKUP DATABASE direto no SQL Server ⚠️
- Retornar → encerra a aplicação (`Application.Exit`)

**Notas de migração:**
- Busca incremental: ADAPTAR como filtro em tempo real no frontend (sem round-trip ao banco a cada tecla).
- Double-click na grid → abre FrmExtrato: ADAPTAR (manter comportamento de duplo clique ou clique na linha).
- Backup: ADAPTAR — no contexto web, acionar backup via endpoint autenticado de administrador; avaliar se deve permanecer no escopo desta fase.
- `Application.Exit` (botão Retornar): DESCARTAR — na web equivale a logout + redirect para login.
- UPDATE de integridade no load: ver B17 do frmLogin — aguarda decisão do cliente.
- `Timer1` para manter foco no txtPesq: DESCARTAR — comportamento de foco é gerenciado pelo browser.

---

### frmClienteNovo
**Complexidade:** Baixa

**Função:** Cadastro de novo cliente. Gera Código automaticamente (busca o
primeiro inteiro >= 10.000 não utilizado, limitado a 20.000). Valida unicidade
do Código antes de salvar.

**Regras de negócio:**
- Código deve ser único na tabela Clientes.
- Código máximo: 20.000.
- Geração automática: percorre sequência a partir de 10.000 e retorna o
  primeiro gap disponível.
- Botão "Gerar Código" preenche o campo automaticamente; usuário pode alterar
  manualmente (sujeito às validações).
- Enter = Tab (ADAPTAR — mesmo padrão do frmLogin B15).

**Notas de migração:**
- Geração de código: mover para o backend (endpoint `GET /clientes/proximo-codigo`).
- `BindingSource.AddNew()`: DESCARTAR — sem equivalente web; formulário inicia em branco.

---

### frmAlterar
**Complexidade:** Média

**Função:** Edição e exclusão do cliente selecionado no FrmPrincipal.

**Regras de negócio:**
- Campos Código e Cliente iniciam em modo somente leitura.
- Botão "Alterar" habilita edição.
- Alteração de Código exige **três confirmações consecutivas** (triplo MsgBox).
- Alteração apenas de Nome exige uma confirmação.
- Botão "Desfazer" recarrega os dados originais do banco.
- Exclusão: abre frmSenha (varSenha=2); se cliente tem lançamentos em Contas,
  exige confirmação adicional antes de deletar cliente e lançamentos.
- Ao fechar, recarrega a grid de clientes no FrmPrincipal se houve alteração (Tag="1").

**Notas de migração:**
- Tripla confirmação de alteração de Código: PRESERVAR a intenção (operação
  de alto risco); ADAPTAR a UX (um modal com aviso claro substitui os três
  MsgBox consecutivos).
- Senha de exclusão hardcoded "4321": ADAPTAR — usar tabela Chaves ou endpoint
  protegido (ver seção frmSenha).
- `FrmPrincipal.Visible = False/True`: DESCARTAR — usar navegação/roteamento.

---

### FrmLancaData
**Complexidade:** Muito Baixa

**Função:** Modal de seleção de data antes de abrir FrmLanca, quando nenhuma
data está armazenada no `RadGridView1.Tag` do FrmPrincipal.

**Notas de migração:** ADAPTAR como date picker inline ou modal simples no
frontend. A data selecionada é passada como parâmetro para FrmLanca — na web,
usar parâmetro de rota ou estado do componente pai.

---

### FrmLanca
**Complexidade:** Média

**Função:** Lançamento de novo movimento (débito ou crédito) na conta corrente
do cliente selecionado.

**Campos:** Pasta (conta), Data, Histórico (Ref), Valor, D/C.

**Regras de negócio:**
- Todos os campos são obrigatórios antes de salvar.
- Campo D/C aceita apenas "D" ou "C" (validação no KeyPress).
- Se Pasta não existe para o cliente, pergunta se é nova pasta antes de continuar.
- Ao salvar: se DC="D", preenche Deb com o valor; se DC="C", preenche Cred.
- Enter = Tab, exceto no campo Histórico onde Enter faz backspace + Tab
  (comportamento específico — avaliar com cliente se deve ser preservado).
- Botões Salvar/Desfazer ficam ocultos até o usuário começar a preencher.

**Notas de migração:**
- Verificação de pasta nova: PRESERVAR (query ao backend antes de confirmar).
- Comportamento Enter no campo Histórico: ADAPTAR ou simplificar — avaliar com cliente.
- `mouse_event` (simulação de clique do mouse): DESCARTAR — artefato WinForms.

---

### FrmExtrato
**Complexidade:** Alta ⚠️

**Função:** Extrato completo do cliente. A tela mais complexa do sistema.

**Funcionalidades:**
- Grid de lançamentos com colunas: Data, Id, IdCliente, CodCliente, Pasta,
  Histórico, ND, VValor, DC, Saldo, Deb, Crédito.
- Filtros combinados: Pasta, ND, Histórico (LIKE), "Sem ND" (checkbox).
- Cálculo de saldo acumulado exibido no rodapé (vermelho=credor, azul=devedor).
- Edição inline de registros (Data, Pasta, Histórico, ND, Deb, Cred) com
  desbloqueio via senha (F10 → frmSenha varSenha=4).
- Transferência de lançamentos selecionados para outro cliente (F9 →
  combo + senha → frmSenha varSenha=3).
- Impressão: abre FrmExtratoRpt com filtros ativos.
- Botões de navegação (primeiro/último registro).
- Ao fechar: executa UPDATE para sincronizar VValor com Deb/Cred.

**Notas de migração:**
- Grid com edição inline: ADAPTAR — React table com células editáveis in-place,
  bloqueadas por padrão e desbloqueadas por senha.
- Filtros combinados: ADAPTAR — filtros no frontend com fetch ao backend.
- Saldo acumulado: ADAPTAR — calcular no backend via window function SQL
  (já existe a query CTE com `sum() over (order by dt, id)` no legado).
- F9/F10 como atalhos de teclado: ADAPTAR — manter atalhos ou expor via botões.
- Transferência de lançamentos: PRESERVAR a lógica; ADAPTAR a UX (seleção
  múltipla + confirmação de senha via modal).
- UPDATE VValor no fechamento: ADAPTAR — executar via endpoint ao sair da tela.
- `SendKeys`, `Application.DoEvents`, `mouse_event`: DESCARTAR.

**Recomendação:** migrar FrmExtrato por último entre as telas principais,
após os padrões de edição inline e autenticação de operações estarem estabelecidos.

---

### FrmExtratoRpt
**Complexidade:** Baixa (dependente da decisão de relatórios)

**Função:** Visualização e impressão do extrato do cliente em formato de relatório.
Recebe filtros ativos do FrmExtrato. Usa C1Report.

**Notas de migração:** BLOQUEADA pela decisão de estratégia de relatórios.
Migrar junto com frmRptDevedores1 e frmCredores1 após a decisão.

---

### frmTotais2
**Complexidade:** Baixa

**Função:** Dashboard com totais do dia (ou data selecionada):
- Quantidade e valor total de clientes credores (TC > TD)
- Quantidade e valor total de clientes devedores (TD > TC)
- DatePicker para consultar em data diferente de hoje

**Notas de migração:** ADAPTAR como página/modal de dashboard. As 4 queries
SQL são bem definidas — migrar para endpoint `GET /totais?data=YYYY-MM-DD`.

---

### frmReports
**Complexidade:** Baixa (é apenas um formulário de parâmetros)

**Função:** Tela de configuração de parâmetros para relatórios de devedores e
credores: data de corte, saldo mínimo, filtro por faixa de código (todos /
>= 10000 / < 10000), ordenação (por código ou por nome).

**Notas de migração:** ADAPTAR como formulário de parâmetros que navega para
as telas de relatório. Depende da decisão de estratégia de relatórios.

---

### frmRptDevedores1 e frmCredores1
**Complexidade:** Baixa (dependente da decisão de relatórios)

**Função:** Relatórios de clientes devedores e credores, respectivamente.
Recebem parâmetros do frmReports. Geram relatório via C1Report com agrupamento
e ordenação configuráveis.

**Notas de migração:** BLOQUEADAS pela decisão de estratégia de relatórios.

---

### FrmUsuarios
**Complexidade:** Média

**Função:** Gestão de usuários, senhas e chaves do sistema. Duas abas:
- Aba de Chaves (senhas de operações críticas) — visível apenas para Administrador.
- Aba de Usuários — Administrador vê todos; perfil comum vê apenas o próprio registro
  e não pode adicionar novos usuários nem alterar o campo Perfil.

**Tabelas:** Usuários, Perfis, Chaves.

**Notas de migração:**
- Controle de visibilidade por perfil: PRESERVAR — implementar via guards no
  frontend e validação no backend.
- Edição inline na grid de usuários/chaves: ADAPTAR como formulário ou tabela
  editável.
- Senhas na tabela Usuários: ADAPTAR — migrar para bcrypt antes do go-live
  (conforme CLAUDE.md).

---

### frmSenha
**Complexidade:** Média (cross-cutting concern)

**Função:** Modal de autenticação reutilizado. Ver seção "Padrão frmSenha" acima.

**Notas de migração:** Não terá form equivalente na web. Cada operação crítica
deve ter seu próprio endpoint `POST` com validação de senha no backend.
O frontend exibe um modal de senha genérico que chama o endpoint correto
conforme o contexto.

---

## Ordem de Migração Recomendada

| Ordem | Form                        | Justificativa                                                  |
|-------|-----------------------------|----------------------------------------------------------------|
| 1     | frmLogin                    | ✅ Concluído                                                   |
| 2     | FrmPrincipal                | Hub de navegação — necessário para testar os demais            |
| 3     | frmClienteNovo              | CRUD simples, sem dependências complexas                       |
| 4     | frmAlterar                  | Depende de FrmPrincipal (cliente selecionado)                  |
| 5     | FrmLancaData + FrmLanca     | Migrar juntos — fluxo único de lançamento                      |
| 6     | frmTotais2                  | Independente, apenas queries de leitura                        |
| 7     | FrmUsuarios                 | Depende do padrão de perfis estabelecido no login              |
| 8     | FrmExtrato                  | Mais complexo — migrar após padrões estabelecidos              |
| 9     | ⚠️ DECISÃO: estratégia de relatórios | Definir antes de iniciar os forms abaixo          |
| 10    | frmReports                  | Formulário de parâmetros                                       |
| 11    | frmRptDevedores1            | Depende da estratégia de relatórios                            |
| 12    | frmCredores1                | Depende da estratégia de relatórios                            |
| 13    | FrmExtratoRpt               | Depende da estratégia de relatórios                            |

> `frmSenha` não gera um form web equivalente — é dissolvido em endpoints
> específicos por operação, implementados junto com cada form que os utiliza.

---

## Decisões Pendentes (antes de iniciar a migração)

| # | Decisão                              | Impacto                                    |
|---|--------------------------------------|--------------------------------------------|
| D1 | Estratégia de geração de relatórios/PDF | Bloqueia forms 10–13                    |
| D2 | Backup de banco na interface web     | Afeta FrmPrincipal (botão Backup)          |
| D3 | Senha de exclusão "4321" hardcoded   | Afeta frmAlterar / frmSenha case "2"       |
| D4 | Comportamento Enter no campo Histórico do FrmLanca | Afeta FrmLanca UX          |

---

## Rastreabilidade Form → Web (atualizar a cada migração)

| Tela Web       | Form Original     | Documentação                           |
|----------------|-------------------|----------------------------------------|
| `/login`       | frmLogin          | docs/frmLogin_documentacao.md          |
| `/principal`   | FrmPrincipal      | docs/FrmPrincipal_documentacao.md      |
| `/clientes/novo` | frmClienteNovo  | docs/frmClienteNovo_documentacao.md    |
| `/clientes/alterar` | frmAlterar   | docs/frmAlterar_documentacao.md        |
| `/lancamentos` | FrmLancaData + FrmLanca | docs/FrmLanca_documentacao.md   |
| `/extrato`     | FrmExtrato        | docs/FrmExtrato_documentacao.md        |
| `/totais`      | frmTotais2        | docs/frmTotais2_documentacao.md        |
| `/usuarios`    | FrmUsuarios       | docs/FrmUsuarios_documentacao.md       |
| `/relatorios`  | frmReports        | docs/frmReports_documentacao.md        |
| `/relatorios/devedores` | frmRptDevedores1 | docs/frmRptDevedores1_documentacao.md |
| `/relatorios/credores`  | frmCredores1     | docs/frmCredores1_documentacao.md     |
| `/extrato/imprimir` | FrmExtratoRpt | docs/FrmExtratoRpt_documentacao.md    |
