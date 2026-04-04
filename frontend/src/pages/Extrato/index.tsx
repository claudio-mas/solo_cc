/**
 * pages/Extrato/index.tsx — Equivalente web de FrmExtrato
 * Sistema: Solo Consultoria de Imóveis — Contas Correntes
 *
 * Regras implementadas:
 *   RN78  — Extrato filtrado por IdCliente
 *   RN79  — Filtro por Pasta (Conta)
 *   RN80  — Filtro por ND
 *   RN81  — Checkbox "Sem ND" (ND IS NULL)
 *   RN82  — Filtro por Histórico (LIKE)
 *   RN83  — Combinação de filtros simultâneos
 *   RN84  — Saldo acumulado via window function
 *   RN85  — Saldo no rodapé (vermelho >= 0, azul < 0)
 *   RN86  — Recálculo do saldo ao aplicar filtros
 *   RN87  — Desbloqueio via senha (F10)
 *   RN88  — Campos editáveis: Data, Pasta, ND, Histórico, Débito, Crédito
 *   RN89  — Campos sempre bloqueados: Id, IdCliente, CodCliente, DC, Saldo
 *   RN90  — Edição inline salva imediatamente (onBlur → PATCH)
 *   RN91  — F10 novamente → bloqueia + recalcula
 *   RN92  — Delete controlado pelo estado de bloqueio
 *   RN93  — Transferência de lançamentos (F9)
 *   RN94  — Validação de senha para transferência
 *   RN95  — Seleção múltipla de linhas
 *   RN96  — Combo destino exclui cliente atual
 *   RN97  — UPDATE IdCliente + CodCliente
 *   RN98  — Recarregar após transferência
 *   RN99  — Sincronizar VValor ao desmontar
 *   RN100 — Impressão do extrato filtrado (D1 resolvido: @react-pdf/renderer)
 *   RN101 — Navegação primeiro/último
 *
 * B45/B46/B47/B48/B49/B50/B51/B52/B53/B54 — DESCARTAR (artefatos WinForms)
 * B55 — Conditional formatting "AGENCIA" → ADAPTAR (CSS)
 */

import React, { useCallback, useEffect, useRef, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import {
  atualizarLancamento,
  ClienteDestinoItem,
  desbloquearEdicao,
  ExtratoFiltros,
  LancamentoExtratoItem,
  listarClientesDestino,
  listarExtrato,
  sincronizarVValor,
  transferirLancamentos,
} from "@/services/extrato";
import styles from "./Extrato.module.css";

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

function formatarValor(v: number | null): string {
  if (v === null || v === undefined) return "";
  return new Intl.NumberFormat("pt-BR", {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(v);
}

function parseValorBR(valor: string): number {
  const normalizado = valor.replace(/\./g, "").replace(",", ".").trim();
  const numero = Number(normalizado);
  return Number.isFinite(numero) ? numero : 0;
}

function formatarData(dt: string): string {
  const d = new Date(dt);
  const dd = String(d.getDate()).padStart(2, "0");
  const mm = String(d.getMonth() + 1).padStart(2, "0");
  const yy = String(d.getFullYear()).slice(-2);
  return `${dd}/${mm}/${yy}`;
}

function formatarDataInput(dt: string): string {
  return dt.slice(0, 10); // yyyy-mm-dd for input[type=date]
}

// ---------------------------------------------------------------------------
// SVG inline icons
// ---------------------------------------------------------------------------

function IconeLock() {
  return (
    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
      <rect x="3" y="11" width="18" height="11" rx="2" ry="2" />
      <path d="M7 11V7a5 5 0 0 1 10 0v4" />
    </svg>
  );
}

function IconeUnlock() {
  return (
    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
      <rect x="3" y="11" width="18" height="11" rx="2" ry="2" />
      <path d="M7 11V7a5 5 0 0 1 9.9-1" />
    </svg>
  );
}

function IconeTransfer() {
  return (
    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
      <polyline points="17 1 21 5 17 9" />
      <path d="M3 11V9a4 4 0 0 1 4-4h14" />
      <polyline points="7 23 3 19 7 15" />
      <path d="M21 13v2a4 4 0 0 1-4 4H3" />
    </svg>
  );
}

function IconePrinter() {
  return (
    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
      <polyline points="6 9 6 2 18 2 18 9" />
      <path d="M6 18H4a2 2 0 0 1-2-2v-5a2 2 0 0 1 2-2h16a2 2 0 0 1 2 2v5a2 2 0 0 1-2 2h-2" />
      <rect x="6" y="14" width="12" height="8" />
    </svg>
  );
}

function IconeUp() {
  return (
    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
      <polyline points="18 15 12 9 6 15" />
    </svg>
  );
}

function IconeDown() {
  return (
    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
      <polyline points="6 9 12 15 18 9" />
    </svg>
  );
}

// ---------------------------------------------------------------------------
// Modal de senha para desbloqueio (RN87)
// ---------------------------------------------------------------------------

interface ModalSenhaProps {
  onConfirmar: () => void;
  onCancelar: () => void;
}

function ModalSenha({ onConfirmar, onCancelar }: ModalSenhaProps) {
  const [chave, setChave] = useState("");
  const [erro, setErro] = useState<string | null>(null);
  const [verificando, setVerificando] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    inputRef.current?.focus();
  }, []);

  const handleConfirmar = async () => {
    if (!chave.trim()) return;
    setVerificando(true);
    setErro(null);
    try {
      const res = await desbloquearEdicao(chave);
      if (res.ok) {
        onConfirmar();
      } else {
        setErro("Senha incorreta");
        inputRef.current?.select();
      }
    } catch {
      setErro("Erro ao verificar senha");
    } finally {
      setVerificando(false);
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === "Enter") handleConfirmar();
    if (e.key === "Escape") onCancelar();
  };

  return (
    <div className={styles.modalOverlay}>
      <div className={styles.modalCard}>
        <h2 className={styles.modalTitulo}>Desbloquear Edição</h2>
        <p className={styles.modalDescricao}>
          Informe a senha para desbloquear a edição dos lançamentos.
        </p>
        <label htmlFor="senha-desbloqueio" className={styles.modalLabel}>Senha</label>
        <input
          id="senha-desbloqueio"
          ref={inputRef}
          type="password"
          className={styles.modalInput}
          value={chave}
          onChange={(e) => setChave(e.target.value)}
          onKeyDown={handleKeyDown}
          disabled={verificando}
          autoComplete="off"
        />
        {erro && <div className={styles.modalErro}>{erro}</div>}
        <div className={styles.modalBotoes}>
          <button
            className={styles.modalBtnConfirmar}
            onClick={handleConfirmar}
            disabled={verificando || !chave.trim()}
          >
            {verificando ? "Verificando..." : "Confirmar"}
          </button>
          <button
            className={styles.modalBtnCancelar}
            onClick={onCancelar}
            disabled={verificando}
          >
            Cancelar
          </button>
        </div>
      </div>
    </div>
  );
}

// ---------------------------------------------------------------------------
// Componente principal
// ---------------------------------------------------------------------------

interface LocationState {
  idCliente: number;
  codCliente: number;
  nomeCliente: string;
}

export default function Extrato() {
  const navigate = useNavigate();
  const location = useLocation();
  const state = location.state as LocationState | null;

  // Dados do cliente
  const idCliente = state?.idCliente ?? 0;
  const codCliente = state?.codCliente ?? 0;
  const nomeCliente = state?.nomeCliente ?? "";

  // Estado de dados
  const [lancamentos, setLancamentos] = useState<LancamentoExtratoItem[]>([]);
  const [saldoTotal, setSaldoTotal] = useState(0);
  const [loading, setLoading] = useState(true);
  const [erro, setErro] = useState<string | null>(null);

  // Filtros
  const [filtroPasta, setFiltroPasta] = useState("");
  const [filtroND, setFiltroND] = useState("");
  const [filtroSemND, setFiltroSemND] = useState(false);
  const [filtroHist, setFiltroHist] = useState("");

  // Edição inline (RN87–RN92)
  const [desbloqueado, setDesbloqueado] = useState(false);
  const [showModalSenha, setShowModalSenha] = useState(false);

  // Transferência (RN93–RN98)
  const [showTransferencia, setShowTransferencia] = useState(false);
  const [linhasSelecionadas, setLinhasSelecionadas] = useState<Set<number>>(new Set());
  const [clientesDestino, setClientesDestino] = useState<ClienteDestinoItem[]>([]);
  const [idDestino, setIdDestino] = useState<number | "">("");
  const [senhaTransf, setSenhaTransf] = useState("");
  const [transferindo, setTransferindo] = useState(false);
  const [msgSucesso, setMsgSucesso] = useState<string | null>(null);

  // Refs
  const tabelaRef = useRef<HTMLTableElement>(null);
  const idClienteRef = useRef(idCliente);
  idClienteRef.current = idCliente;

  // ---------------------------------------------------------------------------
  // Carregar extrato
  // ---------------------------------------------------------------------------

  const carregarExtrato = useCallback(
    async (filtros?: ExtratoFiltros) => {
      if (!idCliente) return;
      setLoading(true);
      setErro(null);
      try {
        const data = await listarExtrato(idCliente, filtros);
        setLancamentos(data.lancamentos);
        setSaldoTotal(data.saldo_total);
      } catch (err) {
        setErro(err instanceof Error ? err.message : "Erro ao carregar extrato");
      } finally {
        setLoading(false);
      }
    },
    [idCliente],
  );

  // Load inicial
  useEffect(() => {
    if (!idCliente) {
      navigate("/principal", { replace: true });
      return;
    }
    carregarExtrato();
  }, [idCliente, carregarExtrato, navigate]);

  // RN99 — sincronizar VValor ao desmontar
  useEffect(() => {
    return () => {
      if (idClienteRef.current) {
        sincronizarVValor(idClienteRef.current).catch(() => {});
      }
    };
  }, []);

  // ---------------------------------------------------------------------------
  // Keyboard shortcuts (F9, F10)
  // ---------------------------------------------------------------------------

  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === "F9") {
        e.preventDefault();
        toggleTransferencia();
      } else if (e.key === "F10") {
        e.preventDefault();
        handleToggleDesbloqueio();
      }
    };
    window.addEventListener("keydown", handleKeyDown);
    return () => window.removeEventListener("keydown", handleKeyDown);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [desbloqueado, showTransferencia]);

  // ---------------------------------------------------------------------------
  // Filtros
  // ---------------------------------------------------------------------------

  const handleAplicarFiltros = () => {
    const filtros: ExtratoFiltros = {};
    if (filtroPasta.trim()) filtros.pasta = Number(filtroPasta);
    if (filtroND.trim() && !filtroSemND) filtros.nd = filtroND.trim();
    if (filtroSemND) filtros.sem_nd = true;
    if (filtroHist.trim()) filtros.hist = filtroHist.trim();
    carregarExtrato(filtros);
  };

  // ---------------------------------------------------------------------------
  // Desbloqueio / Bloqueio (RN87, RN91)
  // ---------------------------------------------------------------------------

  const handleToggleDesbloqueio = () => {
    if (desbloqueado) {
      // RN91 — bloquear + recalcular
      setDesbloqueado(false);
      handleAplicarFiltros();
    } else {
      setShowModalSenha(true);
    }
  };

  const handleDesbloquearConfirmado = () => {
    setShowModalSenha(false);
    setDesbloqueado(true);
  };

  // ---------------------------------------------------------------------------
  // Edição inline (RN90)
  // ---------------------------------------------------------------------------

  const handleCellBlur = async (
    idLanc: number,
    campo: string,
    valor: string,
    valorOriginal: string,
  ) => {
    const valorAtual = valor.trim();
    const valorAnterior = valorOriginal.trim();

    if (campo === "deb" || campo === "cred") {
      if (parseValorBR(valorAtual) === parseValorBR(valorAnterior)) return;
    } else if (valorAtual === valorAnterior) {
      return;
    }

    const payload: Record<string, string | number> = {};
    if (campo === "dt") payload.dt = valorAtual;
    else if (campo === "conta") payload.conta = Number(valorAtual);
    else if (campo === "nd") payload.nd = valorAtual;
    else if (campo === "ref") payload.ref = valorAtual;
    else if (campo === "deb") payload.deb = parseValorBR(valorAtual);
    else if (campo === "cred") payload.cred = parseValorBR(valorAtual);

    try {
      await atualizarLancamento(idLanc, payload);
      // Recarrega para atualizar saldos
      handleAplicarFiltros();
    } catch (err) {
      setErro(err instanceof Error ? err.message : "Erro ao salvar edição");
    }
  };

  // ---------------------------------------------------------------------------
  // Transferência (RN93–RN98)
  // ---------------------------------------------------------------------------

  const toggleTransferencia = async () => {
    if (showTransferencia) {
      setShowTransferencia(false);
      setLinhasSelecionadas(new Set());
      setSenhaTransf("");
      setIdDestino("");
      return;
    }
    // Carregar clientes destino (RN96)
    try {
      const clientes = await listarClientesDestino(idCliente);
      setClientesDestino(clientes);
      setShowTransferencia(true);
    } catch (err) {
      setErro(err instanceof Error ? err.message : "Erro ao carregar clientes");
    }
  };

  const handleToggleLinha = (id: number) => {
    setLinhasSelecionadas((prev) => {
      const next = new Set(prev);
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });
  };

  const handleTransferir = async () => {
    if (linhasSelecionadas.size === 0) {
      setErro("Selecione ao menos um lançamento para transferir.");
      return;
    }
    if (!idDestino) {
      setErro("Selecione o cliente destino.");
      return;
    }
    if (!senhaTransf.trim()) {
      setErro("Informe a senha de autorização.");
      return;
    }

    setTransferindo(true);
    setErro(null);
    setMsgSucesso(null);
    try {
      const res = await transferirLancamentos(idCliente, {
        ids: Array.from(linhasSelecionadas),
        id_destino: Number(idDestino),
        chave: senhaTransf,
      });
      setMsgSucesso(res.mensagem);
      setShowTransferencia(false);
      setLinhasSelecionadas(new Set());
      setSenhaTransf("");
      setIdDestino("");
      // RN98 — recarregar
      handleAplicarFiltros();
    } catch (err) {
      setErro(err instanceof Error ? err.message : "Erro na transferência");
    } finally {
      setTransferindo(false);
    }
  };

  // ---------------------------------------------------------------------------
  // Navegação primeiro/último (RN101)
  // ---------------------------------------------------------------------------

  const scrollToPrimeiro = () => {
    tabelaRef.current?.querySelector("tbody tr:first-child")?.scrollIntoView({ behavior: "smooth", block: "center" });
  };

  const scrollToUltimo = () => {
    tabelaRef.current?.querySelector("tbody tr:last-child")?.scrollIntoView({ behavior: "smooth", block: "center" });
  };

  // ---------------------------------------------------------------------------
  // Render
  // ---------------------------------------------------------------------------

  if (!idCliente) return null;

  return (
    <div className={styles.container}>
      <div className={styles.card}>
        {/* Header com nome do cliente */}
        <h1 className={styles.headerCliente}>
          {nomeCliente} - {codCliente}
        </h1>

        {/* Barra de filtros */}
        <div className={styles.filtros}>
          <label className={styles.filtroLabel}>
            Pasta
            <input
              className={styles.filtroInput}
              type="number"
              value={filtroPasta}
              onChange={(e) => setFiltroPasta(e.target.value)}
              min={1}
            />
          </label>
          <label className={styles.filtroLabel}>
            N.D.
            <input
              className={styles.filtroInput}
              type="text"
              value={filtroND}
              onChange={(e) => setFiltroND(e.target.value)}
              disabled={filtroSemND}
            />
          </label>
          <label className={styles.filtroLabel}>
            Histórico
            <input
              className={styles.filtroInput}
              type="text"
              value={filtroHist}
              onChange={(e) => setFiltroHist(e.target.value)}
              style={{ width: 180 }}
            />
          </label>
          <label className={styles.filtroCheckbox}>
            <input
              type="checkbox"
              checked={filtroSemND}
              onChange={(e) => {
                setFiltroSemND(e.target.checked);
                if (e.target.checked) setFiltroND("");
              }}
            />
            Sem N.D.
          </label>
          <button className={styles.btnFiltrar} onClick={handleAplicarFiltros}>
            Aplicar filtros
          </button>
        </div>

        {/* Barra de ações */}
        <div className={styles.acoes}>
          {/* RN100 — D1 resolvido: navega para /extrato/imprimir com filtros ativos (B81–B85) */}
          <button
            className={styles.btnAcao}
            onClick={() =>
              navigate("/extrato/imprimir", {
                state: {
                  extratoImprimir: {
                    idCliente,
                    codCliente,
                    nomeCliente,
                    filtros: {
                      pasta: filtroPasta.trim() ? Number(filtroPasta) : undefined,
                      nd: filtroND.trim() && !filtroSemND ? filtroND.trim() : undefined,
                      sem_nd: filtroSemND || undefined,
                      hist: filtroHist.trim() || undefined,
                    },
                  },
                },
              })
            }
          >
            <IconePrinter /> Imprimir
          </button>
          <button
            className={showTransferencia ? styles.btnAcaoAtivo : styles.btnAcao}
            onClick={toggleTransferencia}
          >
            <IconeTransfer /> Transferir (F9)
          </button>
          <button
            className={desbloqueado ? styles.btnAcaoAtivo : styles.btnAcao}
            onClick={handleToggleDesbloqueio}
          >
            {desbloqueado ? <IconeUnlock /> : <IconeLock />}
            {desbloqueado ? "Bloquear (F10)" : "Desbloquear (F10)"}
          </button>
          <button className={styles.btnAcao} onClick={scrollToUltimo}>
            <IconeDown /> Último
          </button>
        </div>

        {/* Mensagens */}
        {erro && <div className={styles.erro}>{erro}</div>}
        {msgSucesso && <div className={styles.sucesso}>{msgSucesso}</div>}

        {/* Painel de transferência (RN93–RN98) */}
        {showTransferencia && (
          <div className={styles.painelTransferencia}>
            <h3 className={styles.painelTransferenciaTitulo}>
              Transferir os lançamentos selecionados para o cliente abaixo:
            </h3>
            <div className={styles.painelTransferenciaForm}>
              <label className={styles.painelTransferenciaLabel}>
                Cliente destino
                <select
                  className={styles.selectCliente}
                  value={idDestino}
                  onChange={(e) => setIdDestino(e.target.value ? Number(e.target.value) : "")}
                >
                  <option value="">Selecione...</option>
                  {clientesDestino.map((c) => (
                    <option key={c.id} value={c.id}>
                      {c.cliente} ({c.codigo})
                    </option>
                  ))}
                </select>
              </label>
              <label className={styles.painelTransferenciaLabel} style={{ flex: "0 0 auto" }}>
                Senha
                <input
                  type="password"
                  className={styles.inputSenha}
                  value={senhaTransf}
                  onChange={(e) => setSenhaTransf(e.target.value)}
                  autoComplete="off"
                />
              </label>
              <button
                className={styles.btnTransferir}
                onClick={handleTransferir}
                disabled={transferindo || linhasSelecionadas.size === 0}
              >
                {transferindo ? "Transferindo..." : `Transferir (${linhasSelecionadas.size})`}
              </button>
            </div>
          </div>
        )}

        {/* Tabela de lançamentos */}
        {loading ? (
          <div className={styles.loading}>Carregando extrato...</div>
        ) : (
          <div className={styles.tabelaWrapper}>
            <table className={styles.tabela} ref={tabelaRef}>
              <thead>
                <tr>
                  {showTransferencia && <th style={{ width: 30 }}></th>}
                  <th className={styles.colData}>Data</th>
                  <th className={styles.colPasta}>Pasta</th>
                  <th className={styles.colND}>N.D.</th>
                  <th className={styles.colHistorico}>Histórico</th>
                  <th className={styles.colValor}>Débito</th>
                  <th className={styles.colValor}>Crédito</th>
                  <th className={styles.colSaldo}>Saldo</th>
                </tr>
              </thead>
              <tbody>
                {lancamentos.map((l) => {
                  const isSelected = linhasSelecionadas.has(l.id);
                  const saldoClass =
                    l.saldo >= 0 ? styles.rowSaldoPositivo : styles.rowSaldoNegativo;
                  // B55 — "AGENCIA" em vermelho no histórico
                  const isAgencia = l.ref?.toUpperCase().includes("AGENCIA");

                  return (
                    <tr
                      key={l.id}
                      className={`${saldoClass} ${isSelected ? styles.rowSelected : ""}`}
                    >
                      {showTransferencia && (
                        <td>
                          <input
                            type="checkbox"
                            className={styles.checkboxSel}
                            checked={isSelected}
                            onChange={() => handleToggleLinha(l.id)}
                          />
                        </td>
                      )}
                      {/* Data — RN88: editável após desbloqueio */}
                      <td className={styles.colData}>
                        {desbloqueado ? (
                          <input
                            className={styles.inputInline}
                            type="date"
                            defaultValue={formatarDataInput(l.dt)}
                            onBlur={(e) =>
                              handleCellBlur(l.id, "dt", e.target.value, formatarDataInput(l.dt))
                            }
                            style={{ width: 110 }}
                          />
                        ) : (
                          formatarData(l.dt)
                        )}
                      </td>
                      {/* Pasta — RN88 */}
                      <td className={styles.colPasta}>
                        {desbloqueado ? (
                          <input
                            className={styles.inputInline}
                            type="number"
                            defaultValue={l.conta}
                            onBlur={(e) =>
                              handleCellBlur(l.id, "conta", e.target.value, String(l.conta))
                            }
                            style={{ width: 50, textAlign: "center" }}
                          />
                        ) : (
                          l.conta
                        )}
                      </td>
                      {/* N.D. — RN88: sempre editável quando desbloqueado */}
                      <td className={styles.colND}>
                        {desbloqueado ? (
                          <input
                            className={styles.inputInline}
                            type="text"
                            defaultValue={l.nd ?? ""}
                            onBlur={(e) =>
                              handleCellBlur(l.id, "nd", e.target.value, l.nd ?? "")
                            }
                            style={{ width: 70, textAlign: "center" }}
                          />
                        ) : (
                          l.nd ?? ""
                        )}
                      </td>
                      {/* Histórico — RN88 */}
                      <td className={styles.colHistorico}>
                        {desbloqueado ? (
                          <input
                            className={styles.inputInline}
                            type="text"
                            defaultValue={l.ref ?? ""}
                            onBlur={(e) =>
                              handleCellBlur(l.id, "ref", e.target.value, l.ref ?? "")
                            }
                          />
                        ) : (
                          <span className={isAgencia ? styles.historicoDest : undefined}>
                            {l.ref ?? ""}
                          </span>
                        )}
                      </td>
                      {/* Débito — RN88 */}
                      <td className={styles.colValor}>
                        {desbloqueado ? (
                          <input
                            className={styles.inputInline}
                            type="text"
                            inputMode="decimal"
                            defaultValue={l.deb != null ? formatarValor(l.deb) : ""}
                            onBlur={(e) =>
                              handleCellBlur(
                                l.id,
                                "deb",
                                e.target.value,
                                l.deb != null ? formatarValor(l.deb) : "",
                              )
                            }
                            style={{ width: 90, textAlign: "right" }}
                          />
                        ) : (
                          formatarValor(l.deb)
                        )}
                      </td>
                      {/* Crédito — RN88 */}
                      <td className={styles.colValor}>
                        {desbloqueado ? (
                          <input
                            className={styles.inputInline}
                            type="text"
                            inputMode="decimal"
                            defaultValue={l.cred != null ? formatarValor(l.cred) : ""}
                            onBlur={(e) =>
                              handleCellBlur(
                                l.id,
                                "cred",
                                e.target.value,
                                l.cred != null ? formatarValor(l.cred) : "",
                              )
                            }
                            style={{ width: 90, textAlign: "right" }}
                          />
                        ) : (
                          formatarValor(l.cred)
                        )}
                      </td>
                      {/* Saldo — RN89: sempre readonly */}
                      <td className={styles.colSaldo}>{formatarValor(l.saldo)}</td>
                    </tr>
                  );
                })}
                {lancamentos.length === 0 && (
                  <tr>
                    <td
                      colSpan={showTransferencia ? 8 : 7}
                      style={{ textAlign: "center", color: "#888", padding: "2rem 0" }}
                    >
                      Nenhum lançamento encontrado.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        )}

        {/* Rodapé com saldo total (RN85) */}
        <div className={styles.rodape}>
          <span style={{ fontSize: "0.8125rem", color: "#666" }}>
            {lancamentos.length} lançamento(s)
          </span>
          <span className={saldoTotal >= 0 ? styles.saldoCredor : styles.saldoDevedor}>
            Saldo: {formatarValor(saldoTotal)}
          </span>
        </div>

        {/* Ações de rodapé */}
        <div className={styles.rodapeAcoes}>
          <button className={styles.btnAcao} onClick={scrollToPrimeiro}>
            <IconeUp /> Primeiro
          </button>
          <button className={styles.btnVoltar} onClick={() => navigate("/principal")}>
            Retornar
          </button>
        </div>
      </div>

      {/* Modal de senha para desbloqueio */}
      {showModalSenha && (
        <ModalSenha
          onConfirmar={handleDesbloquearConfirmado}
          onCancelar={() => setShowModalSenha(false)}
        />
      )}
    </div>
  );
}
