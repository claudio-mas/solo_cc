# Documentação Funcional — FrmUsuarios
**Sistema:** Solo Consultoria de Imóveis — Contas Correntes
**Form original:** `FrmUsuarios.vb` (VB.Net / WinForms)
**URL web:** `/usuarios`
**Propósito:** Gestão de usuários, senhas e chaves de operações críticas do sistema

---

## 1. Visão Geral

O `FrmUsuarios` é aberto pelo `FrmPrincipal` após validação de senha via `frmSenha`
(varSenha = "1"), que consulta a tabela `Chaves` onde `Ref = 'Alteração de senhas'`.

Possui duas abas:
- **Aba "Usuários" (C1DockingTabPage1):** grid com Id, Usuário, Psw e Perfil. O Administrador
  vê todos os registros e pode adicionar novos; o perfil comum vê apenas o próprio e não pode
  alterar o campo Perfil nem adicionar usuários.
- **Aba "Senhas" (C1DockingTabPage2):** grid com as senhas de operações críticas (Id, Ref,
  Chave). Visível apenas para Administrador. Ref é somente leitura; não é possível adicionar
  nem excluir linhas.

Na migração web, as duas abas viram **duas seções na mesma página** `/usuarios`, respeitando
a visibilidade por perfil. A página exibe um modal de senha antes de carregar qualquer dado.

---

## 2. Componentes Visuais

### 2.1 Aba Usuários (RadGridView2)

| Controle / Coluna | Tipo                   | Visível | Editável         | Função                              |
|-------------------|------------------------|---------|------------------|-------------------------------------|
| Id                | GridViewDecimalColumn  | Não     | Não (ReadOnly)   | Chave primária (oculta)             |
| Usuário           | GridViewTextBoxColumn  | Sim     | Sim              | Nome de login (exibido como "Login")|
| Psw               | GridViewTextBoxColumn  | Sim     | Sim              | Senha (exibida como "Senha")        |
| Perfil            | GridViewComboBoxColumn | Sim     | Apenas Admin     | Perfil do usuário (FK → Perfis)     |
| AllowAddNewRow    | —                      | —       | Apenas Admin     | Controla criação de novos usuários  |

### 2.2 Aba Senhas / Chaves (RadGridView1)

| Controle / Coluna | Tipo                   | Visível | Editável | Função                              |
|-------------------|------------------------|---------|----------|-------------------------------------|
| Id                | GridViewDecimalColumn  | Não     | Não      | Chave primária (oculta)             |
| Ref               | GridViewTextBoxColumn  | Sim     | Não      | Contexto da chave (somente leitura) |
| Chave             | GridViewTextBoxColumn  | Sim     | Sim      | Senha da operação crítica           |

### 2.3 Botões da Ribbon

| Botão         | Estado inicial  | Visível quando          |
|---------------|-----------------|-------------------------|
| ribRetornar   | Visível         | Sem edições pendentes   |
| btnRibSalvar  | Oculto          | Após qualquer edição    |
| btnRibUndo    | Oculto          | Após qualquer edição    |

---

## 3. Fluxo Funcional

### 3.1 Fluxo original (legado)

```
FrmPrincipal [botão Usuários]
    │
    ▼
frmSenha (varSenha = "1")
    │ Valida contra Chaves WHERE Ref = 'Alteração de senhas'
    ▼
FrmUsuarios
┌─────────────────────────────────────────────────────┐
│ FrmUsuarios_Load:                                   │
│   Fill(Perfis), Fill(Usuários), Fill(Chaves)        │
│   FrmPrincipal.Visible = False                      │
│   btnRibSalvar.Visible = False                      │
│   btnRibUndo.Visible = False                        │
│                                                     │
│   IF varPerfil ≠ "Administrador":                   │
│     C1DockingTabPage2.TabVisible = False   (RN72)   │
│     UsuáriosBindingSource.Filter = varUsu  (RN69)   │
│     RadGridView2.AllowAddNewRow = False    (RN70)   │
│     RadGridView2.Columns(3).ReadOnly = True(RN71)   │
│                                                     │
│ RadGridView1_ValueChanged (Chaves):                 │
│   ribRetornar.Visible = False                       │
│   btnRibSalvar.Visible = True   (RN74)              │
│   btnRibUndo.Visible = True     (RN74)              │
│                                                     │
│ RadGridView2_ValueChanged (Usuários):               │
│   ribRetornar.Visible = False                       │
│   btnRibSalvar.Visible = True   (RN74)              │
│   btnRibUndo.Visible = True     (RN74)              │
│                                                     │
│ btnRibSalvar_Click (comentado no legado):           │
│   Validate → EndEdit → UpdateAll                    │
│   ribRetornar.Visible = True                        │
│   btnRibSalvar.Visible = False                      │
│   btnRibUndo.Visible = False                        │
│                                                     │
│ btnRibUndo_Click (comentado no legado):             │
│   Fill(Chaves) → Fill(Usuários)  [recarrega do BD]  │
│   ribRetornar.Visible = True                        │
│   btnRibSalvar.Visible = False                      │
│   btnRibUndo.Visible = False                        │
│                                                     │
│ FormClosed:                                         │
│   FrmPrincipal.Visible = True                       │
└─────────────────────────────────────────────────────┘
```

### 3.2 Fluxo web (`/usuarios`)

```
/principal [botão Usuários]
    │
    ▼
/usuarios (renderiza modal de senha)
┌─────────────────────────────────────────────────────┐
│ Mount:                                              │
│   Exibe ModalSenha antes de qualquer dado (RN68)    │
│                                                     │
│ ModalSenha:                                         │
│   POST /auth/verificar-senha {ref: 'Alteração de   │
│     senhas', chave: inputSenha}                     │
│   Senha correta → fecha modal, carrega dados        │
│   Senha errada → exibe erro no modal                │
│   Cancelar → navigate('/principal')                 │
│                                                     │
│ Após autenticação:                                  │
│   GET /usuarios → lista usuários (RN69)             │
│   IF perfil === 'Administrador':                    │
│     GET /chaves → lista chaves (RN72)               │
│                                                     │
│ Seção Usuários:                                     │
│   Tabela com Id (oculto), Usuário, Senha, Perfil    │
│   Admin → todas as linhas + botão Adicionar (RN70)  │
│   Comum → apenas próprio registro (RN69)            │
│   Campo Perfil → apenas Admin pode editar (RN71)    │
│   Edição → PUT /usuarios/{id} (RN73: bcrypt)        │
│   Adição → POST /usuarios (apenas Admin) (RN70)     │
│                                                     │
│ Seção Chaves (apenas Admin):                        │
│   Tabela com Ref (RO), Chave editável               │
│   Edição → PUT /chaves/{id} (RN75, RN76)            │
│                                                     │
│ [Voltar] → navigate('/principal')                   │
└─────────────────────────────────────────────────────┘
```

---

## 4. Regras de Negócio (RN68–RN77)

| RN   | Descrição                                                                                         | Categoria    |
|------|---------------------------------------------------------------------------------------------------|--------------|
| RN68 | Acesso à tela exige validação de senha via tabela Chaves (Ref = 'Alteração de senhas')            | Segurança    |
| RN69 | Administrador vê todos os usuários; perfil comum vê apenas o próprio registro                     | Permissão    |
| RN70 | Perfil comum não pode adicionar novos usuários                                                    | Permissão    |
| RN71 | Perfil comum não pode alterar o campo Perfil de nenhum usuário (nem do próprio)                   | Permissão    |
| RN72 | Seção de Chaves (senhas de operações críticas) visível apenas para Administrador                  | Permissão    |
| RN73 | Ao salvar senha de usuário, armazenar sempre como bcrypt — nunca texto plano                      | Segurança    |
| RN74 | Botões Salvar e Cancelar ficam ocultos até o usuário iniciar uma edição; ao editar aparecem       | UX           |
| RN75 | Campo Ref na tabela Chaves é somente leitura — apenas Chave é editável                           | Integridade  |
| RN76 | Tabela Chaves não permite adição nem exclusão de linhas                                           | Integridade  |
| RN77 | Campo Id não é exibido ao usuário em nenhuma das seções                                           | UX           |

---

## 5. Mapeamento de Queries / Tabelas do Banco

### 5.1 Tabelas acessadas

| Tabela    | Colunas utilizadas              | Operações         |
|-----------|---------------------------------|-------------------|
| Usuários  | Id, Usuário, Psw, Perfil        | SELECT, UPDATE, INSERT |
| Perfis    | Perfil                          | SELECT (dropdown) |
| Chaves    | Id, Ref, Chave                  | SELECT, UPDATE    |

### 5.2 Queries

**Listar usuários (todos — Administrador):**
```sql
SELECT Id, Usuário, Perfil FROM Usuários ORDER BY Usuário ASC
-- Nota: Psw nunca retornada nos responses (RN73)
```

**Listar usuário próprio (perfil comum):**
```sql
SELECT Id, Usuário, Perfil FROM Usuários WHERE Usuário = :usuario
```

**Listar perfis disponíveis (para combobox):**
```sql
SELECT Perfil FROM Perfis ORDER BY Perfil ASC
```

**Listar chaves (apenas Administrador):**
```sql
SELECT Id, Ref, Chave FROM Chaves ORDER BY Id ASC
```

**Atualizar usuário:**
```sql
UPDATE Usuários SET Usuário = :usuario, Psw = :psw_bcrypt, Perfil = :perfil
WHERE Id = :id
```

**Inserir usuário:**
```sql
INSERT INTO Usuários (Usuário, Psw, Perfil) VALUES (:usuario, :psw_bcrypt, :perfil)
```

**Atualizar chave:**
```sql
UPDATE Chaves SET Chave = :chave WHERE Id = :id
```

**Verificar senha de acesso (frmSenha varSenha = "1"):**
```sql
SELECT Chave FROM Chaves WHERE Ref = :ref
```

---

## 6. Decisões Registradas

| ID  | Decisão                                                                                                         | Fonte                         |
|-----|-----------------------------------------------------------------------------------------------------------------|-------------------------------|
| D8  | **Duas abas → duas seções:** as abas "Usuários" e "Senhas" do legado são implementadas como duas seções na mesma página `/usuarios`, sem abas React, pois a tela é simples o suficiente para layout linear. | Análise da migração (2026-04-01) |
| D9  | **Modal de senha no frontend:** o `frmSenha` (varSenha="1") é adaptado como modal React exibido antes de carregar qualquer dado, chamando `POST /auth/verificar-senha` com `ref = 'Alteração de senhas'`. | Análise da migração (2026-04-01) |
| D10 | **Senhas bcrypt:** senhas da tabela Usuários estão em texto plano no legado. Na web, qualquer senha salva ou criada será armazenada como hash bcrypt. O endpoint de login já suporta ambos os formatos (ver `auth_router.py`). | CLAUDE.md — obrigatório       |
| D11 | **Edição via formulário:** a edição inline da grid do legado é adaptada como formulário de edição por linha (campos inline editáveis com botão Salvar explícito por registro), mantendo a intenção de RN74. | Análise da migração (2026-04-01) |
| D12 | **Psw nunca retornada:** o campo Psw da tabela Usuários jamais é incluído nos responses da API, mesmo que o Administrador esteja consultando. | Segurança — obrigatório       |

---

## 7. Auditoria — Classificação PRESERVAR / ADAPTAR / DESCARTAR

### Legenda
| Símbolo      | Significado                                                         |
|--------------|---------------------------------------------------------------------|
| ✅ PRESERVAR  | Comportamento idêntico ao original — obrigatório                   |
| 🔄 ADAPTAR    | Comportamento equivalente com ajuste tecnológico — justificado     |
| ❌ DESCARTAR  | Comportamento não migrado — intencional e documentado              |

### 7.1 Tabela de classificação

| ID  | Comportamento original                                                       | Classificação  | Justificativa                                                                                   |
|-----|------------------------------------------------------------------------------|----------------|-------------------------------------------------------------------------------------------------|
| B01 | `FrmPrincipal.Visible = False` ao abrir                                     | ❌ DESCARTAR   | Padrão WinForms — React Router gerencia navegação; sem janelas para ocultar                     |
| B02 | `FrmPrincipal.Visible = True` ao fechar (FormClosed)                        | ❌ DESCARTAR   | Mesmo motivo do B01                                                                             |
| B03 | `PerfisTableAdapter.Fill` / `UsuáriosTableAdapter.Fill` / `ChavesTableAdapter.Fill` no Load | 🔄 ADAPTAR | Equivalente a `GET /usuarios` + `GET /chaves` no mount do componente React            |
| B04 | `btnRibSalvar.Visible = False` / `btnRibUndo.Visible = False` no Load       | 🔄 ADAPTAR     | RN74 — estado `isDirty` no React controla visibilidade dos botões Salvar/Cancelar               |
| B05 | `IF varPerfil ≠ "Administrador"` → oculta aba Chaves                        | ✅ PRESERVAR   | RN72 — seção Chaves condicional por claim `perfil` do JWT                                       |
| B06 | `UsuáriosBindingSource.Filter = "Usuário = '" & varUsu & "'"`               | ✅ PRESERVAR   | RN69 — backend filtra por `WHERE Usuário = :usuario` quando perfil não é Administrador          |
| B07 | `RadGridView2.AllowAddNewRow = False` para perfil comum                     | ✅ PRESERVAR   | RN70 — botão Adicionar oculto no frontend; backend rejeita POST /usuarios por não-Admin         |
| B08 | `RadGridView2.Columns(3).ReadOnly = True` para perfil comum (coluna Perfil) | ✅ PRESERVAR   | RN71 — campo Perfil desabilitado no frontend; backend rejeita alteração de Perfil por não-Admin |
| B09 | `RadGridView1_ValueChanged` → mostra Salvar/Cancelar, oculta Retornar       | 🔄 ADAPTAR     | RN74 — `isDirty` por linha na seção Chaves; botões por registro                                 |
| B10 | `RadGridView2_ValueChanged` → mostra Salvar/Cancelar, oculta Retornar       | 🔄 ADAPTAR     | RN74 — `isDirty` por linha na seção Usuários; botões por registro                               |
| B11 | `btnRibSalvar_Click` → `EndEdit` + `UpdateAll` (comentado no legado)        | 🔄 ADAPTAR     | Equivalente a `PUT /usuarios/{id}` ou `PUT /chaves/{id}` com payload da linha editada           |
| B12 | `btnRibUndo_Click` → `Fill` de volta do banco (comentado no legado)         | 🔄 ADAPTAR     | Botão Cancelar por linha descarta edição local e restaura valores originais no estado React      |
| B13 | Edição inline na grid (WinForms DataGridView)                               | 🔄 ADAPTAR     | D11 — campos editáveis inline por linha com Salvar/Cancelar explícitos; sem edição por célula   |
| B14 | Senha em texto plano (campo Psw)                                            | 🔄 ADAPTAR     | RN73 + D10 — bcrypt obrigatório ao salvar; nunca retornado nos responses (D12)                  |
| B15 | `C1PictureBox1` (logo decorativo)                                           | ❌ DESCARTAR   | Componente WinForms — sem equivalente necessário na web                                          |
| B16 | `Office2010BlackTheme` / Telerik / `C1Ribbon`                               | ❌ DESCARTAR   | Componentes WinForms proprietários sem equivalente web                                           |
| B17 | `ribRetornar.Visible = False` durante edição                                | 🔄 ADAPTAR     | Botão Voltar desabilitado enquanto há edições não salvas                                         |
| B18 | `frmSenha` modal antes de abrir o form (varSenha = "1")                     | 🔄 ADAPTAR     | D9 — modal React + `POST /auth/verificar-senha` com `ref = 'Alteração de senhas'`               |
| B19 | `GridViewComboBoxColumn` para Perfil (alimentado por `PerfisBindingSource`) | 🔄 ADAPTAR     | `<select>` alimentado por `GET /auth/perfis` (ou lista de valores da tabela Perfis)             |
| B20 | `AllowDeleteRow = False` na grid de Chaves                                  | ✅ PRESERVAR   | RN76 — sem botão de exclusão na seção Chaves; backend rejeita DELETE /chaves                    |
| B21 | `AllowAddNewRow` não configurado na grid de Chaves (padrão False por AllowDeleteRow) | ✅ PRESERVAR | RN76 — sem botão de adição na seção Chaves; backend não expõe POST /chaves              |

### 7.2 Resumo

| Classificação   | Quantidade | IDs                                                          |
|-----------------|------------|--------------------------------------------------------------|
| ✅ PRESERVAR    | 6          | B05, B06, B07, B08, B20, B21                                 |
| 🔄 ADAPTAR      | 12         | B03, B04, B09, B10, B11, B12, B13, B14, B17, B18, B19       |
| ❌ DESCARTAR    | 5          | B01, B02, B15, B16, B01 (+ B02)                              |

---

## 8. Mapeamento Web

| Tela Web     | Form Original | Documentação                         |
|--------------|---------------|--------------------------------------|
| `/usuarios`  | `FrmUsuarios` | `docs/FrmUsuarios_documentacao.md`   |
