# Documentação Funcional — frmRptDevedores1
**Sistema:** Solo Consultoria de Imóveis — Contas Correntes
**Form original:** `frmRptDevedores1.vb` (VB.Net / WinForms)
**URL web:** `/relatorios/devedores`
**Propósito:** Relatório de clientes devedores (saldo total de débitos > créditos) até a data de corte, com filtros de faixa de código, saldo mínimo e ordenação

---

## 1. Visão Geral

O `frmRptDevedores1` recebe parâmetros do `frmReports` (via variáveis globais de estado
do WinForms) e executa uma query SQL que calcula o saldo líquido de cada cliente.
Exibe os clientes cujo total de débitos supera o total de créditos, renderizando o
resultado em um C1Report com agrupamento e título dinâmico.

Na migração web, vira a página `/relatorios/devedores` que:
- Recebe os parâmetros via React Router state (passados por `/relatorios`)
- Busca os dados via `GET /relatorios/devedores` com query params
- Exibe tabela com saldo colorido e botão de exportação PDF

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
frmReports [botão Imprimir]
    │ frmRptDevedores1.Show()
    │ frmReports.Visible = False
    ▼
frmRptDevedores1
┌─────────────────────────────────────────────────────┐
│ frmRptDevedores1_Load:                              │
│   Monta strSql conforme Op21/Op22/Op23 (faixa) e   │
│   frmReports.Saldo.Value (saldo mínimo)             │
│   Configura agrupamento: GroupBy CodCliente ou       │
│   Cliente (conforme OpOrd1/OpOrd2)                  │
│   Sort = Ascending                                   │
│   C1Report.DataSource = strSql                      │
│   Renderiza relatório no C1PrintPreviewControl      │
│                                                     │
│ C1Report1_PrintSection:                             │
│   Define rtTitulo = "Clientes devedores em [data]  │
│   com saldo maior ou igual a [valor]"               │
│   Se Op21: rtTitulo2 = "Banco de dados inteiro"     │
│                                                     │
│ ribRetornar_Click:                                  │
│   Me.Close()                                        │
│                                                     │
│ frmRptDevedores1_Closed:                            │
│   frmReports.Visible = True                         │
└─────────────────────────────────────────────────────┘
```

### 3.2 Fluxo web (`/relatorios/devedores`)

```
/relatorios [botão Gerar → tipo=devedores]
    │ navigate('/relatorios/devedores', { state: params })
    ▼
/relatorios/devedores
┌─────────────────────────────────────────────────────┐
│ Mount:                                              │
│   Lê params do location.state                       │
│   GET /relatorios/devedores?data=...&saldo_minimo=  │
│     ...&faixa_codigo=...&ordenacao=...              │
│   Exibe loading spinner                             │
│   Preenche tabela com resposta                      │
│   Exibe título dinâmico (RN112)                     │
│                                                     │
│ Botão "Exportar PDF":                               │
│   Gera PDF via @react-pdf/renderer                  │
│   Colunas: Código | Cliente | Saldo                 │
│   Título dinâmico no PDF                            │
│                                                     │
│ [Voltar] → navigate('/relatorios', { state: params })│
└─────────────────────────────────────────────────────┘
```

---

## 4. Regras de Negócio (RN108–RN113)

| RN    | Descrição                                                                                    | Categoria  |
|-------|----------------------------------------------------------------------------------------------|------------|
| RN108 | Retorna clientes com saldo devedor (TD > TC) até a data de corte                            | Query      |
| RN109 | Filtro por faixa de código: todos / código >= 10000 / código < 10000                        | Filtro     |
| RN110 | Filtro por saldo mínimo: `(TD - TC) > :saldo_minimo` (clientes com saldo acima do valor)    | Filtro     |
| RN111 | Ordenação por código (`ORDER BY CodCliente`) ou por nome (`ORDER BY Cliente`)               | Ordenação  |
| RN112 | Título dinâmico: "Clientes devedores em [data] com saldo maior ou igual a [valor]"          | Exibição   |
| RN113 | Saldo exibido na cor de devedor (vermelho/destaque)                                          | Exibição   |

---

## 5. Mapeamento de Queries / Tabelas do Banco

### 5.1 Tabelas acessadas

| Tabela   | Colunas utilizadas                                          | Operações |
|----------|-------------------------------------------------------------|-----------|
| Contas   | CodCliente, Dt, VValor, DC                                  | SELECT    |
| Clientes | Código, Cliente                                             | SELECT    |

### 5.2 Query SQL original (legado)

```sql
-- faixa = 'todos' (Op21):
SELECT a.CodCliente, dbo.Clientes.Cliente, a.TD - a.TC AS Saldo
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
WHERE (a.TD > a.TC) AND (a.TD - a.TC) > [saldo_minimo]
ORDER BY a.CodCliente | dbo.Clientes.Cliente

-- faixa = 'acima' (Op22): adiciona AND a.CodCliente >= 10000
-- faixa = 'abaixo' (Op23): adiciona AND a.CodCliente < 10000
```

**Nota:** A data no legado usa formato `yyyy-dd-MM` (dia e mês invertidos — aparente bug).
Na migração, a query usa parâmetros SQLAlchemy tipados, eliminando esse problema.

### 5.3 Query SQLAlchemy (migração web)

```sql
-- Equivalente simplificado usando Deb/Cred (consistente com extrato):
SELECT cl.Id, cl.Código AS CodCliente, cl.Cliente,
  SUM(ISNULL(c.Deb, 0)) - SUM(ISNULL(c.Cred, 0)) AS Saldo
FROM Clientes cl
INNER JOIN Contas c ON c.CodCliente = cl.Código
WHERE c.Dt <= :data_corte
  [AND cl.Código >= 10000]   -- quando faixa_codigo = 'acima'
  [AND cl.Código < 10000]    -- quando faixa_codigo = 'abaixo'
GROUP BY cl.Id, cl.Código, cl.Cliente
HAVING SUM(ISNULL(c.Deb, 0)) - SUM(ISNULL(c.Cred, 0)) > :saldo_minimo
ORDER BY cl.Código | cl.Cliente   -- conforme parâmetro ordenacao
```

---

## 6. Decisões Registradas

| ID  | Decisão                                                                                                                    | Fonte                         |
|-----|----------------------------------------------------------------------------------------------------------------------------|-------------------------------|
| D1  | **Estratégia de relatórios: @react-pdf/renderer.** C1Report é descartado. PDF gerado no frontend via biblioteca React.    | Análise (2026-04-04)          |
| D18 | **Deb/Cred vs VValor/DC:** A query original usa `CASE DC WHEN 'D' THEN VValor`. A migração usa as colunas `Deb` e `Cred` diretamente, que são equivalentes e consistentes com o serviço de extrato. | Análise (2026-04-04) |
| D19 | **Formato de data:** O legado usa `yyyy-dd-MM` (possivelmente bugado). A migração usa parâmetros SQLAlchemy tipados como `DATE`, eliminando o problema. | Análise (2026-04-04) |

---

## 7. Auditoria — Classificação PRESERVAR / ADAPTAR / DESCARTAR

| ID  | Comportamento original                                          | Classificação  | Justificativa                                                          |
|-----|-----------------------------------------------------------------|----------------|------------------------------------------------------------------------|
| B67 | Query de devedores com filtros de faixa, data e saldo          | ✅ PRESERVAR   | RN108–RN110 — regras de negócio puras; query migrada para SQLAlchemy   |
| B68 | Ordenação por código ou nome (GroupBy do C1Report)              | ✅ PRESERVAR   | RN111 — ORDER BY no SQL conforme parâmetro                             |
| B69 | Título dinâmico com data e saldo mínimo                         | ✅ PRESERVAR   | RN112 — título montado no componente React com os parâmetros           |
| B70 | Coloração do saldo                                              | ✅ PRESERVAR   | RN113 — CSS condicional no componente React                            |
| B71 | C1Report + C1PrintPreviewControl                               | ❌ DESCARTAR   | Componentes proprietários WinForms; substituídos por tabela React + PDF|
| B72 | frmReports.Visible = False/True                                 | ❌ DESCARTAR   | Padrão WinForms — React Router gerencia navegação                      |
| B73 | Agrupamento C1Report (GroupBy)                                  | 🔄 ADAPTAR     | Sem agrupamento visual: tabela simples ordenada substitui o GroupBy    |

---

## 8. Mapeamento Web

| Tela Web              | Form Original      | Documentação                               |
|-----------------------|--------------------|--------------------------------------------|
| `/relatorios/devedores` | `frmRptDevedores1` | `docs/frmRptDevedores1_documentacao.md` |
