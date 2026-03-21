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
