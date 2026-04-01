# Documentação Funcional — FrmExtrato
**Sistema:** Solo Consultoria de Imóveis — Contas Correntes
**Form original:** `frmExtrato.vb` (VB.Net / WinForms)
**URL web:** `/extrato`
**Propósito:** Extrato completo do cliente com filtros, edição inline, transferência de lançamentos e saldo acumulado

---

## 1. Visão Geral

O `FrmExtrato` é a tela mais complexa do sistema. É aberto pelo `FrmPrincipal` ao
clicar no botão "Extrato" ou dar duplo clique em um cliente na grid. Exibe todos os
lançamentos do cliente selecionado com cálculo de saldo acumulado.

Funcionalidades principais:
- **Grid de lançamentos** com colunas: Data, Pasta, N.D., Histórico, Débito, Crédito, Saldo
- **Filtros combinados:** Pasta, ND, Histórico (LIKE), checkbox "Sem ND"
- **Saldo acumulado** via window function SQL, exibido no rodapé com cores condicionais
- **Edição inline** de registros (desbloqueável via senha — F10)
- **Transferência de lançamentos** para outro cliente (F9 + senha)
- **Impressão** do extrato com filtros ativos (pendente decisão D1)
- **Sincronização VValor** ao fechar a tela

Na migração web, a tela vira a página `/extrato` acessada via React Router a partir
do `/principal`. O cliente é identificado pelo parâmetro de rota ou estado de navegação.

---

## 2. Componentes Visuais

### 2.1 Header

| Controle   | Tipo  | Função                                              |
|------------|-------|-----------------------------------------------------|
| lbCliente  | Label | Exibe "NomeCliente - CódigoCliente" centralizado    |

### 2.2 Grid de Lançamentos (MasterTemplate / RadGridView1)

| Coluna (BD) | Header     | Tipo     | Visível | ReadOnly inicial | Editável após F10 |
|-------------|------------|----------|---------|------------------|-------------------|
| Selec       | Selec      | Integer  | Não     | —                | Não               |
| Dt          | Data       | DateTime | Sim     | Sim              | Sim               |
| Id          | Id         | Integer  | Não     | Sim              | Não               |
| IdCliente   | IdCliente  | Integer  | Não     | —                | Não               |
| CodCliente  | CodCliente | Integer  | Não     | —                | Não               |
| Conta       | Pasta      | Integer  | Sim     | Sim              | Sim               |
| ND          | N.D.       | Text     | Sim     | Não              | Sim (sempre)      |
| Ref         | Histórico  | Text     | Sim     | Não              | Sim (sempre)      |
| VValor      | VValor     | Decimal  | Não     | —                | Não               |
| DC          | DC         | Text     | Não     | —                | Não               |
| Deb         | Débito     | Decimal  | Sim     | Sim              | Sim               |
| Cred        | Crédito    | Decimal  | Sim     | Sim              | Sim               |
| Saldo       | Saldo      | Decimal  | Sim     | Sim              | Não (sempre)      |

**Configurações da grid:**
- `MultiSelect = True` (para seleção de linhas na transferência)
- `AllowAddNewRow = False`
- `AllowDeleteRow = False` (habilitado apenas quando desbloqueado)
- `EnableFiltering = True`, `EnableSorting = False`, `EnableGrouping = False`
- Formatação condicional no Saldo: >= 0 → fundo rosa/vermelho; < 0 → fundo verde/azul
- Formatação condicional no Histórico: "AGENCIA" → texto vermelho

### 2.3 Barra de Filtros

| Controle | Tipo     | Função                           |
|----------|----------|----------------------------------|
| ftPasta  | TextBox  | Filtro por Pasta (Conta)         |
| ftHist   | TextBox  | Filtro por Histórico (LIKE)      |
| ftND     | TextBox  | Filtro por ND                    |
| ftPsND   | CheckBox | "Somente pastas sem N. D."       |

### 2.4 Rodapé

| Controle | Tipo  | Função                                                 |
|----------|-------|--------------------------------------------------------|
| lbSaldo  | Label | "Saldo: X.XXX,XX" — vermelho se >= 0, azul se < 0     |

### 2.5 Barra de Ações

| Controle      | Tipo     | Visível         | Função                                      |
|---------------|----------|-----------------|---------------------------------------------|
| ribRetornar   | Button   | Sempre          | Fecha o form (→ navigate('/principal'))      |
| btnImprimir   | Button   | Sempre          | Abre FrmExtratoRpt (pendente D1)            |
| btnTransfere  | Button   | Após F9         | Executa transferência de lançamentos         |
| cboCliente    | ComboBox | Após F9         | Seletor de cliente destino (exclui atual)   |
| lblTransfere  | Label    | Após F9         | "Transferir os lançamentos selecionados..." |
| btnUp         | Button   | Sempre          | Navega para o primeiro registro             |
| btnDown       | Button   | Sempre          | Navega para o último registro               |

---

## 3. Fluxo Funcional

### 3.1 Fluxo original (legado)

```
FrmPrincipal [botão Extrato ou double-click na grid]
    │
    ▼
FrmExtrato
┌─────────────────────────────────────────────────────┐
│ FrmExtrato_Load:                                    │
│   Fill(Clientes) com filtro [Id] <> cliente atual   │
│   Fill(Extrato) + filtro [IdCliente] = id_cliente   │
│   Calcula saldo acumulado (loop nas linhas)         │
│   lbCliente = "Nome - Código"                       │
│   lbSaldo = "Saldo: X.XXX,XX" (vermelho/azul)      │
│   FrmPrincipal.Visible = False                      │
│   Tag = "1"                                         │
│                                                     │
│ Filtros (ftPasta, ftHist, ftND, ftPsND):            │
│   Validated/CheckedChanged → update_extrato_2()     │
│   Aplica filtro no BindingSource                    │
│   Recalcula saldo acumulado (loop)                  │
│                                                     │
│ F10 (Desbloquear/Bloquear):                         │
│   Se bloqueado:                                     │
│     varSenha = "4" → frmSenha.ShowDialog()          │
│     Se senha OK: desbloqueia cols 1,5,10,11         │
│     AllowDeleteRow = True                           │
│   Se desbloqueado:                                  │
│     update_extrato_2() + MsgBox("Campos bloqueados")│
│     Bloqueia cols 1,5,10,11                         │
│     AllowDeleteRow = False                          │
│                                                     │
│ CellEndEdit:                                        │
│   Salva campo editado via fExecutaText(UPDATE)      │
│   Campos: ND, Ref, Dt, Conta, Deb, Cred            │
│                                                     │
│ F9 (Transferência):                                 │
│   Toggle visibilidade do painel de transferência    │
│   btnTransfere_Click:                               │
│     Valida seleção de cliente destino               │
│     Valida seleção de linhas                        │
│     Confirmação → varSenha = "3" → frmSenha         │
│     Se senha OK: UPDATE IdCliente/CodCliente        │
│     Recarrega extrato                               │
│                                                     │
│ FormClosed:                                         │
│   UPDATE VValor = Deb WHERE Deb > 0                 │
│   UPDATE VValor = Cred WHERE Cred > 0               │
│   FrmPrincipal.Visible = True                       │
└─────────────────────────────────────────────────────┘
```

### 3.2 Fluxo web (`/extrato`)

```
/principal [botão Extrato ou clique na linha]
    │ navigate('/extrato', { state: { idCliente, codCliente, nomeCliente } })
    ▼
/extrato
┌─────────────────────────────────────────────────────┐
│ Mount:                                              │
│   GET /extrato/{id_cliente} → lançamentos + saldo   │
│   Exibe header com nome e código do cliente          │
│   Exibe saldo no rodapé (vermelho/azul)             │
│                                                     │
│ Filtros:                                            │
│   Inputs: Pasta, Histórico, ND, checkbox Sem ND     │
│   Botão "Aplicar filtros" → GET /extrato/{id}       │
│     com query params: pasta, nd, sem_nd, hist        │
│   Recalcula saldo exibido no rodapé                 │
│                                                     │
│ Desbloquear (F10 ou botão):                         │
│   Modal de senha → POST /extrato/desbloquear        │
│     { chave: inputSenha }                           │
│   Se OK → campos Data, Pasta, ND, Hist, Deb, Cred   │
│     ficam editáveis inline                          │
│   AllowDelete habilitado                            │
│                                                     │
│ Edição inline (modo desbloqueado):                  │
│   onBlur → PATCH /lancamentos/{id}                  │
│     { campo: valor }                                │
│   Salva imediatamente (RN90)                        │
│                                                     │
│ Bloquear (F10 novamente ou botão):                  │
│   Campos voltam a readonly                          │
│   Recalcula saldo                                   │
│   AllowDelete desabilitado                          │
│                                                     │
│ Transferir (F9 ou botão):                           │
│   Painel com seletor de cliente + campo de senha     │
│   Requer seleção de ao menos 1 linha                │
│   POST /extrato/transferir                          │
│     { ids, id_destino, chave }                      │
│   Recarrega extrato após sucesso                    │
│                                                     │
│ Imprimir (botão desabilitado):                      │
│   Tooltip: "Disponível após definição da estratégia │
│   de relatórios" (pendente D1)                      │
│                                                     │
│ Primeiro/Último:                                    │
│   Scroll automático para primeira/última linha       │
│                                                     │
│ Unmount (useEffect cleanup):                        │
│   POST /extrato/{id_cliente}/sincronizar-vvalor     │
│                                                     │
│ [Voltar] → navigate('/principal')                   │
└─────────────────────────────────────────────────────┘
```

---

## 4. Regras de Negócio (RN78–RN101)

| RN    | Descrição                                                                                                  | Categoria      |
|-------|------------------------------------------------------------------------------------------------------------|----------------|
| RN78  | Extrato exibe apenas lançamentos do cliente selecionado (`WHERE IdCliente = :id`)                          | Escopo         |
| RN79  | Filtro por Pasta: `WHERE Conta = :valor`                                                                    | Filtro         |
| RN80  | Filtro por ND: `WHERE ND = ':valor'`                                                                        | Filtro         |
| RN81  | Checkbox "Sem ND": `WHERE ND IS NULL`                                                                       | Filtro         |
| RN82  | Filtro por Histórico: `WHERE Ref LIKE '%:texto%'`                                                           | Filtro         |
| RN83  | Combinação de filtros simultâneos (AND entre todos os ativos)                                                | Filtro         |
| RN84  | Saldo acumulado: `SUM(ISNULL(Cred,0) - ISNULL(Deb,0)) OVER (ORDER BY Dt, Id)` — window function no banco   | Cálculo        |
| RN85  | Saldo total no rodapé: vermelho se >= 0 (credor), azul se < 0 (devedor)                                     | Exibição       |
| RN86  | Recálculo do saldo ao aplicar/alterar filtros                                                                | Cálculo        |
| RN87  | Desbloqueio de edição via senha da tabela Chaves (Ref = 'Desbloquear lançamentos')                          | Segurança      |
| RN88  | Campos editáveis após desbloqueio: Data (Dt), Pasta (Conta), N.D. (ND), Histórico (Ref), Débito (Deb), Crédito (Cred) | Edição |
| RN89  | Campos sempre bloqueados: Id, IdCliente, CodCliente, DC, Saldo, VValor, Selec                               | Edição         |
| RN90  | Cada edição de célula salva imediatamente no banco (CellEndEdit → UPDATE individual por campo)               | Persistência   |
| RN91  | F10 novamente → bloqueia campos editáveis e recalcula saldo                                                  | Edição         |
| RN92  | Exclusão de linhas permitida apenas quando edição está desbloqueada                                          | Edição         |
| RN93  | Transferência de lançamentos selecionados para outro cliente (F9)                                            | Transferência  |
| RN94  | Validação de senha para transferência (Chaves WHERE Ref = 'Transferência de lançamentos')                    | Segurança      |
| RN95  | Seleção múltipla de linhas para transferência                                                                | Transferência  |
| RN96  | Combo de cliente destino exclui o cliente atual da lista                                                      | Transferência  |
| RN97  | Transferência: UPDATE IdCliente e CodCliente nos registros selecionados                                      | Transferência  |
| RN98  | Recarregar extrato após transferência bem-sucedida                                                           | Transferência  |
| RN99  | Ao sair da tela: UPDATE VValor = Deb WHERE Deb > 0; UPDATE VValor = Cred WHERE Cred > 0                    | Sincronização  |
| RN100 | Impressão do extrato com filtros ativos (abre FrmExtratoRpt) — pendente D1                                   | Impressão      |
| RN101 | Navegação rápida para primeiro/último registro da grid                                                       | Navegação      |

---

## 5. Mapeamento de Queries / Tabelas do Banco

### 5.1 Tabelas acessadas

| Tabela   | Colunas utilizadas                                                        | Operações              |
|----------|---------------------------------------------------------------------------|------------------------|
| Contas   | Id, IdCliente, CodCliente, Conta, Dt, Ref, VValor, DC, ND, Saldo, Deb, Cred, Selec | SELECT, UPDATE |
| Clientes | Id, Código, Cliente                                                       | SELECT (combo destino) |
| Chaves   | Ref, Chave                                                                | SELECT (verificação)   |

### 5.2 Queries

**Listar lançamentos do cliente (com saldo acumulado):**
```sql
SELECT Dt, Id, IdCliente, CodCliente, Conta, ND, Ref, VValor, DC, Deb, Cred,
  SUM(ISNULL(Cred, 0) - ISNULL(Deb, 0)) OVER (ORDER BY Dt, Id) AS Saldo
FROM Contas
WHERE IdCliente = :id_cliente
  [AND Conta = :pasta]
  [AND ND = :nd]
  [AND ND IS NULL]            -- se checkbox sem_nd ativo
  [AND Ref LIKE '%' + :hist + '%']
ORDER BY Dt, Id
```

**CTE para atualizar saldo no banco (update_extrato legado):**
```sql
;WITH Saldos AS (
  SELECT Dt, Id, DC, VValor, Deb, Cred, Conta,
    SUM(ISNULL(Cred, 0) - ISNULL(Deb, 0)) OVER (ORDER BY Dt, Id) AS Saldo
  FROM Contas
  WHERE [filtros ativos]
)
UPDATE Contas SET Saldo = S.Saldo
FROM Contas INNER JOIN Saldos AS S ON Contas.Id = S.Id
```
**Nota de migração:** Na web, o saldo é calculado na query SELECT e retornado ao frontend. A coluna Saldo da tabela pode ser atualizada opcionalmente, mas o valor exibido vem sempre da window function.

**Edição inline — UPDATE por campo:**
```sql
UPDATE Contas SET ND = :nd WHERE Id = :id
UPDATE Contas SET Ref = :ref WHERE Id = :id
UPDATE Contas SET Dt = :dt WHERE Id = :id
UPDATE Contas SET Conta = :conta WHERE Id = :id
UPDATE Contas SET Deb = :deb WHERE Id = :id
UPDATE Contas SET Cred = :cred WHERE Id = :id
```

**Transferência de lançamentos:**
```sql
-- Buscar código do cliente destino:
SELECT Código FROM Clientes WHERE Id = :id_destino

-- Atualizar registros selecionados:
UPDATE Contas SET IdCliente = :id_destino, CodCliente = :cod_destino
WHERE Id IN (:ids_selecionados)
```

**Sincronização VValor ao fechar:**
```sql
UPDATE Contas SET VValor = Deb WHERE IdCliente = :id_cliente AND Deb > 0
UPDATE Contas SET VValor = Cred WHERE IdCliente = :id_cliente AND Cred > 0
```

**Listar clientes para combo de transferência (excluindo o atual):**
```sql
SELECT Id, Cliente FROM Clientes WHERE Id <> :id_cliente ORDER BY Cliente
```

**Verificar senha de operação crítica:**
```sql
SELECT Chave FROM Chaves WHERE Ref = :ref
```

---

## 6. Decisões Registradas

| ID  | Decisão                                                                                                                   | Fonte                            |
|-----|---------------------------------------------------------------------------------------------------------------------------|----------------------------------|
| D1  | **Estratégia de relatórios: PENDENTE.** Impressão do extrato (FrmExtratoRpt) depende de definição de como gerar PDF/relatório na web. Botão Imprimir fica desabilitado com tooltip explicativo. | escopo_migracao.md — bloqueante  |
| D13 | **Saldo via window function:** O saldo acumulado é sempre calculado pela window function SQL no SELECT, nunca acumulado em loop no frontend. A coluna Saldo da tabela Contas pode ser mantida atualizada opcionalmente. | Análise da migração (2026-04-01) |
| D14 | **Edição inline com onBlur:** A edição inline do legado (CellEndEdit → UPDATE imediato) é adaptada como campos editáveis que salvam ao perder foco (onBlur → PATCH). Substitui o padrão WinForms de edição por célula. | Análise da migração (2026-04-01) |
| D15 | **F9/F10 como botões:** Os atalhos F9 (transferência) e F10 (desbloqueio) são mantidos como atalhos de teclado E expostos via botões visíveis na barra de ações. | Análise da migração (2026-04-01) |
| D16 | **Painel de transferência:** O toggle de visibilidade do painel de transferência (combo + botão) via F9 é adaptado como painel expansível abaixo da tabela, visível apenas quando F9 é acionado. | Análise da migração (2026-04-01) |

---

## 7. Auditoria — Classificação PRESERVAR / ADAPTAR / DESCARTAR

### Legenda
| Símbolo      | Significado                                                         |
|--------------|---------------------------------------------------------------------|
| ✅ PRESERVAR  | Comportamento idêntico ao original — obrigatório                   |
| 🔄 ADAPTAR    | Comportamento equivalente com ajuste tecnológico — justificado     |
| ❌ DESCARTAR  | Comportamento não migrado — intencional e documentado              |

### 7.1 Tabela de classificação

| ID  | Comportamento original                                                              | Classificação  | Justificativa                                                                                     |
|-----|--------------------------------------------------------------------------------------|----------------|----------------------------------------------------------------------------------------------------|
| B22 | Filtro por Pasta (Conta)                                                            | ✅ PRESERVAR   | RN79 — regra de negócio pura, filtro por coluna Conta                                              |
| B23 | Filtro por ND                                                                       | ✅ PRESERVAR   | RN80 — filtro por coluna ND                                                                        |
| B24 | Checkbox "Sem ND" (ND IS NULL)                                                      | ✅ PRESERVAR   | RN81 — filtro especial para registros sem ND                                                       |
| B25 | Filtro por Histórico (LIKE %texto%)                                                 | ✅ PRESERVAR   | RN82 — busca parcial na coluna Ref                                                                 |
| B26 | Combinação de filtros simultâneos                                                   | ✅ PRESERVAR   | RN83 — AND entre todos os filtros ativos                                                           |
| B27 | Saldo acumulado via window function SQL                                             | ✅ PRESERVAR   | RN84 — cálculo no banco via SUM() OVER (ORDER BY Dt, Id), implementado via SQLAlchemy              |
| B28 | Saldo no rodapé (vermelho >= 0, azul < 0)                                           | 🔄 ADAPTAR     | RN85 — manter semântica de cores, adaptar tons ao design system do projeto                         |
| B29 | Recálculo do saldo ao aplicar filtros                                               | ✅ PRESERVAR   | RN86 — ao mudar filtros, o saldo é recalculado pelo backend                                        |
| B30 | Desbloqueio via senha (F10 → frmSenha varSenha=4)                                  | 🔄 ADAPTAR     | RN87 — modal React + POST /extrato/desbloquear; substitui frmSenha global                         |
| B31 | Campos editáveis: Data, Pasta, Histórico, ND, Débito, Crédito                      | ✅ PRESERVAR   | RN88 — mesmos campos editáveis na versão web                                                       |
| B32 | Campos sempre bloqueados: Id, IdCliente, CodCliente, DC, Saldo                     | ✅ PRESERVAR   | RN89 — campos nunca editáveis pelo usuário                                                         |
| B33 | CellEndEdit salva imediatamente no banco                                            | 🔄 ADAPTAR     | RN90 — onBlur → PATCH /lancamentos/{id}; equivalente funcional do CellEndEdit                     |
| B34 | F10 novamente → bloqueia campos + recalcula saldo                                   | ✅ PRESERVAR   | RN91 — toggle de estado bloqueado/desbloqueado                                                     |
| B35 | AllowDeleteRow controlado pelo estado de bloqueio                                   | ✅ PRESERVAR   | RN92 — exclusão só permitida em modo desbloqueado                                                  |
| B36 | Transferência de lançamentos (F9 → painel)                                          | 🔄 ADAPTAR     | RN93 — painel expansível com seletor de cliente e campo de senha                                   |
| B37 | Validação de senha para transferência                                               | ✅ PRESERVAR   | RN94 — POST /auth/verificar-senha com ref = 'Transferência de lançamentos'                        |
| B38 | Seleção múltipla de linhas                                                          | ✅ PRESERVAR   | RN95 — checkboxes ou clique com Ctrl/Shift na tabela                                               |
| B39 | Combo destino exclui cliente atual                                                  | ✅ PRESERVAR   | RN96 — lista de clientes sem o cliente cujo extrato está sendo visualizado                         |
| B40 | UPDATE IdCliente + CodCliente                                                       | ✅ PRESERVAR   | RN97 — transferência altera ambas as colunas nos registros selecionados                            |
| B41 | Recarregar extrato após transferência                                               | ✅ PRESERVAR   | RN98 — GET /extrato/{id} novamente após POST /extrato/transferir                                  |
| B42 | UPDATE VValor = Deb/Cred ao fechar                                                  | 🔄 ADAPTAR     | RN99 — POST /extrato/{id}/sincronizar-vvalor chamado no useEffect cleanup                         |
| B43 | Impressão → FrmExtratoRpt                                                           | 🔄 ADAPTAR     | RN100 — botão desabilitado com tooltip; pendente decisão D1 (estratégia de relatórios)             |
| B44 | Navegação primeiro/último registro                                                  | 🔄 ADAPTAR     | RN101 — scroll automático para primeira/última linha da tabela                                     |
| B45 | FrmPrincipal.Visible = False/True                                                   | ❌ DESCARTAR   | React Router gerencia navegação; sem janelas para ocultar/mostrar                                  |
| B46 | SendKeys.Send("{Tab}") para Enter                                                   | ❌ DESCARTAR   | Artefato WinForms — sem equivalente necessário na web                                              |
| B47 | SendKeys.Send("{ESC}") em btnDown/btnUp                                             | ❌ DESCARTAR   | Artefato WinForms — workaround de foco                                                             |
| B48 | Application.DoEvents                                                                | ❌ DESCARTAR   | Artefato WinForms — processamento de mensagens do Windows                                          |
| B49 | mouse_event (simulação de clique)                                                   | ❌ DESCARTAR   | Artefato WinForms — código comentado, sem função                                                   |
| B50 | Timer1 para impressão                                                               | ❌ DESCARTAR   | Artefato WinForms — workaround de timing para abrir form filho                                     |
| B51 | CircularProgress1 (DevComponents)                                                   | ❌ DESCARTAR   | Componente proprietário WinForms sem equivalente necessário                                         |
| B52 | BalloonTip1 (DevComponents)                                                         | ❌ DESCARTAR   | Componente proprietário WinForms sem equivalente necessário                                         |
| B53 | Office2010BlackTheme / C1Ribbon                                                     | ❌ DESCARTAR   | Componentes proprietários WinForms — styling via CSS no projeto web                                |
| B54 | Button1 (debug, hidden)                                                             | ❌ DESCARTAR   | Botão de debug sem função na produção — código de teste do desenvolvedor                           |
| B55 | Conditional formatting "AGENCIA" em vermelho no Histórico                           | 🔄 ADAPTAR     | Manter destaque visual — implementar via CSS condicional no componente                             |
| B56 | Me.Tag = "1" (controle de estado de inicialização)                                  | 🔄 ADAPTAR     | Estado React (useRef/useState) controla se componente já foi inicializado                          |

### 7.2 Resumo

| Classificação   | Quantidade | IDs                                                                        |
|-----------------|------------|----------------------------------------------------------------------------|
| ✅ PRESERVAR    | 16         | B22, B23, B24, B25, B26, B27, B29, B31, B32, B34, B35, B37, B38, B39, B40, B41 |
| 🔄 ADAPTAR      | 9          | B28, B30, B33, B36, B42, B43, B44, B55, B56                               |
| ❌ DESCARTAR    | 10         | B45, B46, B47, B48, B49, B50, B51, B52, B53, B54                          |

---

## 8. Mapeamento Web

| Tela Web   | Form Original | Documentação                        |
|------------|---------------|-------------------------------------|
| `/extrato` | `FrmExtrato`  | `docs/FrmExtrato_documentacao.md`   |
