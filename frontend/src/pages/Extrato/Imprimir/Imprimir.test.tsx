/**
 * Imprimir.test.tsx — Testes da tela de impressão do extrato (FrmExtratoRpt)
 *
 * Cobertura:
 *   1. Renderização com título correto (RN122)
 *   2. Exibe lançamentos após fetch (RN121)
 *   3. RN124 — botão Voltar preserva filtros ao navegar para /extrato
 *   4. Sem state → redireciona para /extrato
 *   5. Estado de loading enquanto fetch
 */

import { afterEach, describe, expect, it, vi, beforeEach } from "vitest";
import { cleanup, render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter, Route, Routes } from "react-router-dom";

import ExtratoImprimir from "./index";

// ---------------------------------------------------------------------------
// Mocks
// ---------------------------------------------------------------------------

const mockNavigate = vi.fn();
vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return { ...actual, useNavigate: () => mockNavigate };
});

vi.mock("@/services/extrato", () => ({
  listarExtrato: vi.fn(),
}));

import { listarExtrato } from "@/services/extrato";

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

const SAMPLE_STATE = {
  extratoImprimir: {
    idCliente: 1,
    codCliente: 10001,
    nomeCliente: "João da Silva",
    filtros: { sem_nd: false },
  },
};

const SAMPLE_EXTRATO = {
  lancamentos: [
    {
      id: 1,
      dt: "2026-04-01",
      conta: 1,
      nd: "001",
      ref: "Aluguel",
      vvalor: 1000,
      dc: "D",
      deb: 1000,
      cred: null,
      saldo: -1000,
    },
  ],
  saldo_total: -1000,
};

function renderComponent(state?: object) {
  return render(
    <MemoryRouter
      initialEntries={[{ pathname: "/extrato/imprimir", state }]}
    >
      <Routes>
        <Route path="/extrato/imprimir" element={<ExtratoImprimir />} />
        <Route path="/extrato" element={<div>Extrato</div>} />
      </Routes>
    </MemoryRouter>,
  );
}

// ---------------------------------------------------------------------------
// Testes
// ---------------------------------------------------------------------------

describe("ExtratoImprimir (FrmExtratoRpt)", () => {
  afterEach(cleanup);
  beforeEach(() => {
    mockNavigate.mockClear();
    vi.mocked(listarExtrato).mockReset();
  });

  it("RN122 — renderiza título com nome e código do cliente", async () => {
    vi.mocked(listarExtrato).mockResolvedValue(SAMPLE_EXTRATO);
    renderComponent(SAMPLE_STATE);

    await waitFor(() =>
      expect(screen.getByRole("heading", { level: 1 })).toHaveTextContent(
        "João da Silva - 10001",
      ),
    );
  });

  it("RN121 — exibe lançamentos após fetch", async () => {
    vi.mocked(listarExtrato).mockResolvedValue(SAMPLE_EXTRATO);
    renderComponent(SAMPLE_STATE);

    await waitFor(() => expect(screen.getByText("Aluguel")).toBeInTheDocument());
  });

  it("exibe estado de loading antes do fetch completar", () => {
    vi.mocked(listarExtrato).mockReturnValue(new Promise(() => {}));
    renderComponent(SAMPLE_STATE);
    expect(screen.getByText(/Carregando/i)).toBeInTheDocument();
  });

  it("RN124 — botão Voltar navega para /extrato com filtros preservados", async () => {
    vi.mocked(listarExtrato).mockResolvedValue(SAMPLE_EXTRATO);
    renderComponent(SAMPLE_STATE);

    await waitFor(() => expect(screen.getByText("Aluguel")).toBeInTheDocument());

    const user = userEvent.setup();
    await user.click(screen.getByRole("button", { name: /Voltar/i }));

    expect(mockNavigate).toHaveBeenCalledWith(
      "/extrato",
      expect.objectContaining({
        state: expect.objectContaining({ idCliente: 1 }),
      }),
    );
  });

  it("sem state redireciona para /extrato", () => {
    renderComponent();
    expect(mockNavigate).toHaveBeenCalledWith("/extrato", { replace: true });
  });
});
