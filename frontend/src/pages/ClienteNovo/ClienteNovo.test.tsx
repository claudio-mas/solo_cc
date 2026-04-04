/**
 * ClienteNovo.test.tsx — Testes da tela de cadastro de novo cliente
 *
 * Cobertura obrigatória (CLAUDE.md):
 *   1. Renderização — campos, botões e título presentes
 *   2. Geração de código — botão chama API e preenche campo
 *   3. Submissão válida — POST /clientes e redirecionamento
 *   4. Código duplicado — exibe mensagem "Código já existente"
 *   5. Código acima do limite — validação Zod exibe mensagem
 */

import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { cleanup, render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { MemoryRouter } from "react-router-dom";

import ClienteNovo from "./index";

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

vi.mock("@/services/clientes", () => ({
  fetchProximoCodigo: vi.fn(),
  criarCliente: vi.fn(),
}));

vi.mock("@/lib/api", () => ({
  apiFetch: vi.fn(),
}));

import { fetchProximoCodigo, criarCliente } from "@/services/clientes";
import { apiFetch } from "@/lib/api";

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

function renderComponent() {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });
  return render(
    <QueryClientProvider client={queryClient}>
      <MemoryRouter>
        <ClienteNovo />
      </MemoryRouter>
    </QueryClientProvider>,
  );
}

// ---------------------------------------------------------------------------
// Setup
// ---------------------------------------------------------------------------

beforeEach(() => {
  vi.clearAllMocks();
  // Default: API retorna lista vazia de clientes (onBlur check)
  vi.mocked(apiFetch).mockResolvedValue({
    ok: true,
    json: async () => ({ clientes: [] }),
  } as Response);
});

afterEach(() => {
  cleanup();
});

// ---------------------------------------------------------------------------
// Testes
// ---------------------------------------------------------------------------

describe("ClienteNovo", () => {
  // 1. Renderização
  describe("renderização", () => {
    it("exibe título, campos e botões", () => {
      renderComponent();

      expect(screen.getByText("NOVO CLIENTE")).toBeInTheDocument();
      expect(screen.getByLabelText("Código")).toBeInTheDocument();
      expect(screen.getByLabelText("Nome")).toBeInTheDocument();
      expect(
        screen.getByRole("button", { name: /gerar código/i }),
      ).toBeInTheDocument();
      expect(
        screen.getByRole("button", { name: /salvar/i }),
      ).toBeInTheDocument();
      expect(
        screen.getByRole("button", { name: /cancelar/i }),
      ).toBeInTheDocument();
    });
  });

  // 2. Geração de código
  describe("geração de código", () => {
    it("chama API e preenche campo ao clicar em 'Gerar código'", async () => {
      vi.mocked(fetchProximoCodigo).mockResolvedValue({
        proximo_codigo: 10005,
        proximo_id: 51,
      });

      renderComponent();
      const user = userEvent.setup();

      await user.click(
        screen.getByRole("button", { name: /gerar código/i }),
      );

      await waitFor(() => {
        expect(fetchProximoCodigo).toHaveBeenCalledOnce();
      });

      const codigoInput = screen.getByLabelText("Código") as HTMLInputElement;
      await waitFor(() => {
        expect(codigoInput.value).toBe("10005");
      });
    });
  });

  // 3. Submissão válida
  describe("submissão válida", () => {
    it("envia POST e redireciona para /principal", async () => {
      vi.mocked(criarCliente).mockResolvedValue({
        id: 51,
        codigo: 10005,
        cliente: "JOÃO SILVA",
      });

      renderComponent();
      const user = userEvent.setup();

      const codigoInput = screen.getByLabelText("Código");
      const clienteInput = screen.getByLabelText("Nome");

      await user.type(codigoInput, "10005");
      await user.type(clienteInput, "João Silva");
      await user.click(screen.getByRole("button", { name: /salvar/i }));

      await waitFor(() => {
        expect(criarCliente).toHaveBeenCalledWith({
          codigo: 10005,
          cliente: "João Silva",
        });
      });

      await waitFor(() => {
        expect(mockNavigate).toHaveBeenCalledWith("/principal");
      });
    });
  });

  // 4. Código duplicado
  describe("código duplicado", () => {
    it("exibe mensagem de erro quando API retorna 409", async () => {
      vi.mocked(criarCliente).mockRejectedValue(
        new Error("Código já existente"),
      );

      renderComponent();
      const user = userEvent.setup();

      await user.type(screen.getByLabelText("Código"), "10001");
      await user.type(screen.getByLabelText("Nome"), "Teste");
      await user.click(screen.getByRole("button", { name: /salvar/i }));

      await waitFor(() => {
        expect(
          screen.getByText("Código já existente"),
        ).toBeInTheDocument();
      });
    });
  });

  // 5. Código acima do limite
  describe("código acima do limite", () => {
    it("exibe mensagem de validação para código > 20.000", async () => {
      renderComponent();
      const user = userEvent.setup();

      await user.type(screen.getByLabelText("Código"), "20001");
      await user.type(screen.getByLabelText("Nome"), "Teste");
      await user.click(screen.getByRole("button", { name: /salvar/i }));

      await waitFor(() => {
        expect(
          screen.getByText("Tamanho máximo de Código: 20.000"),
        ).toBeInTheDocument();
      });
    });
  });

  // Cancelar navega de volta
  describe("cancelar", () => {
    it("navega para /principal ao clicar em cancelar", async () => {
      renderComponent();
      const user = userEvent.setup();

      await user.click(screen.getByRole("button", { name: /cancelar/i }));

      expect(mockNavigate).toHaveBeenCalledWith("/principal");
    });
  });
});
