/**
 * services/relatorios.ts
 * Camada de acesso à API de relatórios.
 * Equivalente a frmReports.vb + frmRptDevedores1.vb + frmCredores1.vb
 *
 * Regras referenciadas:
 *   RN102–RN113 (ver docs/frmRptDevedores1_documentacao.md)
 *   RN114–RN119 (ver docs/frmCredores1_documentacao.md)
 *
 * Usa apiFetch (autenticado) para todos os endpoints.
 */

import { apiFetch } from "@/lib/api";

// ---------------------------------------------------------------------------
// Tipos — alinhados ao schema Pydantic RelatorioResponse
// ---------------------------------------------------------------------------

export type FaixaCodigo = "todos" | "acima" | "abaixo";
export type Ordenacao = "codigo" | "nome";
export type TipoRelatorio = "devedores" | "credores";

export interface ClienteRelatorioItem {
  id: number;
  cod_cliente: number;
  cliente: string;
  saldo: number;
}

export interface RelatorioResponse {
  titulo: string;
  data_corte: string;
  saldo_minimo: number;
  faixa_codigo: FaixaCodigo;
  ordenacao: Ordenacao;
  itens: ClienteRelatorioItem[];
}

export interface RelatorioParams {
  data_corte: string;
  saldo_minimo: number;
  faixa_codigo: FaixaCodigo;
  ordenacao: Ordenacao;
}

// ---------------------------------------------------------------------------
// Funções de serviço
// ---------------------------------------------------------------------------

/**
 * Busca clientes devedores conforme os filtros informados.
 *
 * RN108–RN113 — equivalente à query de frmRptDevedores1_Load.
 */
export async function buscarDevedores(
  params: RelatorioParams,
): Promise<RelatorioResponse> {
  const qs = new URLSearchParams({
    data_corte: params.data_corte,
    saldo_minimo: String(params.saldo_minimo),
    faixa_codigo: params.faixa_codigo,
    ordenacao: params.ordenacao,
  });

  const res = await apiFetch(`/relatorios/devedores?${qs}`);

  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.detail ?? "Erro ao buscar devedores");
  }

  return res.json();
}

/**
 * Busca clientes credores conforme os filtros informados.
 *
 * RN114–RN119 — equivalente à query de frmCredores1_Load.
 */
export async function buscarCredores(
  params: RelatorioParams,
): Promise<RelatorioResponse> {
  const qs = new URLSearchParams({
    data_corte: params.data_corte,
    saldo_minimo: String(params.saldo_minimo),
    faixa_codigo: params.faixa_codigo,
    ordenacao: params.ordenacao,
  });

  const res = await apiFetch(`/relatorios/credores?${qs}`);

  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.detail ?? "Erro ao buscar credores");
  }

  return res.json();
}
