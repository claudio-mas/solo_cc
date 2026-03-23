/**
 * pages/Lanca/index.tsx — Equivalente web de FrmLancaData + FrmLanca
 * Sistema: Solo Consultoria de Imóveis — Contas Correntes
 *
 * Regras implementadas:
 *   RN49 — FrmLancaData adaptado como date picker integrado na tela
 *   RN50 — Dados do cliente e data recebidos via route state
 *   RN51 — Pasta validada no onBlur: GET /lancamentos/verificar-pasta +
 *           modal "É uma nova pasta?" — "Não" limpa campo e refoca
 *   RN52 — D/C implementado como <select> com opções D e C
 *   RN53 — DC→Deb/Cred executado no backend (service)
 *   RN54 — Todos os campos obrigatórios validados antes de salvar
 *   RN55 — Botões Salvar/Desfazer ocultos até user preencher qualquer campo
 *   RN56 — Botão "Novo" limpa form mantendo cliente e data; foca em Pasta
 *   RN57 — Botão "Desfazer" restaura estado inicial
 *   RN58 — Após salvar: Retornar + Novo visíveis; Salvar/Desfazer ocultos
 *
 * D4 (resolvido 2026-03-22): Enter = Tab em todos os campos.
 */

import React, { useEffect, useRef, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";

import { registrarLancamento, verificarPasta } from "@/services/lancamentos";
import styles from "./Lanca.module.css";

// ---------------------------------------------------------------------------
// Tipos
// ---------------------------------------------------------------------------

interface ClienteState {
  id: number;
  codigo: number;
  cliente: string;
}

type ModalState = { tipo: "nenhum" } | { tipo: "nova-pasta" };

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

function hoje(): string {
  return new Date().toISOString().slice(0, 10);
}

// ---------------------------------------------------------------------------
// Componente principal
// ---------------------------------------------------------------------------

export default function Lanca() {
  const navigate = useNavigate();
  const location = useLocation();
  const state = location.state as ClienteState | null;

  // RN50 — se não veio state válido, redireciona para /principal
  useEffect(() => {
    if (!state?.id) {
      navigate("/principal", { replace: true });
    }
  }, [state, navigate]);

  // -------------------------------------------------------------------------
  // Estado dos campos
  // -------------------------------------------------------------------------

  const dataInicial = hoje();

  const [dt, setDt] = useState<string>(dataInicial);
  const [pasta, setPasta] = useState<string>("");
  const [ref, setRef] = useState<string>("");
  const [vvalor, setVvalor] = useState<string>("");
  const [dc, setDc] = useState<"D" | "C" | "">("");

  // RN55 — controla visibilidade dos botões
  const [dirty, setDirty] = useState(false);
  // RN58 — estado após salvar com sucesso
  const [saved, setSaved] = useState(false);

  // RN51 — flag para evitar re-validação na mesma pasta já confirmada
  const pastaConfirmadaRef = useRef<number | null>(null);

  const [isSaving, setIsSaving] = useState(false);
  const [apiError, setApiError] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
  const [modal, setModal] = useState<ModalState>({ tipo: "nenhum" });

  // Refs para Enter = Tab (D4)
  const pastaRef = useRef<HTMLInputElement>(null);
  const refFieldRef = useRef<HTMLInputElement>(null);
  const vvalorRef = useRef<HTMLInputElement>(null);
  const dcRef = useRef<HTMLSelectElement>(null);
  const salvarRef = useRef<HTMLButtonElement>(null);

  // -------------------------------------------------------------------------
  // Funções auxiliares
  // -------------------------------------------------------------------------

  function markDirty() {
    if (!dirty) {
      setDirty(true);
      setSaved(false);
    }
  }

  function limparCampos() {
    setPasta("");
    setRef("");
    setVvalor("");
    setDc("");
    setFieldErrors({});
    setApiError(null);
    pastaConfirmadaRef.current = null;
  }

  // -------------------------------------------------------------------------
  // RN51 — Validação de pasta no onBlur
  // -------------------------------------------------------------------------

  async function handlePastaBlur() {
    if (!pasta || !state?.id) return;

    const pastaNum = parseInt(pasta, 10);
    if (isNaN(pastaNum) || pastaNum <= 0) return;

    // Evita re-validar se já confirmou esta pasta
    if (pastaConfirmadaRef.current === pastaNum) return;

    try {
      const resultado = await verificarPasta(state.id, pastaNum);
      if (!resultado.existe) {
        setModal({ tipo: "nova-pasta" });
      } else {
        pastaConfirmadaRef.current = pastaNum;
      }
    } catch {
      // Falha de rede — não bloqueia; validação continua no submit
    }
  }

  function handleConfirmarNovaPasta() {
    pastaConfirmadaRef.current = parseInt(pasta, 10);
    setModal({ tipo: "nenhum" });
    refFieldRef.current?.focus();
  }

  function handleRecusarNovaPasta() {
    setPasta("");
    setModal({ tipo: "nenhum" });
    pastaRef.current?.focus();
  }

  // -------------------------------------------------------------------------
  // RN57 — Desfazer: restaura estado inicial
  // -------------------------------------------------------------------------

  function handleDesfazer() {
    setDt(dataInicial);
    limparCampos();
    setDirty(false);
    setSaved(false);
    pastaRef.current?.focus();
  }

  // -------------------------------------------------------------------------
  // RN56 — Novo: limpa campos mantendo cliente e data
  // -------------------------------------------------------------------------

  function handleNovo() {
    limparCampos();
    setDirty(false);
    setSaved(false);
    pastaRef.current?.focus();
  }

  // -------------------------------------------------------------------------
  // Validação local — RN54
  // -------------------------------------------------------------------------

  function validar(): boolean {
    const erros: Record<string, string> = {};

    if (!pasta.trim()) erros.pasta = "Informe a pasta";
    else if (isNaN(parseInt(pasta, 10)) || parseInt(pasta, 10) <= 0)
      erros.pasta = "Pasta deve ser um número positivo";

    if (!ref.trim()) erros.ref = "Informe o histórico";

    if (!vvalor.trim()) erros.vvalor = "Informe o valor";
    else if (isNaN(parseFloat(vvalor)) || parseFloat(vvalor) <= 0)
      erros.vvalor = "Valor deve ser maior que zero";

    if (!dc) erros.dc = "Informe D ou C";

    setFieldErrors(erros);
    return Object.keys(erros).length === 0;
  }

  // -------------------------------------------------------------------------
  // Salvar — RN53/RN54/RN58
  // -------------------------------------------------------------------------

  async function handleSalvar() {
    setApiError(null);
    if (!validar() || !state?.id) return;

    setIsSaving(true);
    try {
      await registrarLancamento({
        id_cliente: state.id,
        cod_cliente: state.codigo,
        dt,
        conta: parseInt(pasta, 10),
        ref: ref.trim(),
        vvalor,
        dc: dc as "D" | "C",
      });

      // RN58 — após salvar: Retornar + Novo visíveis
      setDirty(false);
      setSaved(true);
      pastaConfirmadaRef.current = null;
    } catch (err) {
      setApiError(
        err instanceof Error ? err.message : "Erro ao registrar lançamento",
      );
    } finally {
      setIsSaving(false);
    }
  }

  // -------------------------------------------------------------------------
  // D4 — Enter = Tab em todos os campos
  // -------------------------------------------------------------------------

  function handleEnterTab(
    e: React.KeyboardEvent,
    nextRef: React.RefObject<HTMLElement | null>,
  ) {
    if (e.key === "Enter") {
      e.preventDefault();
      (nextRef.current as HTMLElement | null)?.focus();
    }
  }

  // -------------------------------------------------------------------------
  // Render
  // -------------------------------------------------------------------------

  if (!state?.id) return null;

  const isBusy = isSaving;

  // RN55 — controle de visibilidade de botões
  const mostrarSalvarDesfazer = dirty && !saved;
  const mostrarNovo = saved;
  const mostrarRetornar = !dirty || saved;

  return (
    <div className={styles.container}>
      <div className={styles.card}>
        {/* Cabeçalho — equivalente ao lbCliente + título */}
        <h2 className={styles.header}>LANÇAMENTO</h2>
        <p className={styles.clienteNome}>{state.cliente}</p>

        {apiError && <p className={styles.alertError}>{apiError}</p>}

        {saved && (
          <p className={styles.alertSuccess}>
            Lançamento registrado com sucesso.
          </p>
        )}

        {/* Campo Data — RN49 (date picker integrado) */}
        <div className={styles.fieldRow}>
          <label htmlFor="dt" className={styles.label}>
            Data
          </label>
          <div className={styles.fieldWrapper}>
            <input
              id="dt"
              type="date"
              value={dt}
              disabled={isBusy}
              onChange={(e) => {
                setDt(e.target.value);
                markDirty();
              }}
              onKeyDown={(e) => handleEnterTab(e, pastaRef)}
              className={styles.input}
            />
          </div>
        </div>

        {/* Campo Pasta — RN51 */}
        <div className={styles.fieldRow}>
          <label htmlFor="pasta" className={styles.label}>
            Pasta
          </label>
          <div className={styles.fieldWrapper}>
            <input
              id="pasta"
              type="number"
              min={1}
              value={pasta}
              disabled={isBusy}
              onChange={(e) => {
                setPasta(e.target.value);
                pastaConfirmadaRef.current = null;
                markDirty();
              }}
              onBlur={handlePastaBlur}
              onKeyDown={(e) => handleEnterTab(e, refFieldRef)}
              ref={pastaRef}
              aria-invalid={!!fieldErrors.pasta}
              className={[
                styles.input,
                fieldErrors.pasta ? styles.inputError : "",
              ].join(" ")}
            />
            {fieldErrors.pasta && (
              <span className={styles.errorText}>{fieldErrors.pasta}</span>
            )}
          </div>
        </div>

        {/* Campo Histórico (Ref) — D4: Enter = Tab */}
        <div className={styles.fieldRow}>
          <label htmlFor="ref" className={styles.label}>
            Histórico
          </label>
          <div className={styles.fieldWrapper}>
            <input
              id="ref"
              type="text"
              value={ref}
              disabled={isBusy}
              onChange={(e) => {
                setRef(e.target.value);
                markDirty();
              }}
              onKeyDown={(e) => handleEnterTab(e, vvalorRef)}
              ref={refFieldRef}
              aria-invalid={!!fieldErrors.ref}
              className={[
                styles.input,
                fieldErrors.ref ? styles.inputError : "",
              ].join(" ")}
            />
            {fieldErrors.ref && (
              <span className={styles.errorText}>{fieldErrors.ref}</span>
            )}
          </div>
        </div>

        {/* Campo Valor */}
        <div className={styles.fieldRow}>
          <label htmlFor="vvalor" className={styles.label}>
            Valor
          </label>
          <div className={styles.fieldWrapper}>
            <input
              id="vvalor"
              type="number"
              min={0.01}
              step={0.01}
              value={vvalor}
              disabled={isBusy}
              onChange={(e) => {
                setVvalor(e.target.value);
                markDirty();
              }}
              onKeyDown={(e) => handleEnterTab(e, dcRef)}
              ref={vvalorRef}
              aria-invalid={!!fieldErrors.vvalor}
              className={[
                styles.input,
                fieldErrors.vvalor ? styles.inputError : "",
              ].join(" ")}
            />
            {fieldErrors.vvalor && (
              <span className={styles.errorText}>{fieldErrors.vvalor}</span>
            )}
          </div>
        </div>

        {/* Campo D/C — RN52: select com opções D e C */}
        <div className={styles.fieldRow}>
          <label htmlFor="dc" className={styles.label}>
            D / C
          </label>
          <div className={styles.fieldWrapper}>
            <select
              id="dc"
              value={dc}
              disabled={isBusy}
              onChange={(e) => {
                setDc(e.target.value as "D" | "C" | "");
                markDirty();
              }}
              onKeyDown={(e) => handleEnterTab(e, salvarRef)}
              ref={dcRef}
              aria-invalid={!!fieldErrors.dc}
              className={[
                styles.select,
                fieldErrors.dc ? styles.inputError : "",
              ].join(" ")}
            >
              <option value="">— selecione —</option>
              <option value="D">D — Débito</option>
              <option value="C">C — Crédito</option>
            </select>
            {fieldErrors.dc && (
              <span className={styles.errorText}>{fieldErrors.dc}</span>
            )}
          </div>
        </div>

        {/* Grupo de botões — RN55/RN56/RN57/RN58 */}
        <div className={styles.actions}>
          {/* RN58 — Novo: visível após salvar */}
          {mostrarNovo && (
            <button
              type="button"
              onClick={handleNovo}
              disabled={isBusy}
              className={styles.btnSecondary}
            >
              {/* Ícone: mais / novo */}
              <svg
                xmlns="http://www.w3.org/2000/svg"
                width="13"
                height="13"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                strokeWidth="2"
                strokeLinecap="round"
                strokeLinejoin="round"
                aria-hidden="true"
                style={{ marginRight: "0.375rem", verticalAlign: "middle" }}
              >
                <line x1="12" y1="5" x2="12" y2="19" />
                <line x1="5" y1="12" x2="19" y2="12" />
              </svg>
              Novo
            </button>
          )}

          {/* RN55 — Desfazer: oculto até dirty */}
          {mostrarSalvarDesfazer && (
            <button
              type="button"
              onClick={handleDesfazer}
              disabled={isBusy}
              className={styles.btnSecondary}
            >
              Desfazer
            </button>
          )}

          {/* RN55 — Salvar: oculto até dirty */}
          {mostrarSalvarDesfazer && (
            <button
              type="button"
              ref={salvarRef}
              onClick={handleSalvar}
              disabled={isBusy}
              className={styles.btnPrimary}
            >
              {isSaving ? "Salvando..." : "Salvar"}
            </button>
          )}

          {/* Retornar: visível no estado inicial e após salvar */}
          {mostrarRetornar && (
            <button
              type="button"
              onClick={() => navigate("/principal")}
              disabled={isBusy}
              className={styles.btnSecondary}
            >
              Retornar
            </button>
          )}
        </div>
      </div>

      {/* -------------------------------------------------------------------- */}
      {/* Modal "É uma nova pasta?" — RN51                                      */}
      {/* -------------------------------------------------------------------- */}
      {modal.tipo === "nova-pasta" && (
        <div
          className={styles.modalOverlay}
          role="dialog"
          aria-modal="true"
          aria-labelledby="modal-pasta-title"
        >
          <div className={styles.modal}>
            <h3 id="modal-pasta-title" className={styles.modalTitle}>
              Atenção
            </h3>
            <p className={styles.modalBody}>
              A pasta <strong>{pasta}</strong> não existe para este cliente.{" "}
              <strong>É uma nova pasta?</strong>
            </p>
            <div className={styles.modalActions}>
              {/* RN51 — "Não": limpa pasta e refoca */}
              <button
                type="button"
                onClick={handleRecusarNovaPasta}
                className={styles.btnSecondary}
              >
                Não
              </button>
              {/* RN51 — "Sim": confirma pasta nova e avança */}
              <button
                type="button"
                onClick={handleConfirmarNovaPasta}
                className={styles.btnPrimary}
              >
                Sim
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
