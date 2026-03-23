// eslint.config.js — ESLint 9 flat config (TypeScript + React)
import tseslint from "typescript-eslint";
import reactPlugin from "eslint-plugin-react";
import reactHooksPlugin from "eslint-plugin-react-hooks";
import globals from "globals";

export default tseslint.config(
  // ignore build artifacts
  { ignores: ["dist/", "node_modules/"] },

  // TypeScript recommended rules for .ts and .tsx files
  ...tseslint.configs.recommended,

  {
    files: ["**/*.{ts,tsx}"],
    plugins: {
      react: reactPlugin,
      "react-hooks": reactHooksPlugin,
    },
    languageOptions: {
      globals: {
        ...globals.browser,
        ...globals.es2022,
      },
    },
    settings: {
      react: { version: "detect" },
    },
    rules: {
      // React recommended rules
      ...reactPlugin.configs.recommended.rules,
      // React Hooks — only the two classic rules (rules-of-hooks + exhaustive-deps)
      // The full recommended config of react-hooks v7 includes React Compiler rules
      // which are beyond the scope of basic linting for this project.
      "react-hooks/rules-of-hooks": "error",
      "react-hooks/exhaustive-deps": "warn",
      // Not needed with React 17+ automatic JSX transform
      "react/react-in-jsx-scope": "off",
      // TypeScript handles prop validation
      "react/prop-types": "off",
    },
  },
);
