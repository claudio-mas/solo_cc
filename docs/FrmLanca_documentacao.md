# Documentação Funcional — FrmLancaData + FrmLanca
**Sistema:** Solo Consultoria de Imóveis — Contas Correntes
**Forms originais:** `FrmLancaData.vb` + `FrmLanca.vb` (VB.Net / WinForms)
**URL web:** `/lancamentos`
**Propósito:** Registro de novos lançamentos de débito ou crédito na conta corrente do cliente selecionado

---

## 1. Visão Geral

O fluxo original era composto por dois forms em sequência:

1. **FrmLancaData** — modal de seleção de data. Exibido quando nenhuma data estava
   armazenada em `FrmPrincipal.RadGridView1.Tag`. O usuário escolhia a data em um
   `RadCalendar` e confirmava com "Ok", o que armazenava a data na variável global
   `Tag` e abria `FrmLanca`.

2. **FrmLanca** — tela principal de lançamento. Recebia o cliente e a data da tela
   principal, exibia os campos de preenchimento e gravava o registro na tabela `Contas`.

Na migração web, os dois forms são **consolidados em uma única rota `/lancamentos`**,
com um date picker integrado. A data e os dados do cliente são recebidos via route state
(passado pela tela `/principal`). FrmLancaData não tem equivalente web separado.

---

## 2. Componentes Visuais

### FrmLancaData (original)
| Controle       | Tipo          | Label     | Descrição                                                              |
|----------------|---------------|-----------|------------------------------------------------------------------------|
| `RadCalendar1` | RadCalendar   | —         | Calendário para seleção da data                                        |
| `lbData`       | Label         | —         | Exibe a data selecionada formatada                                     |
| `btnOk`        | Button        | "Ok"      | Salva data em `FrmPrincipal.Tag` e abre `FrmLanca`                     |
| `btnCancelar`  | Button        | "Cancelar"| Fecha o form sem ação                                                  |

### FrmLanca (original)
| Controle       | Tipo            | Label      | Descrição                                                             |
|----------------|-----------------|------------|-----------------------------------------------------------------------|
| `dtpDt`        | DateTimePicker  | "Data"     | Data do lançamento (preenchida via `RadGridView1.Tag`)                |
| `txtPasta`     | TextBox         | "Pasta"    | Número da pasta/conta (coluna `Conta` no banco)                       |
| `txtRef`       | C1TextBox       | —          | Histórico/referência do lançamento (coluna `Ref`)                     |
| `txtVValor`    | C1TextBox       | —          | Valor do lançamento (coluna `VValor`)                                 |
| `txtDC`        | TextBox         | —          | Indicador Débito/Crédito — aceita apenas "D" ou "C" (coluna `DC`)     |
| `txtIdCliente` | TextBox (oculto)| —          | Id do cliente (coluna `IdCliente`)                                    |
| `txtCodCliente`| TextBox (oculto)| —          | Código do cliente (coluna `CodCliente`)                               |
| `txtDeb`       | TextBox (oculto)| —          | Valor de débito — preenchido se DC="D" (coluna `Deb`)                 |
| `txtCred`      | TextBox (oculto)| —          | Valor de crédito — preenchido se DC="C" (coluna `Cred`)               |
| `txtDT`        | TextBox (oculto)| —          | Data como texto (redundante com `dtpDt`; descartado na web)           |
| `lbCliente`    | Label           | —          | Exibe o nome do cliente selecionado                                   |
| `ribRetornar`  | RibbonButton    | "Retornar" | Fecha o form e restaura FrmPrincipal                                  |
| `btnNovo`      | RibbonButton    | "Novo"     | Limpa campos para novo lançamento (visível após salvar)               |
| `btnRibSalvar` | RibbonButton    | "Salvar"   | Valida e salva o lançamento (oculto até user preencher algo)          |
| `btnRibUndo`   | RibbonButton    | "Cancelar" | Restaura o formulário ao estado inicial (oculto até user preencher)   |
| `Highlighter1` | DevComponents   | —          | Destaque de borda vermelha no campo com foco                          |

---

## 3. Mapeamento UI → Banco de Dados

### Tabela `Contas` (schema existente — não alterar)

| Campo UI (FrmLanca) | Controle VB     | Coluna DB   | Notas                                        |
|---------------------|-----------------|-------------|----------------------------------------------|
| Data                | `dtpDt`         | `Dt`        | Date do lançamento                           |
| Pasta               | `txtPasta`      | `Conta`     | Número inteiro identificador da pasta        |
| Histórico           | `txtRef`        | `Ref`       | Texto descritivo do lançamento               |
| Valor               | `txtVValor`     | `VValor`    | Decimal; vai para Deb ou Cred conforme DC    |
| D/C                 | `txtDC`         | `DC`        | "D" = Débito, "C" = Crédito                  |
| —                   | `txtDeb`        | `Deb`       | = VValor se DC="D", senão null               |
| —                   | `txtCred`       | `Cred`      | = VValor se DC="C", senão null               |
| —                   | `txtIdCliente`  | `IdCliente` | Recebido via route state                     |
| —                   | `txtCodCliente` | `CodCliente`| Recebido via route state                     |
| —                   | `txtDT`         | —           | Descartado: redundante com `Dt`              |
| —                   | —               | `ND`        | Não preenchido em FrmLanca; null no insert   |
| —                   | —               | `Saldo`     | Calculado externamente; null no insert       |
| —                   | —               | `IdLote`    | Não usado em FrmLanca; null no insert        |
| —                   | —               | `Selec`     | Não usado em FrmLanca; null no insert        |

---

## 4. Fluxo Funcional

### 4.1 Fluxo original (legado)

```
FrmPrincipal
  [botão Lançamentos]
        │
        ▼
  FrmLancaData (modal de data)
  ┌─────────────────────────────────────────┐
  │ RadCalendar seleciona data              │
  │ btnOk → RadGridView1.Tag = data         │
  │       → FrmLanca.Show()                 │
  │ btnCancelar → fecha sem ação            │
  └─────────────────────────────────────────┘
        │
        ▼
  FrmLanca
  ┌─────────────────────────────────────────┐
  │ Load: dtpDt ← RadGridView1.Tag          │
  │       lbCliente ← nome do cliente       │
  │       IdCliente / CodCliente ← hidden   │
  │       Botões: Retornar visible;         │
  │               Salvar/Desfazer hidden    │
  │                                         │
  │ Preenchimento:                          │
  │   txtPasta → txtPasta_Validated:        │
  │     query Contas WHERE Conta = ? AND    │
  │     IdCliente = ?                       │
  │     If EOF → "É uma nova pasta?" (Y/N)  │
  │                                         │
  │   txtDC → KeyPress: só 'D' ou 'C'       │
  │   Qualquer campo → Retornar hidden,     │
  │     Salvar/Desfazer visible             │
  │                                         │
  │ btnRibSalvar: valida campos obrigatórios│
  │   Pasta, Ref, VValor, DC               │
  │   Se DC="D" → Deb = VValor             │
  │   Se DC="C" → Cred = VValor            │
  │   TableAdapterManager.UpdateAll()       │
  │   Retornar visible, Novo visible,       │
  │   Salvar/Desfazer hidden                │
  │                                         │
  │ btnRibUndo: recarrega dataset + focus   │
  │ btnNovo: (sem handler explícito)        │
  │ ribRetornar → Me.Close() →              │
  │   FrmLanca_Closed → FrmPrincipal.Visible│
  └─────────────────────────────────────────┘
```

### 4.2 Fluxo web (`/lancamentos`)

```
/principal
  [botão Lançamentos] → navigate('/lancamentos', { state: { id, codigo, cliente } })
        │
        ▼
  /lancamentos
  ┌─────────────────────────────────────────┐
  │ Load: verifica route state              │
  │   sem state → navigate('/principal')   │
  │   data = hoje (editável pelo user)      │
  │   exibe nome do cliente                 │
  │   Botões: Retornar visible,             │
  │            Salvar/Desfazer hidden       │
  │                                         │
  │ onBlur(Pasta):                          │
  │   GET /lancamentos/verificar-pasta      │
  │   se !existe → modal "É uma nova pasta?"│
  │     Não → limpa campo, refoca           │
  │                                         │
  │ onChange(qualquer campo):               │
  │   dirty = true → Retornar hidden,      │
  │   Salvar/Desfazer visible               │
  │                                         │
  │ [Salvar]: validação Zod (obrigatórios)  │
  │   POST /lancamentos                     │
  │   sucesso → saved = true               │
  │     Retornar visible, Novo visible,     │
  │     Salvar/Desfazer hidden              │
  │                                         │
  │ [Desfazer]: dirty = false, saved = false│
  │   limpa campos, restaura data original │
  │                                         │
  │ [Novo]: saved = false, dirty = false    │
  │   limpa campos, mantém cliente + data  │
  │   foca em Pasta                         │
  │                                         │
  │ [Retornar] → navigate('/principal')     │
  └─────────────────────────────────────────┘
```

---

## 5. Regras de Negócio (RN49–RN58)

| RN   | Descrição                                                                                       | Categoria   |
|------|-------------------------------------------------------------------------------------------------|-------------|
| RN49 | Data obrigatória antes de lançar — FrmLancaData pedia data antes de abrir FrmLanca             | Fluxo       |
| RN50 | Data e dados do cliente passados como parâmetro de contexto para a tela de lançamento          | Fluxo       |
| RN51 | Pasta validada no `onBlur`: se não existe para o cliente → modal "É uma nova pasta?" — "Não" limpa campo e refoca | Integridade |
| RN52 | Campo D/C aceita apenas "D" (Débito) ou "C" (Crédito)                                          | Dados       |
| RN53 | Se DC="D" → `Deb = VValor`, `Cred = null`; se DC="C" → `Cred = VValor`, `Deb = null`          | Negócio     |
| RN54 | Campos obrigatórios antes de salvar: Pasta, Histórico, Valor e D/C                             | Validação   |
| RN55 | Botões Salvar/Desfazer ficam ocultos até o usuário começar a preencher qualquer campo           | UX          |
| RN56 | Botão "Novo" (visível após salvar): limpa formulário mantendo cliente e data; foca em Pasta    | Fluxo       |
| RN57 | Botão "Desfazer": restaura estado inicial — campos vazios, data original, botões iniciais      | Fluxo       |
| RN58 | Após salvar com sucesso: exibir Novo + Retornar; ocultar Salvar/Desfazer                        | UX          |

---

## 6. Queries ao Banco de Dados

| Query original (VB)                                                                     | Método VB              | Uso                                       |
|-----------------------------------------------------------------------------------------|------------------------|-------------------------------------------|
| `SELECT * FROM Contas WHERE Conta = c AND IdCliente = i` (ADODB)                       | `txtPasta_Validated`   | Verifica existência de pasta para cliente |
| `TableAdapterManager.UpdateAll(SoloDataSet)` (INSERT implícito via BindingSource)      | `btnRibSalvar_Click`   | Grava novo lançamento                     |

**Queries web (SQLAlchemy — nunca concatenação, nunca SELECT \*):**

```sql
-- RN51 — verificar pasta
SELECT COUNT(*) AS total
FROM Contas
WHERE Conta = :conta AND IdCliente = :id_cliente

-- RN53/RN54 — inserir lançamento
INSERT INTO Contas (IdCliente, CodCliente, Conta, Dt, Ref, VValor, DC, Deb, Cred)
OUTPUT INSERTED.Id
VALUES (:id_cliente, :cod_cliente, :conta, :dt, :ref, :vvalor, :dc, :deb, :cred)
```

---

## 7. Decisões Registradas

| ID  | Decisão                                                                                              | Fonte         |
|-----|------------------------------------------------------------------------------------------------------|---------------|
| D4  | Enter no campo Histórico: ADAPTAR como Enter = Tab (campo de linha única, consistente com os demais) | Decisão do cliente (2026-03-22) |

---

## 8. Auditoria — Classificação PRESERVAR / ADAPTAR / DESCARTAR

### Legenda
| Símbolo | Significado |
|---------|-------------|
| ✅ PRESERVAR | Comportamento idêntico ao original — obrigatório |
| 🔄 ADAPTAR | Comportamento equivalente com ajuste tecnológico — justificado |
| ❌ DESCARTAR | Comportamento não migrado — intencional e documentado |

### 8.1 Tabela de classificação

| ID  | Comportamento original                                                               | Classificação  | Justificativa                                                                                            |
|-----|--------------------------------------------------------------------------------------|----------------|----------------------------------------------------------------------------------------------------------|
| B01 | FrmLancaData como modal separado para seleção de data                               | 🔄 ADAPTAR     | RN49 — date picker integrado na tela `/lancamentos`; elimina salto de tela desnecessário                 |
| B02 | `FrmPrincipal.RadGridView1.Tag` como variável global para passar data               | 🔄 ADAPTAR     | RN50 — route state `{ id, codigo, cliente }` + data inicializada com hoje e editável pelo usuário        |
| B03 | `FrmLancaData.btnOk_Click` → `FrmPrincipal.Tag = data; FrmLanca.Show()`             | 🔄 ADAPTAR     | RN49/RN50 — fluxo de seleção de data dissolvido no próprio formulário                                   |
| B04 | `FrmLanca_Load` → campos hidden preenchidos com dados do cliente selecionado        | ✅ PRESERVAR   | RN50 — implementado via route state passado pela tela Principal                                          |
| B05 | `FrmLanca_Load` → `FrmPrincipal.Visible = False`                                    | ❌ DESCARTAR   | Padrão WinForms — React Router + navegação SPA substitui visibilidade de janelas                         |
| B06 | `FrmLanca_Load` → Salvar/Desfazer ocultos; Retornar visível                         | ✅ PRESERVAR   | RN55 — estado `dirty` controla visibilidade de botões                                                   |
| B07 | `txtPasta_Validated` → query ADODB + MsgBox "É uma nova pasta?" Y/N                | ✅ PRESERVAR   | RN51 — `GET /lancamentos/verificar-pasta` + modal de confirmação                                         |
| B08 | `txtPasta.Tag = "1"` (guard para evitar dupla validação)                            | ✅ PRESERVAR   | RN51 — flag `pastaConfirmada` evita re-disparar a validação na mesma pasta                               |
| B09 | `FrmLanca_KeyDown` → `SendKeys.Send("{Tab}")` para Enter em campos gerais           | 🔄 ADAPTAR     | RN55/D4 — `onKeyDown` JS `e.preventDefault()` + focus() no próximo campo                                |
| B10 | `FrmLanca_KeyDown` → `{BACKSPACE}` + `{Tab}` quando foco em txtRef                 | 🔄 ADAPTAR     | **D4 resolvido (2026-03-22):** Enter = Tab em todos os campos; backspace descartado                      |
| B11 | `txtDC_KeyPress` → só aceita 'D' ou 'C'; caso contrário `e.Handled = True`         | 🔄 ADAPTAR     | RN52 — `<select>` com opções fixas "D — Débito" / "C — Crédito"; validação pelo Zod/Pydantic            |
| B12 | `txtDC_KeyPress` → ao digitar D/C: Retornar hidden, Salvar/Desfazer visible        | ✅ PRESERVAR   | RN55 — qualquer `onChange` em qualquer campo ativa `dirty = true`                                        |
| B13 | `btnRibSalvar_Click` → valida Pasta, Ref, VValor, DC obrigatórios com MsgBox       | ✅ PRESERVAR   | RN54 — validação Zod no frontend + campos obrigatórios no schema Pydantic                                |
| B14 | `btnRibSalvar_Click` → se DC="D" Deb=VValor; se DC="C" Cred=VValor                | ✅ PRESERVAR   | RN53 — implementado no `services/lancamentos.py`                                                         |
| B15 | `TableAdapterManager.UpdateAll()` para salvar                                       | 🔄 ADAPTAR     | RN53/RN54 — `POST /lancamentos` com INSERT via SQLAlchemy                                               |
| B16 | Pós-salvo: Retornar visible, Novo visible, Salvar/Desfazer hidden                  | ✅ PRESERVAR   | RN58 — estado `saved` controla visibilidade                                                              |
| B17 | `btnRibUndo_Click` → recarrega dataset, refoca em Pasta                             | ✅ PRESERVAR   | RN57 — restaura estado inicial sem chamar API; data original preservada                                  |
| B18 | `btnNovo` visível após salvar (handler implícito — sem código explícito no VB)      | ✅ PRESERVAR   | RN56 — Novo limpa campos mantendo cliente e data; foca Pasta                                             |
| B19 | `ribRetornar_Click` → `Me.Close()` → `FrmLanca_Closed` → `FrmPrincipal.Visible=True`| 🔄 ADAPTAR    | Padrão WinForms — `navigate('/principal')` substitui Show/Hide de janelas                                |
| B20 | `dtpDt_Validated` → revela Salvar/Desfazer ao trocar a data                        | ✅ PRESERVAR   | RN55 — `onChange` no date picker ativa `dirty = true`                                                   |
| B21 | `txtRef_TextChanged` e `txtVValor_TextChanged` → revelam Salvar/Desfazer           | ✅ PRESERVAR   | RN55 — idem B20                                                                                          |
| B22 | `PerformMouseClick` / `mouse_event` P/Invoke                                        | ❌ DESCARTAR   | Artefato WinForms — simulação de clique de mouse não existe no contexto web                              |
| B23 | `Application.DoEvents()` (comentado no código)                                      | ❌ DESCARTAR   | Artefato WinForms — sem equivalente web                                                                   |
| B24 | `ContasBindingSource` / `ContasTableAdapter` / `SoloDataSet`                       | ❌ DESCARTAR   | Camada de dados WinForms — substituída por fetch REST + SQLAlchemy                                       |
| B25 | `FrmLancaData.btnCancelar_Click` → `Me.Close()`                                    | ❌ DESCARTAR   | FrmLancaData não existe na web; Cancelar/Retornar na tela de lançamento cobre o caso                     |
| B26 | `Highlighter1` — borda vermelha no campo com foco                                   | 🔄 ADAPTAR     | CSS `:focus` com `border-color: #dc2626` e `box-shadow`                                                  |
| B27 | `Office2010BlackTheme` / Telerik components                                         | ❌ DESCARTAR   | Componentes WinForms proprietários — design system do projeto web                                        |
| B28 | `C1Ribbon1` — estrutura de abas e botões ribbon                                     | ❌ DESCARTAR   | Componente WinForms — botões HTML simples com CSS                                                        |

### 8.2 Resumo

| Classificação | Quantidade | IDs                                                               |
|---------------|------------|-------------------------------------------------------------------|
| ✅ PRESERVAR  | 11         | B04, B06, B07, B08, B12, B13, B14, B16, B17, B18, B20, B21      |
| 🔄 ADAPTAR    | 9          | B01, B02, B03, B09, B10, B11, B15, B19, B26                      |
| ❌ DESCARTAR  | 8          | B05, B22, B23, B24, B25, B27, B28                                 |

---

## 9. Mapeamento Web

| Tela Web      | Form Original             | Documentação                      |
|---------------|---------------------------|-----------------------------------|
| `/lancamentos`| `FrmLancaData` + `FrmLanca` | `docs/FrmLanca_documentacao.md` |
