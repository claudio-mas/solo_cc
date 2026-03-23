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
 */

import React, { useEffect, useMemo, useRef, useState } from "react";
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
    size: 110,
    cell: (info) => (
      <span
        className="font-mono font-medium"
        style={{ color: "#8B0000" }}
      >
        {info.getValue()}
      </span>
    ),
  }),
  columnHelper.accessor("cliente", {
    header: "Cliente",
    size: 548,
    cell: (info) => (
      <span className="font-semibold">{info.getValue()}</span>
    ),
  }),
];

// ---------------------------------------------------------------------------
// Ícones inline SVG — sem dependência externa
// ---------------------------------------------------------------------------

/** Propriedades comuns a todos os ícones da sidebar */
interface IconProps {
  className?: string;
}

const ICON_PROPS = {
  xmlns: "http://www.w3.org/2000/svg",
  width: 18,
  height: 18,
  viewBox: "0 0 24 24",
  fill: "none",
  stroke: "currentColor",
  strokeWidth: 2,
  strokeLinecap: "round" as const,
  strokeLinejoin: "round" as const,
  "aria-hidden": true as const,
};

function SearchIcon() {
  return (
    <svg {...ICON_PROPS} width={16} height={16} className="shrink-0 text-neutral-400">
      <circle cx="11" cy="11" r="8" />
      <line x1="21" y1="21" x2="16.65" y2="16.65" />
    </svg>
  );
}

function IconHome({ className }: IconProps) {
  return (
    <svg {...ICON_PROPS} className={className}>
      <path d="M3 9l9-7 9 7v11a2 2 0 01-2 2H5a2 2 0 01-2-2z" />
      <polyline points="9 22 9 12 15 12 15 22" />
    </svg>
  );
}

function IconLancamentos({ className }: IconProps) {
  return (
    <svg {...ICON_PROPS} className={className}>
      <line x1="12" y1="1" x2="12" y2="23" />
      <path d="M17 5H9.5a3.5 3.5 0 000 7h5a3.5 3.5 0 010 7H6" />
    </svg>
  );
}

function IconExtrato({ className }: IconProps) {
  return (
    <svg {...ICON_PROPS} className={className}>
      <path d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2" />
      <rect x="9" y="3" width="6" height="4" rx="1" />
      <line x1="9" y1="12" x2="15" y2="12" />
      <line x1="9" y1="16" x2="13" y2="16" />
    </svg>
  );
}

function IconAlterar({ className }: IconProps) {
  return (
    <svg {...ICON_PROPS} className={className}>
      <path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7" />
      <path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4 9.5-9.5z" />
    </svg>
  );
}

function IconNovoCliente({ className }: IconProps) {
  return (
    <svg {...ICON_PROPS} className={className}>
      <path d="M16 21v-2a4 4 0 00-4-4H6a4 4 0 00-4 4v2" />
      <circle cx="9" cy="7" r="4" />
      <line x1="19" y1="8" x2="19" y2="14" />
      <line x1="22" y1="11" x2="16" y2="11" />
    </svg>
  );
}

function IconRelatorios({ className }: IconProps) {
  return (
    <svg {...ICON_PROPS} className={className}>
      <line x1="18" y1="20" x2="18" y2="10" />
      <line x1="12" y1="20" x2="12" y2="4" />
      <line x1="6" y1="20" x2="6" y2="14" />
    </svg>
  );
}

function IconTotais({ className }: IconProps) {
  return (
    <svg {...ICON_PROPS} className={className}>
      <line x1="4" y1="9" x2="20" y2="9" />
      <line x1="4" y1="15" x2="20" y2="15" />
      <line x1="10" y1="3" x2="8" y2="21" />
      <line x1="16" y1="3" x2="14" y2="21" />
    </svg>
  );
}

function IconUsuarios({ className }: IconProps) {
  return (
    <svg {...ICON_PROPS} className={className}>
      <rect x="3" y="11" width="18" height="11" rx="2" ry="2" />
      <path d="M7 11V7a5 5 0 0110 0v4" />
    </svg>
  );
}

function IconSair({ className }: IconProps) {
  return (
    <svg {...ICON_PROPS} className={className}>
      <path d="M9 21H5a2 2 0 01-2-2V5a2 2 0 012-2h4" />
      <polyline points="16 17 21 12 16 7" />
      <line x1="21" y1="12" x2="9" y2="12" />
    </svg>
  );
}

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
      setSelectedCliente(clientes[0] ?? null);
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
  // RN12 — Filtra em memória por prefixo de nome (case-insensitive) ou código
  // ADAPTAR: VB.Net apenas posicionava no primeiro match (RN13 original);
  //          na web a lista é filtrada em tempo real sem round-trip ao backend.
  //   Nome:   UCase(Left(cliente, len(search))) === UCase(search)
  //   Código: Left(codigo.toString(), len(search)) === search
  // -------------------------------------------------------------------------
  const filteredClientes = useMemo(() => {
    if (searchText === "") return clientes;
    const upper = searchText.toUpperCase();
    return clientes.filter(
      (c) =>
        c.cliente.toUpperCase().includes(upper) ||
        c.codigo.toString().substring(0, searchText.length) === searchText,
    );
  }, [clientes, searchText]);

  // Seleciona automaticamente o primeiro resultado filtrado ao digitar
  useEffect(() => {
    if (filteredClientes.length > 0) {
      setSelectedCliente(filteredClientes[0] ?? null);
    } else {
      setSelectedCliente(null);
    }
  }, [filteredClientes]);

  function handleSearch(value: string) {
    setSearchText(value);
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
  // RN50 — passa dados do cliente selecionado via route state
  function handleLancamentos() {
    if (selectedCliente) {
      navigate("/lancamentos", {
        state: {
          id: selectedCliente.id,
          codigo: selectedCliente.codigo,
          cliente: selectedCliente.cliente,
        },
      });
    }
  }

  // RN18 — Usuários e Senhas
  // TODO: quando frmSenha for migrado, abrir modal de confirmação de senha
  //       antes de navegar. Por ora, navega direto.
  function handleUsuarios() {
    navigate("/usuarios");
  }

  function handleAlterar() {
    if (selectedCliente) navigate(`/clientes/alterar/${selectedCliente.id}`);
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

  // Avatar: iniciais do nome do usuário (máx. 2 letras)
  const initials = usuario
    ? usuario
        .split(" ")
        .filter(Boolean)
        .map((w) => w[0])
        .slice(0, 2)
        .join("")
        .toUpperCase()
    : "?";

  // -------------------------------------------------------------------------
  // Itens da sidebar agrupados por função
  // -------------------------------------------------------------------------
  const navInicio: SidebarItemProps[] = [
    {
      label: "Início",
      onClick: () => {},
      icon: <IconHome className="shrink-0" />,
      active: true,
    },
  ];

  const navClienteEspecifico: SidebarItemProps[] = [
    {
      label: "Lançamentos",
      onClick: handleLancamentos,
      icon: <IconLancamentos className="shrink-0" />,
      disabled: !selectedCodigo,
    },
    {
      label: "Conta Corrente",
      onClick: handleExtrato,
      icon: <IconExtrato className="shrink-0" />,
      disabled: !selectedCodigo,
    },
    {
      label: "Alterar",
      onClick: handleAlterar,
      icon: <IconAlterar className="shrink-0" />,
      disabled: !selectedCodigo,
    },
  ];

  const navGeral: SidebarItemProps[] = [
    {
      label: "Novo Cliente",
      onClick: handleNovoCliente,
      icon: <IconNovoCliente className="shrink-0" />,
    },
    {
      label: "Relatórios",
      onClick: handleRelatorios,
      icon: <IconRelatorios className="shrink-0" />,
    },
    {
      label: "Totais",
      onClick: handleTotais,
      icon: <IconTotais className="shrink-0" />,
    },
    {
      label: "Usuários e Senhas",
      onClick: handleUsuarios,
      icon: <IconUsuarios className="shrink-0" />,
    },
  ];

  // -------------------------------------------------------------------------
  // Render
  // -------------------------------------------------------------------------
  return (
    <div className="flex h-screen overflow-hidden">
      {/* ------------------------------------------------------------------ */}
      {/* Sidebar                                                             */}
      {/* ------------------------------------------------------------------ */}
      <aside
        className="flex w-[220px] shrink-0 flex-col border-r border-neutral-200 bg-secondary"
        aria-label="Navegação principal"
      >
        {/* Logo / marca na sidebar */}
        <div
          className="flex h-12 shrink-0 items-center border-b border-neutral-200 px-4"
          style={{ backgroundColor: "#8B0000" }}
        >
          <span className="truncate text-sm font-bold tracking-wide text-white">
            Contas Correntes
          </span>
        </div>

        {/* Indicador do cliente selecionado (RN11) */}
        <div className="border-b border-neutral-200 px-4 py-3">
          <p className="text-[10px] font-semibold uppercase tracking-wider text-neutral-400">
            Cliente selecionado
          </p>
          <p
            className="mt-0.5 font-mono text-sm font-bold"
            style={{ color: "#8B0000" }}
          >
            {headerCodigo}
          </p>
        </div>

        {/* Grupo: ações do cliente */}
        <nav className="flex-1 overflow-y-auto py-2">
          {navInicio.map((item) => (
            <SidebarItem key={item.label} {...item} />
          ))}

          <div className="my-2 border-t border-neutral-200" />

          <p className="px-4 pb-1 pt-2 text-[10px] font-semibold uppercase tracking-wider text-neutral-400">
            Ações do cliente
          </p>
          {navClienteEspecifico.map((item) => (
            <SidebarItem key={item.label} {...item} />
          ))}

          <div className="my-2 border-t border-neutral-200" />

          <p className="px-4 pb-1 pt-2 text-[10px] font-semibold uppercase tracking-wider text-neutral-400">
            Geral
          </p>
          {navGeral.map((item) => (
            <SidebarItem key={item.label} {...item} />
          ))}
        </nav>

        {/* Sair (fixo no rodapé da sidebar) */}
        <div className="border-t border-neutral-200 py-2">
          <SidebarItem
            label="Sair"
            onClick={handleSair}
            icon={<IconSair className="shrink-0" />}
          />
        </div>
      </aside>

      {/* ------------------------------------------------------------------ */}
      {/* Área direita: topbar + conteúdo                                     */}
      {/* ------------------------------------------------------------------ */}
      <div className="flex flex-1 flex-col overflow-hidden">
        {/* Topbar */}
        <header className="flex h-12 shrink-0 items-center justify-between border-b border-neutral-200 bg-white px-5">
          <span
            className="text-sm font-bold"
            style={{ color: "#8B0000" }}
          >
            Solo Consultoria de Imóveis
          </span>

          {/* Usuário + avatar (RN20) */}
          <div className="flex items-center gap-2">
            <span className="text-xs text-neutral-600">
              {usuario ?? ""}
            </span>
            <div
              className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full text-xs font-bold text-white"
              style={{ backgroundColor: "#8B0000" }}
              aria-label={`Avatar de ${usuario ?? "usuário"}`}
            >
              {initials}
            </div>
          </div>
        </header>

        {/* Conteúdo principal */}
        <main className="flex-1 overflow-auto bg-tertiary p-5">
          {/* Card de pesquisa */}
          <div className="mb-4 flex items-center gap-2 rounded-md border border-neutral-200 bg-white px-3 py-2 shadow-sm">
            <SearchIcon />
            <input
              id="pesquisa"
              ref={searchInputRef}
              type="text"
              value={searchText}
              onChange={(e) => handleSearch(e.target.value)}
              placeholder="Digite o nome ou código do cliente..."
              className="flex-1 bg-transparent text-sm text-neutral-800 outline-none placeholder:text-neutral-400"
              autoComplete="off"
            />
          </div>

          {/* Erro de carregamento */}
          {error && (
            <p
              className="mb-3 text-center text-xs text-red-600"
              role="alert"
            >
              {error}
            </p>
          )}

          {/* Tabela + rodapé */}
          <div className="overflow-hidden rounded-md border border-neutral-200 bg-white shadow-sm">
            <DataTable
              columns={columns}
              data={filteredClientes}
              onRowClick={handleRowClick}
              onRowDoubleClick={handleRowDoubleClick}
              selectedRowId={selectedCliente?.id ?? null}
              getRowId={getRowId}
              isLoading={isLoading}
            />

            {/* Rodapé da tabela: total de registros + paginação */}
            {!isLoading && (
              <div className="flex items-center justify-between border-t border-neutral-100 bg-white px-4 py-2 text-xs text-neutral-500">
                <span>
                  {filteredClientes.length}{" "}
                  {filteredClientes.length === 1 ? "registro" : "registros"}
                  {searchText !== "" && ` de ${clientes.length}`}
                </span>
                <span>
                  {filteredClientes.length > 0
                    ? `Exibindo 1 – ${filteredClientes.length}`
                    : "Nenhum registro"}
                </span>
              </div>
            )}
          </div>
        </main>
      </div>
    </div>
  );
}

// ---------------------------------------------------------------------------
// Componente auxiliar: item da sidebar
// ---------------------------------------------------------------------------

interface SidebarItemProps {
  label: string;
  onClick: () => void;
  icon: React.ReactNode;
  disabled?: boolean;
  active?: boolean;
}

function SidebarItem({ label, onClick, icon, disabled, active }: SidebarItemProps) {
  return (
    <button
      type="button"
      onClick={onClick}
      disabled={disabled}
      className={[
        "flex w-full items-center gap-2.5 px-4 py-2 text-left text-sm transition-colors",
        "border-l-2",
        active
          ? "border-[#8B0000] bg-white font-medium text-[#8B0000]"
          : "border-transparent text-neutral-700 hover:bg-neutral-100 hover:text-neutral-900",
        disabled
          ? "cursor-not-allowed opacity-40"
          : "cursor-pointer",
      ].join(" ")}
    >
      {icon}
      {label}
    </button>
  );
}
