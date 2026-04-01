/**
 * pages/Usuarios/index.tsx — Equivalente web de FrmUsuarios
 * Sistema: Solo Consultoria de Imóveis — Contas Correntes
 *
 * Regras implementadas:
 *   RN68 — Modal de senha antes de exibir dados (frmSenha varSenha="1")
 *   RN69 — Admin vê todos; perfil comum vê apenas o próprio
 *   RN70 — Apenas Admin pode adicionar usuários
 *   RN71 — Campo Perfil editável apenas pelo Admin
 *   RN72 — Seção Chaves visível apenas para Admin
 *   RN73 — Senha salva como bcrypt (responsabilidade do backend)
 *   RN74 — Botões Salvar/Cancelar aparecem somente após edição (isDirty)
 *   RN75 — Ref em Chaves é somente leitura
 *   RN76 — Sem adição ou exclusão na seção Chaves
 *   RN77 — Id não exibido ao usuário
 *
 * B01/B02 — FrmPrincipal.Visible: DESCARTAR (React Router)
 * B18     — frmSenha: ADAPTAR → modal + POST /auth/verificar-senha
 * D8      — duas abas → duas seções na mesma página
 * D11     — edição inline com Salvar/Cancelar por linha
 * D12     — Psw nunca exibida
 */

import React, { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuthStore } from "@/store/authStore";
import {
  atualizarChave,
  atualizarUsuario,
  ChaveResponse,
  criarUsuario,
  listarChaves,
  listarPerfis,
  listarUsuarios,
  PerfilItem,
  UsuarioResponse,
  verificarSenha,
} from "@/services/usuarios";
import styles from "./Usuarios.module.css";

// ---------------------------------------------------------------------------
// Tipos locais
// ---------------------------------------------------------------------------

interface UsuarioEditavel extends UsuarioResponse {
  _novoUsuario: string;
  _novaSenha: string;
  _novoPerfil: string;
  _dirty: boolean;
  _salvando: boolean;
  _erro: string | null;
}

interface ChaveEditavel extends ChaveResponse {
  _novaChave: string;
  _dirty: boolean;
  _salvando: boolean;
  _erro: string | null;
}

// ---------------------------------------------------------------------------
// SVG inline — ícones sem bibliotecas externas
// ---------------------------------------------------------------------------

function IconePlus() {
  return (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      width="16"
      height="16"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2.5"
      strokeLinecap="round"
      strokeLinejoin="round"
      aria-hidden="true"
    >
      <line x1="12" y1="5" x2="12" y2="19" />
      <line x1="5" y1="12" x2="19" y2="12" />
    </svg>
  );
}

// ---------------------------------------------------------------------------
// Modal de senha (RN68 — frmSenha varSenha="1")
// ---------------------------------------------------------------------------

interface ModalSenhaProps {
  onConfirmar: () => void;
  onCancelar: () => void;
}

function ModalSenha({ onConfirmar, onCancelar }: ModalSenhaProps) {
  const [chave, setChave] = useState("");
  const [erro, setErro] = useState<string | null>(null);
  const [verificando, setVerificando] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    inputRef.current?.focus();
  }, []);

  async function handleConfirmar(e: React.FormEvent) {
    e.preventDefault();
    if (!chave.trim()) {
      setErro("Informe a senha.");
      return;
    }
    setVerificando(true);
    setErro(null);
    try {
      const resp = await verificarSenha("Alteração de senhas", chave);
      if (resp.ok) {
        onConfirmar();
      } else {
        setErro("Senha incorreta.");
        setChave("");
        inputRef.current?.focus();
      }
    } catch {
      setErro("Erro ao verificar senha. Tente novamente.");
    } finally {
      setVerificando(false);
    }
  }

  return (
    <div className={styles.modalOverlay} role="dialog" aria-modal="true" aria-labelledby="modal-titulo">
      <div className={styles.modalCard}>
        <h2 id="modal-titulo" className={styles.modalTitulo}>
          Acesso à Gestão de Usuários
        </h2>
        <p className={styles.modalDescricao}>
          Informe a senha para acessar esta tela.
        </p>
        <form onSubmit={handleConfirmar}>
          <label htmlFor="modal-senha" className={styles.modalLabel}>
            Senha
          </label>
          <input
            id="modal-senha"
            ref={inputRef}
            type="password"
            value={chave}
            onChange={(e) => setChave(e.target.value)}
            className={styles.modalInput}
            autoComplete="off"
            disabled={verificando}
          />
          {erro && (
            <p className={styles.modalErro} role="alert">
              {erro}
            </p>
          )}
          <div className={styles.modalBotoes}>
            <button
              type="submit"
              className={styles.modalBtnConfirmar}
              disabled={verificando}
            >
              {verificando ? "Verificando..." : "Confirmar"}
            </button>
            <button
              type="button"
              className={styles.modalBtnCancelar}
              onClick={onCancelar}
              disabled={verificando}
            >
              Cancelar
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

// ---------------------------------------------------------------------------
// Componente principal
// ---------------------------------------------------------------------------

export default function Usuarios() {
  const navigate = useNavigate();
  const perfil = useAuthStore((s) => s.perfil);
  const isAdmin = perfil === "Administrador";

  // RN68 — modal de senha antes de carregar dados
  const [senhaOk, setSenhaOk] = useState(false);

  // Dados
  const [usuarios, setUsuarios] = useState<UsuarioEditavel[]>([]);
  const [chaves, setChaves] = useState<ChaveEditavel[]>([]);
  const [perfisDisponiveis, setPerfisDisponiveis] = useState<PerfilItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [erro, setErro] = useState<string | null>(null);

  // Formulário de adição de usuário
  const [mostraFormAdd, setMostraFormAdd] = useState(false);
  const [novoLogin, setNovoLogin] = useState("");
  const [novaSenha, setNovaSenha] = useState("");
  const [novoPerfil, setNovoPerfil] = useState("");
  const [erroAdd, setErroAdd] = useState<string | null>(null);
  const [adicionando, setAdicionando] = useState(false);

  // RN74 — controla visibilidade do botão Voltar (desabilitado com edições)
  const temEdicaoPendente =
    usuarios.some((u) => u._dirty) || chaves.some((c) => c._dirty);

  // -------------------------------------------------------------------------
  // Carrega dados após validação de senha
  // -------------------------------------------------------------------------

  async function carregarDados() {
    setLoading(true);
    setErro(null);
    try {
      const [usrs, perfs] = await Promise.all([
        listarUsuarios(),
        listarPerfis(),
      ]);

      setUsuarios(
        usrs.map((u) => ({
          ...u,
          _novoUsuario: u.usuario,
          _novaSenha: "",
          _novoPerfil: u.perfil,
          _dirty: false,
          _salvando: false,
          _erro: null,
        })),
      );
      setPerfisDisponiveis(perfs);

      if (isAdmin) {
        const chvs = await listarChaves();
        setChaves(
          chvs.map((c) => ({
            ...c,
            _novaChave: c.chave,
            _dirty: false,
            _salvando: false,
            _erro: null,
          })),
        );
      }
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : "Erro ao carregar dados";
      setErro(msg);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    if (senhaOk) {
      carregarDados();
    }
  }, [senhaOk]); // eslint-disable-line react-hooks/exhaustive-deps

  // -------------------------------------------------------------------------
  // Edição de usuários (RN69–RN71, RN73, RN74)
  // -------------------------------------------------------------------------

  function handleUsuarioChange(
    id: number,
    campo: "login" | "senha" | "perfil",
    valor: string,
  ) {
    setUsuarios((prev) =>
      prev.map((u) => {
        if (u.id !== id) return u;
        const updated = { ...u };
        if (campo === "login") updated._novoUsuario = valor;
        if (campo === "senha") updated._novaSenha = valor;
        if (campo === "perfil") updated._novoPerfil = valor;
        updated._dirty = true; // RN74
        return updated;
      }),
    );
  }

  function handleCancelarUsuario(id: number) {
    setUsuarios((prev) =>
      prev.map((u) =>
        u.id !== id
          ? u
          : {
              ...u,
              _novoUsuario: u.usuario,
              _novaSenha: "",
              _novoPerfil: u.perfil,
              _dirty: false,
              _erro: null,
            },
      ),
    );
  }

  async function handleSalvarUsuario(id: number) {
    const u = usuarios.find((x) => x.id === id);
    if (!u) return;

    setUsuarios((prev) =>
      prev.map((x) => (x.id === id ? { ...x, _salvando: true, _erro: null } : x)),
    );

    try {
      const payload: { usuario?: string; senha?: string; perfil?: string } = {};
      if (u._novoUsuario !== u.usuario) payload.usuario = u._novoUsuario;
      if (u._novaSenha) payload.senha = u._novaSenha;
      if (isAdmin && u._novoPerfil !== u.perfil) payload.perfil = u._novoPerfil;

      const atualizado = await atualizarUsuario(id, payload);

      setUsuarios((prev) =>
        prev.map((x) =>
          x.id !== id
            ? x
            : {
                ...x,
                usuario: atualizado.usuario,
                perfil: atualizado.perfil,
                _novoUsuario: atualizado.usuario,
                _novaSenha: "",
                _novoPerfil: atualizado.perfil,
                _dirty: false,
                _salvando: false,
                _erro: null,
              },
        ),
      );
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : "Erro ao salvar";
      setUsuarios((prev) =>
        prev.map((x) =>
          x.id === id ? { ...x, _salvando: false, _erro: msg } : x,
        ),
      );
    }
  }

  // -------------------------------------------------------------------------
  // Adição de usuário (RN70, RN73)
  // -------------------------------------------------------------------------

  async function handleAdicionarUsuario(e: React.FormEvent) {
    e.preventDefault();
    if (!novoLogin.trim() || !novaSenha.trim() || !novoPerfil.trim()) {
      setErroAdd("Preencha todos os campos.");
      return;
    }
    setAdicionando(true);
    setErroAdd(null);
    try {
      const criado = await criarUsuario({
        usuario: novoLogin.trim(),
        senha: novaSenha,
        perfil: novoPerfil,
      });
      setUsuarios((prev) => [
        ...prev,
        {
          ...criado,
          _novoUsuario: criado.usuario,
          _novaSenha: "",
          _novoPerfil: criado.perfil,
          _dirty: false,
          _salvando: false,
          _erro: null,
        },
      ]);
      setNovoLogin("");
      setNovaSenha("");
      setNovoPerfil("");
      setMostraFormAdd(false);
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : "Erro ao criar usuário";
      setErroAdd(msg);
    } finally {
      setAdicionando(false);
    }
  }

  // -------------------------------------------------------------------------
  // Edição de chaves (RN72, RN75, RN76)
  // -------------------------------------------------------------------------

  function handleChaveChange(id: number, valor: string) {
    setChaves((prev) =>
      prev.map((c) =>
        c.id !== id ? c : { ...c, _novaChave: valor, _dirty: true },
      ),
    );
  }

  function handleCancelarChave(id: number) {
    setChaves((prev) =>
      prev.map((c) =>
        c.id !== id
          ? c
          : { ...c, _novaChave: c.chave, _dirty: false, _erro: null },
      ),
    );
  }

  async function handleSalvarChave(id: number) {
    const c = chaves.find((x) => x.id === id);
    if (!c) return;

    setChaves((prev) =>
      prev.map((x) => (x.id === id ? { ...x, _salvando: true, _erro: null } : x)),
    );

    try {
      const atualizada = await atualizarChave(id, { chave: c._novaChave });
      setChaves((prev) =>
        prev.map((x) =>
          x.id !== id
            ? x
            : {
                ...x,
                chave: atualizada.chave,
                _novaChave: atualizada.chave,
                _dirty: false,
                _salvando: false,
                _erro: null,
              },
        ),
      );
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : "Erro ao salvar chave";
      setChaves((prev) =>
        prev.map((x) =>
          x.id === id ? { ...x, _salvando: false, _erro: msg } : x,
        ),
      );
    }
  }

  // -------------------------------------------------------------------------
  // Render
  // -------------------------------------------------------------------------

  return (
    <>
      {/* RN68 — modal de senha (D9) */}
      {!senhaOk && (
        <ModalSenha
          onConfirmar={() => setSenhaOk(true)}
          onCancelar={() => navigate("/principal")}
        />
      )}

      <div className={styles.container}>
        <div className={styles.card}>
          <h1 className={styles.titulo}>GESTÃO DE USUÁRIOS</h1>

          {loading && (
            <p className={styles.loading} aria-live="polite">
              Carregando...
            </p>
          )}

          {erro && (
            <p className={styles.erro} role="alert">
              {erro}
            </p>
          )}

          {!loading && !erro && senhaOk && (
            <>
              {/* ---- Seção Usuários (RN69–RN74) ---- */}
              <section className={styles.secao}>
                <h2 className={styles.secaoTitulo}>Usuários</h2>
                <table className={styles.tabela}>
                  <thead>
                    <tr>
                      <th>Login</th>
                      <th>Nova Senha</th>
                      <th>Perfil</th>
                      {/* RN74 — cabeçalho de ações */}
                      <th>Ações</th>
                    </tr>
                  </thead>
                  <tbody>
                    {usuarios.map((u) => (
                      <tr key={u.id}>
                        {/* Campo Login */}
                        <td>
                          <input
                            type="text"
                            value={u._novoUsuario}
                            onChange={(e) =>
                              handleUsuarioChange(u.id, "login", e.target.value)
                            }
                            className={styles.inputInline}
                            aria-label={`Login de ${u.usuario}`}
                          />
                        </td>
                        {/* Campo Senha — mascarado, opcional */}
                        <td>
                          <input
                            type="password"
                            value={u._novaSenha}
                            onChange={(e) =>
                              handleUsuarioChange(u.id, "senha", e.target.value)
                            }
                            placeholder="Nova senha..."
                            className={styles.inputSenha}
                            autoComplete="new-password"
                            aria-label={`Nova senha de ${u.usuario}`}
                          />
                        </td>
                        {/* Campo Perfil — RN71: somente Admin edita */}
                        <td>
                          <select
                            value={u._novoPerfil}
                            onChange={(e) =>
                              handleUsuarioChange(u.id, "perfil", e.target.value)
                            }
                            disabled={!isAdmin}
                            className={styles.selectInline}
                            aria-label={`Perfil de ${u.usuario}`}
                          >
                            {perfisDisponiveis.map((p) => (
                              <option key={p.perfil} value={p.perfil}>
                                {p.perfil}
                              </option>
                            ))}
                            {/* Fallback se perfil atual não estiver na lista */}
                            {!perfisDisponiveis.find(
                              (p) => p.perfil === u._novoPerfil,
                            ) && (
                              <option value={u._novoPerfil}>
                                {u._novoPerfil}
                              </option>
                            )}
                          </select>
                        </td>
                        {/* Ações — RN74: visíveis somente se dirty */}
                        <td>
                          {u._dirty ? (
                            <div className={styles.acoes}>
                              <button
                                type="button"
                                className={styles.btnSalvar}
                                onClick={() => handleSalvarUsuario(u.id)}
                                disabled={u._salvando}
                              >
                                {u._salvando ? "Salvando..." : "Salvar"}
                              </button>
                              <button
                                type="button"
                                className={styles.btnCancelar}
                                onClick={() => handleCancelarUsuario(u.id)}
                                disabled={u._salvando}
                              >
                                Cancelar
                              </button>
                            </div>
                          ) : null}
                          {u._erro && (
                            <p className={styles.erroInline} role="alert">
                              {u._erro}
                            </p>
                          )}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>

                {/* RN70 — botão Adicionar somente para Admin */}
                {isAdmin && (
                  <>
                    {!mostraFormAdd ? (
                      <button
                        type="button"
                        className={styles.btnAdicionar}
                        onClick={() => setMostraFormAdd(true)}
                      >
                        <IconePlus />
                        Adicionar usuário
                      </button>
                    ) : (
                      <form
                        className={styles.formAdicionar}
                        onSubmit={handleAdicionarUsuario}
                      >
                        <label className={styles.formLabel}>
                          Login
                          <input
                            type="text"
                            value={novoLogin}
                            onChange={(e) => setNovoLogin(e.target.value)}
                            className={styles.inputInline}
                            placeholder="Login"
                            disabled={adicionando}
                          />
                        </label>
                        <label className={styles.formLabel}>
                          Senha
                          <input
                            type="password"
                            value={novaSenha}
                            onChange={(e) => setNovaSenha(e.target.value)}
                            className={styles.inputSenha}
                            placeholder="Senha"
                            autoComplete="new-password"
                            disabled={adicionando}
                          />
                        </label>
                        <label className={styles.formLabel}>
                          Perfil
                          <select
                            value={novoPerfil}
                            onChange={(e) => setNovoPerfil(e.target.value)}
                            className={styles.selectInline}
                            disabled={adicionando}
                          >
                            <option value="">Selecione...</option>
                            {perfisDisponiveis.map((p) => (
                              <option key={p.perfil} value={p.perfil}>
                                {p.perfil}
                              </option>
                            ))}
                          </select>
                        </label>
                        <div className={styles.acoes}>
                          <button
                            type="submit"
                            className={styles.btnSalvar}
                            disabled={adicionando}
                          >
                            {adicionando ? "Salvando..." : "Salvar"}
                          </button>
                          <button
                            type="button"
                            className={styles.btnCancelar}
                            onClick={() => {
                              setMostraFormAdd(false);
                              setErroAdd(null);
                            }}
                            disabled={adicionando}
                          >
                            Cancelar
                          </button>
                        </div>
                        {erroAdd && (
                          <p
                            className={styles.erroInline}
                            role="alert"
                            style={{ gridColumn: "1 / -1" }}
                          >
                            {erroAdd}
                          </p>
                        )}
                      </form>
                    )}
                  </>
                )}
              </section>

              {/* ---- Seção Chaves — RN72: apenas Admin ---- */}
              {isAdmin && (
                <section className={styles.secao}>
                  <h2 className={styles.secaoTitulo}>
                    Senhas de Operações Críticas
                  </h2>
                  <table className={styles.tabela}>
                    <thead>
                      <tr>
                        {/* RN75 — Ref somente leitura */}
                        <th>Operação</th>
                        <th>Senha</th>
                        {/* RN74 */}
                        <th>Ações</th>
                      </tr>
                    </thead>
                    <tbody>
                      {chaves.map((c) => (
                        <tr key={c.id}>
                          {/* RN75 — Ref readonly */}
                          <td className={styles.cellReadonly}>{c.ref}</td>
                          <td>
                            <input
                              type="text"
                              value={c._novaChave}
                              onChange={(e) =>
                                handleChaveChange(c.id, e.target.value)
                              }
                              className={styles.inputInline}
                              aria-label={`Chave para ${c.ref}`}
                            />
                          </td>
                          {/* RN74 */}
                          <td>
                            {c._dirty ? (
                              <div className={styles.acoes}>
                                <button
                                  type="button"
                                  className={styles.btnSalvar}
                                  onClick={() => handleSalvarChave(c.id)}
                                  disabled={c._salvando}
                                >
                                  {c._salvando ? "Salvando..." : "Salvar"}
                                </button>
                                <button
                                  type="button"
                                  className={styles.btnCancelar}
                                  onClick={() => handleCancelarChave(c.id)}
                                  disabled={c._salvando}
                                >
                                  Cancelar
                                </button>
                              </div>
                            ) : null}
                            {c._erro && (
                              <p className={styles.erroInline} role="alert">
                                {c._erro}
                              </p>
                            )}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                  {/* RN76 — sem botão de adição ou exclusão na seção Chaves */}
                </section>
              )}

              {/* Botão Voltar — B17: desabilitado com edições pendentes */}
              <button
                type="button"
                className={styles.btnVoltar}
                onClick={() => navigate("/principal")}
                disabled={temEdicaoPendente}
                title={
                  temEdicaoPendente
                    ? "Salve ou cancele as edições antes de voltar"
                    : undefined
                }
              >
                Voltar
              </button>
            </>
          )}
        </div>
      </div>
    </>
  );
}
