# Documentação Funcional — frmAlterar
**Sistema:** Solo Consultoria de Imóveis — Contas Correntes
**Form original:** `frmAlterar` (VB.Net / WinForms)
**URL web:** `/clientes/alterar/:id`
**Propósito:** Edição e exclusão do cliente selecionado no FrmPrincipal

---

## 1. Visão Geral

Tela de edição e exclusão de cliente, aberta a partir do FrmPrincipal (botão "Alterar")
com o cliente previamente selecionado. Os campos Código e Nome iniciam em modo somente
leitura; o botão "Alterar" habilita a edição. Alteração de Código exige três confirmações
consecutivas (adaptado para 1 modal de alto risco na web). Exclusão exige confirmação
inicial, senha de operação e, se houver lançamentos, confirmação adicional antes de
deletar os registros das tabelas `Contas` e `Clientes`.

> **Nota sobre discrepância:** O `escopo_migracao.md` original afirma que "Alteração
> apenas de Nome exige uma confirmação". O código-fonte real (`btnRibSalvar_Click`)
> **não** exibia nenhum `MsgBox` para alterações só de nome — salvava diretamente.
> **Decisão do produto:** adotar a confirmação simples (alinhado com escopo_migracao.md),
> documentada em RN37.

> **Nota sobre exclusão no legado:** A versão ativa do `btnExcluir_Click` apenas abre
> `frmSenha.ShowDialog()` e não executa o DELETE — o código de exclusão estava comentado
> (versão ADODB antiga). A web implementa o fluxo correto e completo.

---

## 2. Componentes Visuais

| Controle          | Tipo        | Label       | Descrição                                                                  |
|-------------------|-------------|-------------|----------------------------------------------------------------------------|
| `CódigoTextBox`   | TextBox     | "Código"    | ReadOnly=True inicial. Tag="1". Bound a `Clientes.Código`                  |
| `ClienteTextBox`  | TextBox     | "Nome"      | ReadOnly=True inicial. CharacterCasing=Upper. Bound a `Clientes.Cliente`   |
| `IdTextBox`       | TextBox     | —           | Oculto (ForeColor=White, BorderStyle=None). Bound a `Clientes.Id`          |
| `ribRetornar`     | RibbonButton| "Retornar"  | Fecha o form sem salvar                                                    |
| `btnRibSalvar`    | RibbonButton| "Salvar"    | Salva alterações (com confirmações conforme tipo de mudança)               |
| `btnAlterar`      | RibbonButton| "Alterar"   | Habilita edição dos dois campos + ativa highlight de foco                  |
| `btnRibUndo`      | RibbonButton| "Cancelar"  | Recarrega dados originais do banco e retorna ReadOnly                      |
| `btnExcluir`      | RibbonButton| "Excluir"   | Inicia fluxo de exclusão (confirmação → senha → exclusão)                  |
| `Highlighter1`    | DevComponents | —         | Borda vermelha em campo com foco (FocusHighlightColor=Red)                 |
| `C1PictureBox1`   | PictureBox  | —           | Logo/banner do sistema (404×35px)                                          |

> **Propriedades do form:** `KeyPreview=False` (sem Enter=Tab neste form),
> `FormBorderStyle=Fixed3D`, `StartPosition=CenterScreen`, `BackColor=White`,
> `Size=1012×691`, `ThemeName=Office2010Black` (Telerik).

---

## 3. Estrutura de Dados

### Tabela: `Clientes`

| Campo     | Tipo   | Uso no form                                       |
|-----------|--------|---------------------------------------------------|
| `Id`      | Int    | Identificador. Campo oculto. Filtro do BindingSource |
| `Código`  | Int    | Código do cliente. Editável após "Alterar"        |
| `Cliente` | String | Nome do cliente. Maiúsculas. Editável após "Alterar" |

### Tabela: `Contas`

| Campo       | Tipo | Uso no form                                              |
|-------------|------|----------------------------------------------------------|
| `IdCliente` | Int  | Usado para verificar existência de lançamentos e para DELETE |

### Tabela: `Chaves`

| Campo  | Tipo   | Uso no form                                                  |
|--------|--------|--------------------------------------------------------------|
| `Ref`  | String | Filtro: `'Exclusão de cliente'`                             |
| `Chave`| String | Senha validada contra o input do usuário na exclusão         |

---

## 4. Fluxo de Execução

### 4.1 Carregamento (`frmAlterar_Load`)

```
[Início — Id do cliente recebido via parâmetro de rota]
    │
    ├── 1. ClientesTableAdapter.Fill(SoloDataSet.Clientes)
    │      ClientesBindingSource.Filter = "Id = " & FrmPrincipal.RadGridView1.CurrentRow.Cells(0).Value
    │      → Web: GET /clientes/{id}
    │
    ├── 2. Campos CódigoTextBox e ClienteTextBox: ReadOnly = True
    │
    └── 3. FrmPrincipal.Visible = False
           → Web: DESCARTAR — navegação via React Router
```

### 4.2 Habilitar Edição (`btnAlterar_Click`)

```
[Clique em "Alterar"]
    │
    ├── 1. CódigoTextBox.ReadOnly = False
    ├── 2. ClienteTextBox.ReadOnly = False
    ├── 3. Highlighter1.SetHighlightOnFocus(CódigoTextBox, True)
    ├── 4. Highlighter1.SetHighlightOnFocus(ClienteTextBox, True)
    └── 5. CódigoTextBox.Focus()
```

### 4.3 Salvar (`btnRibSalvar_Click`)

```
[Clique em "Salvar"]
    │
    ├── Código foi alterado?
    │   ├── Sim:
    │   │   ├── MsgBox 1: "TEM CERTEZA QUE DESEJA ALTERAR O CÓDIGO DO CLIENTE {Nome}?"
    │   │   │   └── Não → Abort
    │   │   ├── MsgBox 2: "ATENÇÃO!!! DESEJA MESMO ALTERAR O CÓDIGO DO CLIENTE {Nome}?"
    │   │   │   └── Não → Abort
    │   │   ├── MsgBox 3: "TEM CERTEZA QUE DESEJA ALTERAR O CÓDIGO DO CLIENTE {Nome}?"
    │   │   │   └── Não → Abort
    │   │   └── Confirmar todos os 3: UpdateAll, Tag="1", ReadOnly=True
    │   │      → Web (RN36): 1 modal com aviso de alto risco substitui os 3 MsgBox
    │   │
    │   └── Não / Só nome foi alterado?
    │       ├── Sim (ElseIf):
    │       │   └── UpdateAll direto sem confirmação, Tag="1", ReadOnly=True
    │       │      → Web (RN37): 1 modal simples de confirmação (decisão do produto)
    │       └── Não (nada mudou): nenhuma ação
    │
    └── Pós-salvo (ambos os casos):
        Manipulação de foco para desativar Highlighter1
        → Web: DESCARTAR — artefato WinForms
```

### 4.4 Desfazer/Cancelar (`btnRibUndo_Click`)

```
[Clique em "Cancelar/Desfazer"]
    │
    ├── 1. ClientesTableAdapter.Fill() + Filter por Id
    │      → Web: GET /clientes/{id} (dados frescos do banco)
    │
    ├── 2. CódigoTextBox.ReadOnly = True
    ├── 3. ClienteTextBox.ReadOnly = True
    └── 4. Highlighter1 desativado (manipulação de foco — artefato WinForms)
           → Web: classe CSS .editing removida
```

### 4.5 Excluir (`btnExcluir_Click`)

```
[Clique em "Excluir"]
    │
    ├── 1. MsgBox "Confirma exclusão do cliente {Nome}?"
    │   └── Não → Abort
    │
    ├── 2. varSenha = "2" → frmSenha.ShowDialog()
    │   → Web: modal de senha → POST /clientes/{id}/verificar-senha
    │          (valida contra Chaves WHERE Ref='Exclusão de cliente')
    │          Resposta inclui: {valido, tem_lancamentos}
    │   └── Senha incorreta → MsgBox "Senha incorreta" → Abort
    │
    ├── 3. [Código ADODB comentado — mas lógica preservada na web]
    │   SELECT * FROM Contas WHERE IdCliente = {Id}
    │   └── tem_lancamentos?
    │       ├── Sim:
    │       │   ├── MsgBox "O cliente {Nome} tem lançamentos; Confirma exclusão do cliente e dos lançamentos?"
    │       │   │   └── Não → Abort
    │       │   └── DELETE FROM Contas WHERE IdCliente = {Id}
    │       │       DELETE FROM Clientes WHERE Id = {Id}
    │       └── Não:
    │           └── DELETE FROM Clientes WHERE Id = {Id}
    │
    ├── 4. Me.Tag = "1"
    └── 5. Me.Close()
           → Web: invalida cache ["clientes"] + navigate("/principal")
```

### 4.6 Fechamento (`frmAlterar_Closed`)

```
[Form fechado]
    │
    ├── Tag == "1"? → FrmPrincipal.ClientesTableAdapter.Fill() (recarrega grid)
    │      → Web: queryClient.invalidateQueries(["clientes"])
    │
    └── FrmPrincipal.Visible = True
           → Web: DESCARTAR — navegação via React Router
```

---

## 5. Regras de Negócio

| ID   | Regra                                                                                           | Tipo        |
|------|-------------------------------------------------------------------------------------------------|-------------|
| RN33 | Carregar dados do cliente pelo Id recebido via parâmetro de rota                                | Fluxo       |
| RN34 | Campos Código e Nome iniciam em modo somente leitura                                            | UX          |
| RN35 | Botão "Alterar" habilita edição dos dois campos com destaque de foco                            | UX          |
| RN36 | Alteração de Código exige 3 confirmações (ADAPTAR: 1 modal de alto risco na web)               | Segurança   |
| RN37 | Alteração de Nome exige 1 confirmação simples antes de salvar                                   | Integridade |
| RN38 | Após salvar com sucesso: campos voltam a ReadOnly=True, destaque removido                       | UX          |
| RN39 | Botão "Desfazer": recarregar dados originais do backend e retornar ReadOnly=True                | Fluxo       |
| RN40 | Exclusão passo 1: confirmação "Confirma exclusão do cliente {Nome}?"                            | Segurança   |
| RN41 | Exclusão passo 2: senha validada contra `Chaves WHERE Ref='Exclusão de cliente'`                | Segurança   |
| RN42 | Exclusão passo 3: verificar se cliente tem lançamentos em Contas                                | Integridade |
| RN43 | Se tem lançamentos: confirmação adicional antes de excluir                                      | Integridade |
| RN44 | Exclusão com lançamentos: DELETE Contas + DELETE Clientes em transação                          | Integridade |
| RN45 | Exclusão sem lançamentos: DELETE Clientes apenas                                                | Integridade |
| RN46 | Ao salvar ou excluir com sucesso: invalida cache e navega para /principal                       | Fluxo       |
| RN47 | Nome armazenado em maiúsculas (`CharacterCasing=Upper`)                                         | Dados       |
| RN48 | Código deve ser único ao alterar (excluindo o próprio Id) — melhoria vs. original              | Integridade |

---

## 6. Variáveis Globais Utilizadas

| Variável    | Uso no form                                              | Destino web                                   |
|-------------|----------------------------------------------------------|-----------------------------------------------|
| `varSenha`  | Contexto "2" — exclusão de cliente                       | Endpoint específico `POST /clientes/{id}/verificar-senha` |

---

## 7. Queries ao Banco de Dados

| Query                                                                           | Método                     | Uso                                        |
|---------------------------------------------------------------------------------|----------------------------|--------------------------------------------|
| `ClientesTableAdapter.Fill()` + `Filter = "Id = " & Id`                       | `frmAlterar_Load`           | Carrega dados do cliente pelo Id           |
| `TableAdapterManager.UpdateAll(SoloDataSet)`                                    | `btnRibSalvar_Click`        | Atualiza Código e/ou Cliente               |
| `ClientesTableAdapter.Fill()` + `Filter`                                        | `btnRibUndo_Click`          | Recarrega dados originais                  |
| `SELECT * FROM Contas WHERE IdCliente = {Id}` (ADODB comentado)                | `btnExcluir_Click`          | Verifica existência de lançamentos         |
| `DELETE FROM Contas WHERE IdCliente = {Id}` (ADODB comentado)                  | `btnExcluir_Click`          | Exclui lançamentos (se houver)             |
| `DELETE FROM Clientes WHERE Id = {Id}` (ADODB comentado)                       | `btnExcluir_Click`          | Exclui o cliente                           |
| `SELECT Chave FROM Chaves WHERE Ref = 'Exclusão de cliente'`                   | `frmSenha` (varSenha=2)     | Valida senha de exclusão (ADAPTAR de "4321") |

**Queries web (SQLAlchemy — nunca concatenação, nunca SELECT *):**

```sql
SELECT Id, Código, Cliente FROM Clientes WHERE Id = :id
SELECT COUNT(*) AS total FROM Contas WHERE IdCliente = :id
SELECT Chave FROM Chaves WHERE Ref = 'Exclusão de cliente'
SELECT Id FROM Clientes WHERE Código = :codigo AND Id <> :id
UPDATE Clientes SET Código = :codigo, Cliente = :cliente WHERE Id = :id
DELETE FROM Contas WHERE IdCliente = :id
DELETE FROM Clientes WHERE Id = :id
```

---

## 8. Auditoria — Classificação PRESERVAR / ADAPTAR / DESCARTAR

### Legenda

| Símbolo | Significado |
|---------|-------------|
| ✅ PRESERVAR | Comportamento idêntico ao original — obrigatório |
| 🔄 ADAPTAR | Comportamento equivalente com ajuste tecnológico — justificado |
| ❌ DESCARTAR | Comportamento não migrado — intencional e documentado |

### 8.1 Tabela de classificação por comportamento

| ID   | Comportamento original | Classificação | Justificativa |
|------|------------------------|---------------|---------------|
| B01  | `ClientesTableAdapter.Fill()` + `BindingSource.Filter` no Load | 🔄 ADAPTAR | Substituído por `GET /clientes/{id}` — dados carregados via API REST |
| B02  | `FrmPrincipal.Visible = False` no Load | ❌ DESCARTAR | Padrão WinForms — React Router substitui navegação entre janelas |
| B03  | `CódigoTextBox.ReadOnly = True` e `ClienteTextBox.ReadOnly = True` inicial | ✅ PRESERVAR | RN34 — proteção contra edição acidental |
| B04  | `btnAlterar_Click` → `ReadOnly = False` + Highlighter ativo | ✅ PRESERVAR | RN35 — habilitação de edição com feedback visual (CSS .editing) |
| B05  | Triple MsgBox para alteração de Código | 🔄 ADAPTAR | RN36 — 1 modal com aviso de alto risco; 3 cliques → 1 decisão informada |
| B06  | ElseIf (nome mudou) — salva sem MsgBox | 🔄 ADAPTAR | RN37 — decisão do produto: adicionar 1 modal simples (alinhado ao escopo_migracao.md) |
| B07  | Pós-salvo: `ReadOnly = True` + Tag="1" | ✅ PRESERVAR | RN38 — campo volta para modo leitura; Tag="1" → web: cache invalidado |
| B08  | Pós-salvo: manipulação de foco para desativar Highlighter | ❌ DESCARTAR | Artefato WinForms para forçar evento de perda de foco no componente Highlighter |
| B09  | `btnRibUndo_Click` → `Fill()` + `Filter` (recarrega banco) | ✅ PRESERVAR | RN39 — `GET /clientes/{id}` garante dados frescos do banco, não do estado local |
| B10  | `btnExcluir_Click` → confirmação inicial | ✅ PRESERVAR | RN40 — primeira barreira de segurança antes de iniciar o fluxo de exclusão |
| B11  | `varSenha = "2"` → `frmSenha.ShowDialog()` | 🔄 ADAPTAR | RN41 — `POST /clientes/{id}/verificar-senha` valida contra tabela Chaves |
| B12  | Senha "4321" hardcoded para varSenha=2 | 🔄 ADAPTAR | RN41 — Chaves `WHERE Ref='Exclusão de cliente'` (resolve D3 do escopo_migracao.md) |
| B13  | Código ADODB comentado: SELECT Contas + DELETE | 🔄 ADAPTAR | RN42-RN45 — lógica preservada; implementada no backend via SQLAlchemy |
| B14  | Confirmação adicional se tem lançamentos | ✅ PRESERVAR | RN43 — protege contra exclusão acidental de histórico |
| B15  | `DELETE FROM Contas` + `DELETE FROM Clientes` em sequência | ✅ PRESERVAR | RN44 — executados em transação no backend |
| B16  | `Me.Tag = "1"` + `Me.Close()` → recarrega FrmPrincipal | 🔄 ADAPTAR | RN46 — `queryClient.invalidateQueries(["clientes"])` + `navigate("/principal")` |
| B17  | `ClienteTextBox.CharacterCasing = Upper` | 🔄 ADAPTAR | RN47 — CSS `text-transform: uppercase` + `.upper()` no backend |
| B18  | `FrmPrincipal.Visible = True` ao fechar | ❌ DESCARTAR | Padrão WinForms — navigate("/principal") cobre o caso |
| B19  | `ClientesBindingNavigatorSaveItem_Click` (sem lógica) | ❌ DESCARTAR | Handler gerado pelo Designer sem função ativa |
| B20  | `FormBorderStyle=Fixed3D`, `StartPosition=CenterScreen` | ❌ DESCARTAR | Propriedades de janela WinForms — layout CSS |
| B21  | `ThemeName=Office2010Black` (Telerik) | ❌ DESCARTAR | Tema proprietário WinForms — design system do projeto web |
| B22  | Ribbon (C1Ribbon1) — botões e abas | ❌ DESCARTAR | Componente WinForms — botões HTML simples com CSS |
| B23  | `C1PictureBox1` — logo/banner | ❌ DESCARTAR | Componente WinForms — logo via CSS/img no layout da página |

### 8.2 Resumo

| Classificação | Quantidade | IDs |
|---------------|------------|-----|
| ✅ PRESERVAR  | 6          | B03, B04, B07, B10, B14, B15 |
| 🔄 ADAPTAR    | 10         | B01, B05, B06, B09, B11, B12, B13, B16, B17, B18 (nav) |
| ❌ DESCARTAR  | 7          | B02, B08, B18 (visible), B19, B20, B21, B22, B23 |

> B18 aparece duas vezes: `Visible=True` → DESCARTAR; retornar para tela principal → ADAPTAR via navigate.

---

## 9. Mapeamento Web

| Tela Web            | Form Original | Documentação                       |
|---------------------|---------------|------------------------------------|
| `/clientes/alterar` | frmAlterar    | docs/frmAlterar_documentacao.md    |

### Endpoints Backend

| Endpoint                               | Método | Equivalente VB.Net                     | RNs implementadas           |
|----------------------------------------|--------|----------------------------------------|-----------------------------|
| `GET /clientes/{id}`                   | GET    | `frmAlterar_Load` (Fill + Filter)      | RN33                        |
| `PUT /clientes/{id}`                   | PUT    | `btnRibSalvar_Click` (UpdateAll)       | RN36, RN37, RN38, RN47, RN48 |
| `POST /clientes/{id}/verificar-senha`  | POST   | `frmSenha.ShowDialog()` (varSenha=2)   | RN41, RN42                  |
| `DELETE /clientes/{id}`                | DELETE | Código ADODB comentado em btnExcluir   | RN43, RN44, RN45            |
