/**
 * store/authStore.ts
 * Estado global de autenticação — Zustand.
 *
 * Substitui as variáveis globais varUsu, varPerfil do sistema desktop.
 * Hidrata de localStorage na inicialização para persistir entre reloads.
 *
 * Uso:
 *   const usuario = useAuthStore((s) => s.usuario);
 *   const clearAuth = useAuthStore((s) => s.clearAuth);
 */

import { create } from "zustand";

interface AuthState {
  usuario: string | null;
  perfil: string | null;
  token: string | null;
  isAuthenticated: boolean;
  setAuth: (data: {
    usuario: string;
    perfil: string;
    token: string;
  }) => void;
  clearAuth: () => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  usuario: localStorage.getItem("usuario"),
  perfil: localStorage.getItem("perfil"),
  token: localStorage.getItem("access_token"),
  isAuthenticated: !!localStorage.getItem("access_token"),

  setAuth: ({ usuario, perfil, token }) => {
    localStorage.setItem("usuario", usuario);
    localStorage.setItem("perfil", perfil);
    localStorage.setItem("access_token", token);
    set({ usuario, perfil, token, isAuthenticated: true });
  },

  clearAuth: () => {
    localStorage.removeItem("usuario");
    localStorage.removeItem("perfil");
    localStorage.removeItem("access_token");
    set({
      usuario: null,
      perfil: null,
      token: null,
      isAuthenticated: false,
    });
  },
}));
