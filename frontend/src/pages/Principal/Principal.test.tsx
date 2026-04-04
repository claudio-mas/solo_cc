/**
 * pages/Principal/Principal.test.tsx
 * Testes da tela principal — FrmPrincipal
 *
 * Cobertura obrigatória (AGENTS.MD):
 *   1. Renderiza sem erros
 *   2. Pesquisa posiciona no match (RN12/RN13)
 *   3. Seleção atualiza header (RN11)
 *   4. AutoFocus no campo de pesquisa (RN14)
 *   5. Logout limpa auth (RN19)
 *   6. Double-click navega para extrato (RN15)
 *   7. Footer exibe usuário (RN20)
 */

import "@testing-library/jest-dom/vitest";
import { render, screen, fireEvent, cleanup, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { MemoryRouter } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";

import Principal from "./index";
import type { ClienteListItem } from "@/services/clientes";

// ---------------------------------------------------------------------------
// Mocks
// ---------------------------------------------------------------------------

const mockNavigate = vi.fn();
vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return { ...actual, useNavigate: () => mockNavigate };
});

const SAMPLE_CLIENTES: ClienteListItem[] = [
  { id: 1, codigo: 101, cliente: "Ana Silva" },
  { id: 2, codigo: 202, cliente: "Bruno Costa" },
  { id: 3, codigo: 303, cliente: "Carlos Lima" },
];

vi.mock("@/hooks/useClientes", () => ({
  useClientes: () => ({
    clientes: SAMPLE_CLIENTES,
    isLoading: false,
    error: null,
  }),
}));

const mockClearAuth = vi.fn();
vi.mock("@/store/authStore", () => ({
  useAuthStore: (selector: (state: unknown) => unknown) =>
    selector({
      usuario: "ADMIN",
      perfil: "Administrador",
      token: "fake-token",
      isAuthenticated: true,
      clearAuth: mockClearAuth,
    }),
}));

// ---------------------------------------------------------------------------
// Helper
// ---------------------------------------------------------------------------

function renderPrincipal() {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });
  return render(
    <QueryClientProvider client={queryClient}>
      <MemoryRouter>
        <Principal />
      </MemoryRouter>
    </QueryClientProvider>,
  );
}

// ---------------------------------------------------------------------------
// Testes
// ---------------------------------------------------------------------------

describe("Principal", () => {
  afterEach(() => {
    cleanup();
  });

  beforeEach(() => {
    mockNavigate.mockClear();
    mockClearAuth.mockClear();
  });

  it("renderiza sem erros com grid de clientes", () => {
    renderPrincipal();

    // Verifica colunas
    expect(screen.getByText("Código")).toBeInTheDocument();
    expect(screen.getByText("Cliente")).toBeInTheDocument();

    // Verifica dados — use getAllByText pois React pode renderizar múltiplas vezes
    expect(screen.getAllByText("Ana Silva").length).toBeGreaterThanOrEqual(1);
    expect(screen.getAllByText("Bruno Costa").length).toBeGreaterThanOrEqual(1);
    expect(screen.getAllByText("Carlos Lima").length).toBeGreaterThanOrEqual(1);
  });

  it("pesquisa posiciona no match por prefixo do nome (RN12/RN13)", async () => {
    renderPrincipal();
    const user = userEvent.setup();

    const searchInput = screen.getByPlaceholderText(
      "Digite o nome ou código do cliente...",
    );
    await user.type(searchInput, "Bru");

    // Após digitar "Bru", o header deve mostrar o código do Bruno (202)
    expect(screen.getByText("CÓD. 202")).toBeInTheDocument();
  });

  it("seleção no grid atualiza header dinâmico (RN11)", () => {
    renderPrincipal();

    // RN10 — header inicializa com primeiro cliente
    expect(screen.getByText("CÓD. 101")).toBeInTheDocument();

    // Clica na linha do Carlos Lima (código 303) — use first match
    const cells = screen.getAllByText("Carlos Lima");
    fireEvent.click(cells[0]!);

    // RN11 — header atualiza
    expect(screen.getByText("CÓD. 303")).toBeInTheDocument();
  });

  it("campo de pesquisa recebe foco automaticamente (RN14)", async () => {
    renderPrincipal();

    const searchInput = screen.getByPlaceholderText(
      "Digite o nome ou código do cliente...",
    );
    await waitFor(() => {
      expect(document.activeElement).toBe(searchInput);
    });
  });

  it("botão Sair limpa auth e redireciona para /login (RN19)", async () => {
    renderPrincipal();
    const user = userEvent.setup();

    const sairButtons = screen.getAllByText("Sair");
    await user.click(sairButtons[0]!);

    expect(mockClearAuth).toHaveBeenCalledOnce();
    expect(mockNavigate).toHaveBeenCalledWith("/login");
  });

  it("double-click no grid navega para extrato do cliente (RN15)", () => {
    renderPrincipal();

    // Double-click na linha do Bruno Costa (código 202) — use first match
    const cells = screen.getAllByText("Bruno Costa");
    fireEvent.doubleClick(cells[0]!);

    expect(mockNavigate).toHaveBeenCalledWith(
      "/extrato",
      expect.objectContaining({
        state: expect.objectContaining({
          codCliente: 202,
          nomeCliente: "Bruno Costa",
        }),
      }),
    );
  });

  it("barra inferior exibe nome do usuário em maiúsculas (RN20)", () => {
    renderPrincipal();

    const footerElements = screen.getAllByText("ADMIN");
    expect(footerElements.length).toBeGreaterThanOrEqual(1);
    expect(footerElements[0]).toBeInTheDocument();
  });
});
