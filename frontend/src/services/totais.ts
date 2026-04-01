/**
 * services/totais.ts
 * Camada de acesso à API de totais.
 * Única camada que conhece as URLs e o contrato HTTP do backend.
 *
 * Equivalente ao frmTotais2.vb (VB.Net).
 * Usa apiFetch (autenticado).
 */

import { apiFetch } from "@/lib/api";

// ---------------------------------------------------------------------------
// Tipos — alinhados ao schema Pydantic TotaisResponse (schemas/totais.py)
// ---------------------------------------------------------------------------

export interface TotaisResponse {
  qtde_credores: number;
  valor_credores: string; // Decimal serializado como string
  qtde_devedores: number;
  valor_devedores: string;
}

// ---------------------------------------------------------------------------
// Funções de serviço
// ---------------------------------------------------------------------------

/**
 * Busca os 4 totais de clientes credores e devedores para a data informada.
 *
 * RN59 — se `data` não for passada, o backend usa date.today().
 * RN60 — cada chamada retorna os totais para a data recebida.
 * RN61–RN66 — credores (TC > TD) e devedores (TD > TC) calculados no backend.
 *
 * Equivalente a FrmTotais_Load e Dt_ValueChanged no frmTotais2.vb.
 */
export async function buscarTotais(data?: string): Promise<TotaisResponse> {
  const params = data ? new URLSearchParams({ data }) : new URLSearchParams();
  const query = params.toString() ? `?${params}` : "";

  const res = await apiFetch(`/totais${query}`);

  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.detail ?? "Erro ao buscar totais");
  }

  return res.json();
}
