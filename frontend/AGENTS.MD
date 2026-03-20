# AGENTS.md — Frontend · Solo Consultoria de Imóveis

Este arquivo complementa o `CLAUDE.md` da raiz do monorepo com regras
específicas do frontend React. Leia o `CLAUDE.md` primeiro — ele contém
o contexto de migração e as regras que se sobrepõem a este arquivo.

---

## Contexto de Migração (LEIA ANTES DE CODAR)

Este frontend **não é um projeto novo**. Cada tela é a migração de um form
VB.Net legado com regras de negócio documentadas. Antes de implementar
qualquer página:

1. Consulte a documentação funcional do form correspondente em `docs/`.
2. Verifique a tabela de rastreabilidade no `CLAUDE.md`.
3. Toda validação, comportamento de foco e mensagem de erro deve ser fiel
   ao form original — não inventar UX nova sem aprovação.
4. Regras de negócio `RNxx` devem ser referenciadas nos comentários do
   componente e do hook correspondente.

---

## Stack (OBRIGATÓRIA)

| Tecnologia | Uso |
|---|---|
| React + TypeScript | Framework principal |
| Vite | Build e dev server |
| Tailwind CSS | Estilização — exclusivo |
| shadcn/ui | Base do design system |
| Zustand | Estado global de cliente (auth, UI) |
| TanStack Query | Estado de servidor (fetch, cache, sync) |
| TanStack Table | Todas as tabelas do sistema |
| React Hook Form | Todos os formulários |
| Zod | Validação de schemas |
| React Router | Navegação entre telas |
| Vitest + RTL | Testes |

Não introduzir bibliotecas alternativas sem justificativa documentada e
aprovação. Abrir issue no repositório antes de propor substituições.

---

## Estrutura de Pastas

```
frontend/
└── src/
    ├── app/
    │   └── layout/           # Layout raiz, providers, router config
    │
    ├── components/
    │   ├── ui/               # Componentes base do design system (shadcn/ui)
    │   ├── table/            # Wrapper e extensões do TanStack Table
    │   ├── form/             # Campos de formulário reutilizáveis
    │   └── feedback/         # Toast, Alert, LoadingSpinner, ErrorBoundary
    │
    ├── features/             # Um subdiretório por domínio de negócio
    │   ├── auth/
    │   ├── clientes/
    │   ├── contas/
    │   └── …
    │
    ├── pages/                # Uma pasta por tela — nomeada pelo form de origem
    │   ├── Login/            # ← frmLogin
    │   │   ├── index.tsx
    │   │   └── Login.test.tsx
    │   ├── Principal/        # ← FrmPrincipal
    │   └── …
    │
    ├── services/             # Chamadas à API — um arquivo por domínio
    │   ├── auth.ts
    │   ├── clientes.ts
    │   └── …
    │
    ├── hooks/                # Custom hooks reutilizáveis
    ├── store/                # Stores Zustand
    ├── lib/                  # Utilitários, helpers, configuração do query client
    └── main.tsx
```

**Regras de organização:**
- `components/ui/` contém apenas componentes de design system — sem lógica
  de negócio.
- `features/` contém lógica de domínio (hooks específicos, utils, types de
  um domínio). Componentes de features ficam em `features/<domínio>/components/`.
- `pages/` orquestram features e components — sem lógica própria além de
  composição e roteamento.
- Nunca importar de `pages/` dentro de `features/` ou `components/`.

---

## TypeScript

Todos os arquivos usam `.tsx` (componentes) ou `.ts` (lógica pura).

### Tipagem obrigatória

```typescript
// Props de componente — sempre interface explícita
interface LoginProps {
  onSuccess?: () => void;
}

// Retorno de hook — sempre tipado
function useLogin(): {
  login: (data: LoginFormData) => Promise<void>;
  isLoading: boolean;
  error: string | null;
} { … }

// Response da API — interface alinhada ao schema Pydantic do backend
interface LoginResponse {
  access_token: string;
  token_type: string;
  usuario: string;
  perfil: string;
}

// Zod schema — inferir tipo a partir do schema, não duplicar
const loginSchema = z.object({
  usuario: z.string().min(1, "Por favor, informe o usuário"),
  senha: z.string().min(1, "Por favor, informe a senha"),
});
type LoginFormData = z.infer<typeof loginSchema>;
```

### Proibido

- `any` — usar `unknown` e fazer narrowing quando necessário
- Asserções de tipo (`as X`) sem comentário justificando
- Ignorar erros com `// @ts-ignore` — usar `// @ts-expect-error` com motivo

---

## Design System

### Tailwind CSS

- Usar **exclusivamente** classes Tailwind — sem estilos inline, sem CSS
  customizado exceto em `globals.css` para tokens globais.
- Sem valores arbitrários (`px-[13px]`, `text-[#abc]`) — definir tokens no
  `tailwind.config.ts` e usar as classes geradas.
- Cores: neutras dominam a UI; uma cor primária para ações; uma cor de danger
  para erros. Definir no config antes de iniciar as telas.

### shadcn/ui

- Componentes shadcn/ui são a base — não reescrever o que já existe.
- Customizações via `className` prop ou variantes — nunca editar os arquivos
  em `components/ui/` gerados pelo shadcn CLI diretamente.
- Para novos componentes compostos, usar os primitivos do Radix UI (já
  inclusos via shadcn) antes de buscar outra biblioteca.

### Padrão de componente

```typescript
// ✅ Correto — variantes, tipagem, sem estilos hardcoded
interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: "primary" | "secondary" | "danger";
  size?: "sm" | "md" | "lg";
  isLoading?: boolean;
}

export function Button({
  variant = "primary",
  size = "md",
  isLoading,
  children,
  ...props
}: ButtonProps) { … }
```

---

## Formulários e Validação

Todos os formulários usam React Hook Form + Zod. Sem exceções.

```typescript
const form = useForm<LoginFormData>({
  resolver: zodResolver(loginSchema),
  defaultValues: { usuario: "", senha: "" },
});
```

### Fidelidade ao form legado (CRÍTICO para migração)

- As mensagens de erro devem ser **idênticas** às do form original quando
  documentadas (ex: "Por favor, informe o usuário").
- O campo que falhou na validação deve receber foco automaticamente —
  replicar o comportamento `Me.campo.Focus()` do VB.Net.
- Em caso de senha incorreta: limpar o campo senha e focar nele — `RN04`.
- Ordem de validação: respeitar a sequência do form original (ex: usuário
  antes de senha).

### Wrapper de campo obrigatório

```typescript
// Usar FormField do shadcn/ui para garantir consistência de erros
<FormField
  control={form.control}
  name="usuario"
  render={({ field, fieldState }) => (
    <FormItem>
      <FormLabel>Usuário</FormLabel>
      <FormControl>
        <Input {...field} data-error={!!fieldState.error} />
      </FormControl>
      <FormMessage /> {/* mensagem de erro do Zod */}
    </FormItem>
  )}
/>
```

---

## Chamadas à API

Fluxo obrigatório: `Componente → Hook → TanStack Query → Service → API`

```typescript
// services/auth.ts — única camada que conhece a URL da API
const API_URL = import.meta.env.VITE_API_URL;

export async function loginService(data: LoginFormData): Promise<LoginResponse> {
  const res = await fetch(`${API_URL}/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });
  if (!res.ok) {
    const err = await res.json();
    throw new Error(err.detail ?? "Erro desconhecido");
  }
  return res.json();
}

// hooks/useLogin.ts — orquestra query + side effects
export function useLogin() {
  return useMutation({
    mutationFn: loginService,
    onSuccess: (data) => {
      localStorage.setItem("access_token", data.access_token);
      localStorage.setItem("perfil", data.perfil);
      localStorage.setItem("usuario", data.usuario);
    },
  });
}
```

Componentes nunca chamam `fetch` diretamente — apenas consomem hooks.

---

## Estado Global (Zustand)

Zustand é exclusivo para estado de **cliente** — não cachear dados do servidor
aqui (isso é responsabilidade do TanStack Query).

### Store de autenticação (obrigatória)

```typescript
// store/authStore.ts
interface AuthState {
  usuario: string | null;
  perfil: string | null;
  token: string | null;
  isAuthenticated: boolean;
  setAuth: (data: { usuario: string; perfil: string; token: string }) => void;
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
    set({ usuario: null, perfil: null, token: null, isAuthenticated: false });
  },
}));
```

**O que pode entrar em stores Zustand:** auth, preferências de UI (sidebar
aberta/fechada, tema), filtros persistidos de tela.

**O que não entra:** dados de listas, resultados de queries, dados de
formulários em andamento.

---

## Tabelas

Todas as tabelas do sistema usam TanStack Table. Sem exceções.

Toda tabela deve suportar por padrão:
- Paginação (server-side para listas grandes)
- Ordenação por coluna
- Filtragem
- Ações por linha (editar, excluir) via menu ou botões inline

O componente base em `components/table/` deve encapsular a configuração
repetitiva do TanStack Table — páginas e features nunca configuram o
TanStack Table diretamente.

---

## UX e Feedback

- Preferir edição inline ou side drawers (`Sheet` do shadcn) a navegação
  para nova página — reduzir cliques.
- Toda operação assíncrona exibe estado de loading no elemento acionador
  (botão desabilitado + spinner), não apenas em loading global.
- Toasts para confirmações de ação (salvo, excluído, erro) via `sonner` ou
  o componente `Toaster` do shadcn.
- Erros de validação sempre exibidos abaixo do campo, nunca apenas como toast.
- Nunca deixar a UI em estado ambíguo após uma ação — sempre feedback de
  sucesso ou erro.

---

## Testes

Framework: Vitest + React Testing Library.

Todo componente de formulário deve ter:

```
✅ Renderização sem erros
✅ Submissão válida — chama o service com os dados corretos
✅ Exibição de erro de campo obrigatório
✅ Comportamento de foco após erro (fidelidade ao form legado)
✅ Estado de loading durante submissão
```

Para componentes com guards de perfil:

```
✅ Exibe corretamente para perfil autorizado
✅ Redireciona/oculta para perfil não autorizado
```

Testes ficam no mesmo diretório do componente: `Login/Login.test.tsx`.

---

## Performance

Regras concretas — não aplicar memoização por padrão:

- `React.memo` apenas em componentes que recebem props estáveis e renderizam
  com frequência dentro de listas (ex: linha de tabela).
- `useMemo` apenas para cálculos computacionalmente custosos — não para
  objetos simples.
- `useCallback` apenas para funções passadas como prop a componentes
  memoizados.
- TanStack Query gerencia cache de servidor — não duplicar dados em estado
  local React.
- Listas longas (> 100 itens visíveis) usar paginação server-side, nunca
  carregar tudo no cliente.

---

## Anti-Patterns (PROIBIDO)

- ❌ `fetch` direto em componente — usar `services/`
- ❌ Lógica de negócio em componente — extrair para hook ou `features/`
- ❌ Estilos inline em componentes finais — usar Tailwind
- ❌ Valores arbitrários Tailwind (`px-[13px]`) — definir tokens no config
- ❌ `any` no TypeScript
- ❌ Dados do servidor no Zustand — usar TanStack Query
- ❌ Tabela sem TanStack Table
- ❌ Formulário sem React Hook Form + Zod
- ❌ Componente não reutilizável quando o mesmo padrão aparece 2+ vezes
- ❌ Ignorar RNs do form legado sem registrar a decisão em comentário
- ❌ Mensagem de erro diferente do form original sem aprovação

---

## Qualidade de Código

- ESLint + Prettier configurados — o CI rejeita PRs com erros de lint.
- Componentes com responsabilidade única — se um componente passa de ~150
  linhas, avaliar extração.
- Preferir composição a props booleanas excessivas (`isLarge`, `isRed`,
  `isDisabled`...) — usar variantes tipadas.
- Comentários explicam **por que**, não **o que** — o código explica o que.
- RNs do form legado comentadas no código: `// RN04 — Limpa senha e foca após erro`