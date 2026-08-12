# Quickstart Validation: Quản lý Roles

## Prerequisites

- .NET 10, Node/npm lockfile versions, representative MySQL and externalized development config.
- Account without Roles claims; Applications and system/linked/unlinked Role fixtures.
- See [data-model.md](data-model.md) and [roles.openapi.yaml](contracts/roles.openapi.yaml).

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

Apply migration on disposable DB and verify Version, per-Application case-insensitive uniqueness, FKs and recovery.

## End-to-end

1. Login without Roles claims; `/roles` and CRUD work; anonymous API returns 401.
2. Filter Application/Code/Name, sort/page and verify loading/empty/error/retry/retained rows.
3. Create valid Role; invalid Application, overlength and duplicate requests fail atomically.
4. Update increments Version; stale update/delete returns 409 without overwrite.
5. Cancel delete unchanged; normal unlinked Role deletes; system/linked Role returns 409 unchanged.
6. Expired session/network failure gives no false success/duplicate and retains form.
7. Complete CRUD by keyboard with visible focus and responsive layout.

With 10.000 Roles, verify 95% list operations under two seconds; run formatter, lint, tests/build and manual formatting review.
