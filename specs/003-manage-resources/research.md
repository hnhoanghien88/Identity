# Research: Quản lý Resources

## Decision 1: Mirror CQRS vertical slices của Applications

**Decision**: Dùng MediatR commands/queries; EF Core repository cho writes và Dapper repository cho read projections.

**Rationale**: Đây là kiến trúc hiện hữu, giữ đúng dependency direction và cung cấp CQRS rõ ràng mà không thêm thư viện.

**Alternatives considered**:
- EF Core cho cả reads/writes: không khớp pattern CQRS hiện tại.
- Persistence trực tiếp trong controller: vi phạm layer boundaries.

## Decision 2: Thêm optimistic concurrency Version

**Decision**: Thêm unsigned `Version` mặc định 1, cấu hình concurrency token, expose trong DTO và bắt buộc cho update/delete.

**Rationale**: Entity Resources hiện chưa có token trong khi FR-019 cấm silent overwrite. Pattern Applications/Users đã chứng minh cách triển khai phù hợp.

**Alternatives considered**:
- Last-write-wins: vi phạm spec.
- Chỉ so sánh Version trong handler: còn race giữa check và save.
- Timestamp: kém rõ và không nhất quán với project.

## Decision 3: Application selector và validation server-side

**Decision**: Resource reads trả ApplicationId/Code/Name; create/update nhận ApplicationId và server xác nhận Application active, non-deleted trong write transaction.

**Rationale**: Client cần tên dễ hiểu, còn server phải chống stale/tampered selection và direct API calls.

**Alternatives considered**:
- Chỉ kiểm tra FK: chấp nhận Application inactive.
- Gửi toàn bộ Application object: mở rộng contract không cần thiết.

## Decision 4: Unique Code theo Application, case-insensitive

**Decision**: Trim Code và enforce case-insensitive uniqueness cho `(ApplicationId, Code)`, kể cả soft-deleted rows; map unique violation về field `code`.

**Rationale**: Khớp spec và unique index hiện có, cho phép cùng Code ở Application khác nhưng không tạo ambiguity trong một scope.

**Alternatives considered**:
- Global uniqueness: chặt hơn yêu cầu.
- Cho reuse sau delete: cần filtered-index strategy khác và gây ambiguity.
- Dựa vào default collation: không nhất quán giữa environments.

## Decision 5: Soft-delete với explicit Permission/Menu protection

**Decision**: Delete kiểm tra Permissions và Menus tham chiếu trong cùng transaction trước khi set IsDeleted/IsActive/Version/audit; conflict trả 409.

**Rationale**: Soft-delete là UPDATE nên FK restrict không tự bảo vệ. Explicit check đáp ứng FR-018 mà không cascade ngoài phạm vi.

**Alternatives considered**:
- Physical delete: mất lịch sử.
- Cascade: thay đổi permission/menu ngoài scope.
- Soft-delete không check: để lại authorization/navigation configuration trỏ vào tài nguyên không dùng được.

## Decision 6: Bốn named permission policies

**Decision**: Dùng `Resources.View/Create/Update/Delete` từ JWT permission claims.

**Rationale**: Quyền độc lập đáp ứng spec và bảo vệ direct API; client-side checks chỉ phục vụ UX.

**Alternatives considered**:
- Chỉ `[Authorize]`: không phân tách capability.
- Role names cứng: không biểu diễn quyền chi tiết.
- Client-only: không an toàn.

## Decision 7: HTTP contract mirror Applications

**Decision**: Dùng `/api/resources/search`, `/api/resources`, `/api/resources/{id}`, PUT và DELETE với Version; success wrapper và Problem Details theo project.

**Rationale**: Nhất quán giúp tái sử dụng handling, testing và giảm review risk.

**Alternatives considered**:
- GET search query string: không khớp structured filtering hiện có.
- PATCH riêng cho active/application: tăng endpoints không cần thiết.
- 204 delete: khác convention hiện tại.

## Decision 8: Không thêm libraries

**Decision**: Dùng tooling .NET/Dapper/EF/MediatR/FluentValidation và React/MUI/Vitest hiện có.

**Rationale**: Các capability hiện tại đủ cho một CRUD screen và tránh license/vulnerability/maintenance mới.

**Alternatives considered**: Data-grid hoặc server-state library không cần thiết khi Applications đã có pattern tương đương.
