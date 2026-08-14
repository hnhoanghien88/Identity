# Implementation Plan: PhÃ¢n quyá»n theo Role

**Branch**: `007-role-permissions` | **Date**: 2026-08-12 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/007-role-permissions/spec.md`

## Summary

Bá»• sung mÃ n hÃ¬nh `/role-permissions` gá»“m ba cá»™t Roles, Resources vÃ  Actions. Backend cung cáº¥p má»™t truy váº¥n tá»•ng há»£p cho cáº·p Roleâ€“Resource vÃ  hai thao tÃ¡c idempotent cáº¥p/gá»¡ quyá»n. Thiáº¿t káº¿ giá»¯ mÃ´ hÃ¬nh hiá»‡n cÃ³: `permissions` lÃ  danh má»¥c duy nháº¥t theo Resourceâ€“Action, cÃ²n `role_permissions` lÃ  liÃªn káº¿t Role vá»›i Permission. Frontend React/MUI chá»n máº·c Ä‘á»‹nh dÃ²ng Ä‘áº§u, há»§y request cÅ© khi Ä‘á»•i lá»±a chá»n vÃ  lÆ°u ngay tá»«ng checkbox vá»›i hoÃ n nguyÃªn khi lá»—i.

## Technical Context

**Language/Version**: C# / .NET 10; JavaScript ES modules / React 19  
**Primary Dependencies**: ASP.NET Core, MediatR, EF Core/MySQL, Dapper; React, Vite, Material UI  
**Storage**: MySQL hiá»‡n cÃ³ vá»›i `roles`, `applications`, `resources`, `permission_actions`, `permissions`, `role_permissions`; khÃ´ng cáº§n migration schema  
**Testing**: xUnit; Vitest + Testing Library; oxlint, Prettier, production build  
**Target Platform**: ASP.NET Core API vÃ  trÃ¬nh duyá»‡t hiá»‡n Ä‘áº¡i  
**Project Type**: Layered web API vÃ  React SPA  
**Performance Goals**: 95% lÆ°á»£t Ä‘á»•i Role/Resource vÃ  lÆ°u checkbox hoÃ n táº¥t trong 2 giÃ¢y  
**Constraints**: Chá»‰ yÃªu cáº§u authenticated; khÃ´ng policy riÃªng; fail closed; SQL cÃ³ tham sá»‘; thao tÃ¡c cáº¥p/gá»¡ idempotent vÃ  nháº¥t quÃ¡n; khÃ´ng dependency má»›i  
**Scale/Scope**: Má»™t mÃ n hÃ¬nh, ba endpoint, tá»‘i Ä‘a 1.000 Roles, 10.000 Resources vÃ  100 Actions

## Constitution Check

*GATE: Passed before research and re-checked after design.*

| Principle / gate | Evidence | Status |
|---|---|---|
| Security and identity | Controller yÃªu cáº§u Ä‘Äƒng nháº­p; kiá»ƒm tra toÃ n bá»™ khÃ³a tham chiáº¿u; lá»—i fail closed. | PASS |
| Layer boundaries | Query/commands á»Ÿ Application, persistence á»Ÿ Infrastructure, HTTP á»Ÿ API, UI/API client tÃ¡ch biá»‡t. | PASS |
| Explicit contracts | OpenAPI mÃ´ táº£ request, response, authentication vÃ  lá»—i cho cáº£ ba endpoint. | PASS |
| Tests are gates | CÃ³ unit, API integration, persistence vÃ  frontend tests cho Ä‘á»c/cáº¥p/gá»¡ quyá»n. | PASS |
| Accessible UX | MUI, keyboard/focus, active khÃ´ng chá»‰ báº±ng mÃ u, loading/error/disabled rÃµ rÃ ng. | PASS |
| Readable formatting | Cháº¡y formatter/lint vÃ  review JSX nhiá»u dÃ²ng trÆ°á»›c hoÃ n táº¥t. | PASS |
| Persistence consistency | Giao dá»‹ch báº£o vá»‡ viá»‡c táº¡o Permission vÃ  RolePermission; unique indexes chá»‘ng trÃ¹ng. | PASS |
| Simplicity | TÃ¡i sá»­ dá»¥ng schema vÃ  stack hiá»‡n cÃ³; khÃ´ng dependency hoáº·c migration má»›i. | PASS |

### Post-design re-check

Data model giá»¯ quan há»‡ hiá»‡n cÃ³ vÃ  khÃ´ng phÃ¡ vá»¡ rÃ ng buá»™c xÃ³a. Contract chá»‰ má»Ÿ bá» máº·t cáº§n thiáº¿t, yÃªu cáº§u JWT nhÆ°ng khÃ´ng yÃªu cáº§u claim quyá»n chá»©c nÄƒng. Quickstart bao phá»§ authentication, idempotency, rollback UI vÃ  truy cáº­p bÃ n phÃ­m. KhÃ´ng cÃ³ vi pháº¡m constitution cáº§n ngoáº¡i lá»‡.

## Project Structure

### Documentation (this feature)

```text
specs/007-role-permissions/
â”œâ”€â”€ plan.md
â”œâ”€â”€ research.md
â”œâ”€â”€ data-model.md
â”œâ”€â”€ quickstart.md
â”œâ”€â”€ contracts/role-permissions.openapi.yaml
â””â”€â”€ tasks.md
```

### Source Code (repository root)

```text
Identity-api/
â”œâ”€â”€ src/Identity.Application/Abstractions/Persistence/IRolePermissionsRepository.cs
â”œâ”€â”€ src/Identity.Application/RolePermissions/
â”œâ”€â”€ src/Identity.Infrastructure/Persistence/MySqlRolePermissionsRepository.cs
â”œâ”€â”€ src/Identity.Api/Controllers/RolePermissionsController.cs
â””â”€â”€ tests/*/RolePermissions/

Identity-client/
â”œâ”€â”€ src/features/rolePermissions/
â”œâ”€â”€ src/App.jsx
â”œâ”€â”€ src/App.css
â””â”€â”€ tests/rolePermissions/
```

**Structure Decision**: Giá»¯ bá»‘n layer backend vÃ  feature folder frontend. Má»™t repository chuyÃªn biá»‡t Ä‘Ã³ng gÃ³i transaction nhiá»u báº£ng; controller khÃ´ng truy cáº­p DbContext trá»±c tiáº¿p.

## Implementation Design

- `GET /api/role-permissions?roleId=&resourceId=` tráº£ Role/Resource Ä‘Ã£ chá»n vÃ  toÃ n bá»™ Actions vá»›i `isGranted`.
- `PUT /api/role-permissions/{roleId}/{resourceId}/{actionId}` cáº¥p idempotent: kiá»ƒm tra tham chiáº¿u, tÃ¬m hoáº·c táº¡o Permission Resourceâ€“Action, sau Ä‘Ã³ tÃ¬m hoáº·c táº¡o RolePermission trong má»™t transaction.
- `DELETE` cÃ¹ng Ä‘á»‹a chá»‰ gá»¡ idempotent: xÃ³a RolePermission náº¿u cÃ³ vÃ  giá»¯ Permission dÃ¹ng chung.
- Repository dÃ¹ng truy váº¥n cÃ³ tham sá»‘; unique indexes báº£o vá»‡ cáº¡nh tranh. Duplicate race Ä‘Æ°á»£c Ä‘á»c láº¡i nhÆ° thÃ nh cÃ´ng idempotent.
- Hai cá»™t lá»±a chá»n tÃ¡i sá»­ dá»¥ng search Roles/Resources; Actions láº¥y tá»« response tá»•ng há»£p.
- UI giá»¯ lá»±a chá»n active, há»§y request cÅ© vÃ  kiá»ƒm tra generation Ä‘á»ƒ response cÅ© khÃ´ng ghi Ä‘Ã¨.
- Checkbox cáº­p nháº­t láº¡c quan, disabled khi pending vÃ  hoÃ n nguyÃªn náº¿u request tháº¥t báº¡i.
- Route/menu chá»‰ phá»¥ thuá»™c phiÃªn Ä‘Äƒng nháº­p, khÃ´ng dÃ¹ng capability/policy riÃªng.

## Complexity Tracking

## Update Plan 2026-08-13

- Reuse the existing Role DTO fields `applicationId`, `applicationCode`, and `applicationName`; no schema migration is required.
- Load Roles first. Derive the active Application from the selected Role, then load Resources with the existing `applicationId` filter.
- Clear Resource and Action state whenever the Role Application changes; abort obsolete requests so stale responses cannot cross Application boundaries.
- Enforce the same-Application invariant in the Role Permission persistence boundary for snapshot, grant, and revoke operations.
- Add frontend regression coverage for Application labels, Resource filter requests, and Role switching; validate backend build and existing Role Permission tests.

**Constitution re-check**: PASS. The backend invariant fails closed, layer boundaries remain unchanged, no dependency or migration is added, and automated regression coverage is required.

KhÃ´ng cÃ³ vi pháº¡m constitution cáº§n biá»‡n minh.
