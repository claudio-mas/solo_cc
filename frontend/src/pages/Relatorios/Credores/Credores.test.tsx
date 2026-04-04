/**
 * Credores.test.tsx — Testes da tela de relatório de credores (frmCredores1)
 *
 * Cobertura:
 *   1. Renderização com dados retornados
 *   2. RN118 — exibe título dinâmico
 *   3. RN107 — parâmetros preservados ao clicar Voltar
 *   4. Sem state → redireciona para /relatorios
 *   5. Estado de loading enquanto fetch
 */

import { afterEach, describe, expect, it, vi, beforeEach } from "vitest";
import { cleanup, render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter, Route, Routes } from "react-router-dom";

import Credores from "./index";

// ---------------------------------------------------------------------------
// Mocks
// ---------------------------------------------------------------------------

const mockNavigate = vi.fn();
vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return { ...actual, useNavigate: () => mockNavigate };
});

vi.mock("@/services/relatorios", () => ({
  buscarCredores: vi.fn(),
}));

import { buscarCredores, RelatorioResponse } from "@/services/relatorios";

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

const SAMPLE_PARAMS = {
  tipo: "credores",
  data_corte: "2026-04-04",
  saldo_minimo: 0,
  faixa_codigo: "todos",
  ordenacao: "codigo",
};

const SAMPLE_RESPONSE: RelatorioResponse = {
  titulo: "Clientes credores em 04/04/2026 com saldo maior ou igual a R$ 0,00",
  data_corte: "2026-04-04",
  saldo_minimo: 0,
  faixa_codigo: "todos",
  ordenacao: "codigo",
  itens: [
    { id: 20001, cod_cliente: 20001, cliente: "Credor A", saldo: 800 },
  ],
};

function renderComponent(params?: object) {
  return render(
    <MemoryRouter
      initialEntries={[{ pathname: "/relatorios/credores", state: params ? { params } : undefined }]}
    >
      <Routes>
        <Route path="/relatorios/credores" element={<Credores />} />
        <Route path="/relatorios" element={<div>Relatórios</div>} />
      </Routes>
    </MemoryRouter>,
  );
}

// ---------------------------------------------------------------------------
// Testes
// ---------------------------------------------------------------------------

describe("Credores (frmCredores1)", () => {
  afterEach(cleanup);
  beforeEach(() => {
    mockNavigate.mockClear();
    vi.mocked(buscarCredores).mockReset();
  });

  it("renderiza lista de credores após fetch", async () => {
    vi.mocked(buscarCredores).mockResolvedValue(SAMPLE_RESPONSE);
    renderComponent(SAMPLE_PARAMS);

    await waitFor(() => expect(screen.getByText("Credor A")).toBeInTheDocument());
  });

  it("RN118 — exibe título dinâmico retornado pela API", async () => {
    vi.mocked(buscarCredores).mockResolvedValue(SAMPLE_RESPONSE);
    renderComponent(SAMPLE_PARAMS);

    await waitFor(() =>
      expect(screen.getByRole("heading", { level: 1 })).toHaveTextContent(/credores/i),
    );
  });

  it("exibe estado de loading antes do fetch completar", () => {
    vi.mocked(buscarCredores).mockReturnValue(new Promise(() => {}));
    renderComponent(SAMPLE_PARAMS);
    expect(screen.getByText(/Carregando/i)).toBeInTheDocument();
  });

  it("RN107 — botão Voltar preserva parâmetros na navegação", async () => {
    vi.mocked(buscarCredores).mockResolvedValue(SAMPLE_RESPONSE);
    renderComponent(SAMPLE_PARAMS);

    await waitFor(() => expect(screen.getByText("Credor A")).toBeInTheDocument());

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
