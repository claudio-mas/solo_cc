/**
 * pages/ClienteNovo/index.tsx — Equivalente web do frmClienteNovo
 * Sistema: Solo Consultoria de Imóveis — Contas Correntes
 *
 * Regras implementadas:
 *   RN21 — Código deve ser único (validação server-side, onBlur + submit)
 *   RN22 — Código máximo: 20.000 (validação client + server)
 *   RN23 — Geração automática via GET /clientes/proximo-codigo
 *   RN24 — Campo Cliente obrigatório
 *   RN25 — Nome convertido para maiúsculas (CSS + backend)
 *   RN26 — Campo Código obrigatório
 *   RN27 — Ao gerar código, foca campo Nome
 *   RN28 — Validação de código disparada no onBlur
 *   RN29 — Código duplicado: mensagem + foco no campo
 *   RN30 — Código > 20.000: mensagem + limpa + foco
 *   RN31 — Enter = Tab (avança foco)
 *   RN32 — Após salvar, invalida cache de clientes (React Query)
 */

import { useRef, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useNavigate } from "react-router-dom";
import { useQueryClient } from "@tanstack/react-query";

import {
  fetchProximoCodigo,
  criarCliente,
} from "@/services/clientes";
import styles from "./ClienteNovo.module.css";

// ---------------------------------------------------------------------------
// Schema Zod — RN22, RN24, RN26
// ---------------------------------------------------------------------------

const clienteSchema = z.object({
  codigo: z
    .number({ invalid_type_error: "Por favor, informe o código do cliente" })
    .int()
    .gte(10000, "Código mínimo: 10.000")
    .lte(20000, "Tamanho máximo de Código: 20.000"),
  cliente: z
    .string()
    .min(1, "Por favor, informe o nome do cliente"),
});

type ClienteFormData = z.infer<typeof clienteSchema>;

// ---------------------------------------------------------------------------
// Componente
// ---------------------------------------------------------------------------

export default function ClienteNovo() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const codigoRef = useRef<HTMLInputElement>(null);
  const clienteRef = useRef<HTMLInputElement>(null);

  const [isGenerating, setIsGenerating] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [apiError, setApiError] = useState<string | null>(null);
  const [codigoBlurError, setCodigoBlurError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    setValue,
    setFocus,
    getValues,
    formState: { errors },
  } = useForm<ClienteFormData>({
    resolver: zodResolver(clienteSchema),
  });

  // Registra refs manualmente para ter acesso ao foco programático
  const { ref: codigoRegRef, ...codigoRegRest } = register("codigo", {
    valueAsNumber: true,
  });
  const { ref: clienteRegRef, ...clienteRegRest } = register("cliente");

  // -------------------------------------------------------------------------
  // RN23, RN27 — Gerar código automaticamente
  // -------------------------------------------------------------------------
  async function handleGerarCodigo() {
    setApiError(null);
    setCodigoBlurError(null);
    setIsGenerating(true);
    try {
      const data = await fetchProximoCodigo();
      setValue("codigo", data.proximo_codigo, { shouldValidate: true });
      // RN27 — foca campo Nome após gerar código
      clienteRef.current?.focus();
    } catch (err) {
      setApiError(
        err instanceof Error ? err.message : "Erro ao gerar código",
      );
    } finally {
      setIsGenerating(false);
    }
  }

  // -------------------------------------------------------------------------
  // RN28 — Validação de código no onBlur (unicidade + limite)
  // -------------------------------------------------------------------------
  async function handleCodigoBlur() {
    setCodigoBlurError(null);
    const rawValue = getValues("codigo");
    if (rawValue === undefined || isNaN(rawValue)) return;
    const codigo = Number(rawValue);

    // RN30 — limite 20.000
    if (codigo > 20000) {
      setCodigoBlurError("Tamanho máximo de Código: 20.000");
      setValue("codigo", undefined as unknown as number);
      codigoRef.current?.focus();
      return;
    }

    if (codigo < 10000) return; // ainda preenchendo

    // RN21/RN29 — unicidade via API
    try {
      const res = await (
        await import("@/lib/api")
      ).apiFetch(`/clientes/`);
      if (res.ok) {
        const data = await res.json();
        const exists = data.clientes?.some(
          (c: { codigo: number }) => c.codigo === codigo,
        );
        if (exists) {
          setCodigoBlurError("Código já existente");
          codigoRef.current?.focus();
        }
      }
    } catch {
      // Falha de rede — validação será feita no submit
    }
  }

  // -------------------------------------------------------------------------
  // Submit — RN21, RN22, RN24, RN25, RN26
  // -------------------------------------------------------------------------
  async function onSubmit(formData: ClienteFormData) {
    setApiError(null);
    setCodigoBlurError(null);
    setIsSaving(true);
    try {
      await criarCliente({
        codigo: formData.codigo,
        cliente: formData.cliente,
      });
      // RN32 — invalida cache para atualizar grid do FrmPrincipal
      queryClient.invalidateQueries({ queryKey: ["clientes"] });
      navigate("/principal");
    } catch (err) {
      const msg =
        err instanceof Error ? err.message : "Erro ao criar cliente";
      setApiError(msg);
      // RN29 — se código duplicado, foco no campo
      if (msg.includes("Código já existente")) {
        codigoRef.current?.focus();
      }
    } finally {
      setIsSaving(false);
    }
  }

  // -------------------------------------------------------------------------
  // RN26/RN24 — foco no primeiro campo com erro
  // -------------------------------------------------------------------------
  function onInvalid() {
    if (errors.codigo) {
      setFocus("codigo");
    } else if (errors.cliente) {
      clienteRef.current?.focus();
    }
  }

  // -------------------------------------------------------------------------
  // Cancelar — volta para tela principal
  // -------------------------------------------------------------------------
  function handleCancelar() {
    navigate("/principal");
  }

  // -------------------------------------------------------------------------
  // RN31 — Enter = Tab (avança foco)
  // -------------------------------------------------------------------------
  function handleCodigoKeyDown(e: React.KeyboardEvent) {
    if (e.key === "Enter") {
      e.preventDefault();
      clienteRef.current?.focus();
    }
  }

  const isLoading = isGenerating || isSaving;

  // -------------------------------------------------------------------------
  // Render
  // -------------------------------------------------------------------------
  return (
    <div className={styles.container}>
      <div className={styles.card}>
        {/* Título — equivalente a lbCliente (B18 PRESERVAR) */}
        <h2 className={styles.title}>NOVO CLIENTE</h2>

        <form onSubmit={handleSubmit(onSubmit, onInvalid)} noValidate>
          {/* Campo Código + botão Gerar — RN23, RN26 */}
          <div className={styles.fieldRow}>
            <label htmlFor="codigo" className={styles.label}>
              Código
            </label>
            <div className={styles.fieldWrapper}>
              <input
                id="codigo"
                type="number"
                disabled={isLoading}
                aria-invalid={!!(errors.codigo ?? codigoBlurError)}
                {...codigoRegRest}
                ref={(el) => {
                  codigoRegRef(el);
                  (
                    codigoRef as React.MutableRefObject<HTMLInputElement | null>
                  ).current = el;
                }}
                onBlur={(e) => {
                  codigoRegRest.onBlur(e);
                  handleCodigoBlur();
                }}
                onKeyDown={handleCodigoKeyDown}
                className={[
                  styles.input,
                  errors.codigo ?? codigoBlurError ? styles.inputError : "",
                ].join(" ")}
              />
              {/* RN29/RN30 — erros de validação onBlur têm prioridade */}
              {codigoBlurError ? (
                <span className={styles.errorText}>{codigoBlurError}</span>
              ) : (
                // RN26 — mensagem de campo obrigatório
                errors.codigo && (
                  <span className={styles.errorText}>
                    {errors.codigo.message}
                  </span>
                )
              )}
            </div>
            {/* Botão "Gerar código" — RN23 */}
            <button
              type="button"
              onClick={handleGerarCodigo}
              disabled={isLoading}
              className={styles.btnGerar}
            >
              {isGenerating ? "..." : "Gerar código"}
            </button>
          </div>

          {/* Campo Nome — RN24, RN25 */}
          <div className={styles.fieldRow}>
            <label htmlFor="cliente" className={styles.label}>
              Nome
            </label>
            <div className={styles.fieldWrapper}>
              <input
                id="cliente"
                type="text"
                disabled={isLoading}
                aria-invalid={!!errors.cliente}
                {...clienteRegRest}
                ref={(el) => {
                  clienteRegRef(el);
                  (
                    clienteRef as React.MutableRefObject<HTMLInputElement | null>
                  ).current = el;
                }}
                className={[
                  styles.input,
                  styles.uppercase,
                  errors.cliente ? styles.inputError : "",
                ].join(" ")}
              />
              {/* RN24 — mensagem de campo obrigatório */}
              {errors.cliente && (
                <span className={styles.errorText}>
                  {errors.cliente.message}
                </span>
              )}
            </div>
          </div>

          {/* Erros de API */}
          {apiError && (
            <p className={styles.alertError} role="alert">
              {apiError}
            </p>
          )}

          {/* Botões de ação */}
          <div className={styles.actions}>
            <button
              type="submit"
              disabled={isLoading}
              className={styles.btnPrimary}
            >
              {isSaving ? "Salvando..." : "Salvar"}
            </button>
            <button
              type="button"
              disabled={isLoading}
              onClick={handleCancelar}
              className={styles.btnSecondary}
            >
              Cancelar
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
