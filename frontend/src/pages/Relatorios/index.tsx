/**
 * pages/Relatorios/index.tsx — Equivalente web de frmReports
 * Sistema: Solo Consultoria de Imóveis — Contas Correntes
 *
 * Regras implementadas:
 *   RN102 — Tipo de relatório selecionável: devedores (padrão) ou credores
 *   RN103 — Faixa de código: todos / >= 10000 / < 10000
 *   RN104 — Data de corte; padrão = hoje
 *   RN105 — Saldo mínimo; padrão = 0
 *   RN106 — Ordenação: por código (padrão) ou por nome
 *   RN107 — Parâmetros preservados ao voltar das telas de relatório
 *
 * B63–B66 — PRESERVAR (filtros e ordenação)
 * B62 — ADAPTAR (React Router state substitui variáveis globais WinForms)
 * B63/B64 — DESCARTAR (FrmPrincipal.Visible, SendKeys)
 */

import { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";

import type { FaixaCodigo, Ordenacao, RelatorioParams, TipoRelatorio } from "@/services/relatorios";
import styles from "./Relatorios.module.css";

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

function hoje(): string {
  return new Date().toISOString().slice(0, 10);
}

// ---------------------------------------------------------------------------
// Componente principal
// ---------------------------------------------------------------------------

export default function Relatorios() {
  const navigate = useNavigate();
  const location = useLocation();

  // RN107 — restaura parâmetros ao voltar das telas de relatório
  const prevParams = (location.state as { params?: RelatorioParams & { tipo?: TipoRelatorio } } | null)?.params;

  const [tipo, setTipo] = useState<TipoRelatorio>(prevParams?.tipo ?? "devedores");
  const [faixaCodigo, setFaixaCodigo] = useState<FaixaCodigo>(prevParams?.faixa_codigo ?? "todos");
  const [ordenacao, setOrdenacao] = useState<Ordenacao>(prevParams?.ordenacao ?? "codigo");
  const [dataCorte, setDataCorte] = useState<string>(prevParams?.data_corte ?? hoje());
  const [saldoMinimo, setSaldoMinimo] = useState<string>(
    prevParams ? String(prevParams.saldo_minimo) : "0",
  );

  function handleGerar() {
    const params: RelatorioParams & { tipo: TipoRelatorio } = {
      tipo,
      data_corte: dataCorte,
      saldo_minimo: Number(saldoMinimo) || 0,
      faixa_codigo: faixaCodigo,
      ordenacao,
    };
    navigate(`/relatorios/${tipo}`, { state: { params } });
  }

  return (
    <main className={styles.container}>
      <h1 className={styles.titulo}>RELATÓRIOS</h1>

      <div className={styles.card}>
        {/* Tipo de relatório — RN102 */}
        <fieldset className={styles.fieldset}>
          <legend className={styles.legend}>Tipo</legend>
          <label className={styles.radioLabel}>
            <input
              type="radio"
              name="tipo"
              value="devedores"
              checked={tipo === "devedores"}
              onChange={() => setTipo("devedores")}
            />
            Devedores
          </label>
          <label className={styles.radioLabel}>
            <input
              type="radio"
              name="tipo"
              value="credores"
              checked={tipo === "credores"}
              onChange={() => setTipo("credores")}
            />
            Credores
          </label>
        </fieldset>

        {/* Faixa de código — RN103 */}
        <fieldset className={styles.fieldset}>
          <legend className={styles.legend}>Faixa de código</legend>
          <label className={styles.radioLabel}>
            <input
              type="radio"
              name="faixa"
              value="todos"
              checked={faixaCodigo === "todos"}
              onChange={() => setFaixaCodigo("todos")}
            />
            Banco de dados inteiro
          </label>
          <label className={styles.radioLabel}>
            <input
              type="radio"
              name="faixa"
              value="acima"
              checked={faixaCodigo === "acima"}
              onChange={() => setFaixaCodigo("acima")}
            />
            A partir de 10000
          </label>
          <label className={styles.radioLabel}>
            <input
              type="radio"
              name="faixa"
              value="abaixo"
              checked={faixaCodigo === "abaixo"}
              onChange={() => setFaixaCodigo("abaixo")}
            />
            Abaixo de 10000
          </label>
        </fieldset>

        {/* Ordenação — RN106 */}
        <fieldset className={styles.fieldset}>
          <legend className={styles.legend}>Ordenado por</legend>
          <label className={styles.radioLabel}>
            <input
              type="radio"
              name="ordenacao"
              value="codigo"
              checked={ordenacao === "codigo"}
              onChange={() => setOrdenacao("codigo")}
            />
            Código
          </label>
          <label className={styles.radioLabel}>
            <input
              type="radio"
              name="ordenacao"
              value="nome"
              checked={ordenacao === "nome"}
              onChange={() => setOrdenacao("nome")}
            />
            Nome
          </label>
        </fieldset>

        {/* Data de corte — RN104 */}
        <div className={styles.campo}>
          <label htmlFor="dataCorte" className={styles.campoLabel}>
            Na data:
          </label>
          <input
            id="dataCorte"
            type="date"
            value={dataCorte}
            onChange={(e) => setDataCorte(e.target.value)}
            className={styles.inputData}
          />
        </div>

        {/* Saldo mínimo — RN105 */}
        <div className={styles.campo}>
          <label htmlFor="saldoMinimo" className={styles.campoLabel}>
            Com saldo maior ou igual a:
          </label>
          <input
            id="saldoMinimo"
            type="number"
            min="0"
            step="0.01"
            value={saldoMinimo}
            onChange={(e) => setSaldoMinimo(e.target.value)}
            className={styles.inputSaldo}
          />
        </div>
      </div>

      {/* Ações */}
      <div className={styles.acoes}>
        <button className={styles.btnVoltar} onClick={() => navigate("/principal")}>
          <IconeVoltar /> Voltar
        </button>
        <button className={styles.btnGerar} onClick={handleGerar}>
          <IconeRelatorio /> Gerar Relatório
        </button>
      </div>
    </main>
  );
}

// ---------------------------------------------------------------------------
// SVG inline — sem bibliotecas externas
// ---------------------------------------------------------------------------

function IconeVoltar() {
  return (
    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
      <polyline points="15 18 9 12 15 6" />
    </svg>
  );
}

function IconeRelatorio() {
  return (
    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
      <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" />
      <polyline points="14 2 14 8 20 8" />
      <line x1="16" y1="13" x2="8" y2="13" />
      <line x1="16" y1="17" x2="8" y2="17" />
      <polyline points="10 9 9 9 8 9" />
    </svg>
  );
}
