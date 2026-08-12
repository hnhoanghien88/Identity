# Quickstart Validation: Quản lý Applications

## Prerequisites

- .NET 10 SDK, Node.js/npm compatible with the lockfile, and a representative MySQL instance.
- Development configuration supplies database/JWT settings through local environment-specific configuration; do not commit secrets.
- Test identities provide: View only, View+Create+Update, all four Applications permissions, and no Applications permission.

See [data-model.md](data-model.md) for invariants and [applications.openapi.yaml](contracts/applications.openapi.yaml) for endpoint contracts.

## 1. Restore and baseline

From `Identity-api`:

```powershell
dotnet restore Identity-api.slnx
dotnet build Identity-api.slnx --no-restore
dotnet test Identity-api.slnx --no-build
```

From `Identity-client`:

```powershell
npm ci
npm run lint
npm run test -- --run
npm run build
```

Expected: baseline and feature checks pass without skipped required tests.

## 2. Validate migration and recovery

Apply against a disposable representative database from `Identity-api`:

```powershell
dotnet ef database update --project src/Identity.Infrastructure --startup-project src/Identity.Api --context IdentityDbContext
```

Verify existing rows receive Version 1, Code has case-insensitive uniqueness, dependent data remains intact, and migration fails safely when case-only duplicates are seeded. Test the generated down migration only on a disposable backup, then reapply up and document recovery before production rollout.

## 3. Run the application

From `Identity-api`:

```powershell
dotnet run --project src/Identity.Api
```

From `Identity-client` in another terminal:

```powershell
npm run dev
```

Sign in and open `/applications`. Expected: the CRUD screen replaces the welcome card.

## 4. Primary end-to-end scenarios

1. Seed multiple pages; filter Code, Name, Audience/status; sort supported columns; verify totals and deleted-row exclusion.
2. Create valid data and verify persistence/audit/Version 1. Submit missing, overlength and case-only duplicate Code; verify field errors and no partial record.
3. Update all fields including IsActive with current Version; verify one Version increment and audit stamp.
4. Verify delete confirmation shows Code/Name. Cancel with no change; delete an unreferenced row and prove soft-delete/inactive/version behavior.
5. Delete a referenced Application; expect safe 409 and unchanged data.
6. Edit in two sessions; save one then update/delete stale data; expect 409, no overwrite and reload path.
7. Test every capability against UI/direct route/direct API; expect 401 unauthenticated and 403 without permission with no data leak.
8. Expire the session and interrupt network during reads/mutations; verify no false success/duplicate submission, retained form values and prior rows on reload failure.
9. Complete CRUD by keyboard; verify visible focus, labels, dialog focus restore and supported responsive widths.

## 5. Performance and final gates

With 10,000 representative Applications, at least 95% of list/filter/sort/page actions must display within 2 seconds under normal load. Record environment, dataset and measurements.

Re-run all commands from section 1. Review changed JSX and object/callback formatting with Prettier and the constitution's multi-line rule. Do not release with unresolved authorization, concurrency, migration or correctness failures.
