/**
 * pages/Alterar/index.tsx — Equivalente web do frmAlterar
 * Sistema: Solo Consultoria de Imóveis — Contas Correntes
 *
 * Regras implementadas:
 *   RN33 — Carrega dados do cliente pelo Id via GET /clientes/{id}
 *   RN34 — Campos Código e Nome iniciam em modo somente leitura
 *   RN35 — Botão "Alterar" habilita edição com destaque de foco (borda vermelha)
 *   RN36 — Alteração de Código: modal de aviso de alto risco
 *   RN37 — Alteração de Nome: modal simples de confirmação
 *   RN38 — Após salvar: campos voltam a ReadOnly, destaque removido
 *   RN39 — Botão "Desfazer": recarrega dados do backend
 *   RN40 — Exclusão passo 1: confirmação inicial
 *   RN41 — Exclusão passo 2: modal de senha → POST /verificar-senha
 *   RN42 — Verifica tem_lancamentos antes do DELETE
 *   RN43 — Exclusão passo 3: confirmação adicional se tem lançamentos
 *   RN44 — DELETE Contas + Clientes (executado no backend)
 *   RN45 — DELETE Clientes apenas (executado no backend)
 *   RN46 — Ao salvar/excluir: invalida cache e navega para /principal
 *   RN47 — Nome em maiúsculas (CSS + backend)
 *   RN48 — Código único ao alterar (validado no backend)
 */

import { useEffect, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useQueryClient } from "@tanstack/react-query";

import {
  fetchCliente,
  atualizarCliente,
  verificarSenhaExclusao,
  excluirCliente,
  type ClienteDetail,
} from "@/services/clientes";
import styles from "./Alterar.module.css";

// ---------------------------------------------------------------------------
// Tipos de modal
// ---------------------------------------------------------------------------

type ModalState =
  | { tipo: "nenhum" }
  | { tipo: "confirmacao-nome" }
  | { tipo: "confirmacao-codigo" }
  | { tipo: "confirmacao-exclusao" }
  | { tipo: "senha" }
  | { tipo: "confirmacao-lancamentos" };

// ---------------------------------------------------------------------------
// Componente principal
// ---------------------------------------------------------------------------

export default function Alterar() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const clienteId = Number(id);

  // -------------------------------------------------------------------------
  // Estado
  // -------------------------------------------------------------------------

  const [original, setOriginal] = useState<ClienteDetail | null>(null);
  const [codigo, setCodigo] = useState("");
  const [nome, setNome] = useState("");
  const [modoEdicao, setModoEdicao] = useState(false);
  const [temLancamentos, setTemLancamentos] = useState(false);

  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);

  const [apiError, setApiError] = useState<string | null>(null);
  const [senhaInput, setSenhaInput] = useState("");
  const [senhaError, setSenhaError] = useState<string | null>(null);

  const [modal, setModal] = useState<ModalState>({ tipo: "nenhum" });

  const codigoRef = useRef<HTMLInputElement>(null);
  const senhaRef = useRef<HTMLInputElement>(null);

  // -------------------------------------------------------------------------
  // RN33 — Carregar dados do cliente
  // -------------------------------------------------------------------------

  async function carregarCliente() {
    setIsLoading(true);
    setApiError(null);
    try {
      const data = await fetchCliente(clienteId);
      setOriginal(data);
      setCodigo(String(data.codigo));
      setNome(data.cliente);
      setTemLancamentos(data.tem_lancamentos);
    } catch (err) {
      setApiError(
        err instanceof Error ? err.message : "Erro ao carregar cliente",
      );
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    carregarCliente();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [clienteId]);

  // Foca no campo Código ao entrar em modo edição (RN35)
  useEffect(() => {
    if (modoEdicao) {
      codigoRef.current?.focus();
    }
  }, [modoEdicao]);

  // Foca no campo de senha ao abrir modal de senha (RN41)
  useEffect(() => {
    if (modal.tipo === "senha") {
      setTimeout(() => senhaRef.current?.focus(), 50);
    }
  }, [modal]);

  // -------------------------------------------------------------------------
  // RN35 — Habilitar edição
  // -------------------------------------------------------------------------

  function handleAlterar() {
    setModoEdicao(true);
    setApiError(null);
  }

  // -------------------------------------------------------------------------
  // RN39 — Desfazer: recarrega do banco
  // -------------------------------------------------------------------------

  async function handleDesfazer() {
    setModoEdicao(false);
    setApiError(null);
    await carregarCliente();
  }

  // -------------------------------------------------------------------------
  // RN36/RN37 — Salvar: decide qual modal mostrar
  // -------------------------------------------------------------------------

  function handleSalvar() {
    if (!original) return;
    setApiError(null);

    const codigoMudou = Number(codigo) !== original.codigo;
    const nomeMudou = nome.toUpperCase() !== original.cliente;

    if (!codigoMudou && !nomeMudou) {
      // Nada mudou — sai do modo de edição sem chamar a API
      setModoEdicao(false);
      return;
    }

    if (codigoMudou) {
      setModal({ tipo: "confirmacao-codigo" });
    } else {
      setModal({ tipo: "confirmacao-nome" });
    }
  }

  // -------------------------------------------------------------------------
  // Confirmar alteração após modal
  // -------------------------------------------------------------------------

  async function confirmarAlteracao() {
    setModal({ tipo: "nenhum" });
    setIsSaving(true);
    setApiError(null);
    try {
      const atualizado = await atualizarCliente(clienteId, {
        codigo: Number(codigo),
        cliente: nome,
      });
      // RN38 — campos voltam a ReadOnly; RN46 — invalida cache
      setOriginal({
        ...atualizado,
        tem_lancamentos: temLancamentos,
      });
      setCodigo(String(atualizado.codigo));
      setNome(atualizado.cliente);
      setModoEdicao(false);
      queryClient.invalidateQueries({ queryKey: ["clientes"] });
      navigate("/principal");
    } catch (err) {
      setApiError(
        err instanceof Error ? err.message : "Erro ao atualizar cliente",
      );
    } finally {
      setIsSaving(false);
    }
  }

  // -------------------------------------------------------------------------
  // RN40 — Iniciar exclusão: primeira confirmação
  // -------------------------------------------------------------------------

  function handleExcluir() {
    setApiError(null);
    setModal({ tipo: "confirmacao-exclusao" });
  }

  // -------------------------------------------------------------------------
  // RN41 — Abrir modal de senha após confirmação inicial
  // -------------------------------------------------------------------------

  function handleConfirmarExclusao() {
    setSenhaInput("");
    setSenhaError(null);
    setModal({ tipo: "senha" });
  }

  // -------------------------------------------------------------------------
  // RN41 — Verificar senha
  // -------------------------------------------------------------------------

  async function handleVerificarSenha() {
    setSenhaError(null);
    setIsDeleting(true);
    try {
      const resultado = await verificarSenhaExclusao(clienteId, senhaInput);

      if (!resultado.valido) {
        setSenhaError("Senha incorreta");
        setSenhaInput("");
        senhaRef.current?.focus();
        return;
      }

      // RN42 — verificar lançamentos
      if (resultado.tem_lancamentos) {
        // RN43 — pedir confirmação adicional
        setModal({ tipo: "confirmacao-lancamentos" });
      } else {
        // RN45 — excluir diretamente
        await realizarExclusao();
      }
    } catch (err) {
      setSenhaError(
        err instanceof Error ? err.message : "Erro ao verificar senha",
      );
    } finally {
      setIsDeleting(false);
    }
  }

  // -------------------------------------------------------------------------
  // RN44/RN45 — Executar o DELETE
  // -------------------------------------------------------------------------

  async function realizarExclusao() {
    setModal({ tipo: "nenhum" });
    setIsDeleting(true);
    try {
      await excluirCliente(clienteId);
      // RN46 — invalida cache e navega
      queryClient.invalidateQueries({ queryKey: ["clientes"] });
      navigate("/principal");
    } catch (err) {
      setApiError(
        err instanceof Error ? err.message : "Erro ao excluir cliente",
      );
    } finally {
      setIsDeleting(false);
    }
  }

  function fecharModal() {
    setModal({ tipo: "nenhum" });
    setSenhaInput("");
    setSenhaError(null);
  }

  // -------------------------------------------------------------------------
  // Render de loading / erro de carga
  // -------------------------------------------------------------------------

  if (isLoading) {
    return (
      <div className={styles.container}>
        <div className={styles.card}>
          <p className={styles.loadingText}>Carregando...</p>
        </div>
      </div>
    );
  }

  if (!original) {
    return (
      <div className={styles.container}>
        <div className={styles.card}>
          <p className={styles.alertError}>
            {apiError ?? "Cliente não encontrado"}
          </p>
          <div className={styles.actions}>
            <button
              type="button"
              className={styles.btnSecondary}
              onClick={() => navigate("/principal")}
            >
              Voltar
            </button>
          </div>
        </div>
      </div>
    );
  }

  const isBusy = isSaving || isDeleting;

  // -------------------------------------------------------------------------
  // Render principal
  // -------------------------------------------------------------------------

  return (
    <div className={styles.container}>
      <div className={styles.card}>
        <h2 className={styles.title}>ALTERAÇÃO DO CLIENTE</h2>

        {/* Campo Código — RN34/RN35 */}
        <div className={styles.fieldRow}>
          <label htmlFor="codigo" className={styles.label}>
            Código
          </label>
          <div className={styles.fieldWrapper}>
            <input
              id="codigo"
              ref={codigoRef}
              type="number"
              value={codigo}
              readOnly={!modoEdicao}
              disabled={isBusy}
              onChange={(e) => setCodigo(e.target.value)}
              className={[
                styles.input,
                modoEdicao ? styles.inputEditing : "",
              ].join(" ")}
            />
          </div>
        </div>

        {/* Campo Nome — RN34/RN35/RN47 */}
        <div className={styles.fieldRow}>
          <label htmlFor="nome" className={styles.label}>
            Nome
          </label>
          <div className={styles.fieldWrapper}>
            <input
              id="nome"
              type="text"
              value={nome}
              readOnly={!modoEdicao}
              disabled={isBusy}
              onChange={(e) => setNome(e.target.value)}
              className={[
                styles.input,
                styles.uppercase,
                modoEdicao ? styles.inputEditing : "",
              ].join(" ")}
            />
          </div>
        </div>

        {/* Erro de API */}
        {apiError && (
          <p className={styles.alertError} role="alert">
            {apiError}
          </p>
        )}

        {/* Botões de ação — RN35/RN38/RN39 */}
        <div className={styles.actions}>
          {!modoEdicao ? (
            <button
              type="button"
              onClick={handleAlterar}
              disabled={isBusy}
              className={styles.btnPrimary}
            >
              Alterar
            </button>
          ) : (
            <>
              <button
                type="button"
                onClick={handleSalvar}
                disabled={isBusy}
                className={styles.btnPrimary}
              >
                {isSaving ? "Salvando..." : "Salvar"}
              </button>
              <button
                type="button"
                onClick={handleDesfazer}
                disabled={isBusy}
                className={styles.btnSecondary}
              >
                Desfazer
              </button>
            </>
          )}
          <button
            type="button"
            onClick={() => navigate("/principal")}
            disabled={isBusy}
            className={styles.btnSecondary}
          >
            Retornar
          </button>
        </div>

        {/* Zona de exclusão — RN40 */}
        <hr className={styles.divider} />
        <div className={styles.deleteZone}>
          <button
            type="button"
            onClick={handleExcluir}
            disabled={isBusy || modoEdicao}
            className={styles.btnDanger}
          >
            {/* SVG: ícone de lixeira inline */}
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="14"
              height="14"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
              strokeLinecap="round"
              strokeLinejoin="round"
              aria-hidden="true"
              style={{ marginRight: "0.375rem", verticalAlign: "middle" }}
            >
              <polyline points="3 6 5 6 21 6" />
              <path d="M19 6l-1 14H6L5 6" />
              <path d="M10 11v6M14 11v6" />
              <path d="M9 6V4h6v2" />
            </svg>
            {isDeleting ? "Excluindo..." : "Excluir cliente"}
          </button>
        </div>
      </div>

      {/* ------------------------------------------------------------------ */}
      {/* Modais                                                              */}
      {/* ------------------------------------------------------------------ */}

      {/* RN37 — confirmação simples de alteração de nome */}
      {modal.tipo === "confirmacao-nome" && (
        <div className={styles.modalOverlay} role="dialog" aria-modal="true">
          <div className={styles.modal}>
            <h3 className={styles.modalTitle}>Confirmar alteração</h3>
            <p className={styles.modalBody}>
              Confirma a alteração do nome do cliente{" "}
              <strong>{original.cliente}</strong>?
            </p>
            <div className={styles.modalActions}>
              <button
                type="button"
                onClick={fecharModal}
                className={styles.btnSecondary}
              >
                Não
              </button>
              <button
                type="button"
                onClick={confirmarAlteracao}
                className={styles.btnPrimary}
              >
                Sim
              </button>
            </div>
          </div>
        </div>
      )}

      {/* RN36 — confirmação de alto risco para alteração de código */}
      {modal.tipo === "confirmacao-codigo" && (
        <div className={styles.modalOverlay} role="dialog" aria-modal="true">
          <div className={`${styles.modal} ${styles.modalDanger}`}>
            <h3 className={styles.modalTitle}>
              {/* SVG: ícone de atenção */}
              <svg
                xmlns="http://www.w3.org/2000/svg"
                width="16"
                height="16"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                strokeWidth="2"
                strokeLinecap="round"
                strokeLinejoin="round"
                aria-hidden="true"
                style={{ marginRight: "0.5rem", verticalAlign: "middle" }}
              >
                <path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z" />
                <line x1="12" y1="9" x2="12" y2="13" />
                <line x1="12" y1="17" x2="12.01" y2="17" />
              </svg>
              A T E N Ç Ã O — Alteração de Código
            </h3>
            <p className={styles.modalBody}>
              Você está prestes a alterar o <strong>Código</strong> do cliente{" "}
              <strong>{original.cliente}</strong>.
              <br />
              <br />
              Esta é uma operação de <strong>alto risco</strong>: o código é
              usado como chave de referência nos lançamentos. Confirma a
              alteração?
            </p>
            <div className={styles.modalActions}>
              <button
                type="button"
                onClick={fecharModal}
                className={styles.btnSecondary}
              >
                Cancelar
              </button>
              <button
                type="button"
                onClick={confirmarAlteracao}
                className={styles.btnDanger}
              >
                Sim, alterar código
              </button>
            </div>
          </div>
        </div>
      )}

      {/* RN40 — confirmação inicial de exclusão */}
      {modal.tipo === "confirmacao-exclusao" && (
        <div className={styles.modalOverlay} role="dialog" aria-modal="true">
          <div className={styles.modal}>
            <h3 className={styles.modalTitle}>Confirmar exclusão</h3>
            <p className={styles.modalBody}>
              Confirma exclusão do cliente{" "}
              <strong>{original.cliente}</strong>?
            </p>
            <div className={styles.modalActions}>
              <button
                type="button"
                onClick={fecharModal}
                className={styles.btnSecondary}
              >
                Não
              </button>
              <button
                type="button"
                onClick={handleConfirmarExclusao}
                className={styles.btnDanger}
              >
                Sim
              </button>
            </div>
          </div>
        </div>
      )}

      {/* RN41 — modal de senha */}
      {modal.tipo === "senha" && (
        <div className={styles.modalOverlay} role="dialog" aria-modal="true">
          <div className={styles.modal}>
            <h3 className={styles.modalTitle}>Senha de exclusão</h3>
            <p className={styles.modalBody}>
              Informe a senha para confirmar a exclusão do cliente{" "}
              <strong>{original.cliente}</strong>:
            </p>
            <input
              ref={senhaRef}
              type="password"
              value={senhaInput}
              onChange={(e) => setSenhaInput(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === "Enter") handleVerificarSenha();
              }}
              className={styles.passwordInput}
              placeholder="Senha"
              disabled={isDeleting}
            />
            {senhaError && (
              <p className={styles.alertError} role="alert">
                {senhaError}
              </p>
            )}
            <div className={styles.modalActions}>
              <button
                type="button"
                onClick={fecharModal}
                disabled={isDeleting}
                className={styles.btnSecondary}
              >
                Cancelar
              </button>
              <button
                type="button"
                onClick={handleVerificarSenha}
                disabled={isDeleting || !senhaInput}
                className={styles.btnDanger}
              >
                {isDeleting ? "Verificando..." : "Confirmar"}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* RN43 — confirmação adicional quando tem lançamentos */}
      {modal.tipo === "confirmacao-lancamentos" && (
        <div className={styles.modalOverlay} role="dialog" aria-modal="true">
          <div className={`${styles.modal} ${styles.modalDanger}`}>
            <h3 className={styles.modalTitle}>Cliente com lançamentos</h3>
            <p className={styles.modalBody}>
              O cliente <strong>{original.cliente}</strong> tem lançamentos na
              conta corrente.
              <br />
              <br />
              Confirma a exclusão do cliente <strong>e de todos os
              lançamentos</strong>?
            </p>
            <div className={styles.modalActions}>
              <button
                type="button"
                onClick={fecharModal}
                className={styles.btnSecondary}
              >
                Não
              </button>
              <button
                type="button"
                onClick={realizarExclusao}
                className={styles.btnDanger}
              >
                Sim, excluir tudo
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
