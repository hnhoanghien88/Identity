# Embedded SQL migrations

Create one immutable folder per EF migration:

```text
Persistence/Sql/Migrations/<MigrationName>/
|-- Up/
|   |-- 001_create_view.sql
|   `-- 002_create_procedure.sql
`-- Down/
    |-- 001_drop_procedure.sql
    `-- 002_drop_view.sql
```

`dotnet ef migrations add <MigrationName>` automatically adds `ExecuteFolder`
calls for both folders. SQL files run in ordinal filename order. A missing or empty
folder is a no-op, so regular table migrations continue to work.

Never edit SQL files belonging to an applied migration. Add a new migration folder
for every database-object change.

dotnet ef migrations add UserObjectsV1 `  --project src\Identity.Infrastructure`
--startup-project src\Identity.Api `
--context IdentityDbContext

dotnet ef database update `  --project src\Identity.Infrastructure`
--startup-project src\Identity.Api `
--context IdentityDbContext

dotnet ef database update UserObjectsV1 `  --project src\Identity.Infrastructure`
--startup-project src\Identity.Api `
--context IdentityDbContext
## User Code migration

The Code/Email separation is intentionally staged:

1. Apply only `20260810085053_AddUserCodeIdentity`.
2. Copy and review `UserCodes/001_validate_and_stage.sql`; provide one valid business Code for every existing User.
3. Confirm all three validation queries return no missing, invalid, or duplicate rows.
4. Apply `20260810090000_EnforceUserCodeIdentity`.

The enforcement migration assigns `admin` to User ID 1 and uses the valid unique fallback `user-{Id}` for any existing User not present in the reviewed mapping. These fallback Codes can be renamed through Users management after deployment. The migration does not derive Code from Email and does not modify existing Email or NormalizedEmail values.
