/**
 * Relatorios.test.tsx — Testes da tela de parâmetros de relatórios (frmReports)
 *
 * Cobertura:
 *   1. Renderização com valores padrão
 *   2. Botão "Gerar" navega para /relatorios/devedores com params corretos
 *   3. Selecionando credores, navega para /relatorios/credores
 *   4. RN107 — restaura parâmetros ao voltar via state da rota
 *   5. Botão Voltar navega para /principal
 */

import { afterEach, describe, expect, it, vi, beforeEach } from "vitest";
import { cleanup, render, screen, fireEvent } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router-dom";

import Relatorios from "./index";

// ---------------------------------------------------------------------------
// Mocks
// ---------------------------------------------------------------------------

const mockNavigate = vi.fn();
vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return { ...actual, useNavigate: () => mockNavigate };
});

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

function renderComponent(state?: object) {
  return render(
    <MemoryRouter initialEntries={[{ pathname: "/relatorios", state }]}>
      <Routes>
        <Route path="/relatorios" element={<Relatorios />} />
      </Routes>
    </MemoryRouter>,
  );
}

// ---------------------------------------------------------------------------
// Testes
// ---------------------------------------------------------------------------

describe("Relatorios (frmReports)", () => {
  afterEach(cleanup);
  beforeEach(() => mockNavigate.mockClear());

  it("renderiza com valores padrão", () => {
    renderComponent();
    expect(screen.getByText("RELATÓRIOS")).toBeInTheDocument();
    expect(screen.getByLabelText(/Devedores/i)).toBeChecked();
    expect(screen.getByLabelText(/Banco de dados inteiro/i)).toBeChecked();
    expect(screen.getByLabelText(/Código/i)).toBeChecked();
  });

  it("botão Gerar navega para /relatorios/devedores com devedores selecionado", () => {
    renderComponent();
    fireEvent.click(screen.getByRole("button", { name: /Gerar/i }));
    expect(mockNavigate).toHaveBeenCalledWith(
      "/relatorios/devedores",
      expect.objectContaining({ state: expect.objectContaining({ params: expect.objectContaining({ tipo: "devedores" }) }) }),
    );
  });

  it("selecionando credores, navega para /relatorios/credores", () => {
    renderComponent();
    fireEvent.click(screen.getByLabelText(/Credores/i));
    fireEvent.click(screen.getByRole("button", { name: /Gerar/i }));
    expect(mockNavigate).toHaveBeenCalledWith(
      "/relatorios/credores",
      expect.anything(),
    );
  });

  it("RN107 — restaura parâmetros ao voltar", () => {
    const params = {
      tipo: "credores",
      faixa_codigo: "acima",
      ordenacao: "nome",
      data_corte: "2026-01-01",
      saldo_minimo: 500,
    };
    renderComponent({ params });
    // Verifica os radios pelo value dentro do fieldset correspondente
    const radioTipoCredores = screen.getByDisplayValue("credores");
    expect(radioTipoCredores).toBeChecked();
    expect(screen.getByDisplayValue("acima")).toBeChecked();
    expect(screen.getByDisplayValue("nome")).toBeChecked();
  });

  it("botão Voltar navega para /principal", () => {
    renderComponent();
    fireEvent.click(screen.getByRole("button", { name: /Voltar/i }));
    expect(mockNavigate).toHaveBeenCalledWith("/principal");
  });
});
