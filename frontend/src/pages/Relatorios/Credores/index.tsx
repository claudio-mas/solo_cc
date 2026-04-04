/**
 * pages/Relatorios/Credores/index.tsx — Equivalente web de frmCredores1
 * Sistema: Solo Consultoria de Imóveis — Contas Correntes
 *
 * Regras implementadas:
 *   RN114 — Retorna clientes com SUM(Cred) > SUM(Deb) até a data de corte
 *   RN115 — Filtro por faixa de código
 *   RN116 — Filtro por saldo mínimo
 *   RN117 — Ordenação por código ou nome
 *   RN118 — Título dinâmico com data e saldo mínimo
 *   RN119 — Saldo colorido (credor)
 *   RN107 — Parâmetros preservados ao voltar (ADAPTAR — React Router state)
 *
 * B74 — PRESERVAR (query de credores)
 * B78 — DESCARTAR (C1Report + C1PrintPreviewControl)
 * B79 — DESCARTAR (frmReports.Visible)
 * B80 — ADAPTAR (tabela simples em vez de GroupBy do C1Report)
 */

import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";

import {
  buscarCredores,
  ClienteRelatorioItem,
  RelatorioParams,
  TipoRelatorio,
} from "@/services/relatorios";
import styles from "../RelatorioTabela.module.css";

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

function formatarMoeda(v: number): string {
  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
  }).format(v);
}

// ---------------------------------------------------------------------------
// PDF (D1 — @react-pdf/renderer)
// ---------------------------------------------------------------------------

async function exportarPDF(
  titulo: string,
  itens: ClienteRelatorioItem[],
): Promise<void> {
  const { Document, Page, View, Text, StyleSheet, pdf } = await import(
    "@react-pdf/renderer"
  );

  const pdfStyles = StyleSheet.create({
    page: { padding: 32, fontFamily: "Helvetica" },
    titulo: { fontSize: 13, fontWeight: "bold", marginBottom: 16, textAlign: "center" },
    headerRow: {
      flexDirection: "row",
      backgroundColor: "#e5e7eb",
      padding: "4 6",
      fontSize: 10,
      fontWeight: "bold",
    },
    row: { flexDirection: "row", padding: "3 6", fontSize: 10, borderBottomWidth: 1, borderBottomColor: "#e5e7eb" },
    colCod: { width: "15%" },
    colNome: { width: "60%" },
    colSaldo: { width: "25%", textAlign: "right" },
    saldoCreedor: { color: "#1d4ed8" },
  });

  const Documento = () => (
    <Document>
      <Page size="A4" style={pdfStyles.page}>
        <Text style={pdfStyles.titulo}>{titulo}</Text>
        <View style={pdfStyles.headerRow}>
          <Text style={pdfStyles.colCod}>Código</Text>
          <Text style={pdfStyles.colNome}>Cliente</Text>
          <Text style={pdfStyles.colSaldo}>Saldo</Text>
        </View>
        {itens.map((item) => (
          <View key={item.id} style={pdfStyles.row}>
            <Text style={pdfStyles.colCod}>{item.cod_cliente}</Text>
            <Text style={pdfStyles.colNome}>{item.cliente}</Text>
            <Text style={[pdfStyles.colSaldo, pdfStyles.saldoCreedor]}>
              {formatarMoeda(item.saldo)}
            </Text>
          </View>
        ))}
      </Page>
    </Document>
  );

  const blob = await pdf(<Documento />).toBlob();
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = "credores.pdf";
  a.click();
  URL.revokeObjectURL(url);
}

// ---------------------------------------------------------------------------
// Componente principal
// ---------------------------------------------------------------------------

export default function Credores() {
  const navigate = useNavigate();
  const location = useLocation();

  const params = (
    location.state as { params?: RelatorioParams & { tipo?: TipoRelatorio } } | null
  )?.params;

  const [loading, setLoading] = useState(true);
  const [erro, setErro] = useState<string | null>(null);
  const [titulo, setTitulo] = useState("");
  const [itens, setItens] = useState<ClienteRelatorioItem[]>([]);
  const [exportando, setExportando] = useState(false);

  useEffect(() => {
    if (!params) {
      navigate("/relatorios", { replace: true });
      return;
    }
    setLoading(true);
    setErro(null);
    buscarCredores(params)
      .then((res) => {
        setTitulo(res.titulo);
        setItens(res.itens);
      })
      .catch((e: Error) => setErro(e.message))
      .finally(() => setLoading(false));
  }, []);

  async function handleExportarPDF() {
    setExportando(true);
    try {
      await exportarPDF(titulo, itens);
    } finally {
      setExportando(false);
    }
  }

  function handleVoltar() {
    // RN107 — preserva parâmetros ao voltar
    navigate("/relatorios", { state: { params } });
  }

  return (
    <main className={styles.container}>
      <h1 className={styles.titulo}>
        {titulo || "Clientes Credores"}
      </h1>

      {loading && <p className={styles.loading}>Carregando...</p>}
      {erro && <p className={styles.erro}>{erro}</p>}

      {!loading && !erro && (
        <>
          <div className={styles.infoFaixa}>
            {params?.faixa_codigo === "acima" && <span>Código ≥ 10.000</span>}
            {params?.faixa_codigo === "abaixo" && <span>Código &lt; 10.000</span>}
          </div>

          {itens.length === 0 ? (
            <p className={styles.vazio}>Nenhum cliente encontrado com os critérios informados.</p>
          ) : (
            <table className={styles.tabela}>
              <thead>
                <tr>
                  <th className={styles.thCod}>Código</th>
                  <th className={styles.thNome}>Cliente</th>
                  <th className={styles.thSaldo}>Saldo</th>
                </tr>
              </thead>
              <tbody>
                {itens.map((item) => (
                  <tr key={item.id}>
                    <td className={styles.tdCod}>{item.cod_cliente}</td>
                    <td className={styles.tdNome}>{item.cliente}</td>
                    {/* RN119 — saldo credor em azul */}
                    <td className={`${styles.tdSaldo} ${styles.saldoCreedor}`}>
                      {formatarMoeda(item.saldo)}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}

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
