# Documentação Funcional — frmClienteNovo
**Sistema:** Solo Consultoria de Imóveis — Contas Correntes
**Form original:** `frmClienteNovo` (VB.Net / WinForms)
**URL web:** `/clientes/novo`
**Propósito:** Cadastro de novo cliente com geração automática de código

---

## 1. Visão Geral

Tela de cadastro de novo cliente acessada a partir do FrmPrincipal (botão "Novo Cliente"). Possui dois campos principais (Código e Nome) e um botão "Gerar código" que calcula automaticamente o primeiro código disponível a partir de 10.000. A validação de unicidade e limite máximo (20.000) é realizada ao sair do campo Código (evento `Validated`/onBlur). O nome do cliente é sempre armazenado em maiúsculas.

---

## 2. Componentes Visuais

| Controle          | Tipo       | Label          | Descrição                                                                 |
|-------------------|------------|----------------|---------------------------------------------------------------------------|
| `lbCliente`       | Label      | "NOVO CLIENTE" | Título centralizado, DarkRed, Segoe UI Semibold 12pt                      |
| `C1PictureBox1`   | PictureBox | —              | Logo/banner do sistema (404×35px)                                         |
| `CódigoLabel`     | Label      | "Código"       | Label do campo Código (Segoe UI 9.75pt, alinhado à direita)               |
| `CódigoTextBox`   | TextBox    | —              | Campo numérico, binding com Clientes.Código. Tag="1" inicial (guard)      |
| `btnCodigo`       | Button     | "Gerar código" | TabStop=False. Gera próximo código disponível >= 10.000                    |
| `ClienteLabel`    | Label      | "Nome"         | Label do campo Nome (Segoe UI 9.75pt, alinhado à direita)                 |
| `ClienteTextBox`  | TextBox    | —              | CharacterCasing=Upper. Binding com Clientes.Cliente. Largura 431px        |
| `IdTextBox`       | TextBox    | —              | Oculto (ForeColor=White, BorderStyle=None). Binding com Clientes.Id       |
| `Highlighter1`    | DevComponents | —           | Destaque vermelho em campo com foco (FocusHighlightColor=Red)             |
| `Timer1`          | Timer      | —              | Intervalo 1000ms, reseta CódigoTextBox.Tag para "0"                       |

> **Nota:** A Ribbon (C1Ribbon1) com botões Retornar/Salvar/Cancelar está inteiramente comentada no Designer. Os botões não possuem funcionalidade ativa.

> **Propriedades do form:** KeyPreview=True, FormBorderStyle=Fixed3D, StartPosition=CenterScreen, BackColor=White, Size=1012×691, ThemeName=Office2010Black (Telerik).

---

## 3. Estrutura de Dados

### Tabela: `Clientes` (banco SQL Server — dataset `SoloDataSet`)

| Campo     | Tipo    | Uso no form                                       |
|-----------|---------|---------------------------------------------------|
| `Id`      | Int     | Gerado automaticamente (MAX(Id)+1). Campo oculto  |
| `Código`  | Int     | Código do cliente. Validado por unicidade e limite |
| `Cliente` | String  | Nome do cliente. Armazenado em maiúsculas          |

---

## 4. Fluxo de Execução

### 4.1 Carregamento do Form (`frmClienteNovo_Load`)

```
[Início]
    │
    ├── 1. Try:
    │      ├── ClientesTableAdapter.Fill(SoloDataSet.Clientes) — carrega dataset
    │      └── ClientesBindingSource.AddNew() — prepara registro em branco
    │
    ├── 2. Catch: MsgBox(ex.Message)
    │
    └── 3. Finally: FrmPrincipal.Visible = False — oculta tela principal
```

### 4.2 Gerar Código (`btnCodigo_Click`)

```
[Clique em "Gerar código"]
    │
    ├── 1. SELECT Id FROM [Clientes] ORDER BY Id DESC
    │      └── IdTextBox.Text = primeiro Id + 1 (próximo Id disponível)
    │
    ├── 2. SELECT Código FROM [Clientes] WHERE Código >= 10000 ORDER BY Código
    │      └── Percorre de 10000 a 50000:
    │          └── Se i ≠ Código do registro atual → código encontrado (gap)
    │          └── Se i = Código → rs.MoveNext() → próximo
    │
    ├── 3. CódigoTextBox.Text = i (código encontrado)
    │
    └── 4. ClienteTextBox.Focus() — foco no campo Nome
```

### 4.3 Validação do Código (`CódigoTextBox_TextChanged` — handles `Validated`)

```
[Campo Código perde o foco (Validated/onBlur)]
    │
    ├── CódigoTextBox.Tag == "1"?
    │   └── Sim → Exit Sub (pula validação — guard ativo)
    │
    ├── CódigoTextBox.Text == ""?
    │   └── Sim → nenhuma ação
    │
    └── CódigoTextBox.Text preenchido:
            │
            ├── SELECT Id FROM [Clientes] WHERE Código = {valor}
            │
            ├── Registro encontrado (duplicado)?
            │   └── MsgBox "Código já existente" + foco no CódigoTextBox
            │
            └── Registro não encontrado:
                    │
                    ├── Código > 20000?
                    │   └── Sim → MsgBox "Tamanho máximo de Código: 20.000"
                    │         └── Limpa CódigoTextBox + foco nele
                    │
                    └── Não → validação OK (campo aceito)
```

### 4.4 Salvar (`btnRibSalvar_Click` — COMENTADO no original)

```
[Clique em "Salvar" — código inativo no original]
    │
    ├── Código vazio? → MsgBox "Por favor, informe o código do cliente" + foco
    ├── Cliente vazio? → MsgBox "Por favor, informe o nome do cliente" + foco
    │
    ├── SELECT Id FROM [Clientes] WHERE Código = {valor}
    │   └── Duplicado? → MsgBox "Código já existente" → Exit Sub
    │
    ├── CódigoTextBox.Tag = "1" (marca código como validado)
    ├── ClientesBindingSource.EndEdit() + TableAdapterManager.UpdateAll()
    ├── Tag = "1" (marca form como "dados salvos")
    └── Close()
```

### 4.5 Fechamento do Form (`frmClienteNovo_Closed`)

```
[Form fechado]
    │
    ├── Tag == "1"? → FrmPrincipal.ClientesTableAdapter.Fill() (recarrega grid)
    │
    └── FrmPrincipal.Visible = True (restaura tela principal)
```

### 4.6 Enter = Tab (`FrmLanca_KeyDown` — nome herdado do copy/paste, handles `Me.KeyDown`)

```
[Tecla pressionada — KeyPreview = True]
    │
    └── Enter? → SendKeys("{Tab}") — avança para próximo campo
```

---

## 5. Regras de Negócio

| ID   | Regra                                                                          | Tipo         |
|------|--------------------------------------------------------------------------------|--------------|
| RN21 | Código deve ser único na tabela Clientes                                       | Integridade  |
| RN22 | Código máximo: 20.000 — acima disso, limpar campo e focar nele                 | Validação    |
| RN23 | Geração automática: primeiro inteiro >= 10.000 não utilizado (gap na sequência)| Lógica       |
| RN24 | Campo "Cliente" (Nome) é obrigatório                                           | Integridade  |
| RN25 | Nome do cliente convertido para maiúsculas ao armazenar                        | Apresentação |
| RN26 | Campo "Código" é obrigatório                                                   | Integridade  |
| RN27 | Ao gerar código, obtém próximo Id (MAX(Id)+1) e foca campo Nome               | Fluxo        |
| RN28 | Validação de código disparada no evento Validated (equivalente web: onBlur)     | UX           |
| RN29 | Código duplicado: mensagem "Código já existente" + foco no campo Código        | UX           |
| RN30 | Código > 20.000: mensagem "Tamanho máximo de Código: 20.000" + limpa + foco    | UX           |
| RN31 | Enter = Tab — avança foco para próximo campo (KeyPreview = True)               | UX           |
| RN32 | Ao fechar com dados salvos, recarrega grid de clientes no FrmPrincipal         | Fluxo        |

---

## 6. Variáveis Globais Utilizadas

| Variável    | Uso no form                                              | Destino web         |
|-------------|----------------------------------------------------------|---------------------|
| (nenhuma)   | Form não utiliza variáveis globais diretamente           | —                   |

> O `frmClienteNovo` utiliza apenas variáveis locais e propriedades do form (`Me.Tag`, `CódigoTextBox.Tag`). O FrmPrincipal oculta a si mesmo (`Visible = False`) e restaura ao fechar. Na web, esse padrão é descartado — navegação via React Router.

---

## 7. Queries ao Banco de Dados

| Query                                                              | Método               | Uso                                      |
|--------------------------------------------------------------------|----------------------|------------------------------------------|
| `SELECT Id FROM [Clientes] ORDER BY Id DESC`                      | `btnCodigo_Click`    | Obtém último Id para calcular próximo    |
| `SELECT Código FROM [Clientes] WHERE Código >= 10000 ORDER BY Código` | `btnCodigo_Click` | Encontra primeiro gap na sequência       |
| `SELECT Id FROM [Clientes] WHERE Código = {valor}`                | `CódigoTextBox_TextChanged` | Valida unicidade do código        |
| `ClientesTableAdapter.Fill()`                                      | `Load`               | Carrega dataset para BindingSource       |

---

## 8. Auditoria — Classificação PRESERVAR / ADAPTAR / DESCARTAR

### Legenda

| Símbolo | Significado |
|---------|-------------|
| ✅ PRESERVAR | Comportamento idêntico ao original — obrigatório |
| 🔄 ADAPTAR | Comportamento equivalente com ajuste tecnológico — justificado |
| ❌ DESCARTAR | Comportamento não migrado — intencional e documentado |
| ⚠️ | Decisão pendente — requer investigação ou confirmação |

---

### 8.1 Tabela de classificação por comportamento

| ID   | Comportamento original | Classificação | Justificativa |
|------|------------------------|---------------|---------------|
| B01  | `ClientesTableAdapter.Fill()` no Load — carrega dataset | ❌ DESCARTAR | Artefato de data binding WinForms. Na web, formulário inicia em branco — sem necessidade de carregar todos os clientes. |
| B02  | `ClientesBindingSource.AddNew()` no Load — inicia registro novo no dataset | ❌ DESCARTAR | Padrão de BindingSource WinForms. Formulário web inicia com campos vazios nativamente. |
| B03  | `FrmPrincipal.Visible = False` (Finally do Load) | ❌ DESCARTAR | Padrão de navegação WinForms (ocultar janela pai). Na web → roteamento SPA. |
| B04  | RN23 — Geração automática de código via `btnCodigo_Click` (queries ADODB) | 🔄 ADAPTAR | Lógica de busca de gap movida integralmente para endpoint `GET /clientes/proximo-codigo` no backend. Frontend apenas chama a API. Melhoria de segurança. |
| B05  | RN27 — Próximo Id obtido por query `SELECT Id ... ORDER BY Id DESC` | 🔄 ADAPTAR | Backend calcula `MAX(Id)+1` no mesmo endpoint. Frontend não precisa conhecer o Id — gerado pelo banco no INSERT. |
| B06  | RN21/RN28/RN29 — Validação de unicidade no evento `Validated` (onBlur) | ✅ PRESERVAR | Comportamento preservado: validação via API no `onBlur` do campo Código. Mensagem "Código já existente" mantida. Foco retorna ao campo. |
| B07  | RN22/RN30 — Limite de código 20.000 com MsgBox, limpeza e foco | ✅ PRESERVAR | Validação client-side (campo/Zod) + server-side. Mensagem "Tamanho máximo de Código: 20.000" mantida. |
| B08  | RN24 — Campo Cliente obrigatório com MsgBox + foco | ✅ PRESERVAR | Mensagem "Por favor, informe o nome do cliente" preservada. Validação Zod com foco programático. |
| B09  | RN26 — Campo Código obrigatório com MsgBox + foco | ✅ PRESERVAR | Mensagem "Por favor, informe o código do cliente" preservada. Validação Zod com foco programático. |
| B10  | RN25 — `CharacterCasing = Upper` no ClienteTextBox | 🔄 ADAPTAR | CSS `text-transform: uppercase` no input para feedback visual + `.upper()` no backend ao salvar. |
| B11  | RN31 — Enter = Tab (`KeyPreview = True`, `SendKeys("{Tab}")`) | 🔄 ADAPTAR | `onKeyDown` intercepta Enter e move foco para próximo campo via `ref.focus()`. Mesmo padrão do frmLogin B15. |
| B12  | RN32 — `Tag = "1"` ao salvar → recarrega grid do FrmPrincipal ao fechar | 🔄 ADAPTAR | Após POST bem-sucedido, `queryClient.invalidateQueries(["clientes"])` invalida cache React Query. Grid recarrega ao voltar para `/principal`. |
| B13  | `FrmPrincipal.Visible = True` ao fechar o form | ❌ DESCARTAR | Navegação WinForms → SPA: `navigate("/principal")` após salvar ou cancelar. |
| B14  | `CódigoTextBox.Tag = "1"` — guard para pular validação durante preenchimento automático | ❌ DESCARTAR | Workaround de timing WinForms. Na web, validação onBlur só dispara após interação do usuário. |
| B15  | Timer1 (1000ms) — reseta `CódigoTextBox.Tag` para "0" | ❌ DESCARTAR | Complemento do guard B14. Desnecessário na web. |
| B16  | Highlighter1 — destaque vermelho em campo com foco (`FocusHighlightColor = Red`) | ⚠️ 🔄 ADAPTAR | Mesmo item pendente do frmLogin (B16). Aguarda definição do design system. |
| B17  | C1Ribbon1 (Retornar/Salvar/Cancelar) — inteiramente comentada no Designer | ❌ DESCARTAR | Código inativo no original. Botões de ação recriados como botões HTML simples. |
| B18  | `FormBorderStyle = Fixed3D`, `StartPosition = CenterScreen` | ❌ DESCARTAR | Propriedades de janela WinForms. Layout CSS. |
| B19  | `ThemeName = Office2010Black` (Telerik) | ❌ DESCARTAR | Tema proprietário. Design system do projeto web. |
| B20  | `btnRibSalvar_Click` e `btnRibUndo_Click` — blocos de código comentados | ❌ DESCARTAR | Código inativo. RNs extraídas e reimplementadas nos endpoints FastAPI e formulário React. |
| B21  | `ClienteTextBox_TextChanged` — alterava visibilidade de botões (comentado) | ❌ DESCARTAR | Código comentado, sem função ativa. |
| B22  | `ClientesBindingNavigatorSaveItem_Click` — salva via BindingSource | ❌ DESCARTAR | Método gerado pelo Designer sem lógica de negócio. Save via POST à API. |

---

### 8.2 Decisões pendentes

| Ref  | Descrição | Responsável | Status |
|------|-----------|-------------|--------|
| B16  | Definir estilo de foco no design system (vermelho vs. primária) | Designer / PO | Aberto (mesma pendência do frmLogin B16) |

---

### 8.3 Resumo

| Classificação | Quantidade | IDs |
|---------------|------------|-----|
| ✅ PRESERVAR  | 4          | B06, B07, B08, B09 |
| 🔄 ADAPTAR    | 5          | B04, B05, B10, B11, B12 |
| ❌ DESCARTAR  | 12         | B01, B02, B03, B13, B14, B15, B17, B18, B19, B20, B21, B22 |
| ⚠️ PENDENTE   | 1          | B16 |

---

## 9. Mapeamento Web

| Tela Web         | Form Original    | Documentação                          |
|------------------|------------------|---------------------------------------|
| `/clientes/novo` | frmClienteNovo   | docs/frmClienteNovo_documentacao.md   |

### Endpoints Backend

| Endpoint                        | Método | Equivalente VB.Net           | RNs implementadas            |
|---------------------------------|--------|------------------------------|------------------------------|
| `GET /clientes/proximo-codigo`  | GET    | `btnCodigo_Click`            | RN23, RN27                   |
| `POST /clientes`                | POST   | `btnRibSalvar_Click`         | RN21, RN22, RN24, RN25, RN26 |
