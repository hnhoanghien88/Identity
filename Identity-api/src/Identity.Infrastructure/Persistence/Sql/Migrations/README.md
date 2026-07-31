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
