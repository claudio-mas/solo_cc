/**
 * hooks/useLogin.ts
 * Orquestra a lógica de autenticação: queries, mutações e side effects.
 * Componentes consomem apenas este hook — sem acesso direto aos services.
 */

import { useQuery, useMutation } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { useAuthStore } from "@/store/authStore";
import {
  fetchUsuarios,
  loginService,
  type LoginRequest,
  type UsuarioListItem,
} from "@/services/auth";

// ---------------------------------------------------------------------------
// Hook: lista de usuários para o dropdown
// RN07 — Carregada ao montar; RN08 — ordenada pela API
// ---------------------------------------------------------------------------

export function useUsuarios(): {
  usuarios: UsuarioListItem[];
  isLoading: boolean;
  error: string | null;
} {
  const { data, isLoading, error } = useQuery({
    queryKey: ["auth", "usuarios"],
    queryFn: fetchUsuarios,
    staleTime: 5 * 60 * 1000, // lista de usuários raramente muda
  });

  return {
    usuarios: data ?? [],
    isLoading,
    error: error ? "Não foi possível carregar a lista de usuários." : null,
  };
}

// ---------------------------------------------------------------------------
// Hook: mutação de login
// ---------------------------------------------------------------------------

export function useLogin(): {
  login: (data: LoginRequest) => Promise<void>;
  isLoading: boolean;
  error: string | null;
  resetError: () => void;
} {
  const navigate = useNavigate();
  const setAuth = useAuthStore((s) => s.setAuth);

  const mutation = useMutation({
    mutationFn: loginService,
    onSuccess: (data) => {
      // RN05 — Perfil armazenado para uso nos demais módulos
      // RN06 — usuario já retorna em maiúsculas pela API
      setAuth({
        token: data.access_token,
        perfil: data.perfil,
        usuario: data.usuario,
      });
      navigate("/principal");
    },
  });

  return {
    login: async (data: LoginRequest) => {
      await mutation.mutateAsync(data);
    },
    isLoading: mutation.isPending,
    error: mutation.error ? mutation.error.message : null,
    resetError: mutation.reset,
  };
}
