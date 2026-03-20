# Padrões Legado — Mapeamento VB.Net → Stack Web

Referência dos padrões recorrentes identificados no sistema Solo Consultoria
de Imóveis. Atualizar conforme novos forms forem analisados.

---

## 1. Acesso a Dados

### TableAdapter + DataSet

```vb
' Padrão VB.Net
Me.UsuáriosTableAdapter2.Fill(Me.SoloDataSet.Usuários)
```

**Equivalente FastAPI:**
```python
# SQLAlchemy — query explícita com colunas nomeadas
result = db.execute(
    text("SELECT Id, Usuário, Psw, Perfil FROM Usuários ORDER BY Usuário ASC")
).fetchall()
```

**Observação:** O Fill duplicado no `frmLogin_Load` (linhas 5 e 8 do `.vb`)
sugere copiar/colar acidental. Verificar em outros forms se há padrão similar.

---

### BindingSource com Sort

```vb
' Padrão VB.Net
Me.UsuáriosBindingSource.Sort = "Usuário"
```

**Equivalente FastAPI:** `ORDER BY Usuário ASC` na query SQL.
**Equivalente React:** array já ordenado retornado pela API — não reordenar
no frontend.

---

### LINQ to SQL (ContasDataContext)

```vb
' Padrão VB.Net
Dim dc As New Contas_Correntes.ContasDataContext()
Dim query = (From tb In dc.GetTable(Of Usuários)()
             Select tb Where tb.Usuário = varUsu And tb.Psw = varSenha)
```

**Equivalente FastAPI:**
```python
result = db.execute(
    text("SELECT Id, Usuário, Psw, Perfil FROM Usuários WHERE Usuário = :u AND Psw = :p"),
    {"u": usuario, "p": senha}
).fetchone()
```

---

## 2. Variáveis Globais

O sistema desktop usa variáveis globais de módulo para compartilhar estado
entre forms. Na migração web, cada variável tem um equivalente específico:

| Variável global | Conteúdo | Equivalente web |
|---|---|---|
| `varUsu` | Nome do usuário logado | JWT claim `sub` / `localStorage["usuario"]` |
| `varSenha` | Senha digitada (temporária) | Nunca persistir — usar apenas no request de login |
| `varPerfil` | Perfil do usuário | JWT claim `perfil` / `localStorage["perfil"]` |

**Regra:** Nenhuma variável global do sistema desktop deve virar estado global
React ou variável de módulo Python. Usar JWT claims para dados do usuário
autenticado e props/contexto React para estado de UI.

---

## 3. Navegação entre Forms

### Abrir form filho

```vb
' Padrão VB.Net
FrmPrincipal.Show()
FrmPrincipal.lblUsuario.Text = "USUÁRIO: " & UCase(Me.cboUsuario.Text)
```

**Equivalente React:**
```jsx
// React Router — navigate para a rota correspondente
navigate("/principal");
// Dados do usuário já no localStorage — lidos pelo componente de destino
```

**Observação:** Passagem de dados entre forms via propriedades públicas
(ex: `lblUsuario.Text`) deve ser substituída por dados no token JWT ou
localStorage, nunca por props diretos entre páginas.

---

### Ocultar form atual

```vb
' Padrão VB.Net
Me.Visible = False
```

**Equivalente React:** `navigate()` para a próxima rota — o componente
atual é desmontado automaticamente pelo React Router.

---

### Encerrar aplicação

```vb
' Padrão VB.Net
Application.Exit()
```

**Equivalente web:** Depende do contexto:
- Em botão "Cancelar" no login → `window.close()` (pode ser bloqueado pelo
  browser se a aba não foi aberta por script) ou redirecionar para página
  de encerramento de sessão
- Em erro crítico → `navigate("/logout")` + limpar localStorage
- ⚠️ Sempre alertar: `window.close()` não funciona de forma confiável em
  todos os browsers — discutir com o cliente o comportamento esperado

---

## 4. Controles Visuais

### ComboBox com AutoComplete e DataSource

```vb
' Padrão VB.Net
Me.cboUsuario.AutoCompleteMode = AutoCompleteMode.SuggestAppend
Me.cboUsuario.DataSource = Me.UsuáriosBindingSource
Me.cboUsuario.DisplayMember = "Usuário"
```

**Equivalente React:**
```jsx
<input list="lista-usuarios" value={usuario} onChange={...} />
<datalist id="lista-usuarios">
  {usuarios.map(u => <option key={u.usuario} value={u.usuario} />)}
</datalist>
```

**Ou:** Select controlado com opção de busca via biblioteca (react-select,
Combobox do Headless UI) — decidir conforme design system.

---

### Campo de senha mascarado

```vb
' Padrão VB.Net
Me.txtSenha.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)  ' *
```

**Equivalente React:**
```jsx
<input type="password" ... />
```

---

### Highlighter (DevComponents DotNetBar)

```vb
' Padrão VB.Net
Me.Highlighter1.FocusHighlightColor = eHighlightColor.Red
Me.Highlighter1.SetHighlightOnFocus(Me.cboUsuario, True)
```

**Equivalente CSS:**
```css
input:focus {
  border-color: red;
  box-shadow: 0 0 0 2px rgba(255, 0, 0, 0.15);
}
```

---

### Controle com `Visible = False` e sem evento

Padrão encontrado em `Button2` no `frmLogin`. Indica funcionalidade
desativada ou removida. **Não migrar para a UI.** Documentar na seção
"Observações" da documentação funcional com a nota: "controle inativo no
form original — ignorado na migração."

---

## 5. Tratamento de Erros

### MsgBox genérico no Catch

```vb
' Padrão VB.Net
Catch ex As Exception
    MsgBox(ex.Message)
    Application.Exit()  ' em alguns forms
End Try
```

**Equivalente FastAPI:**
```python
except HTTPException:
    raise
except Exception as e:
    raise HTTPException(
        status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
        detail=f"Erro interno: {str(e)}"
    )
```

**Equivalente React:**
```jsx
setErro("Erro de conexão com o servidor.");
```

**Observação:** O `Application.Exit()` dentro de Catch (presente no
`frmLogin`) indica comportamento de falha catastrófica. Na web, preferir
mostrar o erro e permitir nova tentativa — só redirecionar para logout em
erros de autenticação irrecuperáveis.

---

## 6. Segurança

### Senha em texto plano

Identificada na tabela `Usuários`, campo `Psw`. Todos os forms que fazem
autenticação ou gestão de usuários devem:

1. No backend: usar `passlib[bcrypt]` para verificação
2. Nunca retornar o campo `Psw` em responses da API
3. Antes do go-live: executar script de migração de hashes (a ser criado)

### Perfil como controle de acesso

O campo `Perfil` da tabela `Usuários` controla quais telas/funcionalidades
cada usuário pode acessar. Consultar a tabela `Perfis` no banco para mapear
os valores válidos antes de implementar guards nas rotas React e dependências
FastAPI.

---

## 7. Dataset e Schema

### Tabelas identificadas até agora

| Tabela | Identificada em | Observações |
|---|---|---|
| `Usuários` | `frmLogin` | Id, Usuário, Psw (plain text), Perfil |
| `Clientes` | `frmLogin` (comentado) | Referenciada em query comentada |
| `Contas` | `frmLogin` (comentado) | Referenciada em query comentada |
| `Perfis` | Designer (`TableAdapterManager`) | Valores de perfil a mapear |
| `Chaves` | Designer (`TableAdapterManager`) | Uso a identificar |
| `Extrato` | Designer (`TableAdapterManager`) | Uso a identificar |

Atualizar esta tabela conforme novos forms forem analisados.

---

*Última atualização: análise do `frmLogin` — adicionar entradas a cada novo
form migrado.*