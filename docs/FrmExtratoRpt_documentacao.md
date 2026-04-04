# Documentação Funcional — FrmExtratoRpt
**Sistema:** Solo Consultoria de Imóveis — Contas Correntes
**Form original:** `frmExtratoRpt.vb` (VB.Net / WinForms)
**URL web:** `/extrato/imprimir`
**Propósito:** Visualização e exportação em PDF do extrato filtrado do cliente

---

## 1. Visão Geral

O `FrmExtratoRpt` é aberto pelo `FrmExtrato` ao clicar no botão "Imprimir".
Recebe os filtros ativos do `FrmExtrato` (via acesso direto a variáveis globais
do form no WinForms) e executa uma query SQL montando o extrato filtrado do
cliente para exibição no C1Report.

Na migração web, vira a página `/extrato/imprimir` que recebe os filtros ativos
via React Router state (passados por `/extrato`) e reutiliza o endpoint já
existente `GET /extrato/{id_cliente}` com os filtros como query params.

---

## 2. Componentes Visuais

| Controle               | Função no legado                                          | Equivalente web                         |
|------------------------|-----------------------------------------------------------|-----------------------------------------|
| `C1Report1`            | Relatório de extrato com campo Titulo embutido            | Tabela React com cabeçalho              |
| `C1PrintPreviewControl`| Pré-visualização do relatório                             | Tabela HTML + botão exportar PDF        |
| `ribRetornar`          | Fecha o form (volta para FrmExtrato)                      | Botão "Voltar" → navigate('/extrato')   |
| Campo 15 (Saldo)       | Saldo final: >= 0 → vermelho, < 0 → azul                  | CSS condicional na célula de saldo      |
| Campo `Titulo`         | "Nome - Código" do cliente (embutido na query como literal)| `<h1>` com nome e código do cliente    |

---

## 3. Fluxo Funcional

### 3.1 Fluxo original (legado)

```
FrmExtrato [botão Imprimir / Timer1]
    │ FrmExtratoRpt.ShowDialog()
    │ FrmExtrato.CircularProgress1.Visible = True
    ▼
FrmExtratoRpt
┌─────────────────────────────────────────────────────┐
│ FrmExtratoRpt_Load:                                 │
│   strFiltro = "[IdCliente] = " & id_cliente         │
│   Se ftPsND.Checked: strFiltro += " AND ND IS NULL" │
│   Se FilterDescriptors <> "":                       │
│     strFiltro += " AND " & FilterDescriptors        │
│   strSql = "SELECT Dt, Id, IdCliente, CodCliente,   │
│     Conta, Ref, VValor, DC, ND, Saldo,              │
│     '[lbCliente.Text]' As Titulo,                   │
│     Deb AS Débito, Cred AS Crédito                  │
│     FROM Contas WHERE [filtros] ORDER BY Dt, Id"    │
│   C1Report1.DataSource = strSql                     │
│   Renderiza no C1PrintPreviewControl                │
│   FrmExtrato.Visible = False                        │
│                                                     │
│ C1Report1_PrintSection:                             │
│   Se Fields(15).Value >= 0:                         │
│     ForeColor = RGB(255,0,0) → vermelho             │
│   Senão:                                            │
│     ForeColor = #0000ff → azul                      │
│                                                     │
│ ribRetornar_Click: Me.Close()                       │
│                                                     │
│ FrmExtratoRpt_Closed:                               │
│   FrmExtrato.Visible = True                         │
│   CircularProgress1.Visible = False                 │
│   CircularProgress1.IsRunning = False               │
└─────────────────────────────────────────────────────┘
```

**Nota sobre filtros no legado:** Os filtros de Pasta (`ftPasta`), ND (`ftND`) e
Histórico (`ftHist`) estão comentados no código original. Apenas o filtro de
`ftPsND` (Sem ND) e a expressão `FilterDescriptors` (filtros automáticos da grid)
são aplicados. Na migração, todos os filtros ativos do `/extrato` são repassados.

### 3.2 Fluxo web (`/extrato/imprimir`)

```
/extrato [botão Imprimir]
    │ navigate('/extrato/imprimir', { state: {
    │   idCliente, codCliente, nomeCliente,
    │   pasta, nd, semNd, hist
    │ }})
    ▼
/extrato/imprimir
┌─────────────────────────────────────────────────────┐
│ Mount:                                              │
│   Lê filtros do location.state                      │
│   GET /extrato/{idCliente}?pasta=...&nd=...         │
│     &sem_nd=...&hist=...                            │
│   Exibe loading                                     │
│   Preenche tabela com lançamentos                   │
│   Título: "nomeCliente - codCliente" (RN122)        │
│   Saldo final colorido (RN123)                      │
│                                                     │
│ Botão "Exportar PDF":                               │
│   @react-pdf/renderer gera PDF com a tabela         │
│                                                     │
│ [Voltar] → navigate('/extrato', { state: filtros }) │
│   (RN124)                                           │
└─────────────────────────────────────────────────────┘
```

---

## 4. Regras de Negócio (RN120–RN124)

| RN    | Descrição                                                                                    | Categoria  |
|-------|----------------------------------------------------------------------------------------------|------------|
| RN120 | Recebe filtros ativos do FrmExtrato: sem_nd, pasta, nd, hist — via React Router state       | Parâmetro  |
| RN121 | Query filtra lançamentos combinando filtros ativos com `IdCliente = :id`                    | Query      |
| RN122 | Título do relatório inclui nome e código do cliente ("nomeCliente - codCliente")            | Exibição   |
| RN123 | Saldo final colorido: >= 0 → vermelho (credor), < 0 → azul (devedor) — mesmo padrão do extrato | Exibição |
| RN124 | Botão Retornar volta para `/extrato` preservando os filtros ativos no estado da rota        | Navegação  |

---

## 5. Mapeamento de Queries / Tabelas do Banco

### 5.1 Endpoint reutilizado

O `/extrato/imprimir` **não cria endpoint novo**. Reutiliza:

```
GET /extrato/{id_cliente}
  ?pasta=...
  &nd=...
  &sem_nd=true|false
  &hist=...
```

Este endpoint já implementa RN78–RN86 (filtros + saldo acumulado) e retorna todos
os lançamentos necessários para a tela de impressão.

### 5.2 Query SQL do legado (referência)

```sql
SELECT Dt, Id, IdCliente, CodCliente, Conta, Ref, VValor, DC, ND, Saldo,
  '[lbCliente.Text]' AS Titulo,
  Deb AS Débito, Cred AS Crédito
FROM Contas
WHERE [IdCliente] = :id_cliente
  [AND ND IS NULL]                 -- se ftPsND.Checked
  [AND FilterDescriptors.Expression]
ORDER BY Dt, Id
```

**Nota:** O campo `Titulo` é um literal SQL embutido na query com o texto de
`lbCliente.Text` do FrmExtrato. Na migração, o título é montado no componente
React com os dados recebidos via state da rota.

---

## 6. Decisões Registradas

| ID  | Decisão                                                                                                                   | Fonte                         |
|-----|---------------------------------------------------------------------------------------------------------------------------|-------------------------------|
| D1  | **@react-pdf/renderer** para geração de PDF. C1Report + C1PrintPreviewControl descartados.                               | Análise (2026-04-04)          |
| D20 | **Filtros comentados:** No legado, apenas `ftPsND` e `FilterDescriptors` são aplicados. Na migração, todos os filtros do `/extrato` são repassados, tornando o comportamento mais fiel à intenção original. | Análise (2026-04-04) |
| D21 | **Endpoint reutilizado:** `GET /extrato/{id_cliente}` já retorna os dados necessários. Não criar endpoint duplicado.       | Análise (2026-04-04)          |

---

## 7. Auditoria — Classificação PRESERVAR / ADAPTAR / DESCARTAR

| ID  | Comportamento original                                          | Classificação  | Justificativa                                                           |
|-----|-----------------------------------------------------------------|----------------|-------------------------------------------------------------------------|
| B81 | Filtros ativos do FrmExtrato repassados ao relatório            | 🔄 ADAPTAR     | RN120 — via React Router state em vez de variáveis globais WinForms     |
| B82 | Query com filtros combinados sobre lançamentos do cliente       | ✅ PRESERVAR   | RN121 — endpoint existente GET /extrato/{id} com filtros                |
| B83 | Título com nome e código do cliente                             | ✅ PRESERVAR   | RN122 — montado no componente React com dados do state                  |
| B84 | Saldo final colorido (>= 0 → vermelho, < 0 → azul)             | ✅ PRESERVAR   | RN123 — mesmo padrão de cores do extrato, via CSS condicional           |
| B85 | Botão Retornar volta para FrmExtrato                            | 🔄 ADAPTAR     | RN124 — navigate('/extrato', state) com filtros preservados             |
| B86 | C1Report + C1PrintPreviewControl                               | ❌ DESCARTAR   | Componentes proprietários WinForms; substituídos por tabela React + PDF |
| B87 | FrmExtrato.Visible = False/True                                 | ❌ DESCARTAR   | Padrão WinForms — React Router gerencia navegação                       |
| B88 | CircularProgress1.IsRunning                                     | ❌ DESCARTAR   | Componente DevComponents WinForms; substituído por estado loading React |
| B89 | Campo Titulo embutido na query SQL como literal                 | 🔄 ADAPTAR     | Título montado no componente React; query não precisa incluir o literal |
| B90 | Filtros ftPasta, ftND, ftHist comentados no legado              | 🔄 ADAPTAR     | Na migração todos os filtros ativos são repassados (comportamento mais completo) |

---

## 8. Mapeamento Web

| Tela Web          | Form Original  | Documentação                          |
|-------------------|----------------|---------------------------------------|
| `/extrato/imprimir` | `FrmExtratoRpt` | `docs/FrmExtratoRpt_documentacao.md` |
