import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";
import path from "path";

export default defineConfig({
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
  server: {
    port: 5173,
  },
  test: {
    environment: "jsdom",
    setupFiles: ["./src/test-setup.ts"],
    server: {
      deps: {
        // Force vitest to inline (not pre-bundle) these packages so they are
        // processed in the test environment where NODE_ENV='test', making
        // React's development build (which exports `act`) available.
        inline: [/@testing-library\/react/, /react-dom/],
      },
    },
  },
});
