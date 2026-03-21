/**
 * pages/Principal/index.tsx — Equivalente web do FrmPrincipal
 * Sistema: Solo Consultoria de Imóveis — Contas Correntes
 *
 * Regras implementadas:
 *   RN09 — Grid de clientes carregado do banco, ordenado por nome
 *   RN10 — Código do primeiro cliente exibido no header de ações
 *   RN11 — Seleção no grid atualiza header para "CÓD. {código}"
 *   RN12 — Pesquisa por prefixo do nome (case-insensitive) ou código
 *   RN13 — Pesquisa posiciona no primeiro match (não filtra)
 *   RN14 — Foco automático no campo de pesquisa após carregamento
 *   RN15 — Double-click no grid abre extrato do cliente
 *   RN16 — Botão "Lançamentos" abre form de lançamento
 *   RN17 — Botão "Conta corrente" abre extrato
 *   RN18 — Botão "Usuários e Senhas" requer autenticação
 *   RN19 — Botão "Sair" encerra sessão e redireciona
 *   RN20 — Nome do usuário em maiúsculas na barra inferior
 *
 * TODO: substituir classes Tailwind placeholder pelo design system
 *       definitivo quando os tokens forem definidos.
 */

import { useEffect, useMemo, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { createColumnHelper } from "@tanstack/react-table";

import { useClientes } from "@/hooks/useClientes";
import { useAuthStore } from "@/store/authStore";
import { DataTable } from "@/components/table/DataTable";
import type { ClienteListItem } from "@/services/clientes";

// ---------------------------------------------------------------------------
// Colunas da tabela (TanStack Table)
// ---------------------------------------------------------------------------

const columnHelper = createColumnHelper<ClienteListItem>();

const columns = [
  columnHelper.accessor("codigo", {
    header: "Código",
    size: 114,
    cell: (info) => (
      <span className="block text-center">{info.getValue()}</span>
    ),
  }),
  columnHelper.accessor("cliente", {
    header: "Cliente",
    size: 548,
  }),
];

// ---------------------------------------------------------------------------
// Componente
// ---------------------------------------------------------------------------

export default function Principal() {
  const navigate = useNavigate();
  const { clientes, isLoading, error } = useClientes(); // RN09
  const usuario = useAuthStore((s) => s.usuario); // RN20
  const clearAuth = useAuthStore((s) => s.clearAuth); // RN19

  const [selectedCliente, setSelectedCliente] =
    useState<ClienteListItem | null>(null);
  const [searchText, setSearchText] = useState("");
  const searchInputRef = useRef<HTMLInputElement>(null);

  // -------------------------------------------------------------------------
  // RN10 — Seleciona o primeiro cliente ao carregar a lista
  // -------------------------------------------------------------------------
  useEffect(() => {
    if (clientes.length > 0 && selectedCliente === null) {
      setSelectedCliente(clientes[0]);
    }
  }, [clientes, selectedCliente]);

  // -------------------------------------------------------------------------
  // RN14 — Foco automático no campo de pesquisa após carregamento
  // (B06 ADAPTAR — useEffect substitui Timer1 500ms do VB.Net)
  // -------------------------------------------------------------------------
  useEffect(() => {
    if (!isLoading) {
      searchInputRef.current?.focus();
    }
  }, [isLoading]);

  // -------------------------------------------------------------------------
  // RN12/RN13 — Pesquisa por prefixo (nome case-insensitive, código case-sensitive)
  // Replicar lógica EXATA do VB.Net (linha 146 do frmPrincipal.vb):
  //   Nome: UCase(Left(cliente, len(search))) === UCase(search)
  //   Código: Left(codigo.toString(), len(search)) === search
  // -------------------------------------------------------------------------
  function handleSearch(value: string) {
    setSearchText(value);
    if (value === "") return;

    const match = clientes.find(
      (c) =>
        c.cliente
          .substring(0, value.length)
          .toUpperCase() === value.toUpperCase() ||
        c.codigo
          .toString()
          .substring(0, value.length) === value,
    );

    if (match) {
      setSelectedCliente(match);
    }
  }

  // -------------------------------------------------------------------------
  // Código do cliente selecionado para o header dinâmico (RN11)
  // -------------------------------------------------------------------------
  const headerCodigo = selectedCliente
    ? `CÓD. ${selectedCliente.codigo}`
    : "CÓD.";

  // -------------------------------------------------------------------------
  // RN11 — Click no grid atualiza cliente selecionado
  // -------------------------------------------------------------------------
  function handleRowClick(row: ClienteListItem) {
    setSelectedCliente(row);
  }

  // -------------------------------------------------------------------------
  // RN15 — Double-click abre extrato do cliente selecionado
  // -------------------------------------------------------------------------
  function handleRowDoubleClick(row: ClienteListItem) {
    navigate(`/extrato/${row.codigo}`);
  }

  // -------------------------------------------------------------------------
  // RN19 — Sair: limpa auth e redireciona para login
  // (B16 ADAPTAR — Application.Exit() → clearAuth + redirect)
  // -------------------------------------------------------------------------
  function handleSair() {
    clearAuth();
    navigate("/login");
  }

  // -------------------------------------------------------------------------
  // Botões de navegação
  // -------------------------------------------------------------------------
  const selectedCodigo = selectedCliente?.codigo;

  // RN17 — Conta corrente
  function handleExtrato() {
    if (selectedCodigo) navigate(`/extrato/${selectedCodigo}`);
  }

  // RN16 — Lançamentos
  // TODO: B08 pendente — investigar RadGridView1.Tag quando demais forms
  //       legados estiverem disponíveis. Por ora, usa caminho padrão.
  function handleLancamentos() {
    navigate("/lancamentos");
  }

  // RN18 — Usuários e Senhas
  // TODO: quando frmSenha for migrado, abrir modal de confirmação de senha
  //       antes de navegar. Por ora, navega direto.
  function handleUsuarios() {
    navigate("/usuarios");
  }

  function handleAlterar() {
    if (selectedCodigo) navigate(`/clientes/${selectedCodigo}/editar`);
  }

  function handleNovoCliente() {
    navigate("/clientes/novo");
  }

  function handleTotais() {
    navigate("/totais");
  }

  function handleRelatorios() {
    navigate("/relatorios");
  }

  // Memoize getRowId para estabilidade referencial
  const getRowId = useMemo(
    () => (row: ClienteListItem) => String(row.id),
    [],
  );

  // -------------------------------------------------------------------------
  // Render
  // -------------------------------------------------------------------------
  return (
    <div className="flex min-h-screen flex-col bg-white">
      {/* Banner/logo — equivalente a C1PictureBox1 (B24 ADAPTAR) */}
      <div className="flex items-center justify-center border-b border-neutral-200 bg-white py-3">
        <img
          src="/logo-banner.png"
          alt="Solo Consultoria de Imóveis"
          className="h-9 object-contain"
          onError={(e) => {
            (e.currentTarget as HTMLImageElement).style.display =
              "none";
          }}
        />
      </div>

      {/* Toolbar — equivalente a C1Ribbon1 (B22 ADAPTAR) */}
      <div className="flex flex-wrap items-center gap-1 border-b border-neutral-300 bg-neutral-100 px-4 py-2">
        {/* Grupo 1: ações do cliente selecionado */}
        <span className="mr-2 text-xs font-bold text-neutral-600">
          {headerCodigo}
        </span>
        <ToolbarButton
          label="Lançamentos"
          onClick={handleLancamentos}
          disabled={!selectedCodigo}
        />
        <ToolbarButton
          label="Conta corrente"
          onClick={handleExtrato}
          disabled={!selectedCodigo}
        />
        <div className="mx-1 h-6 w-px bg-neutral-300" />
        <ToolbarButton
          label="Alterar"
          onClick={handleAlterar}
          disabled={!selectedCodigo}
        />

        {/* Separador entre grupos */}
        <div className="mx-2 h-6 w-px bg-neutral-300" />

        {/* Grupo 2: ações administrativas */}
        <ToolbarButton
          label="Novo Cliente"
          onClick={handleNovoCliente}
        />
        <ToolbarButton
          label="Relatórios"
          onClick={handleRelatorios}
        />
        <ToolbarButton label="Totais" onClick={handleTotais} />
        <ToolbarButton
          label="Usuários e Senhas"
          onClick={handleUsuarios}
        />

        {/* Grupo 3: sair */}
        <div className="ml-auto">
          <ToolbarButton label="Sair" onClick={handleSair} />
        </div>
      </div>

      {/* Área de pesquisa — equivalente a Label1 + txtPesq */}
      <div className="flex items-center gap-2 px-4 py-3">
        <label
          htmlFor="pesquisa"
          className="text-sm font-bold text-red-800"
        >
          Localizar cliente:
        </label>
        <input
          id="pesquisa"
          ref={searchInputRef}
          type="text"
          value={searchText}
          onChange={(e) => handleSearch(e.target.value)}
          className="rounded border border-neutral-300 px-2 py-1.5 text-sm outline-none transition-colors focus:border-blue-500"
          autoComplete="off"
        />
      </div>

      {/* Erro de carregamento */}
      {error && (
        <p
          className="mx-4 mb-2 text-center text-xs text-red-600"
          role="alert"
        >
          {error}
        </p>
      )}

      {/* Grid de clientes — equivalente a RadGridView1 (B23 ADAPTAR) */}
      <div className="flex-1 px-4 pb-2">
        <DataTable
          columns={columns}
          data={clientes}
          onRowClick={handleRowClick}
          onRowDoubleClick={handleRowDoubleClick}
          selectedRowId={selectedCliente?.id ?? null}
          getRowId={getRowId}
          isLoading={isLoading}
        />
      </div>

      {/* Barra inferior — equivalente a lblUsuario (RN20) */}
      <div className="border-t border-neutral-300 bg-neutral-200 px-4 py-1">
        <span className="text-xs text-neutral-700">
          USUÁRIO: {usuario ?? ""}
        </span>
      </div>
    </div>
  );
}

// ---------------------------------------------------------------------------
// Componente auxiliar: botão da toolbar
// ---------------------------------------------------------------------------

interface ToolbarButtonProps {
  label: string;
  onClick: () => void;
  disabled?: boolean;
}

function ToolbarButton({
  label,
  onClick,
  disabled,
}: ToolbarButtonProps) {
  return (
    <button
      type="button"
      onClick={onClick}
      disabled={disabled}
      className="rounded px-3 py-1.5 text-xs font-medium text-neutral-700 transition-colors hover:bg-neutral-200 disabled:cursor-not-allowed disabled:opacity-50"
    >
      {label}
    </button>
  );
}
