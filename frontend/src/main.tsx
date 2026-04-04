import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";

import "./index.css";

import Login from "@/pages/Login";
import Principal from "@/pages/Principal";
import ClienteNovo from "@/pages/ClienteNovo";
import Alterar from "@/pages/Alterar";
import Extrato from "@/pages/Extrato";
import ExtratoImprimir from "@/pages/Extrato/Imprimir";
import Lanca from "@/pages/Lanca";
import Totais from "@/pages/Totais";
import Usuarios from "@/pages/Usuarios";
import Relatorios from "@/pages/Relatorios";
import Devedores from "@/pages/Relatorios/Devedores";
import Credores from "@/pages/Relatorios/Credores";
import { useAuthStore } from "@/store/authStore";

const queryClient = new QueryClient();

function PrivateRoute({ children }: { children: React.ReactNode }) {
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated);
  return isAuthenticated ? <>{children}</> : <Navigate to="/login" replace />;
}

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route
            path="/principal"
            element={
              <PrivateRoute>
                <Principal />
              </PrivateRoute>
            }
          />
          <Route
            path="/clientes/novo"
            element={
              <PrivateRoute>
                <ClienteNovo />
              </PrivateRoute>
            }
          />
          <Route
            path="/clientes/alterar/:id"
            element={
              <PrivateRoute>
                <Alterar />
              </PrivateRoute>
            }
          />
          <Route
            path="/extrato"
            element={
              <PrivateRoute>
                <Extrato />
              </PrivateRoute>
            }
          />
          <Route
            path="/extrato/imprimir"
            element={
              <PrivateRoute>
                <ExtratoImprimir />
              </PrivateRoute>
            }
          />
          <Route
            path="/lancamentos"
            element={
              <PrivateRoute>
                <Lanca />
              </PrivateRoute>
            }
          />
          <Route
            path="/totais"
            element={
              <PrivateRoute>
                <Totais />
              </PrivateRoute>
            }
          />
          <Route
            path="/usuarios"
            element={
              <PrivateRoute>
                <Usuarios />
              </PrivateRoute>
            }
          />
          <Route
            path="/relatorios"
            element={
              <PrivateRoute>
                <Relatorios />
              </PrivateRoute>
            }
          />
          <Route
            path="/relatorios/devedores"
            element={
              <PrivateRoute>
                <Devedores />
              </PrivateRoute>
            }
          />
          <Route
            path="/relatorios/credores"
            element={
              <PrivateRoute>
                <Credores />
              </PrivateRoute>
            }
          />
          <Route path="/" element={<Navigate to="/login" replace />} />
          <Route path="*" element={<Navigate to="/login" replace />} />
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  </StrictMode>,
);
