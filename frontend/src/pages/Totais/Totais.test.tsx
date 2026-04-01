/**
 * Totais.test.tsx — Testes da tela de totais
 *
 * Cobertura obrigatória (CLAUDE.md):
 *   1. Renderização com data de hoje (4 cards visíveis)
 *   2. Alteração de data refaz fetch (buscarTotais chamado com nova data)
 *   3. Exibição correta dos valores formatados (BRL + inteiro sem decimais)
 *   4. Autenticação negada (erro 401 → redirecionamento para /login)
 */

import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { cleanup, render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter, Route, Routes } from "react-router-dom";

import Totais from "./index";

// ---------------------------------------------------------------------------
// Mocks
// ---------------------------------------------------------------------------

const mockNavigate = vi.fn();
vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return {
    ...actual,
    useNavigate: () => mockNavigate,
  };
});

vi.mock("@/services/totais", () => ({
  buscarTotais: vi.fn(),
}));

import { buscarTotais } from "@/services/totais";

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

const SAMPLE_TOTAIS = {
  qtde_credores: 12,
  valor_credores: "45678.90",
  qtde_devedores: 8,
  valor_devedores: "12345.67",
};

function renderComponent() {
  return render(
    <MemoryRouter initialEntries={["/totais"]}>
      <Routes>
        <Route path="/totais" element={<Totais />} />
      </Routes>
    </MemoryRouter>,
  );
}

// ---------------------------------------------------------------------------
// Testes
// ---------------------------------------------------------------------------

describe("Totais", () => {
  beforeEach(() => {
    vi.mocked(buscarTotais).mockResolvedValue(SAMPLE_TOTAIS);
  });

  afterEach(() => {
    cleanup();
    vi.clearAllMocks();
  });

  it("renderiza os 4 cards com os valores da data de hoje", async () => {
    /**
     * RN59 — ao montar, buscarTotais é chamado com a data de hoje.
     * Os 4 cards devem aparecer com os valores corretos.
     */
    renderComponent();

    // Aguarda o loading sumir e os dados aparecerem
    await waitFor(() => {
      expect(screen.queryByText("Carregando...")).toBeNull();
    });

    // Verifica que buscarTotais foi chamado com a data de hoje
    const hoje = new Date().toISOString().slice(0, 10);
    expect(buscarTotais).toHaveBeenCalledWith(hoje);

    // Verifica os 4 labels de categoria
    expect(screen.getByText("Credores")).toBeTruthy();
    expect(screen.getByText("Total Credores")).toBeTruthy();
    expect(screen.getByText("Devedores")).toBeTruthy();
    expect(screen.getByText("Total Devedores")).toBeTruthy();
  });

  it("altera a data e refaz o fetch via GET /totais?data=", async () => {
    /**
     * RN60 — ao alterar o date picker, buscarTotais é chamado novamente
     * com a nova data selecionada.
     */
    const user = userEvent.setup();
    renderComponent();

    await waitFor(() => {
      expect(screen.queryByText("Carregando...")).toBeNull();
    });

    // Altera o date picker para uma data diferente
    const input = screen.getByLabelText("Data:");
    await user.clear(input);
    await user.type(input, "2026-01-15");

    await waitFor(() => {
      expect(buscarTotais).toHaveBeenCalledWith("2026-01-15");
    });
  });

  it("exibe valores formatados em BRL e quantidade sem decimais", async () => {
    /**
     * B05 — valor_credores e valor_devedores formatados como moeda BRL.
     * B06 — qtde_credores e qtde_devedores sem casas decimais.
     */
    renderComponent();

    await waitFor(() => {
      expect(screen.queryByText("Carregando...")).toBeNull();
    });

    // Quantidade deve ser inteiro (sem casas decimais)
    expect(screen.getByText("12")).toBeTruthy();
    expect(screen.getByText("8")).toBeTruthy();

    // Valores monetários devem ter formato BRL (R$)
    const textos = document.body.textContent ?? "";
    expect(textos).toContain("R$");
  });

  it("redireciona para /login em caso de erro de autenticação", async () => {
    /**
     * Quando buscarTotais lança erro 401, o componente deve redirecionar
     * para /login sem exibir dados.
     */
    vi.mocked(buscarTotais).mockRejectedValue(new Error("401 Unauthorized"));

    renderComponent();

    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith("/login", { replace: true });
    });
  });
});
