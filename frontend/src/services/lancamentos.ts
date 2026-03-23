/**
 * services/lancamentos.ts
 * Camada de acesso à API de lançamentos.
 * Única camada que conhece as URLs e o contrato HTTP do backend.
 *
 * Equivalente ao fluxo FrmLancaData + FrmLanca (VB.Net).
 * Usa apiFetch (autenticado).
 */

import { apiFetch } from "@/lib/api";

// ---------------------------------------------------------------------------
// Tipos — alinhados aos schemas Pydantic do backend (schemas/lancamentos.py)
// ---------------------------------------------------------------------------

export interface LancamentoCreateRequest {
  id_cliente: number;
  cod_cliente: number;
  dt: string; // "YYYY-MM-DD"
  conta: number; // "Pasta" na UI
  ref: string; // "Histórico"
  vvalor: string; // Decimal como string para evitar ponto flutuante
  dc: "D" | "C";
}

export interface LancamentoCreateResponse {
  id: number;
  mensagem: string;
}

export interface VerificarPastaResponse {
  existe: boolean;
}

// ---------------------------------------------------------------------------
// Funções de serviço
// ---------------------------------------------------------------------------

/**
 * Verifica se a pasta (Conta) já existe para o cliente informado.
 * RN51 — equivalente à query ADODB em txtPasta_Validated no FrmLanca.vb.
 */
export async function verificarPasta(
  idCliente: number,
  pasta: number,
): Promise<VerificarPastaResponse> {
  const params = new URLSearchParams({
    id_cliente: String(idCliente),
    pasta: String(pasta),
  });

  const res = await apiFetch(`/lancamentos/verificar-pasta?${params}`);

  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.detail ?? "Erro ao verificar pasta");
  }

  return res.json();
}

/**
 * Registra um novo lançamento de débito ou crédito.
 * RN52 — DC aceita apenas 'D' ou 'C'.
 * RN53 — DC determina qual campo (Deb ou Cred) recebe VValor (lógica no backend).
 * RN54 — todos os campos são obrigatórios.
 * Equivalente a btnRibSalvar_Click + TableAdapterManager.UpdateAll() no FrmLanca.vb.
 */
export async function registrarLancamento(
  dados: LancamentoCreateRequest,
): Promise<LancamentoCreateResponse> {
  const res = await apiFetch("/lancamentos/", {
    method: "POST",
    body: JSON.stringify(dados),
  });

  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.detail ?? "Erro ao registrar lançamento");
  }

  return res.json();
}
