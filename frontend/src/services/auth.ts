/**
 * services/auth.ts
 * Camada de acesso à API de autenticação.
 * Única camada que conhece as URLs e o contrato HTTP do backend.
 */

const API_URL = import.meta.env.VITE_API_URL ?? "http://localhost:8000";

// ---------------------------------------------------------------------------
// Tipos — alinhados aos schemas Pydantic do backend (auth_router.py)
// ---------------------------------------------------------------------------

export interface UsuarioListItem {
  usuario: string;
}

export interface LoginRequest {
  usuario: string;
  senha: string;
}

export interface LoginResponse {
  access_token: string;
  token_type: string;
  usuario: string; // retornado em maiúsculas pela API (RN06)
  perfil: string;
}

// ---------------------------------------------------------------------------
// Funções de serviço
// ---------------------------------------------------------------------------

/**
 * Busca a lista de usuários para o dropdown de login.
 * Equivalente ao UsuáriosTableAdapter2.Fill() no frmLogin_Load.
 * RN07 — Lista carregada do banco na abertura do form.
 * RN08 — Usuários ordenados alfabeticamente (garantido pela API).
 */
export async function fetchUsuarios(): Promise<UsuarioListItem[]> {
  const res = await fetch(`${API_URL}/auth/usuarios`);
  if (!res.ok) throw new Error("Falha ao carregar usuários");
  return res.json();
}

/**
 * Autentica o usuário com usuário e senha.
 * Equivalente ao evento btnOk_Click do frmLogin.
 * Lança Error com a mensagem do backend em caso de falha (ex: "Senha incorreta").
 */
export async function loginService(data: LoginRequest): Promise<LoginResponse> {
  const res = await fetch(`${API_URL}/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });

  if (!res.ok) {
    const err = await res.json();
    throw new Error(err.detail ?? "Erro desconhecido");
  }

  return res.json();
}
