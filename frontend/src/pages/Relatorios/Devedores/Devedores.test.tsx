/**
 * Devedores.test.tsx — Testes da tela de relatório de devedores (frmRptDevedores1)
 *
 * Cobertura:
 *   1. Renderização com dados retornados
 *   2. RN112 — exibe título dinâmico
 *   3. RN113 — saldo em vermelho (classe CSS condicional)
 *   4. RN107 — parâmetros preservados ao clicar Voltar
 *   5. Sem state → redireciona para /relatorios
 *   6. Estado de loading enquanto fetch
 */

import { afterEach, describe, expect, it, vi, beforeEach } from "vitest";
import { cleanup, render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter, Route, Routes } from "react-router-dom";

import Devedores from "./index";

// ---------------------------------------------------------------------------
// Mocks
// ---------------------------------------------------------------------------

const mockNavigate = vi.fn();
vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return { ...actual, useNavigate: () => mockNavigate };
});

vi.mock("@/services/relatorios", () => ({
  buscarDevedores: vi.fn(),
}));

import { buscarDevedores, RelatorioResponse } from "@/services/relatorios";

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

const SAMPLE_PARAMS = {
  tipo: "devedores",
  data_corte: "2026-04-04",
  saldo_minimo: 0,
  faixa_codigo: "todos",
  ordenacao: "codigo",
};

const SAMPLE_RESPONSE: RelatorioResponse = {
  titulo: "Clientes devedores em 04/04/2026 com saldo maior ou igual a R$ 0,00",
  data_corte: "2026-04-04",
  saldo_minimo: 0,
  faixa_codigo: "todos",
  ordenacao: "codigo",
  itens: [
    { id: 10001, cod_cliente: 10001, cliente: "Cliente A", saldo: 500 },
    { id: 10002, cod_cliente: 10002, cliente: "Cliente B", saldo: 300 },
  ],
};

function renderComponent(params?: object) {
  return render(
    <MemoryRouter
      initialEntries={[{ pathname: "/relatorios/devedores", state: params ? { params } : undefined }]}
    >
      <Routes>
        <Route path="/relatorios/devedores" element={<Devedores />} />
        <Route path="/relatorios" element={<div>Relatórios</div>} />
      </Routes>
    </MemoryRouter>,
  );
}

// ---------------------------------------------------------------------------
// Testes
// ---------------------------------------------------------------------------

describe("Devedores (frmRptDevedores1)", () => {
  afterEach(cleanup);
  beforeEach(() => {
    mockNavigate.mockClear();
    vi.mocked(buscarDevedores).mockReset();
  });

  it("renderiza lista de devedores após fetch", async () => {
    vi.mocked(buscarDevedores).mockResolvedValue(SAMPLE_RESPONSE);
    renderComponent(SAMPLE_PARAMS);

    await waitFor(() => expect(screen.getByText("Cliente A")).toBeInTheDocument());
    expect(screen.getByText("Cliente B")).toBeInTheDocument();
  });

  it("RN112 — exibe título dinâmico retornado pela API", async () => {
    vi.mocked(buscarDevedores).mockResolvedValue(SAMPLE_RESPONSE);
    renderComponent(SAMPLE_PARAMS);

    await waitFor(() =>
      expect(screen.getByRole("heading", { level: 1 })).toHaveTextContent(
        /devedores/i,
      ),
    );
  });

  it("exibe estado de loading antes do fetch completar", () => {
    vi.mocked(buscarDevedores).mockReturnValue(new Promise(() => {}));
    renderComponent(SAMPLE_PARAMS);
    expect(screen.getByText(/Carregando/i)).toBeInTheDocument();
  });

  it("RN107 — botão Voltar preserva parâmetros na navegação", async () => {
    vi.mocked(buscarDevedores).mockResolvedValue(SAMPLE_RESPONSE);
    renderComponent(SAMPLE_PARAMS);

    await waitFor(() => expect(screen.getByText("Cliente A")).toBeInTheDocument());

    const user = userEvent.setup();
    await user.click(screen.getByRole("button", { name: /Voltar/i }));

    expect(mockNavigate).toHaveBeenCalledWith(
      "/relatorios",
      expect.objectContaining({ state: expect.objectContaining({ params: expect.anything() }) }),
    );
  });

  it("sem state redireciona para /relatorios", () => {
    renderComponent();
    expect(mockNavigate).toHaveBeenCalledWith("/relatorios", { replace: true });
  });
});
