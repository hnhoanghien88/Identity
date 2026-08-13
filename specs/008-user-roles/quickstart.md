# Quickstart Validation: Quản lý User theo Role

## Prerequisites

- MySQL development database is available with existing migrations.
- Seed two active Roles and four active Users; assign one User to the first Role.
- Configure API and client using existing project instructions.

## Automated checks

```powershell
dotnet test Identity-api
```

```powershell
Set-Location Identity-client
npm test -- --run
npm run lint
npx prettier --check .
npm run build
```

Expected: all checks pass with no skipped required User Roles coverage.

## End-to-end validation

1. Sign in and open `/user-roles`; confirm two columns, first Role active, and correct members.
2. Change Roles quickly; only the final Role's members remain.
3. Open **Add User**; existing members are absent, search/paging work, Save is disabled when empty.
4. Select three candidates and save; all appear exactly once after reload.
5. Retry or concurrently assign; no duplicate membership is created.
6. Trigger save failure; dialog and selections remain with an actionable error.
7. Cancel one delete, then confirm it; only membership is removed and User becomes a candidate.
8. Expire session; requests fail closed and UI shows no false success.
9. Repeat all interactions using only keyboard; verify focus and labels.

See [data-model.md](data-model.md) and [contracts/user-roles.openapi.yaml](contracts/user-roles.openapi.yaml).
