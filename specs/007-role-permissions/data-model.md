# Data Model: PhÃ¢n quyá»n theo Role

## Role

- `Id`, `Code`, `ApplicationId`, `IsActive` vÃ  thÃ´ng tin audit hiá»‡n cÃ³.
- Má»™t Role cÃ³ nhiá»u RolePermission.

## Resource

- `Id`, `ApplicationId`, `Code`, `ApplicationCode`, `IsActive`, `IsDeleted` vÃ  audit hiá»‡n cÃ³.
- Má»™t Resource cÃ³ thá»ƒ cÃ³ nhiá»u Permission.

## PermissionAction

- `Id`, `Code`, `Name`, `Version` vÃ  audit hiá»‡n cÃ³.

## Permission

- `Id`, `ResourceId`, `ActionId`, `Code`, `Name`, `IsActive` vÃ  audit.
- KhÃ´ng cÃ³ cá»™t `IsDeleted`; khÃ´ng Ã¡p dá»¥ng soft delete.
- Duy nháº¥t theo `(ResourceId, ActionId)`.

## RolePermission

- `Id`, `RoleId`, `PermissionId` vÃ  audit.
- Duy nháº¥t theo `(RoleId, PermissionId)`.

## ActionGrantView

MÃ´ hÃ¬nh Ä‘á»c khÃ´ng lÆ°u riÃªng gá»“m `actionId`, `code`, `name`, `isGranted`. `isGranted` lÃ  true khi tá»“n táº¡i RolePermission ná»‘i Role Ä‘ang chá»n vá»›i Permission cá»§a Resource vÃ  Action.

## Validation vÃ  transitions

- Role, Resource vÃ  Action pháº£i tá»“n táº¡i táº¡i thá»i Ä‘iá»ƒm ghi.
- Grant: náº¿u chÆ°a cÃ³ Permission thÃ¬ táº¡o Permission, sau Ä‘Ã³ táº¡o RolePermission trong cÃ¹ng transaction.
- Grant Ä‘Ã£ tá»“n táº¡i Ä‘Æ°á»£c xem lÃ  thÃ nh cÃ´ng idempotent.
- Revoke: xÃ³a váº­t lÃ½ RolePermission rá»“i xÃ³a váº­t lÃ½ Permission trong cÃ¹ng transaction.
- Náº¿u Permission cÃ²n Ä‘Æ°á»£c Role khÃ¡c tham chiáº¿u, revoke bá»‹ tá»« chá»‘i vÃ  transaction rollback toÃ n bá»™.
- Revoke khi Permission khÃ´ng tá»“n táº¡i Ä‘Æ°á»£c xem lÃ  thÃ nh cÃ´ng idempotent.
- Unique indexes ngÄƒn báº£n ghi trÃ¹ng khi thao tÃ¡c Ä‘á»“ng thá»i.
# Update 2026-08-13: Cross-entity invariant

- A `Role` belongs to one `Application`.
- A `Resource` belongs to one `Application`.
- A Role Permission operation is valid only when `Role.ApplicationId == Resource.ApplicationId`.
- Violating combinations are rejected before permission data is read or changed; no new fields or migration are required.
