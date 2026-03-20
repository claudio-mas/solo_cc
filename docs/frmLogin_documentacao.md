# Documentação Funcional — frmLogin
**Sistema:** Solo Consultoria de Imóveis — Contas Correntes  
**Form original:** `frmLogin` (VB.Net / WinForms)  
**Propósito:** Autenticação de usuários para acesso ao sistema

---

## 1. Visão Geral

Tela de login exibida na inicialização do sistema. Permite que o usuário selecione seu nome em uma lista e informe sua senha para autenticação. Em caso de sucesso, redireciona para a tela principal (`FrmPrincipal`) com o nome do usuário exibido.

---

## 2. Componentes Visuais

| Controle        | Tipo       | Label           | Descrição                                              |
|-----------------|------------|-----------------|--------------------------------------------------------|
| `PictureBox2`   | Imagem     | —               | Logo do sistema (96×96px, canto superior esquerdo)     |
| `Label2`        | Texto      | —               | Título "Controle de Acesso" (DarkRed, fonte 14pt)      |
| `Label1`        | Texto      | "Usuário"       | Label do campo de usuário                              |
| `cboUsuario`    | ComboBox   | —               | Lista de usuários carregada do banco, com autocomplete |
| `Label3`        | Texto      | "Senha"         | Label do campo de senha                                |
| `txtSenha`      | TextBox    | —               | Campo de senha mascarado (`*`)                         |
| `btnOk`         | Botão      | "Ok"            | Dispara a autenticação                                 |
| `btnCancelar`   | Botão      | "Cancelar"      | Encerra a aplicação                                    |

> **Nota:** Existe um `Button2` declarado no Designer, porém com `Visible = False` e sem evento associado. Não possui função ativa — ignorar na migração.

---

## 3. Estrutura de Dados

### Tabela: `Usuários` (banco SQL Server — dataset `SoloDataSet`)

| Campo     | Tipo    | Uso no form                          |
|-----------|---------|--------------------------------------|
| `Id`      | Int     | Contagem para validar login (`Count`)|
| `Usuário` | String  | Exibido no ComboBox (`DisplayMember`)|
| `Psw`     | String  | Comparado com senha digitada         |
| `Perfil`  | String  | Recuperado após login bem-sucedido   |

> ⚠️ **Atenção de segurança:** A senha é armazenada e comparada em **texto plano** no sistema original. A migração deve implementar **hashing** (ex: bcrypt) e adaptar o fluxo de verificação.

---

## 4. Fluxo de Execução

### 4.1 Carregamento do Form (`frmLogin_Load`)
1. Executa `UsuáriosTableAdapter2.Fill(SoloDataSet.Usuários)` — popula o dataset com todos os usuários do banco.
2. Limpa o campo `cboUsuario` (`Text = ""`).
3. Em caso de exceção, exibe `MsgBox` com a mensagem do erro.

### 4.2 Clique em "Ok" (`btnOk_Click`)

```
[Início]
    │
    ├── cboUsuario vazio?
    │       └── Sim → MsgBox "Por favor, informe o usuário" → foco em cboUsuario → [Fim]
    │
    ├── txtSenha vazio?
    │       └── Sim → MsgBox "Por favor, informe a senha" → foco em txtSenha → [Fim]
    │
    └── Ambos preenchidos:
            │
            ├── Consulta banco: Usuários WHERE Usuário = varUsu AND Psw = varSenha
            │
            ├── Count == 0?
            │       └── Sim → MsgBox "Senha incorreta" → limpa txtSenha → foco em txtSenha → [Fim]
            │
            └── Count > 0:
                    ├── Recupera campo `Perfil` do primeiro registro encontrado → salva em `varPerfil`
                    ├── Oculta frmLogin (`Me.Visible = False`)
                    ├── Exibe FrmPrincipal (`FrmPrincipal.Show()`)
                    └── Define `FrmPrincipal.lblUsuario.Text = "USUÁRIO: " + UCase(cboUsuario.Text)`
```

### 4.3 Clique em "Cancelar" (`btnCancelar_Click`)
- Chama `Application.Exit()` — encerra o sistema imediatamente.

### 4.4 Tecla Enter (`frmLogin_KeyDown`)
- A tecla `Enter` é interceptada globalmente no form (`KeyPreview = True`).
- Comportamento: `Enter` equivale a `Tab` (avança para o próximo campo).
- Na web, esse comportamento é nativo em formulários HTML — não requer implementação especial.

---

## 5. Regras de Negócio

| ID   | Regra                                                                 | Tipo       |
|------|-----------------------------------------------------------------------|------------|
| RN01 | Campo "Usuário" é obrigatório                                         | Validação  |
| RN02 | Campo "Senha" é obrigatório                                           | Validação  |
| RN03 | A combinação Usuário + Senha deve existir na tabela `Usuários`        | Autenticação|
| RN04 | Em caso de senha incorreta, o campo senha é limpo e recebe o foco    | UX/Segurança|
| RN05 | O `Perfil` do usuário é recuperado e armazenado em variável global    | Autorização|
| RN06 | O nome do usuário logado é exibido em maiúsculas na tela principal   | Apresentação|
| RN07 | A lista de usuários é carregada do banco na abertura do form          | Dados      |
| RN08 | Usuários disponíveis para seleção ficam ordenados alfabeticamente     | UX         |

---

## 6. Variáveis Globais Utilizadas

| Variável    | Origem         | Destino         | Conteúdo                         |
|-------------|----------------|-----------------|----------------------------------|
| `varUsu`    | `cboUsuario`   | Escopo global   | Nome do usuário selecionado      |
| `varSenha`  | `txtSenha`     | Escopo global   | Senha digitada (texto plano)     |
| `varPerfil` | Query DB       | Escopo global   | Perfil do usuário autenticado    |

> Na migração web, essas variáveis globais são substituídas por **JWT Claims** no token de autenticação.

---

## 7. Observações para a Migração

- O ComboBox com autocomplete (`SuggestAppend`) deve ser substituído por um `<select>` ou input com datalist/autocomplete no React.
- O `HighlightOnFocus` (vermelho ao focar, via DevComponents) pode ser replicado com CSS focus styles.
- O form não tem barra de título funcional (`ControlBox = False`, sem minimize/maximize).
- Não há mecanismo de "esqueci minha senha" no sistema original.
- O código contém um bloco comentado (`DLookUp` / `FrmErrosCodIdClientes`) que sugere uma validação de integridade de dados que foi desativada — verificar com o cliente se deve ser reativada.

---

## 8. Auditoria Retroativa — Classificação PRESERVAR / ADAPTAR / DESCARTAR

> Auditoria realizada em 2026-03-20. Objetivo: classificar cada comportamento do
> form original e verificar se a decisão tomada na migração foi correta.
> Itens marcados com ⚠️ identificam decisões tomadas **silenciosamente** que
> precisam de confirmação ou ação pendente.

### Legenda

| Símbolo | Significado |
|---------|-------------|
| ✅ PRESERVAR | Comportamento idêntico ao original — obrigatório |
| 🔄 ADAPTAR | Comportamento equivalente com ajuste tecnológico — justificado |
| ❌ DESCARTAR | Comportamento não migrado — intencional e documentado |
| ⚠️ | Decisão tomada sem registro explícito — requer confirmação |

---

### 8.1 Tabela de classificação por comportamento

| ID   | Comportamento original | Classificação | Status na migração | Justificativa |
|------|------------------------|---------------|--------------------|---------------|
| B01  | Carregamento da lista de usuários no Load (`UsuáriosTableAdapter.Fill`) | ✅ PRESERVAR | ✅ Implementado — `GET /auth/usuarios` chamado ao montar o componente | RN07/RN08 preservadas. API retorna lista ordenada alfabeticamente. |
| B02  | **Duplo Fill no Load** — `Fill` chamado antes e dentro do `Try` (bug do original) | ❌ DESCARTAR | ✅ Descartado — API chamada uma única vez | Bug identificado no legado (linhas 5 e 8 do `frmLogin.vb`). A chamada duplicada é um defeito, não uma regra de negócio. |
| B03  | Campo `cboUsuario` limpo ao carregar (`Text = ""`) | ✅ PRESERVAR | ✅ Implementado — `defaultValues: { usuario: "", senha: "" }` | Estado inicial vazio preservado. |
| B04  | RN01 — Usuário obrigatório: MsgBox + foco em `cboUsuario` | ✅ PRESERVAR | ✅ Implementado — Zod (`min(1)`), mensagem idêntica, `onInvalid` com `setFocus("usuario")` | Mensagem original: "Por favor, informe o usuário". Texto preservado. |
| B05  | RN02 — Senha obrigatória: MsgBox + foco em `txtSenha` | ✅ PRESERVAR | ✅ Implementado — Zod (`min(1)`), mensagem idêntica, `onInvalid` com foco via `senhaRef` | Mensagem original: "Por favor, informe a senha". Texto preservado. |
| B06  | RN03 — Consulta banco: `Usuário = varUsu AND Psw = varSenha` (texto plano) | 🔄 ADAPTAR | ✅ Implementado — busca por usuário + `verify_password` com bcrypt | Adaptação obrigatória de segurança. Script de migração de senhas necessário antes do go-live. |
| B07  | RN04 — Senha incorreta: limpa `txtSenha` + foco nela | ✅ PRESERVAR | ✅ Implementado — `useEffect` limpa senha e foca via `senhaRef` ao detectar `erroLogin` | Comportamento exato preservado. |
| B08  | RN05 — `varPerfil` recuperada e armazenada globalmente | 🔄 ADAPTAR | ✅ Implementado — `perfil` incluído como claim no JWT; armazenado em `useAuthStore` | Variável global substituída por estado gerenciado (JWT + store React). |
| B09  | RN06 — Nome do usuário em maiúsculas na tela principal | ✅ PRESERVAR | ✅ Implementado — `result.Usuário.upper()` no router; retornado na `LoginResponse` | Comportamento preservado. A capitalização ocorre no backend, como no original. |
| B10  | `varSenha` armazenada em variável global após login | ❌ DESCARTAR | ✅ Descartado — senha nunca persiste após a requisição HTTP | Melhoria de segurança intencional. A senha não precisa ser mantida após a autenticação. |
| B11  | Navegação: `Me.Visible = False` + `FrmPrincipal.Show()` | 🔄 ADAPTAR | ✅ Implementado — redirecionamento para `/principal` via `useLogin` (a confirmar no hook) | Equivalente web: navegação por rota substitui troca de janela. |
| B12  | `FrmPrincipal.lblUsuario.Text = "USUÁRIO: " + UCase(...)` | ✅ PRESERVAR | ✅ Implementado — router retorna `usuario` em maiúsculas; tela principal deve exibir "USUÁRIO: X" | A formatação "USUÁRIO: X" deve ser aplicada na tela `/principal` ao exibir o nome. |
| B13  | `btnCancelar_Click` → `Application.Exit()` | 🔄 ADAPTAR | ✅ Implementado — `window.close()` com comentário `⚠️` no código | `window.close()` pode ser bloqueado por browsers quando a aba não foi aberta por script. Discutir comportamento esperado com o cliente antes do go-live. |
| B14  | `Application.Exit()` no bloco `Catch` do `btnOk_Click` | 🔄 ADAPTAR | ✅ Implementado — router lança `HTTPException 500`; frontend exibe mensagem de erro | O original encerrava o app em erros inesperados. A web permite nova tentativa, o que é mais adequado para ambiente web. |
| B15  | `frmLogin_KeyDown`: `Enter` → `SendKeys("{Tab}")` (avança campo) | ⚠️ 🔄 ADAPTAR | ⚠️ **Decisão silenciosa** — ver detalhes abaixo | |
| B16  | `Highlighter1` — borda vermelha em **qualquer campo com foco** | ⚠️ 🔄 ADAPTAR | ⚠️ **Decisão silenciosa** — ver detalhes abaixo | |
| B17  | Bloco comentado: `DLookUp` / `FrmErrosCodIdClientes` | ⚠️ ❌ DESCARTAR | ⚠️ **Pendente confirmação do cliente** — ver detalhes abaixo | |
| B18  | `Button2` — botão invisível sem evento associado | ❌ DESCARTAR | ✅ Descartado — não renderizado no frontend | Sem função ativa no original. Mencionado na seção 2. |
| B19  | `ControlBox = False`, `MaximizeBox = False`, `MinimizeBox = False` | ❌ DESCARTAR | ✅ Descartado — contexto browser tem seus próprios controles de janela | Não aplicável em SPA. |
| B20  | `StartPosition = CenterScreen` | 🔄 ADAPTAR | ✅ Implementado — `flex min-h-screen items-center justify-center` no layout | Centralização equivalente. |
| B21  | `cboUsuario.ValueMember = "PWD"` — coluna de senha exposta no ComboBox | ❌ DESCARTAR | ✅ Descartado — API nunca retorna a senha | Melhoria de segurança intencional. O campo `PWD` não tem utilidade na web. |
| B22  | Logo (`PictureBox2`, 96×96px, canto superior) | 🔄 ADAPTAR | ✅ Implementado — `<img src="/logo.png" className="h-24 w-24">` com fallback | Asset deve ser fornecido como `/logo.png` em `public/`. |
| B23  | Título "Controle de Acesso" (DarkRed, 14pt, centralizado) | ✅ PRESERVAR | ✅ Implementado — `<h2 className="text-red-800">Controle de Acesso</h2>` | Texto e cor preservados. |
| B24  | Erro no Load: `MsgBox(ex.Message)` | 🔄 ADAPTAR | ✅ Implementado — erro exibido via `erroUsuarios` no JSX (`role="alert"`) | Exibição inline substitui modal. Comportamento equivalente sem interromper o fluxo. |

---

### 8.2 Decisões silenciosas que requerem atenção

#### ⚠️ B15 — Comportamento da tecla Enter

**Original (VB.Net):**
O form tem `KeyPreview = True` e intercepta a tecla `Enter` globalmente,
enviando um `Tab` via `SendKeys`. Resultado: `Enter` **avança o foco** para o
próximo campo sem submeter o form.

**Decisão tomada (implícita):**
A documentação na seção 4.4 afirma que "esse comportamento é nativo em
formulários HTML". Isso é **impreciso**: em um formulário HTML com botão
`type="submit"`, pressionar `Enter` em qualquer campo de texto **submete o
formulário**, não avança o foco.

**Impacto prático:**
- No campo `usuario`: Enter submete o form (em vez de avançar para `senha`).
- No campo `senha`: Enter também submete (coincide com a ação do `btnOk`).

**Classificação correta:** 🔄 ADAPTAR (aceito, comportamento mais idiomático na web)

**Ação recomendada:** Confirmar com o cliente se o comportamento atual (Enter =
submeter) é aceitável. Caso o cliente prefira o comportamento original (Enter =
Tab no campo `usuario`), adicionar `onKeyDown` no campo `usuario` para chamar
`senhaRef.current?.focus()` ao detectar `Enter`.

---

#### ⚠️ B16 — Highlight de foco (Highlighter1)

**Original (VB.Net):**
O componente `Highlighter1` (DevComponents) aplica uma **borda vermelha em
qualquer campo que recebe foco** (`FocusHighlightColor = Red`), independente
de haver erro ou não.

**Decisão tomada (implícita):**
O frontend implementa borda vermelha **apenas em campos com erro de validação**
(`border-red-500 ring-1 ring-red-200`). O estado de foco normal usa
`focus:border-primary` (azul/neutro).

**Classificação correta:** 🔄 ADAPTAR (parcial — foco normal não replicado)

**Ação recomendada:** Definir junto ao responsável pelo design system se o foco
deve destacar os campos com a cor primária (comportamento web padrão) ou se
deve replicar o destaque vermelho do original. Registrar decisão aqui.

> **Decisão pendente** — aguardando definição do design system.

---

#### ⚠️ B17 — Bloco comentado: DLookUp / FrmErrosCodIdClientes

**Original (VB.Net) — linhas 40–46 do `frmLogin.vb`:**
```vb
'If DLookUp("Cliente", "SELECT Clientes.Cliente, ... WHERE ...", "", 1) <> "" Then
'    MsgBox("Existem erros de registros com clientes trocados", ...)
'    FrmErrosCodIdClientes.Show()
'Else
FrmPrincipal.Show()
'End If
```

O bloco sugere que, em algum momento, após o login era exibido um aviso sobre
**inconsistências de dados** (clientes com `CodCliente` e `IdCliente` trocados
entre si) antes de abrir a tela principal. Esse comportamento foi **comentado
no próprio sistema original**, indicando uma decisão anterior de desativação.

**Classificação correta:** ❌ DESCARTAR (pendente confirmação do cliente)

**Ação recomendada:** Confirmar com o cliente/responsável pelo sistema:
1. A inconsistência de dados foi corrigida na base? (tornando o aviso obsoleto)
2. Ou o aviso deve ser reativado na migração web?

Caso a resposta seja (2), será necessário implementar um endpoint de validação
de integridade e um modal/página equivalente ao `FrmErrosCodIdClientes` antes
de concluir a migração do login.

> **Decisão pendente** — aguardando confirmação do cliente.

---

### 8.3 Resumo de pendências

| Ref  | Descrição | Responsável |
|------|-----------|-------------|
| B13  | Confirmar comportamento de "Cancelar" (`window.close()`) com o cliente | Cliente / PO |
| B15  | Confirmar se Enter = Tab no campo usuário é requisito | Cliente / PO |
| B16  | Definir estilo de foco no design system (vermelho vs. primária) | Designer / PO |
| B17  | Confirmar se validação `DLookUp` / `FrmErrosCodIdClientes` deve ser reativada | Cliente / PO |
| —    | Script de migração de senhas para bcrypt necessário antes do go-live | Dev Backend |
