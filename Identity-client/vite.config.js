import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  test: {
    environment: "jsdom",
    setupFiles: "./tests/testSetup.js",
  },
  server: {
    proxy: {
      "/backend": {
        target: "https://localhost:7203",
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.slice(8),
      },
    },
  },
});
