# Quickstart Validation: Quản lý Resources

## Prerequisites

- .NET 10 SDK, Node.js/npm compatible with lockfile, and representative MySQL.
- Development database/JWT configuration supplied outside committed secrets.
- Test identities with View only, View+Create+Update, all four Resources permissions, and no Resources permission.
- At least two active Applications, one inactive Application, and Permission/Menu dependency fixtures.

See [data-model.md](data-model.md) and [resources.openapi.yaml](contracts/resources.openapi.yaml).

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

Expected: baseline and feature checks pass with no skipped required tests.

## 2. Validate migration and recovery

Against a disposable representative database from `Identity-api`:

```powershell
dotnet ef database update --project src/Identity.Infrastructure --startup-project src/Identity.Api --context IdentityDbContext
```

Verify existing Resources receive Version 1; case-insensitive uniqueness is scoped by Application; Permissions/Menus remain intact. Seed a case-only duplicate pair in one Application and prove migration fails safely. Test down migration only on disposable backup, reapply up, and document production recovery.

## 3. Run application

From `Identity-api`:

```powershell
dotnet run --project src/Identity.Api
```

From `Identity-client`:

```powershell
npm run dev
```

Sign in and open `/resources`. Expected: protected Resources CRUD screen and selected navigation item.

## 4. Primary end-to-end scenarios

1. Seed multiple pages across Applications; filter/sort/page and verify totals, Application labels and deleted exclusion.
2. Create valid Resource and verify persistence, audit, active state and Version 1.
3. Submit missing/overlength/case-only duplicate in same Application; verify field errors and no partial row. Reuse Code in another Application successfully.
4. Try inactive/deleted/missing Application; expect safe rejection. Move Resource to valid Application and preserve Id/dependencies.
5. Update with current Version; verify exactly one Version increment and audit stamp.
6. Confirm delete dialog identifies Application/Code/Name. Cancel unchanged; delete unreferenced row and verify soft-delete/inactive/version.
7. Delete Resource referenced by Permission or Menu; expect safe 409 and unchanged data.
8. Save concurrently in two sessions; stale update/delete returns 409 without overwrite and offers reload.
9. Test each UI route/action and direct API with identities above; expect 401 unauthenticated and 403 missing policy.
10. Expire session or interrupt network; verify no false success/duplicate submission, form retention, stale-row retention and Retry.
11. Complete CRUD by keyboard; verify visible focus, labels, dialog focus restore and responsive widths.

## 5. Performance and final gates

With 10,000 representative Resources, 95% of list/filter/sort/page interactions must display within 2 seconds under normal load. Record environment, data and measurements.

Re-run section 1. Review changed JSX/object/callback formatting with Prettier and constitution multi-line rules. Do not release with unresolved authorization, concurrency, migration, dependency or correctness failures.

## Validation Record (2026-08-11)

- Migration `20260811094606_AddResourceVersion` applied successfully to the configured development database.
- Down migration to `20260811054449_AddApplicationVersion` succeeded, then the Resources migration was reapplied successfully.
- The duplicate precheck, explicit Resource Code collation and Version default executed without data loss.
- The transactional 10,000-row Resource benchmark returned a 20-row filtered/sorted page below the 2-second threshold and rolled back all fixture data.
- Backend tests passed: API 19, Application 35, Infrastructure 12 before the benchmark was upgraded; final full-suite validation includes the benchmark.
- Client gates passed: oxlint, 9 Vitest files/15 tests, and Vite production build.