/**
 * services/usuarios.ts
 * Camada de acesso à API de usuários e chaves.
 * Única camada que conhece as URLs e o contrato HTTP do backend.
 *
 * Equivalente ao FrmUsuarios.vb (VB.Net).
 * Usa apiFetch (autenticado) para todos os endpoints.
 *
 * Nota de segurança (D12): o campo Psw nunca é retornado pelo backend
 * e portanto nunca aparece nos tipos de response desta camada.
 */

import { apiFetch } from "@/lib/api";

// ---------------------------------------------------------------------------
// Tipos — alinhados aos schemas Pydantic em schemas/usuarios.py
// ---------------------------------------------------------------------------

export interface PerfilItem {
  perfil: string;
}

export interface UsuarioResponse {
  id: number;
  usuario: string;
  perfil: string;
}

export interface UsuarioCreate {
  usuario: string;
  senha: string;
  perfil: string;
}

export interface UsuarioUpdate {
  usuario?: string;
  senha?: string;
  perfil?: string;
}

export interface ChaveResponse {
  id: number;
  ref: string;
  chave: string;
}

export interface ChaveUpdate {
  chave: string;
}

export interface VerificarSenhaResponse {
  ok: boolean;
}

// ---------------------------------------------------------------------------
// Verificação de senha de acesso (frmSenha — varSenha = "1")
// ---------------------------------------------------------------------------

/**
 * RN68 — valida a senha de uma operação crítica contra a tabela Chaves.
 * Chamado pelo modal de senha antes de exibir os dados da tela /usuarios.
 *
 * POST /auth/verificar-senha
 *   { ref: 'Alteração de senhas', chave: inputSenha }
 *
 * Equivalente ao frmSenha.vb consultando Chaves WHERE Ref = varRef.
 */
export async function verificarSenha(
  ref: string,
  chave: string,
): Promise<VerificarSenhaResponse> {
  const res = await apiFetch("/auth/verificar-senha", {
    method: "POST",
    body: JSON.stringify({ ref, chave }),
  });

  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.detail ?? "Erro ao verificar senha");
  }

  return res.json();
}

// ---------------------------------------------------------------------------
// Perfis
// ---------------------------------------------------------------------------

/**
 * Lista perfis disponíveis para o dropdown de Perfil.
 * Equivalente ao PerfisBindingSource no FrmUsuarios_Load.
 */
export async function listarPerfis(): Promise<PerfilItem[]> {
  const res = await apiFetch("/perfis");

  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.detail ?? "Erro ao carregar perfis");
  }

  return res.json();
}

// ---------------------------------------------------------------------------
// Usuários
// ---------------------------------------------------------------------------

/**
 * RN69 — Administrador recebe todos; perfil comum recebe apenas o próprio.
 * GET /usuarios
 *
 * Equivalente ao UsuáriosTableAdapter.Fill + BindingSource.Filter no legado.
 */
export async function listarUsuarios(): Promise<UsuarioResponse[]> {
  const res = await apiFetch("/usuarios");

  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.detail ?? "Erro ao carregar usuários");
  }

  return res.json();
}

/**
 * RN70 — apenas Administrador pode criar usuários.
 * RN73 — senha será armazenada como bcrypt no backend.
 * POST /usuarios
 */
export async function criarUsuario(
  payload: UsuarioCreate,
): Promise<UsuarioResponse> {
  const res = await apiFetch("/usuarios", {
    method: "POST",
    body: JSON.stringify(payload),
  });

  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.detail ?? "Erro ao criar usuário");
  }

  return res.json();
}

/**
 * RN69 — perfil comum só pode alterar o próprio.
 * RN71 — perfil comum não pode alterar Perfil.
 * RN73 — nova senha será armazenada como bcrypt no backend.
 * PUT /usuarios/{id}
 */
export async function atualizarUsuario(
  id: number,
  payload: UsuarioUpdate,
): Promise<UsuarioResponse> {
  const res = await apiFetch(`/usuarios/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });

  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.detail ?? "Erro ao atualizar usuário");
  }

  return res.json();
}

// ---------------------------------------------------------------------------
// Chaves
// ---------------------------------------------------------------------------

/**
 * RN72 — apenas Administrador pode listar chaves.
 * GET /chaves
 *
 * Equivalente à aba "Senhas" (C1DockingTabPage2) no FrmUsuarios.
 */
export async function listarChaves(): Promise<ChaveResponse[]> {
  const res = await apiFetch("/chaves");

  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.detail ?? "Erro ao carregar chaves");
  }

  return res.json();
}

/**
 * RN72 — apenas Administrador.
 * RN75 — apenas Chave é alterada; Ref não é enviada.
 * PUT /chaves/{id}
 */
export async function atualizarChave(
  id: number,
  payload: ChaveUpdate,
): Promise<ChaveResponse> {
  const res = await apiFetch(`/chaves/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });

  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.detail ?? "Erro ao atualizar chave");
  }

  return res.json();
}
