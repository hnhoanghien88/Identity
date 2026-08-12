# Quickstart Validation: Quáº£n lÃ½ Actions

## Prerequisites

- .NET 10, Node.js/npm theo lockfile vÃ  MySQL Ä‘áº¡i diá»‡n.
- Development database/JWT config ngoÃ i source control.
- TÃ i khoáº£n khÃ´ng cÃ³ Actions permissions vÃ  fixture cÃ³/khÃ´ng Permission dependency.

See [data-model.md](data-model.md) and [actions.openapi.yaml](contracts/actions.openapi.yaml).

## Gates

```powershell
cd Identity-api
dotnet restore Identity-api.slnx
dotnet build Identity-api.slnx --no-restore
dotnet test Identity-api.slnx --no-build
cd ../Identity-client
npm ci
npm run lint
npm run test -- --run
npm run build
```

Apply migration on disposable DB; verify Version 1, case-insensitive Code uniqueness and intact Permissions; test down/up and recovery.

## End-to-end

1. Login without Actions claims; `/actions` and CRUD work; anonymous/direct API gets 401.
2. Filter/sort/page, empty/loading/error/retry and retained rows behave correctly.
3. Create valid row; invalid/overlength/case-only duplicate gets field error and no partial write.
4. Update increments Version; stale update/delete returns 409 without overwrite.
5. Cancel delete unchanged; unreferenced row deleted; referenced row returns 409 unchanged.
6. Expired session/network failure causes no false success/duplicate and retains form.
7. CRUD works by keyboard with visible focus, labels and responsive layout.

With 10.000 Actions, verify 95% list/filter/sort/page under 2 seconds. Re-run gates, formatter and multi-line formatting review.
## Validation Record (2026-08-12)

- Release backend build passed with 0 warnings and 0 errors.
- Migration `20260812031658_AddPermissionActionVersion` applied successfully to the configured development database; existing rows received Version 1.
- Actions application tests were added and compile successfully; the local .NET test runner spawned orphan child processes and did not return results, so full backend test execution must be rerun after cleaning that runner environment.
- Actions frontend tests passed 3/3; lint and production build passed.
- Full frontend suite reached 8 passing files but one pre-existing Resources create test timed out under the parallel run; the focused Actions suite passes deterministically.
- Git whitespace validation passed. Debug build is blocked by the already-running Identity.Api process, while Release build passes.
