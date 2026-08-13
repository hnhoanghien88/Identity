# Research: Quản lý Menus dạng cây

## Tree query and assembly

- **Decision**: Đọc một tập Menu phẳng theo Application bằng Dapper rồi dựng cây O(n) trong Application layer.
- **Rationale**: Phù hợp CQRS hiện có, cho thứ tự ổn định, dễ phát hiện orphan/cycle và tránh phụ thuộc cú pháp recursive CTE đặc thù.
- **Alternatives considered**: Recursive CTE giảm số row trả về khi lazy-load nhưng tăng độ phức tạp contract và database coupling; N+1 query bị loại vì không đạt quy mô.

## Cycle prevention

- **Decision**: Khi update Parent, repository duyệt chuỗi ancestors từ Parent dự kiến trong cùng Application và từ chối nếu gặp chính Menu; UI cũng loại self/descendants để phản hồi sớm.
- **Rationale**: Server-side check là ranh giới tin cậy; UI guard cải thiện trải nghiệm nhưng không thay thế validation.
- **Alternatives considered**: Chỉ kiểm tra client không an toàn; materialized path/nested sets yêu cầu thay đổi schema lớn không cần thiết.

## Application and Resource lookup

- **Decision**: Tái sử dụng endpoints Applications/Resources hiện có với filter active và ApplicationId; Menus API không sao chép danh mục lookup.
- **Rationale**: Dữ liệu luôn động, contract ownership rõ và tránh duplicate query surface.
- **Alternatives considered**: Endpoint form-metadata tổng hợp làm coupling cao; hard-coded options vi phạm yêu cầu.

## Concurrency and delete

- **Decision**: Thêm `Version` vào `menus`; update/delete compare-and-swap. Delete mềm chỉ cho leaf chưa xóa.
- **Rationale**: Đồng nhất với Applications/Resources và ngăn stale form ghi đè. Leaf-only giữ cấu trúc không mồ côi.
- **Alternatives considered**: Last-write-wins không đáp ứng spec; cascade delete cả nhánh ngoài scope; physical delete làm mất audit.

## Authorization

- **Decision**: Thêm bốn capabilities `Menus.View/Create/Update/Delete`, mapping policy và kiểm tra ở API/client như Resources.
- **Rationale**: Feature quản trị identity phải fail closed và spec yêu cầu thao tác theo quyền.
- **Alternatives considered**: Authenticated-only không đủ least privilege; role hard-code khó mở rộng.

## Frontend tree

- **Decision**: Dùng MUI primitives sẵn có để render tree table đệ quy, quản lý expanded IDs cục bộ và cung cấp `aria-expanded`, indentation, expand/collapse all.
- **Rationale**: Không thêm dependency, giữ design system và đáp ứng keyboard/accessibility.
- **Alternatives considered**: Thư viện tree-grid mới tăng supply-chain và styling cost; bảng phẳng không đáp ứng yêu cầu.
## Null display and edit safety

- **Decision**: Giữ `ResourceId`, `resourceName` và `Route` là giá trị trống trong model/form; chỉ render ký hiệu Unicode `—` tại ô bảng khi giá trị không được thiết lập.
- **Rationale**: Tách dữ liệu nghiệp vụ khỏi ký hiệu trình bày, tránh mojibake và bảo đảm callback Edit nhận đúng Menu gốc thay vì chuỗi thay thế.
- **Alternatives considered**: Chuẩn hóa thành chuỗi `—` tại API hoặc khi nạp form bị loại vì làm sai ngữ nghĩa `null` và có nguy cơ lưu ký hiệu trình bày vào database; để ô trống hoàn toàn khó nhận biết hơn cho người dùng.