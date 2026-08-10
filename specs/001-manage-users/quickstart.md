# Quickstart Validation: User Management

## Prerequisites

- .NET SDK 10, Node.js 22+, npm, and MySQL.
- Representative Identity_db and an existing Admin identity with role/authorization data.
- Connection, JWT key, and test credentials through local secrets/environment config only.
- Review [API contract](./contracts/users.openapi.yaml) and [data model](./data-model.md).

## Restore and migrate

From `Identity-api`:

```powershell
dotnet restore Identity-api.slnx
dotnet ef database update --project src/Identity.Infrastructure --startup-project src/Identity.Api --context IdentityDbContext
```

Before applying, verify no Code or Email values collide under case-insensitive comparison. After applying, verify NormalizedCode and NormalizedEmail no longer exist, Code/Email values are unchanged, and direct unique indexes reject mixed-case duplicates. Rehearse rollback on a disposable representative database.

From `Identity-client`:

```powershell
npm ci
```

## Automated release gates

From `Identity-api`:

```powershell
dotnet build Identity-api.slnx --no-restore
dotnet test Identity-api.slnx --no-build
```

Expected coverage: ASCII Code validation; direct case-insensitive Code/Email uniqueness; absence of normalized fields; Code-only authentication and generic 401; DTO/projection separation; CRUD; paged count; soft delete; concurrency; authorization; safe errors; response contracts.

From `Identity-client`:

```powershell
npm run lint
npm run test -- --run
npm run build
```

Expected coverage: Code/password login; Email rejected as login identifier; Code column/filter; required Code and Email create/edit validation; gated menu/route; load/empty/error; pagination; password clearing; conflicts; delete; duplicate-submit prevention; expired session.

## Run locally

API, from `Identity-api`:

```powershell
dotnet run --project src/Identity.Api --launch-profile https
```

UI, from `Identity-client`:

```powershell
npm run dev
```

Confirm the proxy maps `/backend/api/users` to `/api/users` and `/backend/login` to `/login`.

## End-to-end smoke

1. Log in with valid Code/password and confirm roles/permissions. Repeat with Email, unknown Code, wrong password, inactive and deleted accounts; each failure is the same generic 401.
2. Admin/Manager see Users; Employee/anonymous receive no data; only Admin sees Delete. Refresh /users and remain on the route.
3. Confirm the table/filter use Code instead of Email, plus sorting, pagination, empty/loading/retry, keyboard and mobile behavior.
4. Reject whitespace/Unicode/invalid/duplicate Code, missing/invalid/duplicate Email and short password. Create a valid User and verify one request with no observable password.
5. Edit Code and Email independently with current version. Confirm the new Code logs in case-insensitively, while the old Code and Email do not authenticate.
6. Submit a stale edit from a second session; expect 409, reload option and no overwrite.
7. Cancel delete with no change. Delete as Admin; row disappears, target refresh/login fails and history remains. Self-delete returns 409.
8. Stop API during submission and test expired access-token refresh/retry. Inspect network/logs for password, hash, stack, connection or secret leakage.

## Performance and evidence

- Use 10,000 synthetic Users in a disposable environment.
- Include mixed-case Codes/Emails and rejected case-insensitive duplicates; never use production credentials or personal Email data.
- Record p95 visible-result time for list/filter/sort/page; each must be <=2 seconds normally.
- Attach build/test/lint, migration forward/rollback, contract review, and smoke evidence.
- A skipped required test needs documented owner, risk, and remediation task.

## Implementation evidence (2026-08-10)

- Applied `20260810093448_RemoveNormalizedUserIdentity` successfully to `Identity_db`.
- Reverted to `20260810090000_EnforceUserCodeIdentity`, verified rollback backfilled both normalized columns, then re-applied the removal migration successfully.
- Release API build passed with no compiler errors. NuGet vulnerability lookup emitted `NU1900` because the restricted environment could not reach `api.nuget.org`.
- Backend tests passed individually: Application 13/13, Infrastructure 4/4, API 3/3.
- Frontend gates passed: Vitest 6/6, oxlint, and Vite production build.
- Credentialed smoke passed: Admin Code login, paged search, temporary User create, versioned update, soft delete, and rejected login after deletion.
- The smoke run exposed Windows Event Log permission failures; application logging was restricted to Console and Debug providers so logging cannot terminate requests in local development.
