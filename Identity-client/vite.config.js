import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/login': {
        target: 'https://localhost:7203',
        changeOrigin: true,
        secure: false,
      },
      '/refresh': {
        target: 'https://localhost:7203',
        changeOrigin: true,
        secure: false,
      },
      '/logout': {
        target: 'https://localhost:7203',
        changeOrigin: true,
        secure: false,
      },
    },
  },
})
