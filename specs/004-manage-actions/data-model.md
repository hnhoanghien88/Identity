# Data Model: Quản lý Actions

## Action

Aggregate hiện có trong `permission_actions`; feature thêm `Version` và làm rõ uniqueness Code.

| Field | Type | Required | Rules / meaning |
|---|---|---:|---|
| Id | unsigned 64-bit integer | yes | Database-generated, immutable. |
| Code | string(50) | yes | Trimmed; unique case-insensitively. |
| Name | string(100) | yes | Trimmed; non-empty. |
| Version | unsigned 64-bit integer | yes | Default 1; concurrency token incremented per update. |
| CreatedBy | string, nullable | no | Authenticated actor. |
| CreatedDate | UTC datetime | yes | Server-stamped on create. |
| UpdatedBy | string, nullable | no | Server-stamped on update. |
| UpdatedDate | UTC datetime, nullable | no | Server-stamped on update. |

### Invariants and transitions

- Code/Name không whitespace; Code ≤ 50, Name ≤ 100; Code unique case-insensitive.
- Update/delete yêu cầu matching Version; Action có Permission không được xóa.
- Create → Existing (Version 1); update tăng Version/audit; delete xóa vật lý khi không dependency; conflict giữ nguyên.

## Permission dependency

`permissions.ActionId` bắt buộc tham chiếu Action. Bất kỳ row nào tồn tại đều chặn delete; feature không cascade hoặc chuyển Permission.

## Read models

`ActionDto`: Id, Code, Name, CreatedDate, Version. `PagedActionsDto`: Items, TotalCount, Page, PageSize.

Filters Code/Name hỗ trợ contains/startsWith/endsWith/exact values. Sort allow-list: Id, Code, Name, CreatedDate; default CreatedDate DESC, Id DESC. Page mặc định 1, PageSize 20, tối đa 100.

## Migration and recovery

Add Version default 1/concurrency token; precheck case-only duplicate trước explicit case-insensitive collation; validate Permission FKs. Rollback cần backup và coordinated application rollback; physical deletes chỉ phục hồi từ backup.

