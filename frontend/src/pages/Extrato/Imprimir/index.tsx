/**
 * pages/Extrato/Imprimir/index.tsx — Equivalente web de FrmExtratoRpt
 * Sistema: Solo Consultoria de Imóveis — Contas Correntes
 *
 * Regras implementadas:
 *   RN120 — Recebe filtros ativos do FrmExtrato via React Router state
 *   RN121 — Reutiliza GET /extrato/{id_cliente} com filtros combinados
 *   RN122 — Título inclui nome e código do cliente
 *   RN123 — Saldo final colorido: >= 0 vermelho, < 0 azul
 *   RN124 — Botão Voltar navega para /extrato preservando filtros
 *
 * B81 — ADAPTAR (filtros via React Router state em vez de variáveis globais)
 * B82 — PRESERVAR (endpoint GET /extrato/{id} com filtros)
 * B83 — PRESERVAR (título com nome e código)
 * B84 — PRESERVAR (saldo colorido)
 * B85 — ADAPTAR (navigate com state em vez de FrmExtrato.Visible = True)
 * B86 — DESCARTAR (C1Report + C1PrintPreviewControl)
 * B87 — DESCARTAR (FrmExtrato.Visible = False/True)
 * B88 — DESCARTAR (CircularProgress1 WinForms)
 * B89 — ADAPTAR (título no componente React, não embutido na query)
 * B90 — ADAPTAR (todos os filtros repassados — comportamento mais completo)
 */

import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";

import { listarExtrato, LancamentoExtratoItem, ExtratoFiltros } from "@/services/extrato";
import styles from "../../Relatorios/RelatorioTabela.module.css";

// ---------------------------------------------------------------------------
// Tipos do state de rota
// ---------------------------------------------------------------------------

interface ExtratoImprimirState {
  idCliente: number;
  codCliente: number;
  nomeCliente: string;
  filtros: ExtratoFiltros;
}

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

function formatarData(dt: string): string {
  const d = new Date(dt + "T00:00:00");
  const dd = String(d.getDate()).padStart(2, "0");
  const mm = String(d.getMonth() + 1).padStart(2, "0");
  const yy = String(d.getFullYear()).slice(-2);
  return `${dd}/${mm}/${yy}`;
}

// ---------------------------------------------------------------------------
// PDF (D1 — @react-pdf/renderer)
// ---------------------------------------------------------------------------

async function exportarPDF(
  titulo: string,
  saldoTotal: number,
  itens: LancamentoExtratoItem[],
): Promise<void> {
  const { Document, Page, View, Text, StyleSheet, pdf } = await import(
    "@react-pdf/renderer"
  );

  const pdfStyles = StyleSheet.create({
    page: { padding: 28, fontFamily: "Helvetica", fontSize: 9 },
    titulo: { fontSize: 12, fontWeight: "bold", marginBottom: 12, textAlign: "center" },
    headerRow: {
      flexDirection: "row",
      backgroundColor: "#e5e7eb",
      padding: "3 4",
      fontWeight: "bold",
    },
    row: { flexDirection: "row", padding: "2 4", borderBottomWidth: 1, borderBottomColor: "#f3f4f6" },
    colData: { width: "9%" },
    colPasta: { width: "8%" },
    colND: { width: "8%" },
    colHist: { width: "35%" },
    colDeb: { width: "13%", textAlign: "right" },
    colCred: { width: "13%", textAlign: "right" },
    colSaldo: { width: "14%", textAlign: "right" },
    saldoPositivo: { color: "#b91c1c" },
    saldoNegativo: { color: "#1d4ed8" },
    rodape: { marginTop: 12, textAlign: "right", fontSize: 10, fontWeight: "bold" },
  });

  const Documento = () => (
    <Document>
      <Page size="A4" orientation="landscape" style={pdfStyles.page}>
        <Text style={pdfStyles.titulo}>{titulo}</Text>
        <View style={pdfStyles.headerRow}>
          <Text style={pdfStyles.colData}>Data</Text>
          <Text style={pdfStyles.colPasta}>Pasta</Text>
          <Text style={pdfStyles.colND}>N.D.</Text>
          <Text style={pdfStyles.colHist}>Histórico</Text>
          <Text style={pdfStyles.colDeb}>Débito</Text>
          <Text style={pdfStyles.colCred}>Crédito</Text>
          <Text style={pdfStyles.colSaldo}>Saldo</Text>
        </View>
        {itens.map((item) => (
          <View key={item.id} style={pdfStyles.row}>
            <Text style={pdfStyles.colData}>{formatarData(item.dt)}</Text>
            <Text style={pdfStyles.colPasta}>{item.conta ?? ""}</Text>
            <Text style={pdfStyles.colND}>{item.nd ?? ""}</Text>
            <Text style={pdfStyles.colHist}>{item.ref ?? ""}</Text>
            <Text style={pdfStyles.colDeb}>{item.deb ? formatarValor(item.deb) : ""}</Text>
            <Text style={pdfStyles.colCred}>{item.cred ? formatarValor(item.cred) : ""}</Text>
            <Text style={[pdfStyles.colSaldo, saldoTotal >= 0 ? pdfStyles.saldoPositivo : pdfStyles.saldoNegativo]}>
              {formatarValor(item.saldo)}
            </Text>
          </View>
        ))}
        <Text style={[pdfStyles.rodape, saldoTotal >= 0 ? pdfStyles.saldoPositivo : pdfStyles.saldoNegativo]}>
          Saldo: {formatarValor(saldoTotal)}
        </Text>
      </Page>
    </Document>
  );

  const blob = await pdf(<Documento />).toBlob();
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = "extrato.pdf";
  a.click();
  URL.revokeObjectURL(url);
}

// ---------------------------------------------------------------------------
// Componente principal
// ---------------------------------------------------------------------------

export default function ExtratoImprimir() {
  const navigate = useNavigate();
  const location = useLocation();

  const state = (location.state as { extratoImprimir?: ExtratoImprimirState } | null)
    ?.extratoImprimir;

  const [loading, setLoading] = useState(true);
  const [erro, setErro] = useState<string | null>(null);
  const [itens, setItens] = useState<LancamentoExtratoItem[]>([]);
  const [saldoTotal, setSaldoTotal] = useState(0);
  const [exportando, setExportando] = useState(false);

  useEffect(() => {
    if (!state) {
      navigate("/extrato", { replace: true });
      return;
    }
    setLoading(true);
    setErro(null);
    listarExtrato(state.idCliente, state.filtros)
      .then((res) => {
        setItens(res.lancamentos);
        setSaldoTotal(res.saldo_total);
      })
      .catch((e: Error) => setErro(e.message))
      .finally(() => setLoading(false));
  }, []);

  // RN122 — título com nome e código do cliente
  const titulo = state
    ? `${state.nomeCliente} - ${state.codCliente}`
    : "Extrato";

  async function handleExportarPDF() {
    setExportando(true);
    try {
      await exportarPDF(titulo, saldoTotal, itens);
    } finally {
      setExportando(false);
    }
  }

  function handleVoltar() {
    // RN124 — preserva filtros ao voltar para /extrato
    navigate("/extrato", { state: { idCliente: state?.idCliente, filtros: state?.filtros } });
  }

  return (
    <main className={styles.container}>
      {/* RN122 — título */}
      <h1 className={styles.titulo}>{titulo}</h1>

      {loading && <p className={styles.loading}>Carregando...</p>}
      {erro && <p className={styles.erro}>{erro}</p>}

      {!loading && !erro && (
        <>
          {itens.length === 0 ? (
            <p className={styles.vazio}>Nenhum lançamento encontrado.</p>
          ) : (
            <table className={styles.tabela}>
              <thead>
                <tr>
                  <th>Data</th>
                  <th>Pasta</th>
                  <th>N.D.</th>
                  <th>Histórico</th>
                  <th className={styles.thSaldo}>Débito</th>
                  <th className={styles.thSaldo}>Crédito</th>
                  <th className={styles.thSaldo}>Saldo</th>
                </tr>
              </thead>
              <tbody>
                {itens.map((item) => (
                  <tr key={item.id}>
                    <td>{formatarData(item.dt)}</td>
                    <td>{item.conta ?? ""}</td>
                    <td>{item.nd ?? ""}</td>
                    <td>{item.ref ?? ""}</td>
                    <td className={styles.tdSaldo}>{item.deb ? formatarValor(item.deb) : ""}</td>
                    <td className={styles.tdSaldo}>{item.cred ? formatarValor(item.cred) : ""}</td>
                    {/* RN123 — saldo colorido */}
                    <td className={`${styles.tdSaldo} ${item.saldo >= 0 ? styles.saldoPositivo : styles.saldoNegativo}`}>
                      {formatarValor(item.saldo)}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}

          {/* RN123 — saldo final colorido no rodapé */}
          <p className={`${styles.rodapeSaldo} ${saldoTotal >= 0 ? styles.saldoPositivo : styles.saldoNegativo}`}>
            Saldo: {formatarValor(saldoTotal)}
          </p>

          <div className={styles.acoes}>
            <button className={styles.btnVoltar} onClick={handleVoltar}>
              <IconeVoltar /> Voltar
            </button>
            <button
              className={styles.btnPDF}
              onClick={handleExportarPDF}
              disabled={exportando || itens.length === 0}
            >
              <IconePDF /> {exportando ? "Gerando..." : "Exportar PDF"}
            </button>
          </div>
        </>
      )}
    </main>
  );
}

// ---------------------------------------------------------------------------
// SVG inline
// ---------------------------------------------------------------------------

function IconeVoltar() {
  return (
    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
      <polyline points="15 18 9 12 15 6" />
    </svg>
  );
}

function IconePDF() {
  return (
    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
      <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" />
      <polyline points="14 2 14 8 20 8" />
      <line x1="16" y1="13" x2="8" y2="13" />
      <line x1="16" y1="17" x2="8" y2="17" />
    </svg>
  );
}
