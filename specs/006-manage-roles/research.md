# Research: Quản lý Roles

## Decisions

- Mirror Actions CQRS: MediatR, EF writes, Dapper reads and existing React/MUI patterns.
- Enforce trimmed, case-insensitive `(ApplicationId, Code)` uniqueness.
- Add unsigned `Version` default 1 and require it for update/delete concurrency.
- Physically delete only normal Roles without `user_roles` or `role_permissions` dependencies.
- Require authentication but define/check no Roles.* policies or claims.
- Reuse the authenticated Applications search contract for selectors.

These decisions preserve existing architecture, authorization integrity and dependencies without adding libraries. Alternatives rejected were global uniqueness, cascade/auto-unassign, last-write-wins, anonymous access and duplicate lookup endpoints.
