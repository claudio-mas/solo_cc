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
