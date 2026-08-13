# Implementation Plan: Quản lý User theo Role

**Branch**: `008-user-roles` | **Date**: 2026-08-12 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/008-user-roles/spec.md`

## Summary

Bổ sung màn hình `/user-roles` gồm hai cột Roles và Users thuộc Role active. Backend cung cấp truy vấn thành viên, truy vấn ứng viên phân trang, thêm nhiều idempotent trong transaction và gỡ idempotent. Tái sử dụng `user_roles` cùng unique key `(UserId, RoleId)`, chỉ chấp nhận Role/User khả dụng, yêu cầu đăng nhập nhưng không policy riêng. Frontend React/MUI tự chọn Role đầu, hủy request lỗi thời, dialog checkbox nhiều User và xác nhận trước khi gỡ.

## Technical Context

**Language/Version**: C# / .NET 10; JavaScript ES modules / React 19  
**Primary Dependencies**: ASP.NET Core, MediatR, EF Core/MySQL, Dapper; React, Vite, Material UI  
**Storage**: MySQL hiện có với `users`, `roles`, `user_roles`; không cần migration  
**Testing**: xUnit; Vitest + Testing Library; oxlint, Prettier, production build  
**Target Platform**: ASP.NET Core API và trình duyệt hiện đại  
**Project Type**: Layered web API và React SPA  
**Performance Goals**: 95% lượt đổi Role/tải thành viên trong 2 giây; thêm ít nhất 20 Users/lần lưu  
**Constraints**: Authenticated, không policy riêng, fail closed, SQL tham số hóa, writes idempotent, không dependency mới  
**Scale/Scope**: Một màn hình, bốn endpoint, Roles tải tối đa 100 và Users phân trang 20 mục

## Constitution Check

*GATE: Passed before research and re-checked after design.*

| Principle / gate | Evidence | Status |
|---|---|---|
| Security and identity | Controller yêu cầu đăng nhập; Role/User phải active, chưa xóa; lỗi fail closed. | PASS |
| Layer boundaries | Use cases ở Application, persistence ở Infrastructure, HTTP ở API, UI/API client tách biệt. | PASS |
| Explicit contracts | OpenAPI định nghĩa request, response, authentication và lỗi cho bốn endpoint. | PASS |
| Tests are gates | Application, API, persistence và frontend tests bao phủ đọc/thêm/xóa, lỗi/concurrency. | PASS |
| Accessible UX | MUI, keyboard/focus, dialog labels, loading/error/disabled và xác nhận xóa rõ ràng. | PASS |
| Readable formatting | Chạy Prettier/oxlint, .NET formatter và review JSX nhiều dòng. | PASS |
| Persistence consistency | Transaction cho batch add; unique key chống trùng; delete đúng membership. | PASS |
| Simplicity | Tái sử dụng schema, stack, paging và conventions; không dependency/migration mới. | PASS |

### Post-design re-check

Data model giữ nguyên schema và ràng buộc. Contract yêu cầu bearer token nhưng không permission claim. Batch add chạy transaction, bỏ qua membership đã có theo ngữ nghĩa idempotent và validation ngăn ghi một phần. Quickstart bao phủ authentication, concurrency, xác nhận xóa, rollback UI và keyboard. Không có vi phạm constitution cần ngoại lệ.

## Project Structure

### Documentation (this feature)

```text
specs/008-user-roles/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/user-roles.openapi.yaml
└── tasks.md
```

### Source Code (repository root)

```text
Identity-api/
├── src/Identity.Application/Abstractions/Persistence/IUserRolesRepository.cs
├── src/Identity.Application/UserRoles/
├── src/Identity.Infrastructure/Persistence/MySqlUserRolesRepository.cs
├── src/Identity.Api/Controllers/UserRolesController.cs
└── tests/*/UserRoles/

Identity-client/
├── src/features/userRoles/
├── src/App.jsx
├── src/App.css
└── tests/userRoles/
```

**Structure Decision**: Giữ bốn layer backend và feature folder frontend. Repository management tách khỏi `IUserRolesReadRepository` đang phục vụ token authorization.

## Implementation Design

- `GET /api/user-roles?roleId=&page=&pageSize=` trả thành viên theo User Code.
- `GET /api/user-roles/candidates?roleId=&search=&page=&pageSize=` trả Users active, chưa xóa/chưa thuộc Role.
- `POST /api/user-roles/{roleId}` nhận tối đa 100 `userIds`, kiểm tra tham chiếu rồi thêm phần còn thiếu trong một transaction; trả riêng IDs đã có.
- `DELETE /api/user-roles/{roleId}/{userId}` gỡ idempotent, không sửa/xóa User.
- Query dùng no-tracking projection; write repository dùng transaction và ánh xạ concurrency thành conflict an toàn.
- UI tải Roles qua API hiện có; AbortController và generation token ngăn response cũ ghi đè.
- Dialog có search/paging, checkbox nhiều trang, Save disabled khi rỗng/pending, giữ lựa chọn nếu lỗi và reload sau thành công.
- Delete dialog nêu User/Role; UI chỉ bỏ dòng sau server success.
- Route/menu chỉ phụ thuộc phiên đăng nhập, không capability/policy riêng.

## Complexity Tracking

Không có vi phạm constitution cần biện minh.
