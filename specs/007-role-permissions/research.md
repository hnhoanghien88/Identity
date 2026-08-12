# Research: PhÃ¢n quyá»n theo Role

## MÃ´ hÃ¬nh lÆ°u quyá»n

**Decision**: Giá»¯ hai lá»›p dá»¯ liá»‡u hiá»‡n cÃ³: `permissions` duy nháº¥t theo `(ResourceId, ActionId)` vÃ  `role_permissions` duy nháº¥t theo `(RoleId, PermissionId)`.

**Rationale**: Domain, EF configuration vÃ  database hiá»‡n táº¡i Ä‘Ã£ biá»ƒu diá»…n Permission nhÆ° má»™t hÃ nh Ä‘á»™ng trÃªn Resource dÃ¹ng chung giá»¯a nhiá»u Role. Chá»‰ liÃªn káº¿t RolePermission thá»ƒ hiá»‡n checkbox cá»§a Role Ä‘ang chá»n.

**Alternatives considered**: ThÃªm `RoleId` trá»±c tiáº¿p vÃ o `permissions` lÃ m thay Ä‘á»•i mÃ´ hÃ¬nh authorization hiá»‡n cÃ³ vÃ  cáº§n migration; xÃ³a Permission khi bá» check cÃ³ thá»ƒ lÃ m máº¥t liÃªn káº¿t cá»§a Role khÃ¡c.

## Contract cáº¥p/gá»¡ quyá»n

**Decision**: DÃ¹ng `PUT` vÃ  `DELETE` idempotent trÃªn Ä‘á»‹nh danh Roleâ€“Resourceâ€“Action.

**Rationale**: Checkbox biá»ƒu diá»…n tráº¡ng thÃ¡i mong muá»‘n. Gá»­i láº¡i cÃ¹ng tráº¡ng thÃ¡i pháº£i an toÃ n khi retry, refresh token hoáº·c cáº¡nh tranh máº¡ng.

**Alternatives considered**: Endpoint toggle khÃ´ng idempotent vÃ  cÃ³ thá»ƒ Ä‘áº£o sai tráº¡ng thÃ¡i khi retry; batch save táº¡o tráº¡ng thÃ¡i chá»‰nh sá»­a chÆ°a cam káº¿t trÃ¡i vá»›i spec.

## Truy váº¥n cá»™t Actions

**Decision**: Má»™t endpoint tá»•ng há»£p tráº£ toÃ n bá»™ Action cÃ¹ng `isGranted` cho cáº·p Roleâ€“Resource.

**Rationale**: Má»™t snapshot duy nháº¥t trÃ¡nh ghÃ©p nhiá»u response khÃ´ng Ä‘á»“ng bá»™ á»Ÿ client vÃ  cho phÃ©p kiá»ƒm tra Role/Resource há»£p lá»‡ á»Ÿ má»™t ranh giá»›i.

**Alternatives considered**: Client táº£i Actions vÃ  Permissions riÃªng lÃ m tÄƒng race condition; nhÃºng vÃ o endpoint Resources lÃ m láº«n trÃ¡ch nhiá»‡m feature.

## Äá»“ng thá»i vÃ  giao dá»‹ch

**Decision**: Thao tÃ¡c cáº¥p cháº¡y trong transaction, dá»±a vÃ o unique indexes hiá»‡n cÃ³ vÃ  xá»­ lÃ½ duplicate race nhÆ° tráº¡ng thÃ¡i Ä‘Ã£ Ä‘áº¡t; gá»¡ khÃ´ng tá»“n táº¡i cÅ©ng thÃ nh cÃ´ng.

**Rationale**: Äáº¡t tÃ­nh idempotent, khÃ´ng táº¡o báº£n ghi má»™t pháº§n vÃ  khÃ´ng cáº§n thÃªm cá»™t version cho liÃªn káº¿t dáº¡ng set membership.

**Alternatives considered**: ThÃªm version vÃ o Permission/RolePermission tÄƒng migration vÃ  khÃ´ng mang thÃªm giÃ¡ trá»‹ cho PUT/DELETE idempotent; khÃ³a bi quan má»Ÿ rá»™ng lÃ m giáº£m concurrency.

## Táº£i dá»¯ liá»‡u lá»±a chá»n vÃ  chá»‘ng response cÅ©

**Decision**: TÃ¡i sá»­ dá»¥ng search Roles/Resources, chá»n dÃ²ng Ä‘áº§u theo thá»© tá»± á»•n Ä‘á»‹nh; dÃ¹ng AbortController cá»™ng kiá»ƒm tra generation cho request Actions.

**Rationale**: Giá»¯ contract hiá»‡n cÃ³, Ä‘Ã¡p á»©ng chá»n máº·c Ä‘á»‹nh vÃ  báº£o Ä‘áº£m response cÅ© khÃ´ng ghi Ä‘Ã¨ lá»±a chá»n má»›i.

**Alternatives considered**: Táº¡o endpoint lookup má»›i lÃ m rá»™ng API khÃ´ng cáº§n thiáº¿t; chá»‰ abort request chÆ°a Ä‘á»§ náº¿u response Ä‘Ã£ hoÃ n táº¥t gáº§n Ä‘á»“ng thá»i.

## Authorization

**Decision**: Controller dÃ¹ng yÃªu cáº§u authenticated chung vÃ  khÃ´ng cÃ³ policy/claim Role Permissions riÃªng.

**Rationale**: PhÃ¹ há»£p yÃªu cáº§u â€œkhÃ´ng yÃªu cáº§u quyá»nâ€ nhÆ°ng váº«n tuÃ¢n thá»§ constitution fail-closed cho há»‡ thá»‘ng quáº£n trá»‹ identity.

**Alternatives considered**: Anonymous access vi pháº¡m constitution; policy CRUD riÃªng trÃ¡i yÃªu cáº§u feature.

