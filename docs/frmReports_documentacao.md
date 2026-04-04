# Documentação Funcional — frmReports
**Sistema:** Solo Consultoria de Imóveis — Contas Correntes
**Form original:** `frmReports.vb` (VB.Net / WinForms)
**URL web:** `/relatorios`
**Propósito:** Tela de parâmetros para seleção do tipo e filtros dos relatórios de clientes devedores e credores

---

## 1. Visão Geral

O `frmReports` é um formulário de parâmetros simples, sem acesso ao banco de dados.
Ele coleta os filtros e preferências do usuário e os repassa para os forms de relatório
(`frmRptDevedores1` ou `frmCredores1`) que executam as queries e exibem os resultados.

Na migração web, o form vira a página `/relatorios`, que coleta os parâmetros e navega
para `/relatorios/devedores` ou `/relatorios/credores` passando os filtros via React
Router state.

---

## 2. Componentes Visuais

### 2.1 Controles ativos

| Controle | Tipo             | Label original                   | Padrão               | Parâmetro web       |
|----------|------------------|----------------------------------|----------------------|---------------------|
| `Op3`    | RadioButton      | "Devedores"                      | Checked              | `tipo = 'devedores'`|
| `Op4`    | RadioButton      | "Credores"                       | —                    | `tipo = 'credores'` |
| `Op21`   | RadioButton      | "Banco de dados inteiro"         | Checked              | `faixa = 'todos'`   |
| `Op22`   | RadioButton      | "A partir de 10000"              | —                    | `faixa = 'acima'`   |
| `Op23`   | RadioButton      | "Abaixo de 10000"                | —                    | `faixa = 'abaixo'`  |
| `OpOrd1` | RadioButton      | "Código"                         | Checked              | `ordenacao = 'codigo'` |
| `OpOrd2` | RadioButton      | "Nome"                           | —                    | `ordenacao = 'nome'`|
| `Dt`     | DateTimePicker   | "Na data:"                       | Hoje                 | `data_corte`        |
| `Saldo`  | C1TextBox (*)    | "Com saldo devedor maior ou igual a:" | 0               | `saldo_minimo`      |

(*) O controle `Saldo` está comentado no Designer atual do legado. A query ainda o
referencia. Na migração, o campo é preservado com valor padrão 0.

### 2.2 Controles DESCARTADOS

| Controle      | Motivo                                                        |
|---------------|---------------------------------------------------------------|
| `btnTotais`   | Comentado no legado — funcionalidade de consulta de totais   |
| `btnRibUndo`  | Comentado no legado — equivalente ao botão "Imprimir"        |
| `ribRetornar` | Comentado no legado — botão Retornar                         |
| `C1Ribbon1`   | Componente WinForms — substituído por botões React            |
| `Highlighter1`| Componente DevComponents WinForms                             |

---

## 3. Fluxo Funcional

### 3.1 Fluxo original (legado)

```
FrmPrincipal [botão Relatórios]
    │
    ▼
frmReports
┌─────────────────────────────────────────────────────┐
│ frmReports_Load:                                    │
│   FrmPrincipal.Visible = False                      │
│                                                     │
│ Usuário seleciona: tipo, faixa, ordenação, data,    │
│   saldo mínimo                                      │
│                                                     │
│ [btnRibUndo — comentado]:                           │
│   Se Op3.Checked (devedores):                       │
│     frmRptDevedores1.Show()                         │
│   Senão (credores):                                 │
│     frmCredores1.Show()                             │
│                                                     │
│ frmReports_Closed:                                  │
│   FrmPrincipal.Visible = True                       │
└─────────────────────────────────────────────────────┘
```

### 3.2 Fluxo web (`/relatorios`)

```
/principal [botão Relatórios]
    │ navigate('/relatorios')
    ▼
/relatorios
┌─────────────────────────────────────────────────────┐
│ Mount: formulário com valores padrão                │
│   tipo = 'devedores'                                │
│   faixa = 'todos'                                   │
│   ordenacao = 'codigo'                              │
│   data_corte = hoje                                 │
│   saldo_minimo = 0                                  │
│                                                     │
│ Botão "Gerar Relatório":                            │
│   Se tipo = 'devedores':                            │
│     navigate('/relatorios/devedores', { state: params }) │
│   Senão:                                            │
│     navigate('/relatorios/credores', { state: params })  │
│                                                     │
│ [Voltar] → navigate('/principal')                   │
└─────────────────────────────────────────────────────┘
```

---

## 4. Regras de Negócio (RN102–RN107)

| RN    | Descrição                                                                                    | Categoria    |
|-------|----------------------------------------------------------------------------------------------|--------------|
| RN102 | Tipo de relatório selecionável: devedores (padrão) ou credores                               | Parâmetro    |
| RN103 | Faixa de código: todos (padrão) / código >= 10000 / código < 10000                          | Parâmetro    |
| RN104 | Data de corte: lançamentos até essa data (`Dt <= :data`); padrão = hoje                     | Parâmetro    |
| RN105 | Saldo mínimo: retorna apenas clientes com saldo (absoluto) acima do valor informado; padrão = 0 | Parâmetro |
| RN106 | Ordenação: por código (padrão) ou por nome do cliente                                        | Parâmetro    |
| RN107 | Parâmetros preservados ao navegar de volta das telas de relatório                            | Navegação    |

---

## 5. Mapeamento de Queries / Tabelas do Banco

O `frmReports` não acessa o banco diretamente. As queries são executadas pelos forms
filhos (`frmRptDevedores1` e `frmCredores1`) com os valores dos controles deste form.

---

## 6. Decisões Registradas

| ID  | Decisão                                                                                                                   | Fonte                         |
|-----|---------------------------------------------------------------------------------------------------------------------------|-------------------------------|
| D1  | **Estratégia de relatórios: @react-pdf/renderer.** Gera PDF real com texto selecionável, suporte nativo a tabelas, sem dependência do DOM. Alternativas `react-to-pdf` e `jsPDF+html2canvas` descartadas por gerarem imagem em vez de PDF de texto. | Análise (2026-04-04) |
| D17 | **Controle Saldo comentado:** O campo `Saldo` está comentado no Designer do legado, mas a query ainda o referencia. Na migração, o parâmetro `saldo_minimo` é preservado com valor padrão 0. | Leitura do legado (2026-04-04) |

---

## 7. Auditoria — Classificação PRESERVAR / ADAPTAR / DESCARTAR

| ID  | Comportamento original                                          | Classificação  | Justificativa                                                        |
|-----|-----------------------------------------------------------------|----------------|----------------------------------------------------------------------|
| B57 | Seleção do tipo de relatório (devedores/credores)               | ✅ PRESERVAR   | RN102 — regra de negócio pura                                        |
| B58 | Filtro por faixa de código (todos/acima/abaixo)                 | ✅ PRESERVAR   | RN103 — regra de negócio pura                                        |
| B59 | Filtro de data de corte                                         | ✅ PRESERVAR   | RN104 — filtro temporal                                              |
| B60 | Filtro de saldo mínimo                                          | ✅ PRESERVAR   | RN105 — filtro por valor; mantido mesmo com controle comentado       |
| B61 | Ordenação por código ou por nome                                | ✅ PRESERVAR   | RN106 — configuração de ordenação                                    |
| B62 | Parâmetros preservados ao voltar                                | 🔄 ADAPTAR     | RN107 — React Router state substitui variáveis globais WinForms      |
| B63 | FrmPrincipal.Visible = False/True                               | ❌ DESCARTAR   | Padrão WinForms — React Router gerencia navegação                    |
| B64 | SendKeys.Send("{Tab}") no Enter                                 | ❌ DESCARTAR   | Artefato WinForms — sem equivalente necessário na web                |
| B65 | btnTotais (cálculo de totais inline)                            | ❌ DESCARTAR   | Comentado no legado; funcionalidade equivalente em /totais           |
| B66 | C1Ribbon, C1PictureBox, Highlighter1                            | ❌ DESCARTAR   | Componentes proprietários WinForms sem equivalente necessário        |

---

## 8. Mapeamento Web

| Tela Web      | Form Original | Documentação                        |
|---------------|---------------|-------------------------------------|
| `/relatorios` | `frmReports`  | `docs/frmReports_documentacao.md`   |
