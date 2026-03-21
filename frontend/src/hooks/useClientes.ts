/**
 * hooks/useClientes.ts
 * Orquestra a busca da lista de clientes para a tela principal.
 * Componentes consomem apenas este hook — sem acesso direto aos services.
 *
 * RN09 — Lista carregada do banco ao montar, ordenada por nome (API).
 */

import { useQuery } from "@tanstack/react-query";
import {
  fetchClientes,
  type ClienteListItem,
} from "@/services/clientes";

export function useClientes(): {
  clientes: ClienteListItem[];
  isLoading: boolean;
  error: string | null;
} {
  const { data, isLoading, error } = useQuery({
    queryKey: ["clientes"],
    queryFn: fetchClientes,
    staleTime: 2 * 60 * 1000, // lista de clientes muda com pouca frequência
  });

  return {
    clientes: data ?? [],
    isLoading,
    error: error
      ? "Não foi possível carregar a lista de clientes."
      : null,
  };
}
