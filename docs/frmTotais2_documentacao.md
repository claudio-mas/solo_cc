# Documentação Funcional — frmTotais2
**Sistema:** Solo Consultoria de Imóveis — Contas Correntes
**Form original:** `frmTotais2.vb` (VB.Net / WinForms)
**URL web:** `/totais`
**Propósito:** Dashboard de totais do dia — exibe quantidade e valor acumulado de clientes credores e devedores até a data selecionada

---

## 1. Visão Geral

O `frmTotais2` é um form de consulta simples, sem edição. Ao abrir, executa 4 queries
com `Date.Today` e exibe os resultados em Labels coloridos. Um `DateTimePicker` permite
consultar datas anteriores; ao mudar a data, as 4 queries são re-executadas.

Na migração web, o form é convertido em uma página `/totais` com 4 cards de métricas
e um date picker. As 4 queries ADODB separadas são **consolidadas em uma única query
SQLAlchemy** parametrizada por data, exposta via `GET /totais?data=YYYY-MM-DD`.

---

## 2. Componentes Visuais

| Controle | Tipo               | Cor original          | Função                                  |
|----------|--------------------|-----------------------|-----------------------------------------|
| `Dt`     | DateTimePicker     | —                     | Seleção de data; padrão = hoje          |
| `C1`     | Label (resultado)  | Azul (HotTrack)       | Quantidade de clientes credores         |
| `C2`     | Label (resultado)  | Azul                  | Valor total dos credores                |
| `D1`     | Label (resultado)  | Vermelho              | Quantidade de clientes devedores        |
| `D2`     | Label (resultado)  | Vermelho              | Valor total dos devedores               |
| `Label1` | Label (estático)   | —                     | "Somente clientes a partir de 10000"    |
| `Label2` | Label (estático)   | Azul                  | "Credores:"                             |
| `Label3` | Label (estático)   | Vermelho              | "Devedores:"                            |

**Título do form:** "TOTAIS DE CLIENTES"

---

## 3. Mapeamento UI → Banco de Dados

### Tabela `Contas` (colunas utilizadas nas queries)

| Coluna     | Tipo    | Uso nas queries                                              |
|------------|---------|--------------------------------------------------------------|
| `CodCliente` | int   | Agrupamento e contagem de clientes                           |
| `DC`       | char(1) | Classifica cada linha como Débito ('D') ou Crédito ('C')     |
| `VValor`   | decimal | Valor do lançamento; somado por categoria DC                 |
| `Dt`       | date    | Filtro `WHERE Dt <= :data` (acumulado até a data inclusive)  |

As demais colunas selecionadas na camada interna (`IdCliente, Conta, Ref, ND, Saldo,
Deb, Cred, IdLote`) são carregadas mas não utilizadas pelos cálculos — substituídas
na versão web pela query consolidada que seleciona apenas o necessário.

---

## 4. Fluxo Funcional

### 4.1 Fluxo original (legado)

```
FrmPrincipal
  [botão Totais]
      │
      ▼
  frmTotais2
  ┌──────────────────────────────────────────┐
  │ FrmTotais_Load:                          │
  │   data = Date.Today                      │
  │   Executa Q1 → C1.Text = FormatNumber    │
  │   Executa Q2 → C2.Text = FormatCurrency  │
  │   Executa Q3 → D1.Text = FormatNumber    │
  │   Executa Q4 → D2.Text = FormatCurrency  │
  │                                          │
  │ Dt_ValueChanged:                         │
  │   data = Dt.Value                        │
  │   Re-executa Q1..Q4 com nova data        │
  │                                          │
  │ Fechar via X da janela                   │
  │   → FrmPrincipal.Visible = True          │
  └──────────────────────────────────────────┘
```

### 4.2 Fluxo web (`/totais`)

```
/principal
  [botão Totais] → navigate('/totais')
      │
      ▼
  /totais
  ┌──────────────────────────────────────────┐
  │ Mount:                                   │
  │   data = hoje (RN59)                     │
  │   GET /totais?data=hoje → 4 cards        │
  │   estado loading enquanto aguarda        │
  │                                          │
  │ onChange(date picker):                   │
  │   GET /totais?data=nova → atualiza cards │
  │   (RN60)                                 │
  │                                          │
  │ [Voltar] → navigate('/principal')        │
  └──────────────────────────────────────────┘
```

---

## 5. Regras de Negócio (RN59–RN67)

| RN   | Descrição                                                                              | Categoria   |
|------|----------------------------------------------------------------------------------------|-------------|
| RN59 | Data padrão de consulta: hoje (equivalente a `Date.Today` no Load)                    | Fluxo       |
| RN60 | Ao alterar a data no date picker, os 4 totais são recalculados com a nova data         | Fluxo       |
| RN61 | Credores = clientes onde TC > TD (TC = total créditos, TD = total débitos acumulados até a data) | Negócio |
| RN62 | Devedores = clientes onde TD > TC acumulado até a data                                 | Negócio     |
| RN63 | Quantidade de credores = COUNT(CodCliente) WHERE TC > TD                               | Cálculo     |
| RN64 | Valor total de credores = SUM(TC − TD) WHERE TC > TD                                  | Cálculo     |
| RN65 | Quantidade de devedores = COUNT(CodCliente) WHERE TD > TC                              | Cálculo     |
| RN66 | Valor total de devedores = SUM(TD − TC) WHERE TD > TC                                 | Cálculo     |
| RN67 | Filtro temporal: `WHERE Dt <= :data` — acumulado desde o início até a data (inclusive) | Dados       |

---

## 6. Queries ao Banco de Dados

### 6.1 Queries originais (VB.Net / ADODB)

Todas as 4 queries usam a mesma estrutura de 3 camadas. Diferem apenas no SELECT externo
e no filtro `WHERE (TC > TD)` ou `WHERE (TD > TC)`.

**Estrutura comum (3 camadas aninhadas):**

```sql
-- Camada 1 (interna): filtra por data, lista colunas explícitas
SELECT IdCliente, CodCliente, Conta, Dt, Ref, VValor, DC, ND, Saldo, Deb, Cred, IdLote
FROM Contas
WHERE Dt <= '<data>'

-- Camada 2: pivota DC em TotDeb e TotCred
SELECT CodCliente,
       CASE DC WHEN 'D' THEN VValor ELSE 0 END AS TotDeb,
       CASE DC WHEN 'C' THEN VValor ELSE 0 END AS TotCred
FROM <camada1> AS X

-- Camada 3 (agrupamento): soma por cliente
SELECT CodCliente, SUM(TotDeb) AS TD, SUM(TotCred) AS TC
FROM <camada2> AS C
GROUP BY CodCliente
```

| Query | SELECT externo                        | Filtro externo   | Exibido em | Formatação         |
|-------|---------------------------------------|------------------|------------|--------------------|
| Q1    | `COUNT(CodCliente) AS QtdeC`          | `WHERE TC > TD`  | `C1`       | FormatNumber(n, 0) |
| Q2    | `SUM(TC) - SUM(TD) AS TotC`           | `WHERE TC > TD`  | `C2`       | FormatCurrency(n, 2) |
| Q3    | `COUNT(CodCliente) AS QtdeD`          | `WHERE TD > TC`  | `D1`       | FormatNumber(n, 0) |
| Q4    | `SUM(TD) - SUM(TC) AS TotD`           | `WHERE TD > TC`  | `D2`       | FormatCurrency(n, 2) |

### 6.2 Query consolidada (web — SQLAlchemy)

As 4 queries são consolidadas em **uma única query com CTE**. Elimina 3 round-trips
ao banco e remove a duplicação de lógica:

```sql
-- RN61–RN67: query consolidada parametrizada por data
WITH saldos AS (
    SELECT
        CodCliente,
        SUM(CASE WHEN DC = 'D' THEN VValor ELSE 0 END) AS TD,
        SUM(CASE WHEN DC = 'C' THEN VValor ELSE 0 END) AS TC
    FROM Contas
    WHERE Dt <= :data
    GROUP BY CodCliente
)
SELECT
    COUNT(CASE WHEN TC > TD THEN 1 END)                        AS qtde_credores,
    ISNULL(SUM(CASE WHEN TC > TD THEN TC - TD END), 0)         AS valor_credores,
    COUNT(CASE WHEN TD > TC THEN 1 END)                        AS qtde_devedores,
    ISNULL(SUM(CASE WHEN TD > TC THEN TD - TC END), 0)         AS valor_devedores
FROM saldos
```

**Parâmetros:** `:data` = date ISO 8601 (YYYY-MM-DD)
**Implementação:** `backend/app/services/totais.py` → função `buscar_totais(db, data)`

---

## 7. Decisões Registradas

| ID  | Decisão                                                                                                          | Fonte              |
|-----|------------------------------------------------------------------------------------------------------------------|--------------------|
| D5  | **Bug de data no legado:** `Date.Today.ToString("yyyy-dd-MM")` produz "2026-31-03" (dia e mês invertidos). Web usa ISO 8601 `yyyy-MM-dd`. | Análise da migração (2026-03-31) |
| D6  | **Discrepância Label1:** o texto "Somente clientes a partir de 10000" está no form, mas as queries não filtram por CodCliente >= 10000. O texto é mantido como nota informacional. As queries são migradas como-estão (sem filtro por código). | Análise da migração (2026-03-31) |
| D7  | **Consolidação de queries:** as 4 queries ADODB separadas são consolidadas em 1 query CTE. Comportamento idêntico ao original, com melhor performance (1 round-trip ao banco). | Análise da migração (2026-03-31) |

---

## 8. Auditoria — Classificação PRESERVAR / ADAPTAR / DESCARTAR

### Legenda
| Símbolo     | Significado                                                         |
|-------------|---------------------------------------------------------------------|
| ✅ PRESERVAR | Comportamento idêntico ao original — obrigatório                   |
| 🔄 ADAPTAR   | Comportamento equivalente com ajuste tecnológico — justificado     |
| ❌ DESCARTAR | Comportamento não migrado — intencional e documentado              |

### 8.1 Tabela de classificação

| ID  | Comportamento original                                           | Classificação    | Justificativa                                                               |
|-----|------------------------------------------------------------------|------------------|-----------------------------------------------------------------------------|
| B01 | `FrmTotais_Load` executa 4 queries com `Date.Today`             | 🔄 ADAPTAR       | RN59 — `useEffect` no mount chama `GET /totais?data=hoje`                   |
| B02 | `Dt_ValueChanged` re-executa 4 queries com data selecionada     | 🔄 ADAPTAR       | RN60 — `onChange` no date picker chama `GET /totais?data=nova`              |
| B03 | `Date.Today` como valor padrão do `DateTimePicker`              | ✅ PRESERVAR     | RN59 — frontend inicializa `<input type="date">` com data de hoje           |
| B04 | 4 queries ADODB separadas                                        | 🔄 ADAPTAR       | D7 — consolidadas em 1 query CTE no `services/totais.py`                   |
| B05 | `FormatCurrency(value, 2)` para valores monetários              | 🔄 ADAPTAR       | `Intl.NumberFormat('pt-BR', { style:'currency', currency:'BRL' })`          |
| B06 | `FormatNumber(value, 0)` para quantidades                       | 🔄 ADAPTAR       | `Intl.NumberFormat('pt-BR', { maximumFractionDigits:0 })`                   |
| B07 | `C1`/`C2` azul (SystemColors.HotTrack); `D1`/`D2` vermelho      | 🔄 ADAPTAR       | CSS classes `.credores` (azul ~#0033aa) e `.devedores` (vermelho #cc0000)   |
| B08 | Label1 "Somente clientes a partir de 10000"                     | ✅ PRESERVAR     | D6 — texto informacional mantido como nota na página                        |
| B09 | Título "TOTAIS DE CLIENTES"                                     | 🔄 ADAPTAR       | `<h1>` da página web                                                        |
| B10 | `FrmPrincipal.Visible = False/True` ao abrir/fechar             | ❌ DESCARTAR     | Padrão WinForms — React Router gerencia navegação; sem janelas para ocultar |
| B11 | `Office2010BlackTheme` / Telerik / `Fixed3D` border             | ❌ DESCARTAR     | Componentes WinForms proprietários sem equivalente web                      |
| B12 | Data format `"yyyy-dd-MM"` (BUG no legado — dia e mês invertidos) | 🔄 ADAPTAR/CORRIGIR | D5 — backend recebe e processa ISO 8601 `yyyy-MM-dd` corretamente        |
| B13 | Sem botão Retornar (fechamento via X da janela)                 | 🔄 ADAPTAR       | Adicionar botão "Voltar" → `navigate('/principal')` conforme padrão da app  |

### 8.2 Resumo

| Classificação    | Quantidade | IDs                              |
|------------------|------------|----------------------------------|
| ✅ PRESERVAR     | 2          | B03, B08                         |
| 🔄 ADAPTAR       | 9          | B01, B02, B04, B05, B06, B07, B09, B12, B13 |
| ❌ DESCARTAR     | 2          | B10, B11                         |

---

## 9. Mapeamento Web

| Tela Web  | Form Original | Documentação                      |
|-----------|---------------|-----------------------------------|
| `/totais` | `frmTotais2`  | `docs/frmTotais2_documentacao.md` |
