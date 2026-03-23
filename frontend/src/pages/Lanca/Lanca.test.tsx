/**
 * Lanca.test.tsx — Testes da tela de lançamentos
 *
 * Cobertura obrigatória (CLAUDE.md):
 *   1. Renderização com route state válido
 *   2. Lançamento débito válido (submit → POST chamado)
 *   3. Lançamento crédito válido
 *   4. Validação de campos obrigatórios (erro exibido sem chamar API)
 *   5. Pasta nova → modal exibido; "Não" limpa campo
 *   6. Redirect para /principal se route state ausente
 */

import { afterEach, describe, it, expect, vi, beforeEach } from "vitest";
import { cleanup, render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter, Route, Routes } from "react-router-dom";

import Lanca from "./index";

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

vi.mock("@/services/lancamentos", () => ({
  registrarLancamento: vi.fn(),
  verificarPasta: vi.fn(),
}));

import {
  registrarLancamento,
  verificarPasta,
} from "@/services/lancamentos";

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

const CLIENTE_STATE = {
  id: 1,
  codigo: 10001,
  cliente: "ANA SILVA",
};

function renderComponent(state: object | null = CLIENTE_STATE) {
  return render(
    <MemoryRouter
      initialEntries={[{ pathname: "/lancamentos", state }]}
    >
      <Routes>
        <Route path="/lancamentos" element={<Lanca />} />
      </Routes>
    </MemoryRouter>,
  );
}

// ---------------------------------------------------------------------------
// Setup
// ---------------------------------------------------------------------------

afterEach(cleanup);

beforeEach(() => {
  vi.clearAllMocks();
  vi.mocked(verificarPasta).mockResolvedValue({ existe: true });
  vi.mocked(registrarLancamento).mockResolvedValue({
    id: 42,
    mensagem: "Lançamento registrado com sucesso.",
  });
});

// ---------------------------------------------------------------------------
// Testes
// ---------------------------------------------------------------------------

describe("Lanca — renderização", () => {
  it("exibe título e nome do cliente", () => {
    renderComponent();

    expect(screen.getByText("LANÇAMENTO")).toBeInTheDocument();
    expect(screen.getByText("ANA SILVA")).toBeInTheDocument();
  });

  it("exibe campos do formulário", () => {
    renderComponent();

    expect(screen.getByLabelText("Data")).toBeInTheDocument();
    expect(screen.getByLabelText("Pasta")).toBeInTheDocument();
    expect(screen.getByLabelText("Histórico")).toBeInTheDocument();
    expect(screen.getByLabelText("Valor")).toBeInTheDocument();
    expect(screen.getByLabelText("D / C")).toBeInTheDocument();
  });

  it("exibe botão Retornar e oculta Salvar/Desfazer no estado inicial (RN55)", () => {
    renderComponent();

    expect(screen.getByRole("button", { name: "Retornar" })).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: /salvar/i })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: /desfazer/i })).not.toBeInTheDocument();
  });

  it("redireciona para /principal se state ausente (RN50)", async () => {
    renderComponent(null);

    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith("/principal", { replace: true });
    });
  });
});

describe("Lanca — visibilidade de botões (RN55)", () => {
  it("revela Salvar e Desfazer ao preencher qualquer campo", async () => {
    const user = userEvent.setup();
    renderComponent();

    await user.type(screen.getByLabelText("Pasta"), "5");

    expect(screen.getByRole("button", { name: /salvar/i })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /desfazer/i })).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Retornar" })).not.toBeInTheDocument();
  });
});

describe("Lanca — lançamento débito (RN53)", () => {
  it("chama registrarLancamento com DC='D' e navega após sucesso (RN58)", async () => {
    const user = userEvent.setup();
    renderComponent();

    await user.type(screen.getByLabelText("Pasta"), "5");
    await user.type(screen.getByLabelText("Histórico"), "Aluguel março");
    await user.type(screen.getByLabelText("Valor"), "1500");
    await user.selectOptions(screen.getByLabelText("D / C"), "D");

    await user.click(screen.getByRole("button", { name: /salvar/i }));

    await waitFor(() => {
      expect(registrarLancamento).toHaveBeenCalledWith(
        expect.objectContaining({
          id_cliente: 1,
          conta: 5,
          ref: "Aluguel março",
          vvalor: "1500",
          dc: "D",
        }),
      );
    });

    // RN58 — botão Novo aparece após salvar
    await waitFor(() => {
      expect(screen.getByRole("button", { name: /novo/i })).toBeInTheDocument();
    });
  });
});

describe("Lanca — lançamento crédito (RN53)", () => {
  it("chama registrarLancamento com DC='C'", async () => {
    const user = userEvent.setup();
    renderComponent();

    await user.type(screen.getByLabelText("Pasta"), "3");
    await user.type(screen.getByLabelText("Histórico"), "Devolução");
    await user.type(screen.getByLabelText("Valor"), "500");
    await user.selectOptions(screen.getByLabelText("D / C"), "C");

    await user.click(screen.getByRole("button", { name: /salvar/i }));

    await waitFor(() => {
      expect(registrarLancamento).toHaveBeenCalledWith(
        expect.objectContaining({ dc: "C" }),
      );
    });
  });
});

describe("Lanca — validação de campos obrigatórios (RN54)", () => {
  it("exibe erro de Pasta vazia sem chamar a API", async () => {
    const user = userEvent.setup();
    renderComponent();

    // Preenche só Histórico para ativar dirty e revelar Salvar
    await user.type(screen.getByLabelText("Histórico"), "Teste");

    await user.click(screen.getByRole("button", { name: /salvar/i }));

    await waitFor(() => {
      expect(screen.getByText("Informe a pasta")).toBeInTheDocument();
    });
    expect(registrarLancamento).not.toHaveBeenCalled();
  });

  it("exibe erro de Histórico vazio sem chamar a API", async () => {
    const user = userEvent.setup();
    renderComponent();

    await user.type(screen.getByLabelText("Pasta"), "5");

    await user.click(screen.getByRole("button", { name: /salvar/i }));

    await waitFor(() => {
      expect(screen.getByText("Informe o histórico")).toBeInTheDocument();
    });
    expect(registrarLancamento).not.toHaveBeenCalled();
  });

  it("exibe erro de D/C não selecionado sem chamar a API", async () => {
    const user = userEvent.setup();
    renderComponent();

    await user.type(screen.getByLabelText("Pasta"), "5");
    await user.type(screen.getByLabelText("Histórico"), "Aluguel");
    await user.type(screen.getByLabelText("Valor"), "100");
    // D/C não selecionado

    await user.click(screen.getByRole("button", { name: /salvar/i }));

    await waitFor(() => {
      expect(screen.getByText("Informe D ou C")).toBeInTheDocument();
    });
    expect(registrarLancamento).not.toHaveBeenCalled();
  });
});

describe("Lanca — pasta nova (RN51)", () => {
  it("exibe modal quando pasta não existe", async () => {
    vi.mocked(verificarPasta).mockResolvedValueOnce({ existe: false });
    const user = userEvent.setup();
    renderComponent();

    const campoPasta = screen.getByLabelText("Pasta");
    await user.type(campoPasta, "99");
    await user.tab(); // dispara onBlur

    await waitFor(() => {
      expect(screen.getByRole("dialog")).toBeInTheDocument();
      expect(screen.getByText(/nova pasta/i)).toBeInTheDocument();
    });
  });

  it("limpa campo e fecha modal ao clicar Não (RN51)", async () => {
    vi.mocked(verificarPasta).mockResolvedValueOnce({ existe: false });
    const user = userEvent.setup();
    renderComponent();

    const campoPasta = screen.getByLabelText("Pasta");
    await user.type(campoPasta, "99");
    await user.tab();

    await waitFor(() => {
      expect(screen.getByRole("dialog")).toBeInTheDocument();
    });

    await user.click(screen.getByRole("button", { name: "Não" }));

    await waitFor(() => {
      expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
    });
    expect((screen.getByLabelText("Pasta") as HTMLInputElement).value).toBe("");
  });
});

describe("Lanca — botão Desfazer (RN57)", () => {
  it("limpa campos e volta ao estado inicial", async () => {
    const user = userEvent.setup();
    renderComponent();

    await user.type(screen.getByLabelText("Pasta"), "5");
    await user.type(screen.getByLabelText("Histórico"), "Teste");

    expect(screen.getByRole("button", { name: /salvar/i })).toBeInTheDocument();

    await user.click(screen.getByRole("button", { name: /desfazer/i }));

    // RN55 — Salvar volta a ficar oculto
    await waitFor(() => {
      expect(screen.queryByRole("button", { name: /salvar/i })).not.toBeInTheDocument();
    });
    expect((screen.getByLabelText("Pasta") as HTMLInputElement).value).toBe("");
  });
});
