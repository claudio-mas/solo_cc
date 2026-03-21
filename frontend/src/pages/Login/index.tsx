/**
 * pages/Login/index.tsx — Equivalente web do frmLogin
 * Sistema: Solo Consultoria de Imóveis — Contas Correntes
 *
 * Regras implementadas:
 *   RN01 — Usuário obrigatório
 *   RN02 — Senha obrigatória
 *   RN03 — Validação de credenciais via API
 *   RN04 — Limpa senha e recoloca foco em caso de erro
 *   RN05 — Perfil armazenado via useAuthStore (JWT)
 *   RN06 — Nome exibido em maiúsculas (retornado pela API)
 *   RN07 — Lista de usuários carregada da API ao montar
 *   RN08 — Usuários ordenados alfabeticamente (garantido pela API)
 *
 * TODO: substituir classes Tailwind placeholder pelo design system
 *       definitivo quando os tokens forem definidos.
 */

import { useEffect, useRef } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useUsuarios, useLogin } from "@/hooks/useLogin";

// ---------------------------------------------------------------------------
// Schema Zod — mensagens idênticas ao form original (RN01, RN02)
// ---------------------------------------------------------------------------

const loginSchema = z.object({
  usuario: z.string().min(1, "Por favor, informe o usuário"),
  senha: z.string().min(1, "Por favor, informe a senha"),
});

type LoginFormData = z.infer<typeof loginSchema>;

// ---------------------------------------------------------------------------
// Componente
// ---------------------------------------------------------------------------

export default function Login() {
  const { usuarios, error: erroUsuarios } = useUsuarios(); // RN07, RN08
  const { login, isLoading, error: erroLogin, resetError } = useLogin();

  const senhaRef = useRef<HTMLInputElement>(null);

  const {
    register,
    handleSubmit,
    setValue,
    setFocus,
    formState: { errors },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
    defaultValues: { usuario: "", senha: "" },
  });

  // Registra o ref da senha manualmente para ter acesso ao foco programático
  const { ref: senhaRegisterRef, ...senhaRegisterRest } = register("senha");

  // -------------------------------------------------------------------------
  // RN04 — Ao receber erro de autenticação da API: limpa senha e foca nela
  // -------------------------------------------------------------------------
  useEffect(() => {
    if (erroLogin) {
      setValue("senha", "");
      senhaRef.current?.focus();
    }
  }, [erroLogin, setValue]);

  // -------------------------------------------------------------------------
  // Submit — RN01, RN02 validados pelo Zod; RN03 delegado ao hook
  // -------------------------------------------------------------------------
  async function onSubmit(data: LoginFormData) {
    resetError();
    await login(data);
  }

  // -------------------------------------------------------------------------
  // RN01, RN02 — ao falhar validação Zod, focar no primeiro campo com erro
  // -------------------------------------------------------------------------
  function onInvalid() {
    if (errors.usuario) {
      setFocus("usuario");
    } else if (errors.senha) {
      senhaRef.current?.focus();
    }
  }

  // -------------------------------------------------------------------------
  // Cancelar — equivalente a btnCancelar_Click (Application.Exit)
  // ⚠️ window.close() pode ser bloqueado por alguns browsers — discutir
  //    com o cliente o comportamento esperado antes do go-live.
  // -------------------------------------------------------------------------
  function handleCancelar() {
    window.close();
  }

  // -------------------------------------------------------------------------
  // Render
  // -------------------------------------------------------------------------
  return (
    // TODO: ajustar classes para o design system definitivo
    <div className="flex min-h-screen items-center justify-center bg-neutral-100">
      <div className="w-full max-w-md rounded-lg bg-white p-10 shadow-md">

        {/* Logo — equivalente ao PictureBox2 (96×96px) */}
        <div className="mb-3 flex justify-center">
          <img
            src="/logo.png"
            alt="Logo Solo Consultoria"
            className="h-24 w-24 object-contain"
            onError={(e) => {
              (e.currentTarget as HTMLImageElement).style.display = "none";
            }}
          />
        </div>

        {/* Título — equivalente ao Label2 (DarkRed, centralizado) */}
        <h2 className="mb-6 text-center font-sans text-lg font-normal text-red-800">
          Controle de Acesso
        </h2>

        <form onSubmit={handleSubmit(onSubmit, onInvalid)} noValidate>

          {/* Campo Usuário — equivalente ao cboUsuario (ComboBox + autocomplete) */}
          <div className="mb-4 flex items-center gap-2">
            <label
              htmlFor="usuario"
              className="w-20 shrink-0 text-right text-sm font-normal"
            >
              Usuário
            </label>
            <div className="flex flex-1 flex-col">
              <input
                id="usuario"
                list="lista-usuarios"
                autoComplete="off"
                disabled={isLoading}
                aria-invalid={!!errors.usuario}
                {...register("usuario")}
                // B15 — Enter avança foco para senha (equivalente ao SendKeys("{Tab}") do frmLogin_KeyDown)
                onKeyDown={(e) => {
                  if (e.key === "Enter") {
                    e.preventDefault();
                    senhaRef.current?.focus();
                  }
                }}
                className={[
                  "rounded border px-2 py-1.5 text-sm outline-none transition-colors",
                  errors.usuario
                    ? "border-red-500 ring-1 ring-red-200"
                    : "border-neutral-300 focus:border-primary",
                ].join(" ")}
              />
              {/* RN01 — Mensagem idêntica ao form original */}
              {errors.usuario && (
                <span className="mt-1 text-xs text-red-600">
                  {errors.usuario.message}
                </span>
              )}
            </div>
            {/* RN08 — Datalist com usuários ordenados alfabeticamente */}
            <datalist id="lista-usuarios">
              {usuarios.map((u) => (
                <option key={u.usuario} value={u.usuario} />
              ))}
            </datalist>
          </div>

          {/* Campo Senha — equivalente ao txtSenha (PasswordChar = *) */}
          <div className="mb-4 flex items-center gap-2">
            <label
              htmlFor="senha"
              className="w-20 shrink-0 text-right text-sm font-normal"
            >
              Senha
            </label>
            <div className="flex flex-1 flex-col">
              <input
                id="senha"
                type="password"
                disabled={isLoading}
                aria-invalid={!!errors.senha}
                {...senhaRegisterRest}
                ref={(el) => {
                  senhaRegisterRef(el);
                  (senhaRef as React.MutableRefObject<HTMLInputElement | null>).current = el;
                }}
                className={[
                  "rounded border px-2 py-1.5 text-sm outline-none transition-colors",
                  errors.senha
                    ? "border-red-500 ring-1 ring-red-200"
                    : "border-neutral-300 focus:border-primary",
                ].join(" ")}
              />
              {/* RN02 — Mensagem idêntica ao form original */}
              {errors.senha && (
                <span className="mt-1 text-xs text-red-600">
                  {errors.senha.message}
                </span>
              )}
            </div>
          </div>

          {/* Erros de API (RN03, RN04) e erros de carregamento */}
          {(erroLogin ?? erroUsuarios) && (
            <p className="mb-3 text-center text-xs text-red-600" role="alert">
              {erroLogin ?? erroUsuarios}
            </p>
          )}

          {/* Botões — equivalentes a btnOk e btnCancelar */}
          <div className="mt-2 flex justify-end gap-2">
            <button
              type="submit"
              disabled={isLoading}
              className="rounded bg-blue-700 px-6 py-1.5 text-sm text-white hover:bg-blue-800 disabled:opacity-60"
            >
              {isLoading ? "Aguarde..." : "Ok"}
            </button>
            <button
              type="button"
              disabled={isLoading}
              onClick={handleCancelar}
              className="rounded bg-neutral-200 px-4 py-1.5 text-sm text-neutral-700 hover:bg-neutral-300 disabled:opacity-60"
            >
              Cancelar
            </button>
          </div>

        </form>
      </div>
    </div>
  );
}
