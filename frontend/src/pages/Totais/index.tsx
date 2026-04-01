/**
 * pages/Totais/index.tsx — Equivalente web de frmTotais2
 * Sistema: Solo Consultoria de Imóveis — Contas Correntes
 *
 * Regras implementadas:
 *   RN59 — Data padrão = hoje (equivalente a Date.Today no Load)
 *   RN60 — Ao alterar o date picker, busca novamente via GET /totais?data=
 *   RN61 — Credores: clientes onde TC > TD acumulado até a data
 *   RN62 — Devedores: clientes onde TD > TC acumulado até a data
 *   RN63 — qtde_credores: COUNT(CodCliente) WHERE TC > TD
 *   RN64 — valor_credores: SUM(TC − TD) WHERE TC > TD
 *   RN65 — qtde_devedores: COUNT(CodCliente) WHERE TD > TC
 *   RN66 — valor_devedores: SUM(TD − TC) WHERE TD > TC
 *   RN67 — Filtro: WHERE Dt <= data (acumulado até a data, inclusive)
 *
 * B03 — date picker inicializado com hoje (PRESERVAR)
 * B05 — FormatCurrency → Intl.NumberFormat BRL (ADAPTAR)
 * B06 — FormatNumber(0) → Intl.NumberFormat sem decimais (ADAPTAR)
 * B07 — cores: credores=azul, devedores=vermelho (ADAPTAR)
 * B08 — nota "Somente clientes a partir de 10000" (PRESERVAR)
 * B13 — botão Voltar → navigate('/principal') (ADAPTAR)
 */

import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

import { buscarTotais, TotaisResponse } from "@/services/totais";
import styles from "./Totais.module.css";

// ---------------------------------------------------------------------------
// Helpers de formatação
// ---------------------------------------------------------------------------

function formatarMoeda(valor: string | number): string {
  // B05 — equivalente a FormatCurrency(value, 2) do legado
  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
  }).format(Number(valor));
}

function formatarQuantidade(qtde: number): string {
  // B06 — equivalente a FormatNumber(value, 0) do legado
  return new Intl.NumberFormat("pt-BR", { maximumFractionDigits: 0 }).format(
    qtde,
  );
}

function hoje(): string {
  return new Date().toISOString().slice(0, 10);
}

// ---------------------------------------------------------------------------
// SVG inline — ícones sem bibliotecas externas
// ---------------------------------------------------------------------------

function IconeCredor() {
  return (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      width="24"
      height="24"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
      aria-hidden="true"
    >
      <polyline points="23 6 13.5 15.5 8.5 10.5 1 18" />
      <polyline points="17 6 23 6 23 12" />
    </svg>
  );
}

function IconeDevedor() {
  return (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      width="24"
      height="24"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
      aria-hidden="true"
    >
      <polyline points="23 18 13.5 8.5 8.5 13.5 1 6" />
      <polyline points="17 18 23 18 23 12" />
    </svg>
  );
}

// ---------------------------------------------------------------------------
// Componente principal
// ---------------------------------------------------------------------------

export default function Totais() {
  const navigate = useNavigate();

  // RN59 — data padrão = hoje
  const [data, setData] = useState<string>(hoje());
  const [totais, setTotais] = useState<TotaisResponse | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [erro, setErro] = useState<string | null>(null);

  // RN60 — busca ao montar e ao trocar data
  useEffect(() => {
    setLoading(true);
    setErro(null);

    buscarTotais(data)
      .then((res) => {
        setTotais(res);
      })
      .catch((err: Error) => {
        if (err.message.includes("401") || err.message.toLowerCase().includes("autenti")) {
          navigate("/login", { replace: true });
        } else {
          setErro(err.message);
        }
      })
      .finally(() => {
        setLoading(false);
      });
  }, [data, navigate]);

  function handleDataChange(e: React.ChangeEvent<HTMLInputElement>) {
    setData(e.target.value);
  }

  return (
    <div className={styles.container}>
      <div className={styles.card}>
        {/* Cabeçalho */}
        <h1 className={styles.titulo}>TOTAIS DE CLIENTES</h1>

        {/* Date picker — B03/RN59/RN60 */}
        <div className={styles.datepickerRow}>
          <label htmlFor="data-consulta" className={styles.labelData}>
            Data:
          </label>
          <input
            id="data-consulta"
            type="date"
            value={data}
            onChange={handleDataChange}
            className={styles.inputData}
          />
        </div>

        {/* Estado de loading */}
        {loading && (
          <p className={styles.loading} aria-live="polite">
            Carregando...
          </p>
        )}

        {/* Erro */}
        {!loading && erro && (
          <p className={styles.erro} role="alert">
            {erro}
          </p>
        )}

        {/* 4 cards de métricas */}
        {!loading && !erro && totais && (
          <div className={styles.grid}>
            {/* Card: Quantidade credores — RN63 */}
            <div className={`${styles.metricCard} ${styles.credores}`}>
              <div className={styles.metricIcone}>
                <IconeCredor />
              </div>
              <span className={styles.metricLabel}>Credores</span>
              <span className={styles.metricValorQtde}>
                {formatarQuantidade(totais.qtde_credores)}
              </span>
              <span className={styles.metricSubLabel}>clientes</span>
            </div>

            {/* Card: Valor total credores — RN64 */}
            <div className={`${styles.metricCard} ${styles.credores}`}>
              <div className={styles.metricIcone}>
                <IconeCredor />
              </div>
              <span className={styles.metricLabel}>Total Credores</span>
              <span className={styles.metricValorMoeda}>
                {formatarMoeda(totais.valor_credores)}
              </span>
              <span className={styles.metricSubLabel}>saldo credor</span>
            </div>

            {/* Card: Quantidade devedores — RN65 */}
            <div className={`${styles.metricCard} ${styles.devedores}`}>
              <div className={styles.metricIcone}>
                <IconeDevedor />
              </div>
              <span className={styles.metricLabel}>Devedores</span>
              <span className={styles.metricValorQtde}>
                {formatarQuantidade(totais.qtde_devedores)}
              </span>
              <span className={styles.metricSubLabel}>clientes</span>
            </div>

            {/* Card: Valor total devedores — RN66 */}
            <div className={`${styles.metricCard} ${styles.devedores}`}>
              <div className={styles.metricIcone}>
                <IconeDevedor />
              </div>
              <span className={styles.metricLabel}>Total Devedores</span>
              <span className={styles.metricValorMoeda}>
                {formatarMoeda(totais.valor_devedores)}
              </span>
              <span className={styles.metricSubLabel}>saldo devedor</span>
            </div>
          </div>
        )}

        {/* Nota informacional — B08 */}
        <p className={styles.nota}>Somente clientes a partir de 10000</p>

        {/* Botão Voltar — B13 */}
        <button
          type="button"
          className={styles.btnVoltar}
          onClick={() => navigate("/principal")}
        >
          Voltar
        </button>
      </div>
    </div>
  );
}
