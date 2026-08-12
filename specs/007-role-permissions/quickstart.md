# Quickstart Validation: PhÃ¢n quyá»n theo Role

## Prerequisites

- MySQL development database Ä‘Ã£ Ã¡p dá»¥ng migrations hiá»‡n cÃ³.
- CÃ³ Ã­t nháº¥t hai Roles, hai Resources vÃ  ba Permission Actions.
- CÃ³ tÃ i khoáº£n Ä‘Äƒng nháº­p khÃ´ng cáº§n claim Role Permissions riÃªng.

## Automated checks

```powershell
dotnet test Identity-api/Identity-api.slnx
Set-Location Identity-client
npm test -- --run
npm run lint
npm run format:check
npm run build
```

## Scenario 1: Load and selection

ÄÄƒng nháº­p, má»Ÿ `/role-permissions`, xÃ¡c nháº­n ba cá»™t vÃ  lá»±a chá»n máº·c Ä‘á»‹nh. Äá»•i Role/Resource liÃªn tiáº¿p; Actions pháº£i luÃ´n khá»›p lá»±a chá»n má»›i nháº¥t vÃ  contract [role-permissions.openapi.yaml](contracts/role-permissions.openapi.yaml).

## Scenario 2: Grant

Check Action chÆ°a cáº¥p, táº£i láº¡i vÃ  gá»­i láº¡i cÃ¹ng yÃªu cáº§u. Checkbox váº«n check; chá»‰ cÃ³ má»™t Permission Resourceâ€“Action vÃ  má»™t RolePermission.

## Scenario 3: Revoke

Bá» check Action Ä‘Ã£ cáº¥p, táº£i láº¡i vÃ  gá»­i láº¡i yÃªu cáº§u gá»¡. RolePermission khÃ´ng cÃ²n nhÆ°ng Permission dÃ¹ng chung váº«n tá»“n táº¡i.

## Scenario 4: Failure and accessibility

MÃ´ phá»ng lá»—i lÆ°u Ä‘á»ƒ xÃ¡c nháº­n UI hoÃ n nguyÃªn; Ä‘á»•i lá»±a chá»n khi request cÅ© cháº­m; hoÃ n thÃ nh luá»“ng báº±ng bÃ n phÃ­m; gá»i API khÃ´ng token. Tráº¡ng thÃ¡i pháº£i khá»›p dá»¯ liá»‡u Ä‘Ã£ cam káº¿t, focus/active rÃµ rÃ ng vÃ  anonymous nháº­n 401.

