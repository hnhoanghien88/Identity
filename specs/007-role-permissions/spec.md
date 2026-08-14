# Feature Specification: PhÃ¢n quyá»n theo Role

**Feature Branch**: `007-role-permissions`

**Created**: 2026-08-12

**Status**: Draft

**Input**: Chá»©c nÄƒng má»›i role-permissions, khÃ´ng yÃªu cáº§u quyá»n. MÃ n hÃ¬nh chÃ­nh gá»“m ba cá»™t: Roles, Resources vÃ  Actions; ngÆ°á»i dÃ¹ng chá»n Role vÃ  Resource Ä‘á»ƒ xem, thÃªm hoáº·c gá»¡ cÃ¡c Permission tÆ°Æ¡ng á»©ng báº±ng checkbox.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Chá»n Role vÃ  Resource Ä‘á»ƒ xem quyá»n (Priority: P1)

NgÆ°á»i quáº£n trá»‹ Ä‘Ã£ Ä‘Äƒng nháº­p má»Ÿ mÃ n hÃ¬nh **Role Permissions**, chá»n má»™t Role á»Ÿ cá»™t thá»© nháº¥t vÃ  má»™t Resource á»Ÿ cá»™t thá»© hai Ä‘á»ƒ xem toÃ n bá»™ Actions cÃ¹ng tráº¡ng thÃ¡i Ä‘Æ°á»£c cáº¥p hoáº·c chÆ°a Ä‘Æ°á»£c cáº¥p á»Ÿ cá»™t thá»© ba.

**Why this priority**: ÄÃ¢y lÃ  luá»“ng ná»n táº£ng giÃºp ngÆ°á»i dÃ¹ng hiá»ƒu chÃ­nh xÃ¡c quyá»n hiá»‡n táº¡i cá»§a tá»«ng cáº·p Roleâ€“Resource trÆ°á»›c khi thay Ä‘á»•i.

**Independent Test**: Chuáº©n bá»‹ má»™t Role, hai Resources vÃ  cÃ¡c Permission Ä‘Ã£ biáº¿t; láº§n lÆ°á»£t chá»n tá»«ng dÃ²ng vÃ  xÃ¡c nháº­n dÃ²ng active cÃ¹ng tráº¡ng thÃ¡i checkbox pháº£n Ã¡nh Ä‘Ãºng dá»¯ liá»‡u Ä‘Ã£ lÆ°u.

**Acceptance Scenarios**:

1. **Given** ngÆ°á»i dÃ¹ng Ä‘Ã£ Ä‘Äƒng nháº­p vÃ  cÃ³ dá»¯ liá»‡u Roles, Resources, **When** má»Ÿ **Role Permissions**, **Then** mÃ n hÃ¬nh hiá»ƒn thá»‹ ba cá»™t vÃ  tá»± chá»n Role cÃ¹ng Resource kháº£ dá»¥ng Ä‘áº§u tiÃªn Ä‘á»ƒ táº£i Actions.
2. **Given** má»™t Role vÃ  Resource Ä‘ang active, **When** cá»™t Actions Ä‘Æ°á»£c táº£i, **Then** má»—i Action hiá»ƒn thá»‹ Code vÃ  checkbox Action, trong Ä‘Ã³ checkbox Ä‘Æ°á»£c check khi Permission tÆ°Æ¡ng á»©ng tá»“n táº¡i vÃ  khÃ´ng check khi Permission khÃ´ng tá»“n táº¡i.
3. **Given** má»™t cáº·p Roleâ€“Resource Ä‘ang active, **When** ngÆ°á»i dÃ¹ng chá»n Role khÃ¡c, **Then** Role má»›i trá»Ÿ thÃ nh active vÃ  tráº¡ng thÃ¡i Actions Ä‘Æ°á»£c táº£i láº¡i theo Role má»›i vá»›i Resource Ä‘ang active.
4. **Given** má»™t cáº·p Roleâ€“Resource Ä‘ang active, **When** ngÆ°á»i dÃ¹ng chá»n Resource khÃ¡c, **Then** Resource má»›i trá»Ÿ thÃ nh active vÃ  tráº¡ng thÃ¡i Actions Ä‘Æ°á»£c táº£i láº¡i theo Resource má»›i vá»›i Role Ä‘ang active.
5. **Given** ngÆ°á»i dÃ¹ng Ä‘iá»u hÆ°á»›ng báº±ng bÃ n phÃ­m, **When** focus vÃ  kÃ­ch hoáº¡t má»™t dÃ²ng Role hoáº·c Resource, **Then** dÃ²ng Ä‘Ã³ cÃ³ tráº¡ng thÃ¡i active nhÃ¬n tháº¥y rÃµ vÃ  Actions Ä‘Æ°á»£c cáº­p nháº­t nhÆ° thao tÃ¡c chuá»™t.

---

### User Story 2 - Cáº¥p Permission báº±ng checkbox (Priority: P1)

NgÆ°á»i quáº£n trá»‹ check má»™t Action Ä‘á»ƒ cáº¥p Action Ä‘Ã³ cho cáº·p Roleâ€“Resource Ä‘ang active.

**Why this priority**: Cáº¥p quyá»n lÃ  giÃ¡ trá»‹ nghiá»‡p vá»¥ chÃ­nh cá»§a mÃ n hÃ¬nh vÃ  pháº£i Ä‘Æ°á»£c lÆ°u chÃ­nh xÃ¡c Ä‘á»ƒ kiá»ƒm soÃ¡t truy cáº­p.

**Independent Test**: Chá»n má»™t Role, Resource vÃ  Action chÆ°a cÃ³ Permission, check Action, táº£i láº¡i mÃ n hÃ¬nh rá»“i xÃ¡c nháº­n checkbox váº«n Ä‘Æ°á»£c check vÃ  Permission tÆ°Æ¡ng á»©ng tá»“n táº¡i duy nháº¥t.

**Acceptance Scenarios**:

1. **Given** Action chÆ°a Ä‘Æ°á»£c cáº¥p cho cáº·p Roleâ€“Resource active, **When** ngÆ°á»i dÃ¹ng check Action, **Then** há»‡ thá»‘ng lÆ°u má»™t Permission tÆ°Æ¡ng á»©ng vÃ  xÃ¡c nháº­n tráº¡ng thÃ¡i Ä‘Ã£ lÆ°u.
2. **Given** thao tÃ¡c cáº¥p Permission Ä‘ang Ä‘Æ°á»£c xá»­ lÃ½, **When** ngÆ°á»i dÃ¹ng tiáº¿p tá»¥c tÆ°Æ¡ng tÃ¡c vá»›i cÃ¹ng checkbox, **Then** há»‡ thá»‘ng ngÄƒn yÃªu cáº§u trÃ¹ng láº·p cho Ä‘áº¿n khi cÃ³ káº¿t quáº£.
3. **Given** viá»‡c lÆ°u Permission tháº¥t báº¡i, **When** há»‡ thá»‘ng tráº£ lá»—i, **Then** checkbox trá»Ÿ vá» tráº¡ng thÃ¡i chÆ°a check Ä‘Ã£ lÆ°u gáº§n nháº¥t, khÃ´ng cÃ³ Permission má»™t pháº§n hoáº·c trÃ¹ng láº·p vÃ  ngÆ°á»i dÃ¹ng nháº­n thÃ´ng bÃ¡o cÃ³ thá»ƒ hÃ nh Ä‘á»™ng.

---

### User Story 3 - Gá»¡ Permission báº±ng checkbox (Priority: P1)

NgÆ°á»i quáº£n trá»‹ bá» check má»™t Action Ä‘á»ƒ gá»¡ Action Ä‘Ã³ khá»i cáº·p Roleâ€“Resource Ä‘ang active.

**Why this priority**: Gá»¡ quyá»n quan trá»ng ngang vá»›i cáº¥p quyá»n vÃ¬ quyá»n dÆ° thá»«a lÃ m tÄƒng rá»§i ro truy cáº­p khÃ´ng phÃ¹ há»£p.

**Independent Test**: Chá»n má»™t Permission Ä‘ang tá»“n táº¡i, bá» check Action, táº£i láº¡i mÃ n hÃ¬nh rá»“i xÃ¡c nháº­n checkbox khÃ´ng cÃ²n Ä‘Æ°á»£c check vÃ  Permission tÆ°Æ¡ng á»©ng khÃ´ng cÃ²n tá»“n táº¡i.

**Acceptance Scenarios**:

1. **Given** Action Ä‘Ã£ Ä‘Æ°á»£c cáº¥p cho cáº·p Roleâ€“Resource active, **When** ngÆ°á»i dÃ¹ng bá» check Action, **Then** Permission tÆ°Æ¡ng á»©ng bá»‹ xÃ³a vÃ  tráº¡ng thÃ¡i chÆ°a check Ä‘Æ°á»£c xÃ¡c nháº­n.
2. **Given** viá»‡c gá»¡ Permission tháº¥t báº¡i, **When** há»‡ thá»‘ng tráº£ lá»—i, **Then** checkbox trá»Ÿ vá» tráº¡ng thÃ¡i check Ä‘Ã£ lÆ°u gáº§n nháº¥t, Permission Ä‘Æ°á»£c giá»¯ nguyÃªn vÃ  ngÆ°á»i dÃ¹ng nháº­n thÃ´ng bÃ¡o cÃ³ thá»ƒ hÃ nh Ä‘á»™ng.
3. **Given** Permission Ä‘Ã£ bá»‹ ngÆ°á»i khÃ¡c thay Ä‘á»•i, **When** ngÆ°á»i dÃ¹ng thao tÃ¡c trÃªn tráº¡ng thÃ¡i cÅ©, **Then** há»‡ thá»‘ng khÃ´ng ghi Ä‘Ã¨ Ã¢m tháº§m vÃ  táº£i láº¡i tráº¡ng thÃ¡i hiá»‡n táº¡i Ä‘á»ƒ ngÆ°á»i dÃ¹ng biáº¿t káº¿t quáº£ thá»±c táº¿.

### Edge Cases

- KhÃ´ng cÃ³ Role: cá»™t Roles hiá»ƒn thá»‹ tráº¡ng thÃ¡i rá»—ng; khÃ´ng cÃ³ Role active vÃ  cá»™t Actions khÃ´ng cho phÃ©p thay Ä‘á»•i.
- KhÃ´ng cÃ³ Resource: cá»™t Resources hiá»ƒn thá»‹ tráº¡ng thÃ¡i rá»—ng; khÃ´ng cÃ³ Resource active vÃ  cá»™t Actions khÃ´ng cho phÃ©p thay Ä‘á»•i.
- KhÃ´ng cÃ³ Action: cá»™t Actions hiá»ƒn thá»‹ tráº¡ng thÃ¡i rá»—ng cho cáº·p Roleâ€“Resource Ä‘ang chá»n.
- Role hoáº·c Resource active bá»‹ xÃ³a hay ngá»«ng kháº£ dá»¥ng trong lÃºc mÃ n hÃ¬nh Ä‘ang má»Ÿ: há»‡ thá»‘ng bá» lá»±a chá»n khÃ´ng cÃ²n há»£p lá»‡, chá»n láº¡i má»¥c kháº£ dá»¥ng Ä‘áº§u tiÃªn náº¿u cÃ³ vÃ  khÃ´ng lÆ°u Permission vá»›i tham chiáº¿u lá»—i thá»i.
- NgÆ°á»i dÃ¹ng Ä‘á»•i Role hoáº·c Resource liÃªn tiáº¿p trong khi dá»¯ liá»‡u Actions Ä‘ang táº£i: chá»‰ káº¿t quáº£ cá»§a lá»±a chá»n má»›i nháº¥t Ä‘Æ°á»£c hiá»ƒn thá»‹; káº¿t quáº£ cÅ© khÃ´ng Ä‘Æ°á»£c ghi Ä‘Ã¨ lÃªn lá»±a chá»n má»›i.
- PhiÃªn Ä‘Äƒng nháº­p háº¿t háº¡n khi Ä‘ang táº£i hoáº·c lÆ°u: thao tÃ¡c tháº¥t báº¡i an toÃ n, tráº¡ng thÃ¡i chÆ°a Ä‘Æ°á»£c xÃ¡c nháº­n khÃ´ng Ä‘Æ°á»£c giá»¯ nhÆ° Ä‘Ã£ lÆ°u vÃ  ngÆ°á»i dÃ¹ng Ä‘Æ°á»£c hÆ°á»›ng dáº«n Ä‘Äƒng nháº­p láº¡i.
- Dá»‹ch vá»¥ táº¡m thá»i khÃ´ng kháº£ dá»¥ng: má»—i cá»™t liÃªn quan hiá»ƒn thá»‹ lá»—i vÃ  kháº£ nÄƒng thá»­ láº¡i; dá»¯ liá»‡u há»£p lá»‡ cÃ²n dÃ¹ng Ä‘Æ°á»£c khÃ´ng bá»‹ xÃ³a khá»i mÃ n hÃ¬nh náº¿u cÃ³ thá»ƒ giá»¯ an toÃ n.
- Hai ngÆ°á»i cÃ¹ng thay Ä‘á»•i má»™t Permission: káº¿t quáº£ hiá»ƒn thá»‹ cuá»‘i cÃ¹ng pháº£i khá»›p vá»›i tráº¡ng thÃ¡i Ä‘Ã£ cam káº¿t, khÃ´ng táº¡o Permission trÃ¹ng vÃ  khÃ´ng bÃ¡o thÃ nh cÃ´ng sai.
- Action Ä‘Æ°á»£c thÃªm hoáº·c xÃ³a trong lÃºc mÃ n hÃ¬nh Ä‘ang má»Ÿ: láº§n táº£i láº¡i tiáº¿p theo pháº£n Ã¡nh danh sÃ¡ch má»›i mÃ  khÃ´ng lÃ m sai tráº¡ng thÃ¡i Permission cá»§a cÃ¡c Action cÃ²n tá»“n táº¡i.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Há»‡ thá»‘ng MUST cung cáº¥p mÃ n hÃ¬nh **Role Permissions** vá»›i bá»‘ cá»¥c chÃ­nh gá»“m Ä‘Ãºng ba cá»™t theo thá»© tá»± Roles, Resources vÃ  Actions.
- **FR-002**: Má»i ngÆ°á»i dÃ¹ng Ä‘Ã£ Ä‘Äƒng nháº­p MUST cÃ³ thá»ƒ xem vÃ  thay Ä‘á»•i Role Permissions mÃ  khÃ´ng cáº§n quyá»n chá»©c nÄƒng riÃªng; ngÆ°á»i chÆ°a Ä‘Äƒng nháº­p MUST bá»‹ tá»« chá»‘i an toÃ n.
- **FR-003**: Cá»™t Roles MUST táº£i dá»¯ liá»‡u Roles vÃ  hiá»ƒn thá»‹ Role Code cho tá»«ng dÃ²ng.
- **FR-004**: Cá»™t Resources MUST táº£i dá»¯ liá»‡u Resources vÃ  hiá»ƒn thá»‹ Application Code cÃ¹ng Resource Code cho tá»«ng dÃ²ng.
- **FR-005**: Cá»™t Actions MUST táº£i toÃ n bá»™ dá»¯ liá»‡u tá»« permission_actions vÃ  hiá»ƒn thá»‹ Code cÃ¹ng má»™t checkbox Action cho tá»«ng dÃ²ng.
- **FR-006**: Táº¡i má»i thá»i Ä‘iá»ƒm, tá»‘i Ä‘a má»™t Role vÃ  má»™t Resource Ä‘Æ°á»£c active; tráº¡ng thÃ¡i active MUST nhÃ¬n tháº¥y rÃµ vÃ  khÃ´ng chá»‰ Ä‘Æ°á»£c truyá»n Ä‘áº¡t báº±ng mÃ u sáº¯c.
- **FR-007**: Khi cÃ³ dá»¯ liá»‡u kháº£ dá»¥ng, mÃ n hÃ¬nh MUST tá»± chá»n Role Ä‘áº§u tiÃªn vÃ  Resource Ä‘áº§u tiÃªn theo thá»© tá»± hiá»ƒn thá»‹ á»•n Ä‘á»‹nh; khi má»™t cá»™t khÃ´ng cÃ³ dá»¯ liá»‡u, Actions MUST khÃ´ng cho phÃ©p thay Ä‘á»•i.
- **FR-008**: Khi Role hoáº·c Resource active thay Ä‘á»•i, há»‡ thá»‘ng MUST táº£i tráº¡ng thÃ¡i Permission cho Ä‘Ãºng cáº·p lá»±a chá»n má»›i vÃ  MUST NOT hiá»ƒn thá»‹ káº¿t quáº£ Ä‘áº¿n muá»™n cá»§a lá»±a chá»n cÅ©.
- **FR-009**: Checkbox cá»§a má»™t Action MUST Ä‘Æ°á»£c check khi Permission cho Ä‘Ãºng Role, Resource vÃ  Action tá»“n táº¡i; MUST khÃ´ng Ä‘Æ°á»£c check khi Permission Ä‘Ã³ khÃ´ng tá»“n táº¡i.
- **FR-010**: Check má»™t Action chÆ°a Ä‘Æ°á»£c cáº¥p MUST táº¡o Ä‘Ãºng má»™t Permission liÃªn káº¿t Role active, Resource active vÃ  Action Ä‘Ã³.
- **FR-011**: Bá» check má»™t Action Ä‘Ã£ Ä‘Æ°á»£c cáº¥p MUST xÃ³a Ä‘Ãºng Permission liÃªn káº¿t Role active, Resource active vÃ  Action Ä‘Ã³.
- **FR-012**: Má»—i thay Ä‘á»•i checkbox MUST Ä‘Æ°á»£c lÆ°u ngay mÃ  khÃ´ng cáº§n nÃºt lÆ°u chung vÃ  MUST cung cáº¥p pháº£n há»“i Ä‘ang xá»­ lÃ½, thÃ nh cÃ´ng hoáº·c tháº¥t báº¡i rÃµ rÃ ng.
- **FR-013**: Trong khi má»™t Action Ä‘ang Ä‘Æ°á»£c lÆ°u, há»‡ thá»‘ng MUST ngÄƒn thao tÃ¡c trÃ¹ng trÃªn Action Ä‘Ã³ nhÆ°ng khÃ´ng cháº·n viá»‡c Ä‘á»c tráº¡ng thÃ¡i cá»§a cÃ¡c Action khÃ¡c.
- **FR-014**: Náº¿u lÆ°u tháº¥t báº¡i, há»‡ thá»‘ng MUST Ä‘Æ°a checkbox vá» tráº¡ng thÃ¡i Ä‘Ã£ cam káº¿t gáº§n nháº¥t, giá»¯ dá»¯ liá»‡u nháº¥t quÃ¡n vÃ  cung cáº¥p kháº£ nÄƒng thá»­ láº¡i.
- **FR-015**: Viá»‡c cáº¥p Permission MUST lÃ  thao tÃ¡c nháº¥t quÃ¡n: khÃ´ng táº¡o báº£n ghi má»™t pháº§n vÃ  khÃ´ng táº¡o nhiá»u Permission cho cÃ¹ng má»™t tá»• há»£p Roleâ€“Resourceâ€“Action.
- **FR-016**: Há»‡ thá»‘ng MUST xá»­ lÃ½ thay Ä‘á»•i Ä‘á»“ng thá»i mÃ  khÃ´ng ghi Ä‘Ã¨ Ã¢m tháº§m, táº£i láº¡i tráº¡ng thÃ¡i hiá»‡n táº¡i khi cÃ³ xung Ä‘á»™t vÃ  thÃ´ng bÃ¡o káº¿t quáº£ rÃµ rÃ ng.
- **FR-017**: Má»—i cá»™t MUST cÃ³ tráº¡ng thÃ¡i táº£i, rá»—ng vÃ  lá»—i phÃ¹ há»£p; lá»—i táº£i láº¡i MUST NOT xÃ³a dá»¯ liá»‡u há»£p lá»‡ Ä‘ang hiá»ƒn thá»‹ náº¿u dá»¯ liá»‡u Ä‘Ã³ cÃ²n dÃ¹ng Ä‘Æ°á»£c an toÃ n.
- **FR-018**: Há»‡ thá»‘ng MUST xÃ¡c thá»±c Role, Resource vÃ  Action váº«n tá»“n táº¡i trÆ°á»›c khi cam káº¿t thay Ä‘á»•i Permission vÃ  MUST tá»« chá»‘i tham chiáº¿u khÃ´ng há»£p lá»‡.
- **FR-019**: CÃ¡c thay Ä‘á»•i Permission thÃ nh cÃ´ng MUST Ä‘Æ°á»£c ghi nháº­n theo cÆ¡ cháº¿ audit hiá»‡n cÃ³, gá»“m tá»‘i thiá»ƒu thá»i Ä‘iá»ƒm vÃ  danh tÃ­nh ngÆ°á»i thao tÃ¡c.
- **FR-020**: Giao diá»‡n MUST há»— trá»£ chuá»™t vÃ  bÃ n phÃ­m, focus nhÃ¬n tháº¥y rÃµ, nhÃ£n checkbox cÃ³ Ã½ nghÄ©a, tráº¡ng thÃ¡i disabled/loading/error rÃµ rÃ ng vÃ  sá»­ dá»¥ng Ä‘Æ°á»£c trÃªn cÃ¡c kÃ­ch thÆ°á»›c mÃ n hÃ¬nh dá»± Ã¡n há»— trá»£.
- **FR-021**: Chá»©c nÄƒng MUST NOT Ä‘á»‹nh nghÄ©a, táº¡o, gÃ¡n hoáº·c kiá»ƒm tra má»™t Permission quáº£n trá»‹ riÃªng Ä‘á»ƒ quyáº¿t Ä‘á»‹nh quyá»n truy cáº­p mÃ n hÃ¬nh Role Permissions.
- **FR-022**: Chá»©c nÄƒng MUST chá»‰ quáº£n lÃ½ liÃªn káº¿t Permission; táº¡o, sá»­a hoáº·c xÃ³a Role, Resource, Application vÃ  Action náº±m ngoÃ i pháº¡m vi.

### Key Entities

- **Role**: Vai trÃ² Ä‘Æ°á»£c chá»n á»Ÿ cá»™t thá»© nháº¥t, nháº­n diá»‡n trÃªn giao diá»‡n báº±ng Role Code vÃ  lÃ  chá»§ thá»ƒ nháº­n Permission.
- **Resource**: TÃ i nguyÃªn Ä‘Æ°á»£c chá»n á»Ÿ cá»™t thá»© hai, nháº­n diá»‡n báº±ng Application Code vÃ  Resource Code, xÃ¡c Ä‘á»‹nh Ä‘á»‘i tÆ°á»£ng mÃ  Permission Ã¡p dá»¥ng.
- **Permission Action**: HÃ nh Ä‘á»™ng cÃ³ thá»ƒ cáº¥p hoáº·c gá»¡, nháº­n diá»‡n báº±ng Code vÃ  hiá»ƒn thá»‹ cÃ¹ng checkbox á»Ÿ cá»™t thá»© ba.
- **Permission**: LiÃªn káº¿t duy nháº¥t giá»¯a má»™t Role, má»™t Resource vÃ  má»™t Permission Action; sá»± tá»“n táº¡i cá»§a liÃªn káº¿t quyáº¿t Ä‘á»‹nh checkbox Ä‘Æ°á»£c check.
- **Authenticated administrator**: NgÆ°á»i dÃ¹ng cÃ³ phiÃªn Ä‘Äƒng nháº­p há»£p lá»‡, khÃ´ng cáº§n Permission riÃªng cá»§a chá»©c nÄƒng Ä‘á»ƒ xem hoáº·c thay Ä‘á»•i cáº¥u hÃ¬nh.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Ãt nháº¥t 95% ngÆ°á»i dÃ¹ng xÃ¡c Ä‘á»‹nh Ä‘Æ°á»£c tráº¡ng thÃ¡i cáº¥p quyá»n cá»§a má»™t cáº·p Roleâ€“Resource trong khÃ´ng quÃ¡ 30 giÃ¢y á»Ÿ láº§n thá»­ Ä‘áº§u tiÃªn.
- **SC-002**: Ãt nháº¥t 95% lÆ°á»£t chá»n Role hoáº·c Resource hiá»ƒn thá»‹ Ä‘Ãºng tráº¡ng thÃ¡i Actions trong khÃ´ng quÃ¡ 2 giÃ¢y dÆ°á»›i táº£i váº­n hÃ nh bÃ¬nh thÆ°á»ng.
- **SC-003**: Ãt nháº¥t 95% thao tÃ¡c check hoáº·c bá» check há»£p lá»‡ Ä‘Æ°á»£c xÃ¡c nháº­n trong khÃ´ng quÃ¡ 2 giÃ¢y dÆ°á»›i táº£i váº­n hÃ nh bÃ¬nh thÆ°á»ng.
- **SC-004**: 100% ká»‹ch báº£n cáº¥p vÃ  gá»¡ quyá»n thÃ nh cÃ´ng váº«n giá»¯ Ä‘Ãºng tráº¡ng thÃ¡i sau khi táº£i láº¡i mÃ n hÃ¬nh.
- **SC-005**: 100% lá»—i lÆ°u vÃ  xung Ä‘á»™t trong cÃ¡c ká»‹ch báº£n cháº¥p nháº­n khÃ´ng táº¡o Permission trÃ¹ng, khÃ´ng lÃ m máº¥t Permission ngoÃ i Ã½ muá»‘n vÃ  khÃ´ng Ä‘á»ƒ checkbox thá»ƒ hiá»‡n sai tráº¡ng thÃ¡i Ä‘Ã£ cam káº¿t.
- **SC-006**: 100% ngÆ°á»i dÃ¹ng Ä‘Ã£ Ä‘Äƒng nháº­p cÃ³ thá»ƒ dÃ¹ng chá»©c nÄƒng mÃ  khÃ´ng cáº§n Permission quáº£n trá»‹ riÃªng; 100% ngÆ°á»i chÆ°a Ä‘Äƒng nháº­p bá»‹ tá»« chá»‘i an toÃ n.
- **SC-007**: ToÃ n bá»™ luá»“ng chá»n Role, chá»n Resource, cáº¥p vÃ  gá»¡ Action cÃ³ thá»ƒ hoÃ n thÃ nh chá»‰ báº±ng bÃ n phÃ­m vá»›i tráº¡ng thÃ¡i focus vÃ  active nháº­n biáº¿t Ä‘Æ°á»£c.
- **SC-008**: Trong kiá»ƒm thá»­ vá»›i tá»‘i Ä‘a 1.000 Roles, 10.000 Resources vÃ  100 Actions, ngÆ°á»i dÃ¹ng váº«n cÃ³ thá»ƒ chá»n Ä‘á»‘i tÆ°á»£ng vÃ  hoÃ n thÃ nh thay Ä‘á»•i Permission mÃ  khÃ´ng gáº·p lá»—i dá»¯ liá»‡u hoáº·c tráº¡ng thÃ¡i giao diá»‡n sai.

## Assumptions

## Update 2026-08-13: Application-scoped selection

### User Story 4 - Select Resources within the Role Application (Priority: P1)

An administrator can identify each Role's Application in column 1. After selecting a Role, column 2 lists only active Resources that belong to that Role's Application.

**Independent Test**: Select Roles from two different Applications and verify that the displayed Application changes with the Role, the previous Resource selection is cleared, and every newly loaded Resource belongs to the newly selected Role's Application.

**Acceptance Scenarios**:

1. **Given** Roles from multiple Applications, **When** column 1 is displayed, **Then** every Role shows its Role Code, Application Code, and Application Name.
2. **Given** a Role in Application A, **When** it is selected, **Then** column 2 requests and displays only active Resources in Application A and selects the first result when available.
3. **Given** a selected Role in Application A, **When** the user selects a Role in Application B, **Then** the old Resource and Actions are cleared before Resources in Application B are loaded.
4. **Given** a direct request combining a Role and Resource from different Applications, **When** permissions are read, granted, or revoked, **Then** the request is rejected without changing permission data.

### Additional Functional Requirements

- **FR-023**: Column 1 MUST display Role Code, Application Code, and Application Name for every Role.
- **FR-024**: Column 2 MUST load Resources using the selected Role's Application as a mandatory filter.
- **FR-025**: Changing Role MUST clear the previous Resource selection and Action state before loading the new Application scope.
- **FR-026**: Stale Resource results from a previously selected Role MUST NOT replace the current Role's Resource list.
- **FR-027**: The system MUST reject reads, grants, and revocations where Role and Resource belong to different Applications.

### Additional Success Criteria

- **SC-009**: 100% of displayed Roles identify their Application using both code and name.
- **SC-010**: 100% of Resource lists contain only Resources from the selected Role's Application.
- **SC-011**: 100% of cross-Application Role–Resource permission requests are rejected without data changes.

### Additional Assumptions

- Each Role and Resource belongs to exactly one Application.
- Existing Role responses already expose Application identity, and existing Resource search supports Application filtering.

- â€œKhÃ´ng yÃªu cáº§u quyá»nâ€ nghÄ©a lÃ  khÃ´ng kiá»ƒm tra Permission riÃªng cá»§a chá»©c nÄƒng; yÃªu cáº§u Ä‘Äƒng nháº­p hiá»‡n cÃ³ váº«n Ä‘Æ°á»£c giá»¯ theo constitution.
- MÃ n hÃ¬nh dÃ¹ng tÃªn **Role Permissions** vÃ  Ä‘á»‹a chá»‰ `/role-permissions` theo quy Æ°á»›c tÃªn chá»©c nÄƒng hiá»‡n cÃ³.
- Má»—i thao tÃ¡c checkbox Ä‘Æ°á»£c lÆ°u ngay; khÃ´ng cÃ³ nÃºt lÆ°u hÃ ng loáº¡t hay tráº¡ng thÃ¡i chá»‰nh sá»­a chÆ°a cam káº¿t kÃ©o dÃ i.
- Khi má»Ÿ mÃ n hÃ¬nh, Role vÃ  Resource kháº£ dá»¥ng Ä‘áº§u tiÃªn theo thá»© tá»± hiá»ƒn thá»‹ á»•n Ä‘á»‹nh Ä‘Æ°á»£c chá»n máº·c Ä‘á»‹nh Ä‘á»ƒ táº£i Actions ngay.
- Táº¥t cáº£ Actions trong permission_actions Ã¡p dá»¥ng Ä‘Æ°á»£c cho má»i Resource; Permission hiá»‡n cÃ³ quyáº¿t Ä‘á»‹nh tráº¡ng thÃ¡i check, khÃ´ng cÃ³ quy táº¯c lá»c Actions bá»• sung theo Resource.
- Roles, Resources vÃ  Actions Ä‘Ã£ cÃ³ cÆ¡ cháº¿ xÃ¡c Ä‘á»‹nh báº£n ghi kháº£ dá»¥ng; chá»©c nÄƒng nÃ y khÃ´ng thay Ä‘á»•i vÃ²ng Ä‘á»i cá»§a cÃ¡c thá»±c thá»ƒ Ä‘Ã³.
- Permission Ä‘Æ°á»£c nháº­n diá»‡n duy nháº¥t bá»Ÿi tá»• há»£p Role, Resource vÃ  Action; thao tÃ¡c bá» check xÃ³a liÃªn káº¿t tÆ°Æ¡ng á»©ng thay vÃ¬ Ä‘Ã¡nh dáº¥u khÃ´ng hoáº¡t Ä‘á»™ng.
- Danh sÃ¡ch lá»›n sá»­ dá»¥ng cÆ¡ cháº¿ duyá»‡t hoáº·c tÃ¬m kiáº¿m phÃ¹ há»£p theo chuáº©n chung cá»§a dá»± Ã¡n; chi tiáº¿t tÆ°Æ¡ng tÃ¡c Ä‘Æ°á»£c quyáº¿t Ä‘á»‹nh á»Ÿ bÆ°á»›c láº­p káº¿ hoáº¡ch mÃ  khÃ´ng thay Ä‘á»•i luá»“ng ba cá»™t.
