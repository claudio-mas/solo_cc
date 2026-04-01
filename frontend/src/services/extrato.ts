/**
 * services/extrato.ts
 * Camada de acesso à API de extrato.
 * Equivalente ao FrmExtrato.vb (VB.Net).
 *
 * Regras referenciadas:
 *   RN78–RN101 (ver docs/FrmExtrato_documentacao.md)
 *
 * Usa apiFetch (autenticado) para todos os endpoints.
 */

import { apiFetch } from "@/lib/api";

// ---------------------------------------------------------------------------
// Tipos — alinhados aos schemas Pydantic em schemas/extrato.py
// ---------------------------------------------------------------------------

export interface LancamentoExtratoItem {
  id: number;
  dt: string;
  conta: number;
  nd: string | null;
  ref: string | null;
  vvalor: number | null;
  dc: string | null;
  deb: number | null;
  cred: number | null;
  saldo: number;
}

export interface ExtratoResponse {
  lancamentos: LancamentoExtratoItem[];
  saldo_total: number;
}

export interface LancamentoPatch {
  dt?: string;
  conta?: number;
  nd?: string;
  ref?: string;
  deb?: number;
  cred?: number;
}

export interface LancamentoPatchResponse {
  ok: boolean;
  mensagem: string;
}

export interface TransferenciaRequest {
  ids: number[];
  id_destino: number;
  chave: string;
}

export interface TransferenciaResponse {
  ok: boolean;
  mensagem: string;
  transferidos: number;
}

export interface DesbloquearRequest {
  chave: string;
}

export interface DesbloquearResponse {
  ok: boolean;
}

export interface SincronizarVValorResponse {
  ok: boolean;
  mensagem: string;
}

export interface ClienteDestinoItem {
  id: number;
  codigo: number;
  cliente: string;
}

// ---------------------------------------------------------------------------
// Filtros de extrato
// ---------------------------------------------------------------------------

export interface ExtratoFiltros {
  pasta?: number;
  nd?: string;
  sem_nd?: boolean;
  hist?: string;
}

// ---------------------------------------------------------------------------
// GET /extrato/{id_cliente} — lançamentos com saldo acumulado
// ---------------------------------------------------------------------------

/**
 * RN78–RN86 — lista lançamentos do cliente com saldo acumulado.
 * Aceita filtros opcionais: pasta (RN79), nd (RN80), sem_nd (RN81), hist (RN82).
 *
 * Equivalente ao ExtratoTableAdapter.Fill + BindingSource.Filter no legado.
 */
export async function listarExtrato(
  idCliente: number,
  filtros?: ExtratoFiltros,
): Promise<ExtratoResponse> {
  const params = new URLSearchParams();
  if (filtros?.pasta !== undefined) params.set("pasta", String(filtros.pasta));
  if (filtros?.nd !== undefined) params.set("nd", filtros.nd);
  if (filtros?.sem_nd) params.set("sem_nd", "true");
  if (filtros?.hist !== undefined) params.set("hist", filtros.hist);

  const qs = params.toString();
  const url = `/extrato/${idCliente}${qs ? `?${qs}` : ""}`;

  const res = await apiFetch(url);

  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.detail ?? "Erro ao carregar extrato");
  }

  return res.json();
}

// ---------------------------------------------------------------------------
// PATCH /lancamentos/{id} — edição inline de campo
// ---------------------------------------------------------------------------

/**
 * RN88/RN90 — atualiza campo específico de um lançamento.
 * Chamado no onBlur de cada célula editável.
 *
 * Equivalente ao CellEndEdit → fExecutaText(UPDATE) no legado.
 */
export async function atualizarLancamento(
  idLancamento: number,
  payload: LancamentoPatch,
): Promise<LancamentoPatchResponse> {
  const res = await apiFetch(`/lancamentos/${idLancamento}`, {
    method: "PATCH",
    body: JSON.stringify(payload),
  });

  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.detail ?? "Erro ao atualizar lançamento");
  }

  return res.json();
}

// ---------------------------------------------------------------------------
// POST /extrato/desbloquear — validação de senha para edição
// ---------------------------------------------------------------------------

/**
 * RN87 — valida senha de desbloqueio contra Chaves.
 * Equivalente ao F10 → frmSenha (varSenha=4) no legado.
 */
export async function desbloquearEdicao(
  chave: string,
): Promise<DesbloquearResponse> {
  const res = await apiFetch("/extrato/desbloquear", {
    method: "POST",
    body: JSON.stringify({ chave }),
  });

  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.detail ?? "Erro ao desbloquear edição");
  }

  return res.json();
}

// ---------------------------------------------------------------------------
// POST /extrato/transferir — transferência de lançamentos
// ---------------------------------------------------------------------------

/**
 * RN93–RN98 — transfere lançamentos selecionados para outro cliente.
 * Equivalente ao btnTransfere_Click → frmSenha (varSenha=3) no legado.
 */
export async function transferirLancamentos(
  idClienteOrigem: number,
  payload: TransferenciaRequest,
): Promise<TransferenciaResponse> {
  const res = await apiFetch(
    `/extrato/transferir?id_cliente_origem=${idClienteOrigem}`,
    {
      method: "POST",
      body: JSON.stringify(payload),
    },
  );

  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.detail ?? "Erro ao transferir lançamentos");
  }

  return res.json();
}

// ---------------------------------------------------------------------------
// POST /extrato/{id_cliente}/sincronizar-vvalor
// ---------------------------------------------------------------------------

/**
 * RN99 — sincroniza VValor com Deb/Cred ao sair da tela.
 * Chamado no useEffect cleanup do componente Extrato.
 *
 * Equivalente ao FrmExtrato_Closed no legado.
 */
export async function sincronizarVValor(
  idCliente: number,
): Promise<SincronizarVValorResponse> {
  const res = await apiFetch(`/extrato/${idCliente}/sincronizar-vvalor`, {
    method: "POST",
  });

  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.detail ?? "Erro ao sincronizar VValor");
  }

  return res.json();
}

// ---------------------------------------------------------------------------
// GET /extrato/{id_cliente}/clientes-destino
// ---------------------------------------------------------------------------

/**
 * RN96 — lista clientes disponíveis para transferência (exclui o atual).
 * Equivalente ao ClientesBindingSource com filtro [Id] <> id_atual no legado.
 */
export async function listarClientesDestino(
  idCliente: number,
): Promise<ClienteDestinoItem[]> {
  const res = await apiFetch(`/extrato/${idCliente}/clientes-destino`);

  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.detail ?? "Erro ao carregar clientes");
  }

  return res.json();
}
