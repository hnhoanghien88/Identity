# Data Model: Quản lý Menus dạng cây

## Menu

| Field | Type / optionality | Rules |
|---|---|---|
| Id | unsigned identifier | Generated, immutable |
| ApplicationId | required identifier | Existing, active, not deleted; editable only while the Menu has no active children |
| ParentId | optional identifier | Existing Menu in same Application; not self/descendant; null means root |
| ResourceId | optional identifier | Existing active Resource in same Application |
| Code | required string, max 120 | Trimmed; unique case-insensitive per Application |
| Name | required string, max 150 | Trimmed, non-empty |
| Route | optional string, max 300 | Trimmed; blank normalized to null |
Resource và Route giữ giá trị `null` khi không được thiết lập. Ký hiệu `—` trên màn hình index chỉ là presentation state và không thuộc dữ liệu Menu.
| Icon | optional string, max 100 | Trimmed; blank normalized to null |
| SortOrder | integer | Default 0; siblings sort ascending |
| IsVisible | boolean | Default true |
| IsActive | boolean | Default true |
| IsDeleted | boolean | Default false; soft delete only |
| Version | unsigned integer | Default 1; increment once per successful update |
| CreatedBy/CreatedDate | audit | Set on create |
| UpdatedBy/UpdatedDate | audit | Set on update/delete |

## Relationships and invariants

- Application 1—N Menu; every hierarchy is isolated within one Application.
- Menu 0..1—N Menu through ParentId; the graph MUST be acyclic and each node has at most one parent.
- Resource 0..1—N Menu; Resource and Menu MUST share ApplicationId.
- Active tree reads exclude `IsDeleted=true`; parent candidates additionally exclude the edited node and descendants.
- A Menu with a non-deleted direct child cannot transition to deleted.

## State transitions

```text
Create -> Active/Visible defaults
Active <-> Inactive
Visible <-> Hidden
Leaf -> Soft Deleted
Parent A -> Parent B/root (same Application, acyclic)
Application A -> Application B (leaf Menu only; Parent/Resource revalidated in B)
```

`ApplicationId` may transition for a leaf Menu. The transition clears or replaces incompatible Parent/Resource choices and is rejected if the Menu has active children. Soft-deleted records are not restored in this feature.
