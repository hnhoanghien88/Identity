# Research: Quản lý Actions

## Decision 1: Mirror CQRS vertical slices của Applications

**Decision**: MediatR commands/queries, EF Core writes và Dapper reads.

**Rationale**: Khớp kiến trúc hiện hữu, layer boundaries và không thêm dependency.

**Alternatives considered**: EF cho cả reads/writes; persistence trong controller.

## Decision 2: Thêm optimistic concurrency Version

**Decision**: Thêm unsigned `Version` mặc định 1, concurrency token, bắt buộc update/delete.

**Rationale**: Entity chưa có token trong khi spec cấm silent overwrite; Applications đã có pattern phù hợp.

**Alternatives considered**: Last-write-wins; timestamp; check-only ngoài database.

## Decision 3: Physical delete có dependency protection

**Decision**: Kiểm tra Permission trước khi xóa vật lý; map dependency/FK race thành conflict.

**Rationale**: Bảng không có delete/status fields và Permission phải được bảo toàn.

**Alternatives considered**: Thêm soft-delete; cascade Permission; không cho xóa.

## Decision 4: Authenticated-only, không Actions policies

**Decision**: Mọi endpoint dùng authentication chung, không tạo/check claims Actions.*; menu hiện với session hợp lệ.

**Rationale**: Thực hiện “không cần phân quyền” nhưng giữ fail-closed trust boundary.

**Alternatives considered**: Anonymous; bốn policies; client-only gating.

## Decision 5: Contract mirror Applications

**Decision**: `/api/actions/search`, collection/detail, PUT và DELETE với Version; ApiResponse/Problem Details.

**Rationale**: Nhất quán patterns và giảm rủi ro.

**Alternatives considered**: GET-only search; PATCH; 204 delete.

## Decision 6: Không thêm libraries

**Decision**: Dùng tooling .NET/React hiện có.

**Rationale**: Đủ capability và tránh maintenance/security surface mới.

**Alternatives considered**: Data-grid hoặc server-state library mới.

