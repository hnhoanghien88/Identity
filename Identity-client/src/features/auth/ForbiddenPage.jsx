import { Alert, Box, Typography } from "@mui/material";

export function ForbiddenPage() {
  return (
    <Box role="main" aria-labelledby="forbidden-title">
      <Alert severity="error">
        <Typography id="forbidden-title" variant="h5" component="h1">
          403 — Bạn chưa có quyền
        </Typography>
        <Typography>
          Tài khoản của bạn chưa được cấp quyền sử dụng chức năng này.
        </Typography>
      </Alert>
    </Box>
  );
}
