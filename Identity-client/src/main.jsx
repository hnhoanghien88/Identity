import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { createTheme, CssBaseline, ThemeProvider } from "@mui/material";
import "./index.css";
import App from "./App.jsx";

const theme = createTheme({
  palette: {
    primary: { main: "rgb(75, 94, 211)", dark: "rgb(62, 79, 181)" },
    background: { default: "#f3f4f6", paper: "#ffffff" },
    text: { primary: "#343a40", secondary: "#8a969c" },
  },
  typography: {
    fontFamily: '"Nunito", Inter, "Segoe UI", sans-serif',
    fontSize: 14,
    h4: { fontSize: "1.15rem", fontWeight: 700 },
    h6: { fontSize: "1rem", fontWeight: 700 },
    button: { textTransform: "none", fontWeight: 600 },
  },
  shape: { borderRadius: 4 },
  components: {
    MuiButton: { styleOverrides: { root: { boxShadow: "none" } } },
    MuiPaper: { styleOverrides: { root: { backgroundImage: "none" } } },
    MuiTextField: { defaultProps: { size: "small" } },
    MuiTable: { defaultProps: { size: "small" } },
    MuiDialog: { defaultProps: { fullWidth: true } },
  },
});

createRoot(document.getElementById("root")).render(
  <StrictMode>
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <App />
    </ThemeProvider>
  </StrictMode>,
);
