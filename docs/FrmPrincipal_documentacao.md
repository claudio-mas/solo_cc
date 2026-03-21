# Documentação Funcional — FrmPrincipal
**Sistema:** Solo Consultoria de Imóveis — Contas Correntes
**Form original:** `FrmPrincipal` (VB.Net / WinForms)
**Propósito:** Tela principal do sistema — hub de navegação e consulta de clientes

---

## 1. Visão Geral

Tela exibida após login bem-sucedido. Apresenta a lista completa de clientes em um grid com pesquisa por prefixo e disponibiliza ações via toolbar ribbon: lançamentos, extrato/conta corrente, alteração de cadastro, novo cliente, totais, relatórios, gestão de usuários e logout. O código do cliente selecionado é exibido dinamicamente no header da seção de ações.

---

## 2. Componentes Visuais

| Controle         | Tipo              | Label               | Descrição                                                                     |
|------------------|-------------------|----------------------|-------------------------------------------------------------------------------|
| `RadGridView1`   | Telerik RadGridView | —                  | Grid de clientes — ReadOnly, colunas: Id (oculta), Código, Cliente. Filtro habilitado, agrupamento desabilitado. 698×470px |
| `C1Ribbon1`      | C1 Ribbon         | —                    | Toolbar com botões de ação organizados em grupos (parcialmente desabilitada)  |
| `RibbonGroup6`   | Ribbon Group      | "CÓD. {código}"     | Header dinâmico que exibe o código do cliente selecionado                     |
| `txtPesq`        | TextBox           | —                    | Campo de pesquisa — busca por prefixo (nome ou código). TabStop=False         |
| `Label1`         | Label             | "Localizar cliente:" | Label do campo de pesquisa (DarkRed, Bold, Segoe UI 9pt)                     |
| `lblUsuario`     | Label             | —                    | Barra inferior — exibe "USUÁRIO: {nome}" em maiúsculas. BorderStyle=Fixed3D, BackColor=Gainsboro |
| `C1PictureBox1`  | PictureBox        | —                    | Logo/banner do sistema (404×35px)                                             |
| `Timer1`         | Timer             | —                    | Intervalo 500ms, dispara uma vez para focar txtPesq após Load                |
| `btnLançar`      | RibbonButton      | "Lançamentos"        | Abre FrmLancaData (modal) ou FrmLanca (conforme Tag do grid)                 |
| `btnExtrato`     | RibbonButton      | "Conta corrente"     | Abre FrmExtrato                                                               |
| `btnAlterar`     | RibbonButton      | "Alterar"            | Abre frmAlterar                                                               |
| `btnNovoCliente` | RibbonButton      | "Novo Cliente"       | Abre frmClienteNovo                                                           |
| `btnTotais`      | RibbonButton      | "Totais"             | Abre frmTotais2 (modal)                                                       |
| `btnRpt`         | RibbonButton      | "Relatórios"         | Abre frmReports                                                               |
| `btnBackup`      | RibbonButton      | "Backup"             | Executa backup do banco — **Enabled=False** no Designer                       |
| `btnUsuarios`    | RibbonButton      | "Usuários e Senhas"  | Abre frmSenha (modal) após setar flag varSenha="1"                           |
| `ribRetornar`    | RibbonButton      | "Sair"               | Encerra a aplicação (`Application.Exit()`)                                    |

> **Nota:** O `C1Ribbon1` é comentado/desabilitado parcialmente no `InitializeComponent`. Os elementos de ribbon são declarados mas a ribbon em si está inativa em favor de um layout mais simples. Todos os botões ribbon possuem LargeImage e SmallImage no `.resx`.

> **Nota:** O `btnBackup` está com `Enabled = False` no Designer — funcionalidade desativada no sistema original.

---

## 3. Estrutura de Dados

### Tabela: `Clientes` (banco SQL Server — dataset `SoloDataSet`)

| Campo       | Tipo    | Uso no form                                              |
|-------------|---------|----------------------------------------------------------|
| `Id`        | Int     | Coluna oculta no grid — identificador interno            |
| `Código`    | Int     | Exibido no grid (coluna 1) e no header dinâmico          |
| `Cliente`   | String  | Exibido no grid (coluna 2) — nome do cliente             |
| `CodCliente`| Int     | Referenciado na CTE de integridade (Load)                |

### Tabela: `Contas` (referenciada na CTE do Load e em código comentado)

| Campo       | Tipo    | Uso no form                                              |
|-------------|---------|----------------------------------------------------------|
| `IdCliente` | Int     | JOIN com Clientes.Id na CTE de integridade               |
| `CodCliente`| Int     | Atualizado pela CTE para sincronizar com Clientes.Código |
| `Dt`        | DateTime| Filtro na CTE: `>= '2016-01-01'`                        |
| `Deb`       | Decimal | Referenciado no cálculo de saldo (código comentado)      |
| `Cred`      | Decimal | Referenciado no cálculo de saldo (código comentado)      |
| `Saldo`     | Decimal | Atualizado no cálculo de saldo (código comentado)        |

> ⚠️ A CTE de integridade e o cálculo de saldo existem apenas em blocos comentados ou descartados. As colunas são listadas para referência.

---

## 4. Fluxo de Execução

### 4.1 Carregamento do Form (`FrmPrincipal_Load`)

```
[Início]
    │
    ├── 1. Executa CTE de integridade via fExecutaText()
    │      └── UPDATE sincroniza Clientes.CodCliente com Clientes.Código
    │          para registros com Contas.Dt >= '2016-01-01'
    │          (⚠️ DESCARTADO na migração — ver seção 8)
    │
    ├── 2. ClientesTableAdapter.Fill(SoloDataSet.Clientes)
    │      └── Carrega todos os clientes no grid
    │
    ├── 3. LINQ via ContasDataContext:
    │      └── Obtém o Código do primeiro cliente (ordem alfabética)
    │      └── Atualiza RibbonGroup6.Text = "CÓD. {código}"
    │
    └── 4. Timer1 dispara após 500ms → txtPesq.Focus()
```

### 4.2 Pesquisa de cliente (`txtPesq_TextChanged`)

```
[Texto digitado em txtPesq]
    │
    └── Para cada linha do RadGridView1:
            │
            ├── Compara prefixo do nome (coluna 2) — case-insensitive
            │   UCase(Left(Cliente, Len(txtPesq))) == UCase(txtPesq)
            │
            ├── OU compara prefixo do código (coluna 1) — case-sensitive
            │   Left(Código, Len(txtPesq)) == txtPesq
            │
            ├── Match encontrado?
            │   └── Sim → ClientesBindingSource.Position = índice da linha → Exit For
            │
            └── Não → continua para próxima linha
```

### 4.3 Seleção de cliente (`ClientesBindingSource_CurrentChanged` / `RadGridView1_Click`)

```
[Seleção muda ou grid clicado]
    │
    ├── RadGridView1.CurrentRow não é Nothing?
    │   └── Sim → RibbonGroup6.Text = "CÓD. " & CurrentRow.Cells(1).Value
    │   └── Não → RibbonGroup6.Text = "CÓD."
    │
    └── On Error Resume Next (suprime erros genéricos)
```

### 4.4 Ações dos botões

#### 4.4.1 Lançamentos (`btnLançar_Click`)

```
[Clique em "Lançamentos"]
    │
    ├── RadGridView1.Tag == "" ?
    │   └── Sim → FrmLancaData.ShowDialog() (modal)
    │   └── Não → FrmLanca.Show() (modeless)
    │
    └── ⚠️ Origem da atribuição do Tag desconhecida neste form
```

#### 4.4.2 Conta corrente (`btnExtrato_Click`)

```
[Clique em "Conta corrente"]
    │
    └── FrmExtrato.Show()
        (código comentado com cálculo de saldo CTE/ADODB — não executado)
```

#### 4.4.3 Alterar (`btnAlterar_Click`)

```
[Clique em "Alterar"]
    │
    └── frmAlterar.Show()
```

#### 4.4.4 Novo Cliente (`btnNovoCliente_Click`)

```
[Clique em "Novo Cliente"]
    │
    └── frmClienteNovo.Show()
```

#### 4.4.5 Totais (`btnTotais_Click`)

```
[Clique em "Totais"]
    │
    └── frmTotais2.ShowDialog() (modal)
```

#### 4.4.6 Relatórios (`btnRpt_Click`)

```
[Clique em "Relatórios"]
    │
    └── frmReports.Show()
```

#### 4.4.7 Backup (`btnBackup_Click`) — DESATIVADO

```
[Clique em "Backup"] (botão desabilitado)
    │
    ├── Try:
    │   ├── Monta caminho: C:\CONTAS CORRENTES\Backup\Backup{ddMMyyyy}_{HHmm}.BAK
    │   ├── fExecutaText("BACKUP DATABASE Solo TO DISK = '...' WITH COPY_ONLY")
    │   └── MsgBox "Backup realizado com sucesso!" + caminho
    │
    └── Catch: MsgBox(ex.Message)
```

#### 4.4.8 Usuários e Senhas (`btnUsuarios_Click`)

```
[Clique em "Usuários e Senhas"]
    │
    ├── varSenha = "1" (flag global de contexto)
    │
    └── frmSenha.ShowDialog() (modal)
        (código comentado com DLookup em qpst_Chaves + XOR — não executado)
```

#### 4.4.9 Sair (`ribRetornar_Click`)

```
[Clique em "Sair"]
    │
    └── Application.Exit()
        (código comentado com GetPassword — não executado)
```

### 4.5 Double-click no grid (`RadGridView1_DoubleClick`)

```
[Double-click em linha do grid]
    │
    └── FrmExtrato.Show() (mesmo comportamento do btnExtrato)
```

### 4.6 Timer (`Timer1_Tick`)

```
[Timer dispara após 500ms]
    │
    ├── txtPesq.Focus()
    └── Timer1.Enabled = False (desativa-se)
```

---

## 5. Regras de Negócio

| ID   | Regra                                                                                  | Tipo         |
|------|----------------------------------------------------------------------------------------|--------------|
| RN09 | Grid de clientes é carregado do banco na abertura, ordenado por nome do cliente         | Dados        |
| RN10 | O código do primeiro cliente (ordem alfabética) é exibido no header da seção de ações   | Apresentação |
| RN11 | Ao selecionar um cliente no grid, o header atualiza para "CÓD. {código}"               | Apresentação |
| RN12 | Campo de pesquisa filtra clientes por prefixo do nome (case-insensitive) ou do código   | UX           |
| RN13 | A pesquisa posiciona no primeiro resultado encontrado (não filtra os demais)             | UX           |
| RN14 | Foco automático no campo de pesquisa após carregamento                                  | UX           |
| RN15 | Double-click no grid abre o extrato do cliente selecionado                              | Fluxo        |
| RN16 | Botão "Lançamentos" abre form de lançamento (condicional — investigação pendente)       | Fluxo        |
| RN17 | Botão "Conta corrente" abre o extrato do cliente selecionado                            | Fluxo        |
| RN18 | Botão "Usuários e Senhas" exige autenticação via diálogo de senha antes de abrir gestão | Autorização  |
| RN19 | Botão "Sair" encerra a sessão e redireciona para login                                  | Fluxo        |
| RN20 | Nome do usuário logado exibido em maiúsculas na barra inferior ("USUÁRIO: {nome}")      | Apresentação |

> **RNs descartadas:** A CTE de integridade de dados (sincronização CodCliente) e o botão Backup foram descartados por decisão do cliente em 2026-03-21. Ver seção 8 para justificativas completas.

---

## 6. Variáveis Globais Utilizadas

| Variável    | Origem         | Destino          | Conteúdo                                              |
|-------------|----------------|------------------|-------------------------------------------------------|
| `varSenha`  | `btnUsuarios`  | `frmSenha`       | Flag "1" — indica contexto de abertura do form senha  |
| `varPerfil` | Login (herdada)| Escopo global    | Perfil do usuário autenticado — controla permissões   |
| `varUsu`    | Login (herdada)| `lblUsuario`     | Nome do usuário logado (exibido em maiúsculas)        |

> Na migração web, essas variáveis globais são substituídas por **JWT Claims** (`perfil`, `usuario`) armazenados em `localStorage` e gerenciados via estado React.

---

## 7. Observações para a Migração

- ⚠️ **CTE de integridade no Load** — executa UPDATE direto no banco a cada abertura do form. **DESCARTADO** por decisão do cliente (2026-03-21): inconsistência já corrigida na base.
- ⚠️ **btnBackup desabilitado** — backup de banco em disco local não se aplica a web. **DESCARTADO** por decisão do cliente (2026-03-21): backup gerenciado pela infra do servidor.
- ⚠️ **RadGridView1.Tag** — usado como flag de estado para decidir qual form de lançamento abrir. Origem da atribuição não está no FrmPrincipal — será investigada quando os demais forms legados estiverem disponíveis.
- ⚠️ **varSenha = "1"** — flag global para indicar contexto ao frmSenha. Equivalente web: estado do componente, parâmetro de rota ou modal com prop.
- ⚠️ **Código comentado no btnExtrato** — cálculo de saldo com CTE/ADODB (Window Function). Verificar se essa lógica foi movida para FrmExtrato ou se precisa de endpoint dedicado quando o form de extrato for migrado.
- ⚠️ **Código comentado no btnUsuarios** — verificação de senha via `DLookup` na tabela `qpst_Chaves` com criptografia XOR (`dhXORText`). Substituído por `frmSenha.ShowDialog()`.
- ⚠️ **`On Error Resume Next`** presente em 3 handlers (`ClientesBindingSource_CurrentChanged`, `btnExtrato_Click`, `RadGridView1_Click`). Substituir por tratamento de erros adequado.
- O C1Ribbon (ComponentOne) e o RadGridView (Telerik) são controles licenciados de terceiros. Na web, serão substituídos por componentes React equivalentes (toolbar + tabela HTML com filtro).
- Todos os botões ribbon possuem ícones (LargeImage/SmallImage no `.resx`). Substituir por ícones SVG ou biblioteca de ícones na web.
- O grid possui `EnableFiltering = True` mas `EnableGrouping = False` — manter filtro, não adicionar agrupamento.

### Dependências de forms (navegação)

| Form destino     | Ação                    | Tipo (VB.Net) | Equivalente Web                |
|------------------|-------------------------|---------------|--------------------------------|
| FrmLancaData     | btnLançar (Tag="")      | Modal         | Rota ou modal                  |
| FrmLanca         | btnLançar (Tag≠"")      | Modeless      | Rota                           |
| FrmExtrato       | btnExtrato / DoubleClick| Modeless      | `/extrato/:codCliente`         |
| frmAlterar       | btnAlterar              | Modeless      | `/clientes/:id/editar`         |
| frmClienteNovo   | btnNovoCliente          | Modeless      | `/clientes/novo`               |
| frmTotais2       | btnTotais               | Modal         | Modal ou `/totais`             |
| frmReports       | btnRpt                  | Modeless      | `/relatorios`                  |
| frmSenha         | btnUsuarios             | Modal         | Modal de confirmação de senha  |

---

## 8. Auditoria Retroativa — Classificação PRESERVAR / ADAPTAR / DESCARTAR

> Auditoria realizada em 2026-03-21. Objetivo: classificar cada comportamento do
> form original e registrar a decisão de migração com justificativa.
> Itens marcados com ⚠️ identificam decisões pendentes que requerem investigação.

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
| B01  | CTE de integridade no Load — UPDATE sincroniza CodCliente com Código | ❌ DESCARTAR | Inconsistência corrigida na base. Decisão confirmada pelo cliente em 2026-03-21. |
| B02  | Fill clientes ordenado por nome (`ClientesTableAdapter.Fill` + `BindingSource.Sort = "Cliente"`) | ✅ PRESERVAR | Endpoint GET /clientes com `ORDER BY Cliente`. RN09 preservada. |
| B03  | LINQ para obter código do primeiro cliente (ordem alfabética) | 🔄 ADAPTAR | Incluir na resposta do endpoint de listagem ou calcular client-side a partir da lista ordenada. |
| B04  | Header "CÓD. {código}" atualizado dinamicamente ao selecionar cliente | 🔄 ADAPTAR | Estado local React — atualizado ao selecionar linha na tabela. |
| B05  | Pesquisa por prefixo do nome (case-insensitive) ou código | ✅ PRESERVAR | Filtro client-side com lógica equivalente. RN12/RN13. |
| B06  | Timer 500ms para focar campo de pesquisa | 🔄 ADAPTAR | `useEffect` com `autoFocus` ou `ref.current.focus()` no React. Sem necessidade de timer. |
| B07  | Double-click no grid abre FrmExtrato | ✅ PRESERVAR | `onDoubleClick` ou `onClick` na linha da tabela → navega para `/extrato/:codCliente`. |
| B08  | btnLançar condicional — Tag="" → FrmLancaData (modal), Tag≠"" → FrmLanca | ⚠️ PENDENTE | Origem da atribuição do Tag desconhecida. Será investigada quando todos os forms legados estiverem disponíveis. Documentar ambos os caminhos. |
| B09  | btnExtrato → FrmExtrato.Show() | ✅ PRESERVAR | Navegação para `/extrato/:codCliente`. |
| B10  | btnUsuarios → varSenha="1" + frmSenha.ShowDialog() | 🔄 ADAPTAR | Modal de confirmação de senha ou guard de rota. Flag global substituída por estado do componente. |
| B11  | btnBackup — BACKUP DATABASE para disco local (desabilitado) | ❌ DESCARTAR | Não aplicável a ambiente web/VPS. Backup gerenciado pela infra do servidor. Decisão confirmada pelo cliente em 2026-03-21. |
| B12  | btnNovoCliente → frmClienteNovo.Show() | ✅ PRESERVAR | Navegação para `/clientes/novo`. |
| B13  | btnTotais → frmTotais2.ShowDialog() (modal) | ✅ PRESERVAR | Modal ou navegação para `/totais`. |
| B14  | btnRpt → frmReports.Show() | ✅ PRESERVAR | Navegação para `/relatorios`. |
| B15  | btnAlterar → frmAlterar.Show() | ✅ PRESERVAR | Navegação para `/clientes/:id/editar`. |
| B16  | ribRetornar → Application.Exit() | 🔄 ADAPTAR | Logout: limpar JWT/localStorage + redirect para `/login`. `Application.Exit()` não tem equivalente direto em SPA. |
| B17  | lblUsuario exibe "USUÁRIO: {nome}" em maiúsculas | ✅ PRESERVAR | Já implementado no login (RN06). O frontend deve exibir `"USUÁRIO: " + usuario` na barra inferior. RN20. |
| B18  | `On Error Resume Next` em 3 handlers | ❌ DESCARTAR | Supressão genérica de erros. Substituir por try/catch ou validação condicional no React. |
| B19  | Código comentado — cálculo de saldo CTE/ADODB no btnExtrato | ❌ DESCARTAR | Código inativo no original. Documentado como referência histórica para quando FrmExtrato for migrado. |
| B20  | Código comentado — DLookup + XOR no btnUsuarios | ❌ DESCARTAR | Mecanismo de senha via tabela `qpst_Chaves` com criptografia XOR. Substituído por frmSenha.ShowDialog(). |
| B21  | Código comentado — GetPassword no ribRetornar | ❌ DESCARTAR | Prompt de senha ao sair — desativado no original. Sem necessidade na web. |
| B22  | C1Ribbon como toolbar de navegação | 🔄 ADAPTAR | Substituir por sidebar ou toolbar web com botões/ícones. Manter mesmas ações e agrupamentos lógicos. |
| B23  | RadGridView com filtro e ordenação | 🔄 ADAPTAR | Tabela HTML/componente React com filtro client-side. Manter ReadOnly, colunas Código e Cliente, Id oculto. |
| B24  | Logo/banner C1PictureBox1 (404×35px) | 🔄 ADAPTAR | `<img>` com asset SVG/PNG. Dimensões ajustadas ao layout responsivo. |
| B25  | `FormBorderStyle = Fixed3D`, `StartPosition = CenterScreen` | ❌ DESCARTAR | Propriedades de janela WinForms sem equivalente em SPA. Layout gerenciado pelo CSS. |
| B26  | `ThemeName = Office2010Black` (Telerik) | ❌ DESCARTAR | Tema proprietário Telerik. Substituído pelo design system do projeto web. |
| B27  | Código comentado — btnLançar.Text e btnAlterar.Text dinâmicos no CurrentChanged | ❌ DESCARTAR | Textos dinâmicos nos botões desativados no original. Header "CÓD." já cumpre essa função. |

---

### 8.2 Decisões pendentes que requerem atenção

#### ⚠️ B08 — RadGridView1.Tag como flag condicional

**Original (VB.Net) — linha 58 do `FrmPrincipal.vb`:**
```vb
If Me.RadGridView1.Tag = "" Then
    FrmLancaData.ShowDialog()
Else
    FrmLanca.Show()
End If
```

O `Tag` do grid é usado como flag para decidir entre dois forms de lançamento
diferentes. A atribuição desse Tag **não existe** dentro do FrmPrincipal —
provavelmente é setada por outro form (FrmLanca, FrmExtrato, etc.).

**Decisão:** ⚠️ PENDENTE — será investigada quando todos os forms legados
estiverem disponíveis para análise. Por ora, documentar ambos os caminhos de
navegação e implementar o caminho padrão (Tag vazio → FrmLancaData).

---

### 8.3 Resumo de pendências

| Ref  | Descrição | Responsável | Status |
|------|-----------|-------------|--------|
| B08  | Investigar origem do RadGridView1.Tag — afeta qual form de lançamento é aberto | Dev (análise de forms legados) | Aberto |
| B19  | Cálculo de saldo (CTE/ADODB) — verificar se FrmExtrato contém lógica equivalente | Dev (migração FrmExtrato) | Aberto |
| —    | Definir design system (toolbar, tabela, ícones) antes de implementar o frontend | Designer / PO | Aberto |
| —    | Confirmar comportamento de "Sair" na web (logout + redirect vs. window.close) | Cliente / PO | Aberto |
