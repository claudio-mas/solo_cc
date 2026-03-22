/**
 * services/clientes.ts
 * Camada de acesso à API de clientes.
 * Única camada que conhece as URLs e o contrato HTTP do backend.
 *
 * Usa apiFetch (autenticado) — diferente de auth.ts que usa fetch nativo.
 */

import { apiFetch } from "@/lib/api";

// ---------------------------------------------------------------------------
// Tipos — alinhados aos schemas Pydantic do backend (schemas/clientes.py)
// ---------------------------------------------------------------------------

export interface ClienteListItem {
  id: number;
  codigo: number;
  cliente: string;
}

interface ClientesResponse {
  clientes: ClienteListItem[];
}

export interface ProximoCodigoResponse {
  proximo_codigo: number;
  proximo_id: number;
}

export interface ClienteCreateRequest {
  codigo: number;
  cliente: string;
}

export interface ClienteCreateResponse {
  id: number;
  codigo: number;
  cliente: string;
}

// ---------------------------------------------------------------------------
// Funções de serviço
// ---------------------------------------------------------------------------

/**
 * Busca a lista completa de clientes, ordenada por nome.
 * Equivalente ao ClientesTableAdapter.Fill() no FrmPrincipal_Load.
 * RN09 — Lista carregada do banco, ordenada por nome.
 */
export async function fetchClientes(): Promise<ClienteListItem[]> {
  const res = await apiFetch("/clientes/");

  if (!res.ok) {
    const err = await res.json();
    throw new Error(
      err.detail ?? "Falha ao carregar clientes",
    );
  }

  const data: ClientesResponse = await res.json();
  return data.clientes;
}

/**
 * Gera o próximo código disponível >= 10.000.
 * Equivalente ao btnCodigo_Click no frmClienteNovo.
 * RN23 — Primeiro inteiro >= 10.000 não utilizado.
 * RN27 — Retorna também o próximo Id.
 */
export async function fetchProximoCodigo(): Promise<ProximoCodigoResponse> {
  const res = await apiFetch("/clientes/proximo-codigo");

  if (!res.ok) {
    const err = await res.json();
    throw new Error(
      err.detail ?? "Falha ao gerar próximo código",
    );
  }

  return res.json();
}

/**
 * Cria um novo cliente.
 * Equivalente ao btnRibSalvar_Click no frmClienteNovo.
 * RN21, RN22, RN24, RN25, RN26.
 */
export async function criarCliente(
  data: ClienteCreateRequest,
): Promise<ClienteCreateResponse> {
  const res = await apiFetch("/clientes/", {
    method: "POST",
    body: JSON.stringify(data),
  });

  if (!res.ok) {
    const err = await res.json();
    throw new Error(
      err.detail ?? "Falha ao criar cliente",
    );
  }

  return res.json();
}

// ---------------------------------------------------------------------------
// frmAlterar — funções de edição e exclusão
// ---------------------------------------------------------------------------

export interface ClienteDetail {
  id: number;
  codigo: number;
  cliente: string;
  /** RN42 — indica se há lançamentos em Contas para o cliente */
  tem_lancamentos: boolean;
}

export interface ClienteUpdateRequest {
  codigo: number;
  cliente: string;
}

export interface ClienteUpdateResponse {
  id: number;
  codigo: number;
  cliente: string;
}

export interface VerificarSenhaResponse {
  /** RN41 — True se senha confere com Chaves WHERE Ref='Exclusão de cliente' */
  valido: boolean;
  /** RN42 — True se cliente tem registros em Contas */
  tem_lancamentos: boolean;
}

/**
 * Busca dados de um cliente pelo Id.
 * Equivalente ao frmAlterar_Load (Fill + Filter por Id).
 * RN33, RN42.
 */
export async function fetchCliente(id: number): Promise<ClienteDetail> {
  const res = await apiFetch(`/clientes/${id}`);

  if (!res.ok) {
    const err = await res.json();
    throw new Error(err.detail ?? "Falha ao carregar cliente");
  }

  return res.json();
}

/**
 * Atualiza Código e/ou Nome do cliente.
 * Equivalente a TableAdapterManager.UpdateAll() no btnRibSalvar_Click.
 * RN36, RN37, RN47, RN48.
 */
export async function atualizarCliente(
  id: number,
  data: ClienteUpdateRequest,
): Promise<ClienteUpdateResponse> {
  const res = await apiFetch(`/clientes/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });

  if (!res.ok) {
    const err = await res.json();
    throw new Error(err.detail ?? "Falha ao atualizar cliente");
  }

  return res.json();
}

/**
 * Valida a senha de exclusão contra a tabela Chaves.
 * Equivalente a frmSenha.ShowDialog() com varSenha="2".
 * RN41, RN42.
 */
export async function verificarSenhaExclusao(
  id: number,
  senha: string,
): Promise<VerificarSenhaResponse> {
  const res = await apiFetch(`/clientes/${id}/verificar-senha`, {
    method: "POST",
    body: JSON.stringify({ senha }),
  });

  if (!res.ok) {
    const err = await res.json();
    throw new Error(err.detail ?? "Falha ao verificar senha");
  }

  return res.json();
}

/**
 * Exclui o cliente e seus lançamentos (se houver).
 * Equivalente ao código ADODB comentado no btnExcluir_Click.
 * RN44, RN45.
 */
export async function excluirCliente(id: number): Promise<void> {
  const res = await apiFetch(`/clientes/${id}`, {
    method: "DELETE",
  });

  if (!res.ok) {
    const err = await res.json();
    throw new Error(err.detail ?? "Falha ao excluir cliente");
  }
}
