/**
 * components/table/DataTable.tsx
 * Componente base de tabela com TanStack Table.
 *
 * AGENTS.MD: "pages e features nunca configuram TanStack Table diretamente"
 * Este componente encapsula a configuração repetitiva.
 *
 * Features:
 * - Alternating row colors (equivale a EnableAlternatingRowColor = True)
 * - Highlight na linha selecionada
 * - Suporte a onRowClick e onRowDoubleClick
 */

import {
  flexRender,
  getCoreRowModel,
  useReactTable,
  type ColumnDef,
} from "@tanstack/react-table";

interface DataTableProps<TData> {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  columns: ColumnDef<TData, any>[];
  data: TData[];
  onRowClick?: (row: TData) => void;
  onRowDoubleClick?: (row: TData) => void;
  selectedRowId?: string | number | null;
  getRowId?: (row: TData) => string;
  isLoading?: boolean;
}

export function DataTable<TData>({
  columns,
  data,
  onRowClick,
  onRowDoubleClick,
  selectedRowId,
  getRowId,
  isLoading,
}: DataTableProps<TData>) {
  const table = useReactTable({
    data,
    columns,
    getCoreRowModel: getCoreRowModel(),
    getRowId: getRowId
      ? (row) => getRowId(row)
      : undefined,
  });

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-12 text-sm text-neutral-500">
        Carregando...
      </div>
    );
  }

  return (
    <div className="overflow-auto">
      <table className="w-full border-collapse text-sm">
        <thead>
          {table.getHeaderGroups().map((headerGroup) => (
            <tr
              key={headerGroup.id}
              className="border-b border-neutral-800 text-white"
              style={{ backgroundColor: "#1a1a1a" }}
            >
              {headerGroup.headers.map((header) => (
                <th
                  key={header.id}
                  className="px-3 py-2 text-left font-semibold"
                  style={{
                    width: header.getSize() !== 150
                      ? header.getSize()
                      : undefined,
                  }}
                >
                  {header.isPlaceholder
                    ? null
                    : flexRender(
                        header.column.columnDef.header,
                        header.getContext(),
                      )}
                </th>
              ))}
            </tr>
          ))}
        </thead>
        <tbody>
          {table.getRowModel().rows.length === 0 ? (
            <tr>
              <td
                colSpan={columns.length}
                className="py-8 text-center text-neutral-500"
              >
                Nenhum registro encontrado.
              </td>
            </tr>
          ) : (
            table.getRowModel().rows.map((row, index) => {
              const isSelected = selectedRowId != null
                && row.id === String(selectedRowId);
              return (
                <tr
                  key={row.id}
                  onClick={() => onRowClick?.(row.original)}
                  onDoubleClick={() =>
                    onRowDoubleClick?.(row.original)
                  }
                  style={
                    isSelected
                      ? { backgroundColor: "#ffe8e8" }
                      : undefined
                  }
                  className={[
                    "cursor-pointer transition-colors",
                    "border-b border-neutral-100",
                    isSelected
                      ? "font-medium"
                      : index % 2 === 0
                        ? "bg-white"
                        : "bg-neutral-50",
                    "hover:bg-[#fff5f5]",
                  ].join(" ")}
                >
                  {row.getVisibleCells().map((cell) => (
                    <td key={cell.id} className="px-3 py-1.5">
                      {flexRender(
                        cell.column.columnDef.cell,
                        cell.getContext(),
                      )}
                    </td>
                  ))}
                </tr>
              );
            })
          )}
        </tbody>
      </table>
    </div>
  );
}
