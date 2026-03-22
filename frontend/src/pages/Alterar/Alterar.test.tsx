/**
 * Alterar.test.tsx — Testes da tela de edição/exclusão de cliente
 *
 * Cobertura obrigatória (CLAUDE.md):
 *   1. Renderização em modo leitura (campos readOnly)
 *   2. Clicar "Alterar" habilita edição
 *   3. Salvar com alteração de nome (modal simples → sucesso)
 *   4. Salvar com alteração de código (modal de alto risco → sucesso)
 *   5. Senha incorreta na exclusão (mensagem de erro)
 *   6. Exclusão sem lançamentos (sucesso + navigate)
 *   7. Exclusão com lançamentos (modal adicional → sucesso)
 */

import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { MemoryRouter, Route, Routes } from "react-router-dom";

import Alterar from "./index";

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
  fetchCliente: vi.fn(),
  atualizarCliente: vi.fn(),
  verificarSenhaExclusao: vi.fn(),
  excluirCliente: vi.fn(),
}));

import {
  fetchCliente,
  atualizarCliente,
  verificarSenhaExclusao,
  excluirCliente,
} from "@/services/clientes";

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

const CLIENTE_SEM_LANCAMENTOS = {
  id: 1,
  codigo: 10001,
  cliente: "ANA SILVA",
  tem_lancamentos: false,
};

const CLIENTE_COM_LANCAMENTOS = {
  id: 2,
  codigo: 10002,
  cliente: "BRUNO COSTA",
  tem_lancamentos: true,
};

function renderComponent(clienteId = "1") {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });
  return render(
    <QueryClientProvider client={queryClient}>
      <MemoryRouter initialEntries={[`/clientes/alterar/${clienteId}`]}>
        <Routes>
          <Route
            path="/clientes/alterar/:id"
            element={<Alterar />}
          />
        </Routes>
      </MemoryRouter>
    </QueryClientProvider>,
  );
}

// ---------------------------------------------------------------------------
// Setup
// ---------------------------------------------------------------------------

beforeEach(() => {
  vi.clearAllMocks();
  vi.mocked(fetchCliente).mockResolvedValue(CLIENTE_SEM_LANCAMENTOS);
});

// ---------------------------------------------------------------------------
// Testes
// ---------------------------------------------------------------------------

describe("Alterar", () => {
  // 1. Renderização em modo leitura
  describe("renderização em modo leitura", () => {
    it("exibe título, campos readOnly e botões Alterar/Retornar/Excluir", async () => {
      renderComponent();

      await waitFor(() => {
        expect(
          screen.getByText("ALTERAÇÃO DO CLIENTE"),
        ).toBeInTheDocument();
      });

      // RN34 — campos readOnly
      const codigoInput = screen.getByLabelText(
        "Código",
      ) as HTMLInputElement;
      const nomeInput = screen.getByLabelText("Nome") as HTMLInputElement;
      expect(codigoInput.readOnly).toBe(true);
      expect(nomeInput.readOnly).toBe(true);

      // Dados carregados
      expect(codigoInput.value).toBe("10001");
      expect(nomeInput.value).toBe("ANA SILVA");

      // Botões presentes
      expect(
        screen.getByRole("button", { name: /alterar/i }),
      ).toBeInTheDocument();
      expect(
        screen.getByRole("button", { name: /retornar/i }),
      ).toBeInTheDocument();
      expect(
        screen.getByRole("button", { name: /excluir cliente/i }),
      ).toBeInTheDocument();
    });
  });

  // 2. Habilitar edição
  describe("habilitar edição", () => {
    it("clicar em 'Alterar' remove readOnly dos campos", async () => {
      renderComponent();
      const user = userEvent.setup();

      await waitFor(() => {
        expect(
          screen.getByRole("button", { name: /alterar/i }),
        ).toBeInTheDocument();
      });

      await user.click(screen.getByRole("button", { name: /alterar/i }));

      // RN35 — campos editáveis
      const codigoInput = screen.getByLabelText(
        "Código",
      ) as HTMLInputElement;
      const nomeInput = screen.getByLabelText("Nome") as HTMLInputElement;
      expect(codigoInput.readOnly).toBe(false);
      expect(nomeInput.readOnly).toBe(false);

      // Botão Salvar aparece
      expect(
        screen.getByRole("button", { name: /salvar/i }),
      ).toBeInTheDocument();
    });
  });

  // 3. Salvar alteração de nome (modal simples)
  describe("salvar alteração de nome", () => {
    it("exibe modal simples de confirmação e salva", async () => {
      vi.mocked(atualizarCliente).mockResolvedValue({
        id: 1,
        codigo: 10001,
        cliente: "ANA COSTA",
      });

      renderComponent();
      const user = userEvent.setup();

      await waitFor(() =>
        screen.getByRole("button", { name: /alterar/i }),
      );
      await user.click(screen.getByRole("button", { name: /alterar/i }));

      // Altera nome
      const nomeInput = screen.getByLabelText("Nome");
      await user.clear(nomeInput);
      await user.type(nomeInput, "ANA COSTA");

      await user.click(screen.getByRole("button", { name: /salvar/i }));

      // RN37 — modal simples aparece
      await waitFor(() => {
        expect(
          screen.getByText(/confirmar alteração/i),
        ).toBeInTheDocument();
      });

      // Confirmar
      await user.click(screen.getByRole("button", { name: /^sim$/i }));

      await waitFor(() => {
        expect(atualizarCliente).toHaveBeenCalledWith(1, {
          codigo: 10001,
          cliente: "ANA COSTA",
        });
        expect(mockNavigate).toHaveBeenCalledWith("/principal");
      });
    });
  });

  // 4. Salvar alteração de código (modal de alto risco)
  describe("salvar alteração de código", () => {
    it("exibe modal de alto risco e salva após confirmação", async () => {
      vi.mocked(atualizarCliente).mockResolvedValue({
        id: 1,
        codigo: 10099,
        cliente: "ANA SILVA",
      });

      renderComponent();
      const user = userEvent.setup();

      await waitFor(() =>
        screen.getByRole("button", { name: /alterar/i }),
      );
      await user.click(screen.getByRole("button", { name: /alterar/i }));

      // Altera código
      const codigoInput = screen.getByLabelText("Código");
      await user.clear(codigoInput);
      await user.type(codigoInput, "10099");

      await user.click(screen.getByRole("button", { name: /salvar/i }));

      // RN36 — modal de alto risco aparece
      await waitFor(() => {
        expect(
          screen.getByText(/A T E N Ç Ã O/i),
        ).toBeInTheDocument();
      });

      // Confirmar
      await user.click(
        screen.getByRole("button", { name: /sim, alterar código/i }),
      );

      await waitFor(() => {
        expect(atualizarCliente).toHaveBeenCalledWith(1, {
          codigo: 10099,
          cliente: "ANA SILVA",
        });
        expect(mockNavigate).toHaveBeenCalledWith("/principal");
      });
    });
  });

  // 5. Senha incorreta na exclusão
  describe("senha incorreta", () => {
    it("exibe mensagem de erro quando senha incorreta", async () => {
      vi.mocked(verificarSenhaExclusao).mockResolvedValue({
        valido: false,
        tem_lancamentos: false,
      });

      renderComponent();
      const user = userEvent.setup();

      await waitFor(() =>
        screen.getByRole("button", { name: /excluir cliente/i }),
      );

      // Passo 1 — confirmação inicial
      await user.click(
        screen.getByRole("button", { name: /excluir cliente/i }),
      );
      await waitFor(() =>
        screen.getByText(/confirma exclusão/i),
      );
      await user.click(screen.getByRole("button", { name: /^sim$/i }));

      // Passo 2 — modal de senha
      await waitFor(() =>
        screen.getByPlaceholderText("Senha"),
      );
      await user.type(screen.getByPlaceholderText("Senha"), "ERRADA");
      await user.click(
        screen.getByRole("button", { name: /confirmar/i }),
      );

      // RN41 — mensagem de senha incorreta
      await waitFor(() => {
        expect(screen.getByText("Senha incorreta")).toBeInTheDocument();
      });

      expect(excluirCliente).not.toHaveBeenCalled();
    });
  });

  // 6. Exclusão sem lançamentos
  describe("exclusão sem lançamentos", () => {
    it("fluxo completo sem confirmação adicional → navigate", async () => {
      vi.mocked(verificarSenhaExclusao).mockResolvedValue({
        valido: true,
        tem_lancamentos: false,
      });
      vi.mocked(excluirCliente).mockResolvedValue(undefined);

      renderComponent();
      const user = userEvent.setup();

      await waitFor(() =>
        screen.getByRole("button", { name: /excluir cliente/i }),
      );

      await user.click(
        screen.getByRole("button", { name: /excluir cliente/i }),
      );
      await waitFor(() => screen.getByText(/confirma exclusão/i));
      await user.click(screen.getByRole("button", { name: /^sim$/i }));

      await waitFor(() => screen.getByPlaceholderText("Senha"));
      await user.type(screen.getByPlaceholderText("Senha"), "4321");
      await user.click(
        screen.getByRole("button", { name: /confirmar/i }),
      );

      // RN45 — sem confirmação adicional; executa diretamente
      await waitFor(() => {
        expect(excluirCliente).toHaveBeenCalledWith(1);
        expect(mockNavigate).toHaveBeenCalledWith("/principal");
      });
    });
  });

  // 7. Exclusão com lançamentos
  describe("exclusão com lançamentos", () => {
    it("exibe confirmação adicional e executa exclusão", async () => {
      vi.mocked(fetchCliente).mockResolvedValue(CLIENTE_COM_LANCAMENTOS);
      vi.mocked(verificarSenhaExclusao).mockResolvedValue({
        valido: true,
        tem_lancamentos: true,
      });
      vi.mocked(excluirCliente).mockResolvedValue(undefined);

      renderComponent("2");
      const user = userEvent.setup();

      await waitFor(() =>
        screen.getByRole("button", { name: /excluir cliente/i }),
      );

      await user.click(
        screen.getByRole("button", { name: /excluir cliente/i }),
      );
      await waitFor(() => screen.getByText(/confirma exclusão/i));
      await user.click(screen.getByRole("button", { name: /^sim$/i }));

      await waitFor(() => screen.getByPlaceholderText("Senha"));
      await user.type(screen.getByPlaceholderText("Senha"), "4321");
      await user.click(
        screen.getByRole("button", { name: /confirmar/i }),
      );

      // RN43 — modal de confirmação adicional
      await waitFor(() => {
        expect(
          screen.getByText(/tem lançamentos na conta corrente/i),
        ).toBeInTheDocument();
      });

      await user.click(
        screen.getByRole("button", { name: /sim, excluir tudo/i }),
      );

      // RN44 — DELETE executado
      await waitFor(() => {
        expect(excluirCliente).toHaveBeenCalledWith(2);
        expect(mockNavigate).toHaveBeenCalledWith("/principal");
      });
    });
  });
});
