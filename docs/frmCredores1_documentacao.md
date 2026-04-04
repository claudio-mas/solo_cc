# Documentação Funcional — frmCredores1
**Sistema:** Solo Consultoria de Imóveis — Contas Correntes
**Form original:** `frmCredores1.vb` (VB.Net / WinForms)
**URL web:** `/relatorios/credores`
**Propósito:** Relatório de clientes credores (saldo total de créditos > débitos) até a data de corte, com filtros de faixa de código, saldo mínimo e ordenação

---

## 1. Visão Geral

O `frmCredores1` é estruturalmente idêntico ao `frmRptDevedores1`, diferindo apenas
na direção do saldo: aqui o total de créditos supera o total de débitos.
Recebe parâmetros do `frmReports` e renderiza o resultado via C1Report.

Na migração web, vira a página `/relatorios/credores` com a mesma estrutura de
`/relatorios/devedores`, diferindo nos parâmetros de query e na semântica da cor.

---

## 2. Componentes Visuais

| Controle               | Função no legado                                         | Equivalente web                        |
|------------------------|----------------------------------------------------------|----------------------------------------|
| `C1Report1`            | Componente de relatório com agrupamento e título         | Tabela React + @react-pdf/renderer     |
| `C1PrintPreviewControl`| Visualização do relatório antes de imprimir              | Tabela HTML renderizada diretamente    |
| `ribRetornar`          | Fecha o form (volta para frmReports)                     | Botão "Voltar" → navigate('/relatorios') |
| Campo `rtTitulo`       | Título dinâmico do relatório                             | `<h1>` dinâmico no componente React    |
| Campo `rtTitulo2`      | Subtítulo "Banco de dados inteiro" quando Op21           | Subtítulo condicional                  |

---

## 3. Fluxo Funcional

### 3.1 Fluxo original (legado)

```
frmReports [botão Imprimir → tipo = credores]
    │ frmCredores1.Show()
    │ frmReports.Visible = False
    ▼
frmCredores1
┌─────────────────────────────────────────────────────┐
│ frmCredores1_Load:                                  │
│   Monta strSql: TC - TD (invertido de devedores)    │
│   WHERE (a.TC > a.TD) — créditos superam débitos    │
│   Configura agrupamento igual ao de devedores       │
│   C1Report.DataSource = strSql                      │
│   Renderiza relatório                               │
│                                                     │
│ C1Report1_PrintSection:                             │
│   rtTitulo = "Clientes credores em [data] com       │
│   saldo maior ou igual a [valor]"                   │
│   Se Op21: rtTitulo2 = "Banco de dados inteiro"     │
│                                                     │
│ ribRetornar_Click: Me.Close()                       │
│ frmCredores1_Closed: frmReports.Visible = True      │
└─────────────────────────────────────────────────────┘
```

### 3.2 Fluxo web (`/relatorios/credores`)

```
/relatorios [botão Gerar → tipo=credores]
    │ navigate('/relatorios/credores', { state: params })
    ▼
/relatorios/credores
┌─────────────────────────────────────────────────────┐
│ Mount:                                              │
│   Lê params do location.state                       │
│   GET /relatorios/credores?data=...&saldo_minimo=   │
│     ...&faixa_codigo=...&ordenacao=...              │
│   Exibe loading spinner                             │
│   Preenche tabela com resposta                      │
│   Exibe título dinâmico (RN118)                     │
│                                                     │
│ Botão "Exportar PDF":                               │
│   Gera PDF via @react-pdf/renderer                  │
│                                                     │
│ [Voltar] → navigate('/relatorios', { state: params })│
└─────────────────────────────────────────────────────┘
```

---

## 4. Regras de Negócio (RN114–RN119)

| RN    | Descrição                                                                                    | Categoria  |
|-------|----------------------------------------------------------------------------------------------|------------|
| RN114 | Retorna clientes com saldo credor (TC > TD) até a data de corte                             | Query      |
| RN115 | Filtro por faixa de código: todos / código >= 10000 / código < 10000                        | Filtro     |
| RN116 | Filtro por saldo mínimo: `(TC - TD) > :saldo_minimo`                                        | Filtro     |
| RN117 | Ordenação por código (`ORDER BY CodCliente`) ou por nome (`ORDER BY Cliente`)               | Ordenação  |
| RN118 | Título dinâmico: "Clientes credores em [data] com saldo maior ou igual a [valor]"           | Exibição   |
| RN119 | Saldo exibido na cor de credor (azul/destaque) — positivo = empresa deve ao cliente         | Exibição   |

---

## 5. Mapeamento de Queries / Tabelas do Banco

### 5.1 Tabelas acessadas

| Tabela   | Colunas utilizadas       | Operações |
|----------|--------------------------|-----------|
| Contas   | CodCliente, Dt, Deb, Cred| SELECT    |
| Clientes | Código, Cliente          | SELECT    |

### 5.2 Query SQL original (legado)

```sql
-- Estrutura idêntica à de devedores, com TC - TD:
SELECT a.CodCliente, dbo.Clientes.Cliente, a.TC - a.TD AS Saldo
FROM (
  SELECT CodCliente,
    SUM(CASE DC WHEN 'D' THEN VValor ELSE 0 END) AS TD,
    SUM(CASE DC WHEN 'C' THEN VValor ELSE 0 END) AS TC
  FROM (
    SELECT CodCliente, VValor, DC FROM Contas
    WHERE Dt <= '[data_corte]'
  ) AS X
  GROUP BY CodCliente
) AS a
INNER JOIN dbo.Clientes ON a.CodCliente = dbo.Clientes.Código
WHERE (a.TC > a.TD) AND (a.TC - a.TD) > [saldo_minimo]
ORDER BY a.CodCliente | dbo.Clientes.Cliente
```

### 5.3 Query SQLAlchemy (migração web)

```sql
SELECT cl.Id, cl.Código AS CodCliente, cl.Cliente,
  SUM(ISNULL(c.Cred, 0)) - SUM(ISNULL(c.Deb, 0)) AS Saldo
FROM Clientes cl
INNER JOIN Contas c ON c.CodCliente = cl.Código
WHERE c.Dt <= :data_corte
  [AND cl.Código >= 10000]   -- quando faixa_codigo = 'acima'
  [AND cl.Código < 10000]    -- quando faixa_codigo = 'abaixo'
GROUP BY cl.Id, cl.Código, cl.Cliente
HAVING SUM(ISNULL(c.Cred, 0)) - SUM(ISNULL(c.Deb, 0)) > :saldo_minimo
ORDER BY cl.Código | cl.Cliente
```

---

## 6. Decisões Registradas

| ID  | Decisão                                                                                      | Fonte               |
|-----|----------------------------------------------------------------------------------------------|---------------------|
| D1  | **@react-pdf/renderer** para geração de PDF. C1Report descartado.                           | Análise (2026-04-04)|
| D18 | **Deb/Cred vs VValor/DC:** Mesma decisão do frmRptDevedores1 — usar Deb/Cred.               | Análise (2026-04-04)|
| D19 | **Formato de data:** Parâmetros SQLAlchemy tipados eliminam o bug `yyyy-dd-MM` do legado.    | Análise (2026-04-04)|

---

## 7. Auditoria — Classificação PRESERVAR / ADAPTAR / DESCARTAR

| ID  | Comportamento original                                          | Classificação  | Justificativa                                                          |
|-----|-----------------------------------------------------------------|----------------|------------------------------------------------------------------------|
| B74 | Query de credores com filtros de faixa, data e saldo            | ✅ PRESERVAR   | RN114–RN116 — regras de negócio puras                                  |
| B75 | Ordenação por código ou nome                                    | ✅ PRESERVAR   | RN117                                                                  |
| B76 | Título dinâmico com data e saldo mínimo                         | ✅ PRESERVAR   | RN118                                                                  |
| B77 | Coloração do saldo (credor)                                     | ✅ PRESERVAR   | RN119 — CSS condicional                                                |
| B78 | C1Report + C1PrintPreviewControl                               | ❌ DESCARTAR   | Componentes proprietários WinForms                                     |
| B79 | frmReports.Visible = False/True                                 | ❌ DESCARTAR   | Padrão WinForms — React Router gerencia navegação                      |
| B80 | Agrupamento C1Report (GroupBy)                                  | 🔄 ADAPTAR     | Tabela simples ordenada substitui o GroupBy visual do C1Report         |

---

## 8. Mapeamento Web

| Tela Web             | Form Original  | Documentação                             |
|----------------------|----------------|------------------------------------------|
| `/relatorios/credores` | `frmCredores1` | `docs/frmCredores1_documentacao.md`   |
