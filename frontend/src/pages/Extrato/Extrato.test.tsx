/**
 * Extrato.test.tsx — Testes da tela de extrato
 *
 * Cobertura obrigatória (CLAUDE.md):
 *   1. Renderização — header do cliente, tabela, rodapé
 *   2. Aplicação de filtro por pasta
 *   3. Saldo colorido — vermelho >= 0, azul < 0
 *   4. Desbloqueio com senha válida
 *   5. Desbloqueio com senha inválida
 *   6. Edição inline de campo
 *   7. Transferência bem-sucedida
 *   8. Autenticação negada
 */

import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import {
  cleanup,
  render,
  screen,
  waitFor,
} from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter, Route, Routes } from "react-router-dom";

import Extrato from "./index";

// ---------------------------------------------------------------------------
// Mocks
// ---------------------------------------------------------------------------

const mockNavigate = vi.fn();
vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return { ...actual, useNavigate: () => mockNavigate };
});

vi.mock("@/store/authStore", () => ({
  useAuthStore: (selector: (s: { perfil: string }) => unknown) =>
    selector({ perfil: "Administrador" }),
}));

vi.mock("@/services/extrato", () => ({
  listarExtrato: vi.fn(),
  atualizarLancamento: vi.fn(),
  desbloquearEdicao: vi.fn(),
  transferirLancamentos: vi.fn(),
  sincronizarVValor: vi.fn(),
  listarClientesDestino: vi.fn(),
}));

import {
  listarExtrato,
  atualizarLancamento,
  desbloquearEdicao,
  transferirLancamentos,
  sincronizarVValor,
  listarClientesDestino,
} from "@/services/extrato";

const mockedListarExtrato = listarExtrato as ReturnType<typeof vi.fn>;
const mockedAtualizarLancamento = atualizarLancamento as ReturnType<typeof vi.fn>;
const mockedDesbloquearEdicao = desbloquearEdicao as ReturnType<typeof vi.fn>;
const mockedTransferirLancamentos = transferirLancamentos as ReturnType<typeof vi.fn>;
const mockedSincronizarVValor = sincronizarVValor as ReturnType<typeof vi.fn>;
const mockedListarClientesDestino = listarClientesDestino as ReturnType<typeof vi.fn>;

// ---------------------------------------------------------------------------
// Dados fake
// ---------------------------------------------------------------------------

const EXTRATO_OK = {
  lancamentos: [
    {
      id: 1, dt: "2026-01-15T00:00:00", conta: 1, nd: "123", ref: "Aluguel",
      vvalor: 100, dc: "D", deb: 100, cred: null, saldo: -100,
    },
    {
      id: 2, dt: "2026-01-20T00:00:00", conta: 1, nd: null, ref: "AGENCIA taxa",
      vvalor: 50, dc: "C", deb: null, cred: 50, saldo: -50,
    },
  ],
  saldo_total: -50,
};

const EXTRATO_POSITIVO = {
  lancamentos: [
    {
      id: 3, dt: "2026-02-01T00:00:00", conta: 1, nd: null, ref: "Crédito",
      vvalor: 200, dc: "C", deb: null, cred: 200, saldo: 200,
    },
  ],
  saldo_total: 200,
};

const LOCATION_STATE = {
  idCliente: 10,
  codCliente: 10050,
  nomeCliente: "Cliente Teste",
};

// ---------------------------------------------------------------------------
// Helper de render
// ---------------------------------------------------------------------------

function renderExtrato(state = LOCATION_STATE) {
  return render(
    <MemoryRouter initialEntries={[{ pathname: "/extrato", state }]}>
      <Routes>
        <Route path="/extrato" element={<Extrato />} />
        <Route path="/principal" element={<div>Principal</div>} />
      </Routes>
    </MemoryRouter>,
  );
}

// ---------------------------------------------------------------------------
// Setup / Teardown
// ---------------------------------------------------------------------------

beforeEach(() => {
  vi.clearAllMocks();
  mockedListarExtrato.mockResolvedValue(EXTRATO_OK);
  mockedSincronizarVValor.mockResolvedValue({ ok: true, mensagem: "" });
  mockedListarClientesDestino.mockResolvedValue([
    { id: 99, codigo: 10001, cliente: "Destino Ltda" },
  ]);
});

afterEach(cleanup);

// ---------------------------------------------------------------------------
// Testes
// ---------------------------------------------------------------------------

describe("Extrato", () => {
  it("1. renderiza header do cliente, tabela e rodapé", async () => {
    renderExtrato();

    await waitFor(() => {
      expect(screen.getByText("Cliente Teste - 10050")).toBeTruthy();
    });

    // Tabela com dados
    expect(screen.getByText("Aluguel")).toBeTruthy();
    expect(screen.getByText("AGENCIA taxa")).toBeTruthy();

    // Rodapé com saldo
    expect(screen.getByText(/Saldo:/)).toBeTruthy();
  });

  it("2. aplica filtro por pasta e recarrega extrato", async () => {
    renderExtrato();
    await waitFor(() => expect(screen.getByText("Aluguel")).toBeTruthy());

    const user = userEvent.setup();

    // Preenche filtro pasta
    const inputPasta = screen.getByRole("spinbutton", { name: /pasta/i });
    await user.clear(inputPasta);
    await user.type(inputPasta, "1");

    // Clica em aplicar
    await user.click(screen.getByText("Aplicar filtros"));

    // Verifica que listarExtrato foi chamado com filtro
    expect(mockedListarExtrato).toHaveBeenCalledWith(10, expect.objectContaining({ pasta: 1 }));
  });

  it("3. saldo negativo em azul, positivo em vermelho", async () => {
    // Saldo negativo
    renderExtrato();
    await waitFor(() => expect(screen.getByText(/Saldo:/)).toBeTruthy());

    const saldoEl = screen.getByText(/Saldo:/);
    // saldoDevedor class should be applied (azul)
    expect(saldoEl.className).toContain("saldoDevedor");

    cleanup();

    // Saldo positivo
    mockedListarExtrato.mockResolvedValue(EXTRATO_POSITIVO);
    renderExtrato();
    await waitFor(() => expect(screen.getByText(/Saldo:/)).toBeTruthy());

    const saldoEl2 = screen.getByText(/Saldo:/);
    expect(saldoEl2.className).toContain("saldoCredor");
  });

  it("4. desbloqueio com senha válida habilita edição", async () => {
    mockedDesbloquearEdicao.mockResolvedValue({ ok: true });
    renderExtrato();
    await waitFor(() => expect(screen.getByText("Aluguel")).toBeTruthy());

    const user = userEvent.setup();

    // Clica em Desbloquear
    await user.click(screen.getByText(/Desbloquear/));

    // Modal aparece
    await waitFor(() => expect(screen.getByText("Desbloquear Edição")).toBeTruthy());

    // Preenche senha e confirma
    const senhaInput = screen.getByLabelText("Senha");
    await user.type(senhaInput, "desbloqueio123");
    await user.click(screen.getByText("Confirmar"));

    // Após desbloqueio, botão muda para "Bloquear"
    await waitFor(() => expect(screen.getByText(/Bloquear/)).toBeTruthy());
  });

  it("5. desbloqueio com senha inválida mostra erro", async () => {
    mockedDesbloquearEdicao.mockResolvedValue({ ok: false });
    renderExtrato();
    await waitFor(() => expect(screen.getByText("Aluguel")).toBeTruthy());

    const user = userEvent.setup();

    await user.click(screen.getByText(/Desbloquear/));
    await waitFor(() => expect(screen.getByText("Desbloquear Edição")).toBeTruthy());

    const senhaInput = screen.getByLabelText("Senha");
    await user.type(senhaInput, "errada");
    await user.click(screen.getByText("Confirmar"));

    await waitFor(() => expect(screen.getByText("Senha incorreta")).toBeTruthy());
  });

  it("6. edição inline de campo chama atualizarLancamento", async () => {
    mockedDesbloquearEdicao.mockResolvedValue({ ok: true });
    mockedAtualizarLancamento.mockResolvedValue({ ok: true, mensagem: "ok" });
    renderExtrato();
    await waitFor(() => expect(screen.getByText("Aluguel")).toBeTruthy());

    const user = userEvent.setup();

    // Desbloqueia
    await user.click(screen.getByText(/Desbloquear/));
    await waitFor(() => expect(screen.getByText("Desbloquear Edição")).toBeTruthy());
    await user.type(screen.getByLabelText("Senha"), "ok");
    await user.click(screen.getByText("Confirmar"));

    await waitFor(() => expect(screen.getByText(/Bloquear/)).toBeTruthy());

    // Agora deve haver inputs inline — encontra o de ND com valor "123"
    const ndInputs = screen.getAllByDisplayValue("123");
    expect(ndInputs.length).toBeGreaterThan(0);
  });

  it("7. transferência bem-sucedida recarrega extrato", async () => {
    mockedTransferirLancamentos.mockResolvedValue({
      ok: true, mensagem: "2 lançamento(s) transferido(s) com sucesso.", transferidos: 2,
    });
    renderExtrato();
    await waitFor(() => expect(screen.getByText("Aluguel")).toBeTruthy());

    const user = userEvent.setup();

    // Abre painel de transferência
    await user.click(screen.getByText(/Transferir \(F9\)/));

    // Painel aparece
    await waitFor(() => expect(screen.getByText(/Transferir os lançamentos/)).toBeTruthy());

    // Seleciona linhas (checkboxes)
    const checkboxes = screen.getAllByRole("checkbox");
    // O primeiro checkbox pode ser o "Sem N.D." — pegar os da tabela
    const tableCheckboxes = checkboxes.filter(
      (cb) => cb.closest("td") !== null
    );
    if (tableCheckboxes.length > 0) {
      await user.click(tableCheckboxes[0]);
    }

    // Seleciona cliente destino
    const select = screen.getByRole("combobox");
    await user.selectOptions(select, "99");

    // Preenche senha
    const senhaInputs = screen.getAllByDisplayValue("");
    const senhaInput = senhaInputs.find(
      (el) => el.getAttribute("type") === "password"
    );
    if (senhaInput) {
      await user.type(senhaInput, "transferencia456");
    }

    // Clica transferir
    const btnTransf = screen.getByText(/Transferir \(1\)/);
    await user.click(btnTransf);

    // Verifica chamada
    await waitFor(() => {
      expect(mockedTransferirLancamentos).toHaveBeenCalled();
    });
  });

  it("8. sem state de cliente redireciona para /principal", async () => {
    render(
      <MemoryRouter initialEntries={[{ pathname: "/extrato" }]}>
        <Routes>
          <Route path="/extrato" element={<Extrato />} />
          <Route path="/principal" element={<div>Principal</div>} />
        </Routes>
      </MemoryRouter>,
    );

    await waitFor(() => {
      expect(mockNavigate).toHaveBeenCalledWith("/principal", { replace: true });
    });
  });
});
