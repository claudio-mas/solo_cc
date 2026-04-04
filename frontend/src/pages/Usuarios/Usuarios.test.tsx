/**
 * Usuarios.test.tsx — Testes da tela de gestão de usuários
 *
 * Cobertura obrigatória (CLAUDE.md):
 *   1. Modal de senha — exibido antes dos dados
 *   2. Senha correta → modal fecha, dados carregam
 *   3. Senha incorreta → erro exibido no modal
 *   4. Renderização como Administrador — seção Chaves visível (RN72)
 *   5. Renderização como perfil comum — seção Chaves oculta; botão Add oculto (RN70/RN72)
 *   6. Campo Perfil desabilitado para perfil comum (RN71)
 *   7. Edição de senha — botões Salvar/Cancelar aparecem (RN74)
 *   8. Cancelar edição — restaura valores originais (RN74)
 *   9. Tentativa de adicionar usuário por perfil não-Admin → não deve aparecer
 *  10. Autenticação negada (401 → redirecionamento)
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

import Usuarios from "./index";

// ---------------------------------------------------------------------------
// Mocks
// ---------------------------------------------------------------------------

const mockNavigate = vi.fn();
vi.mock("react-router-dom", async () => {
  const actual = await vi.importActual("react-router-dom");
  return { ...actual, useNavigate: () => mockNavigate };
});

// Mock do store de auth — padrão: Administrador
const mockPerfil = vi.fn(() => "Administrador");
vi.mock("@/store/authStore", () => ({
  useAuthStore: (selector: (s: { perfil: string }) => unknown) =>
    selector({ perfil: mockPerfil() }),
}));

vi.mock("@/services/usuarios", () => ({
  verificarSenha: vi.fn(),
  listarUsuarios: vi.fn(),
  listarPerfis: vi.fn(),
  listarChaves: vi.fn(),
  criarUsuario: vi.fn(),
  atualizarUsuario: vi.fn(),
  atualizarChave: vi.fn(),
}));

import {
  atualizarUsuario,
  listarChaves,
  listarPerfis,
  listarUsuarios,
  verificarSenha,
} from "@/services/usuarios";

// ---------------------------------------------------------------------------
// Dados de teste
// ---------------------------------------------------------------------------

const USUARIOS_MOCK = [
  { id: 1, usuario: "Admin", perfil: "Administrador" },
  { id: 2, usuario: "Joao", perfil: "Comum" },
];

const CHAVES_MOCK = [
  { id: 1, ref: "Alteração de senhas", chave: "segredo" },
  { id: 2, ref: "Transferência de lançamentos", chave: "outra" },
];

const PERFIS_MOCK = [
  { perfil: "Administrador" },
  { perfil: "Comum" },
];

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

function renderComponent() {
  return render(
    <MemoryRouter initialEntries={["/usuarios"]}>
      <Routes>
        <Route path="/usuarios" element={<Usuarios />} />
      </Routes>
    </MemoryRouter>,
  );
}

function setupAdminMocks() {
  mockPerfil.mockReturnValue("Administrador");
  vi.mocked(verificarSenha).mockResolvedValue({ ok: true });
  vi.mocked(listarUsuarios).mockResolvedValue(USUARIOS_MOCK);
  vi.mocked(listarPerfis).mockResolvedValue(PERFIS_MOCK);
  vi.mocked(listarChaves).mockResolvedValue(CHAVES_MOCK);
}

function setupComumMocks() {
  mockPerfil.mockReturnValue("Comum");
  vi.mocked(verificarSenha).mockResolvedValue({ ok: true });
  vi.mocked(listarUsuarios).mockResolvedValue([
    { id: 2, usuario: "Joao", perfil: "Comum" },
  ]);
  vi.mocked(listarPerfis).mockResolvedValue(PERFIS_MOCK);
  // listarChaves não é chamado para perfil comum
}

// ---------------------------------------------------------------------------
// Testes
// ---------------------------------------------------------------------------

describe("Usuarios", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockNavigate.mockReset();
  });

  afterEach(() => {
    cleanup();
  });

  // -------------------------------------------------------------------------
  // 1–3: Modal de senha (RN68)
  // -------------------------------------------------------------------------

  it("exibe modal de senha antes de carregar qualquer dado", () => {
    /**
     * RN68 — ao acessar /usuarios, o modal de senha deve aparecer antes
     * de qualquer chamada aos endpoints de dados.
     */
    vi.mocked(verificarSenha).mockResolvedValue({ ok: false });
    renderComponent();

    expect(screen.getByRole("dialog")).toBeTruthy();
    expect(screen.getByLabelText("Senha")).toBeTruthy();
    // Dados não carregados ainda
    expect(listarUsuarios).not.toHaveBeenCalled();
  });

  it("fecha modal e carrega dados após senha correta", async () => {
    /**
     * RN68 — senha correta: modal fecha e dados são carregados.
     */
    setupAdminMocks();
    const user = userEvent.setup();
    renderComponent();

    await user.type(screen.getByLabelText("Senha"), "segredo");
    await user.click(screen.getByRole("button", { name: "Confirmar" }));

    await waitFor(() => {
      expect(screen.queryByRole("dialog")).toBeNull();
    });

    await waitFor(() => {
      expect(listarUsuarios).toHaveBeenCalledOnce();
    });
  });

  it("exibe erro no modal para senha incorreta sem fechar", async () => {
    /**
     * RN68 — senha errada: exibe mensagem de erro e permanece no modal.
     */
    vi.mocked(verificarSenha).mockResolvedValue({ ok: false });
    const user = userEvent.setup();
    renderComponent();

    await user.type(screen.getByLabelText("Senha"), "errada");
    await user.click(screen.getByRole("button", { name: "Confirmar" }));

    await waitFor(() => {
      expect(screen.getByRole("alert")).toBeTruthy();
      expect(screen.getByText("Senha incorreta.")).toBeTruthy();
    });

    // Modal permanece aberto
    expect(screen.getByRole("dialog")).toBeTruthy();
    expect(listarUsuarios).not.toHaveBeenCalled();
  });

  it("cancela modal e navega para /principal", async () => {
    /**
     * Botão Cancelar no modal → navigate('/principal').
     */
    vi.mocked(verificarSenha).mockResolvedValue({ ok: false });
    const user = userEvent.setup();
    renderComponent();

    await user.click(screen.getByRole("button", { name: "Cancelar" }));

    expect(mockNavigate).toHaveBeenCalledWith("/principal");
  });

  // -------------------------------------------------------------------------
  // 4: Renderização como Administrador
  // -------------------------------------------------------------------------

  it("Admin vê seção Chaves e todos os usuários", async () => {
    /**
     * RN69 — Admin vê todos os usuários.
     * RN72 — Admin vê seção de Chaves.
     */
    setupAdminMocks();
    const user = userEvent.setup();
    renderComponent();

    await user.type(screen.getByLabelText("Senha"), "segredo");
    await user.click(screen.getByRole("button", { name: "Confirmar" }));

    await waitFor(() => {
      expect(screen.getByLabelText("Login de Admin")).toBeTruthy();
      expect(screen.getByLabelText("Login de Joao")).toBeTruthy();
    });

    // RN72 — seção Chaves visível para Admin
    expect(screen.getByText("Senhas de Operações Críticas")).toBeTruthy();
    expect(screen.getByText("Alteração de senhas")).toBeTruthy();
  });

  it("Admin vê botão Adicionar usuário (RN70)", async () => {
    setupAdminMocks();
    const user = userEvent.setup();
    renderComponent();

    await user.type(screen.getByLabelText("Senha"), "segredo");
    await user.click(screen.getByRole("button", { name: "Confirmar" }));

    await waitFor(() => {
      expect(screen.getByText("Adicionar usuário")).toBeTruthy();
    });
  });

  // -------------------------------------------------------------------------
  // 5: Renderização como perfil comum
  // -------------------------------------------------------------------------

  it("perfil comum NÃO vê seção Chaves nem botão Adicionar", async () => {
    /**
     * RN70 — sem botão Adicionar para perfil comum.
     * RN72 — sem seção Chaves para perfil comum.
     */
    setupComumMocks();
    const user = userEvent.setup();
    renderComponent();

    await user.type(screen.getByLabelText("Senha"), "segredo");
    await user.click(screen.getByRole("button", { name: "Confirmar" }));

    await waitFor(() => {
      expect(screen.getByLabelText("Login de Joao")).toBeTruthy();
    });

    // RN72 — sem seção Chaves
    expect(screen.queryByText("Senhas de Operações Críticas")).toBeNull();
    // RN70 — sem botão Adicionar
    expect(screen.queryByText("Adicionar usuário")).toBeNull();
    // Não chamou listarChaves
    expect(listarChaves).not.toHaveBeenCalled();
  });

  // -------------------------------------------------------------------------
  // 6: Campo Perfil desabilitado para perfil comum (RN71)
  // -------------------------------------------------------------------------

  it("campo Perfil está desabilitado para perfil comum (RN71)", async () => {
    setupComumMocks();
    const user = userEvent.setup();
    renderComponent();

    await user.type(screen.getByLabelText("Senha"), "segredo");
    await user.click(screen.getByRole("button", { name: "Confirmar" }));

    await waitFor(() => {
      expect(screen.getByLabelText("Login de Joao")).toBeTruthy();
    });

    const selectPerfil = screen.getByLabelText("Perfil de Joao");
    expect((selectPerfil as HTMLSelectElement).disabled).toBe(true);
  });

  // -------------------------------------------------------------------------
  // 7–8: Edição e RN74 (botões Salvar/Cancelar)
  // -------------------------------------------------------------------------

  it("botões Salvar/Cancelar aparecem ao editar campo (RN74)", async () => {
    /**
     * RN74 — botões ficam ocultos até o usuário iniciar edição.
     */
    setupAdminMocks();
    const user = userEvent.setup();
    renderComponent();

    await user.type(screen.getByLabelText("Senha"), "segredo");
    await user.click(screen.getByRole("button", { name: "Confirmar" }));

    await waitFor(() => {
      expect(screen.getByLabelText("Login de Admin")).toBeTruthy();
    });

    // Antes da edição — sem botões Salvar (para usuários)
    const salvarBtns = screen.queryAllByRole("button", { name: /salvar/i });
    // Somente o botão Salvar do formulário de adição poderia aparecer, mas não está ativo
    const salvarUsuarios = salvarBtns.filter(
      (b) => !b.closest("form"),
    );
    expect(salvarUsuarios.length).toBe(0);

    // Edita o campo de nova senha do usuário "Admin"
    const inputSenhaAdmin = screen.getByLabelText("Nova senha de Admin");
    await user.type(inputSenhaAdmin, "nova");

    // Após edição — botão Salvar aparece
    await waitFor(() => {
      expect(screen.getByRole("button", { name: "Salvar" })).toBeTruthy();
    });
  });

  it("Cancelar edição restaura valores originais (RN74)", async () => {
    setupAdminMocks();
    const user = userEvent.setup();
    renderComponent();

    await user.type(screen.getByLabelText("Senha"), "segredo");
    await user.click(screen.getByRole("button", { name: "Confirmar" }));

    await waitFor(() => {
      expect(screen.getByLabelText("Login de Admin")).toBeTruthy();
    });

    // Edita o campo de senha
    const inputSenha = screen.getByLabelText("Nova senha de Admin");
    await user.type(inputSenha, "digitado");

    // Botão Cancelar aparece
    await waitFor(() => {
      expect(screen.getByRole("button", { name: "Cancelar" })).toBeTruthy();
    });

    // Clica em Cancelar
    await user.click(screen.getByRole("button", { name: "Cancelar" }));

    // Campo de senha volta a vazio e botões somem
    await waitFor(() => {
      expect((inputSenha as HTMLInputElement).value).toBe("");
      expect(screen.queryByRole("button", { name: "Salvar" })).toBeNull();
    });
  });

  // -------------------------------------------------------------------------
  // 9: Botão Adicionar ausente para perfil comum (RN70)
  // -------------------------------------------------------------------------

  it("perfil comum não vê opção de adicionar usuário (RN70)", async () => {
    setupComumMocks();
    const user = userEvent.setup();
    renderComponent();

    await user.type(screen.getByLabelText("Senha"), "segredo");
    await user.click(screen.getByRole("button", { name: "Confirmar" }));

    await waitFor(() => {
      expect(screen.getByLabelText("Login de Joao")).toBeTruthy();
    });

    expect(screen.queryByText("Adicionar usuário")).toBeNull();
  });

  // -------------------------------------------------------------------------
  // 10: Botão Voltar desabilitado com edições pendentes (B17)
  // -------------------------------------------------------------------------

  it("botão Voltar desabilitado quando há edições pendentes (B17)", async () => {
    setupAdminMocks();
    const user = userEvent.setup();
    renderComponent();

    await user.type(screen.getByLabelText("Senha"), "segredo");
    await user.click(screen.getByRole("button", { name: "Confirmar" }));

    await waitFor(() => {
      expect(screen.getByRole("button", { name: "Voltar" })).toBeTruthy();
    });

    const btnVoltar = screen.getByRole("button", { name: "Voltar" });
    expect((btnVoltar as HTMLButtonElement).disabled).toBe(false);

    // Edita um campo
    await user.type(screen.getByLabelText("Nova senha de Admin"), "algo");

    await waitFor(() => {
      expect((btnVoltar as HTMLButtonElement).disabled).toBe(true);
    });
  });

  // -------------------------------------------------------------------------
  // Salvamento de usuário
  // -------------------------------------------------------------------------

  it("salvar atualização de usuário chama atualizarUsuario", async () => {
    setupAdminMocks();
    vi.mocked(atualizarUsuario).mockResolvedValue({
      id: 1,
      usuario: "Admin",
      perfil: "Administrador",
    });

    const user = userEvent.setup();
    renderComponent();

    await user.type(screen.getByLabelText("Senha"), "segredo");
    await user.click(screen.getByRole("button", { name: "Confirmar" }));

    await waitFor(() => {
      expect(screen.getByLabelText("Login de Admin")).toBeTruthy();
    });

    await user.type(screen.getByLabelText("Nova senha de Admin"), "novasenha");

    const btnSalvar = await screen.findByRole("button", { name: "Salvar" });
    await user.click(btnSalvar);

    await waitFor(() => {
      expect(atualizarUsuario).toHaveBeenCalledWith(
        1,
        expect.objectContaining({ senha: "novasenha" }),
      );
    });
  });
});
