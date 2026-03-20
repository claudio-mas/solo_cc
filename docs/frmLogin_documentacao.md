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
