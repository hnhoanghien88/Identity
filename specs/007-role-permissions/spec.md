# Feature Specification: Phân quyền theo Role

**Feature Branch**: `007-role-permissions`

**Created**: 2026-08-12

**Updated**: 2026-08-20

**Status**: Implemented

**Input**: Quản trị Permission theo tổ hợp Role, Resource và Action trong phạm vi cùng Application.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Xem quyền của Role theo Application (Priority: P1)

Người quản trị có quyền đọc chọn một Role, xem các Resources thuộc Application của Role và trạng thái từng Action đã được cấp.

**Why this priority**: Người quản trị phải thấy chính xác trạng thái hiện hành trước khi thay đổi quyền truy cập.

**Independent Test**: Chuẩn bị Role, Resources và Permissions đã biết; chọn Role và Resource rồi xác nhận các checkbox phản ánh đúng dữ liệu đã lưu.

**Acceptance Scenarios**:

1. **Given** người dùng có `RolePermissions.Read`, **When** mở màn hình, **Then** hệ thống hiển thị Roles, Resources đúng Application và Actions của Resource được chọn.
2. **Given** người dùng thiếu `RolePermissions.Read`, **When** yêu cầu dữ liệu, **Then** hệ thống từ chối an toàn.
3. **Given** người dùng đổi Role, **When** dữ liệu mới đang tải, **Then** lựa chọn Resource và trạng thái Action cũ được xóa và kết quả đến muộn không ghi đè lựa chọn mới.

---

### User Story 2 - Cấp Permission (Priority: P1)

Người quản trị có quyền cập nhật chọn một Action chưa được cấp để tạo Permission cho Role và Resource hiện hành.

**Why this priority**: Cấp quyền là giá trị nghiệp vụ cốt lõi của tính năng.

**Independent Test**: Cấp một Action chưa có Permission và xác nhận đúng một liên kết được tạo, checkbox giữ trạng thái đã chọn sau khi tải lại.

**Acceptance Scenarios**:

1. **Given** Role, Resource và Action hợp lệ cùng Application, **When** người có `RolePermissions.Update` cấp quyền, **Then** đúng một Permission được tạo.
2. **Given** Permission đã tồn tại, **When** yêu cầu cấp lại, **Then** hệ thống không tạo bản ghi trùng.

---

### User Story 3 - Thu hồi Permission (Priority: P1)

Người quản trị có quyền xóa bỏ chọn một Action đã cấp để thu hồi Permission tương ứng.

**Why this priority**: Thu hồi kịp thời là điều kiện bắt buộc để kiểm soát truy cập an toàn.

**Independent Test**: Thu hồi một Permission hiện có và xác nhận chỉ liên kết mục tiêu bị xóa.

**Acceptance Scenarios**:

1. **Given** Permission tồn tại, **When** người có `RolePermissions.Delete` thu hồi, **Then** đúng Permission mục tiêu bị xóa.
2. **Given** người dùng thiếu quyền xóa, **When** yêu cầu thu hồi, **Then** dữ liệu không thay đổi.

### Edge Cases

- Role hoặc Resource bị xóa/vô hiệu hóa trong khi màn hình đang mở.
- Role và Resource thuộc hai Applications khác nhau.
- Hai quản trị viên đồng thời cấp hoặc thu hồi cùng Permission.
- Kết quả tải cũ trả về sau khi người dùng đã đổi Role hoặc Resource.
- Một cột không có dữ liệu hoặc dịch vụ tạm thời không khả dụng.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Hệ thống MUST yêu cầu `RolePermissions.Read` để đọc, `RolePermissions.Update` để cấp và `RolePermissions.Delete` để thu hồi Permission.
- **FR-002**: Màn hình MUST hiển thị ba khu vực Roles, Resources và Actions với trạng thái lựa chọn rõ ràng, không chỉ dựa vào màu sắc.
- **FR-003**: Mỗi Role MUST hiển thị Code cùng Application Code và Name để tránh chọn nhầm phạm vi.
- **FR-004**: Resources MUST chỉ được tải từ Application của Role đang chọn.
- **FR-005**: Actions MUST phản ánh Permission của đúng tổ hợp Role–Resource hiện hành.
- **FR-006**: Thay đổi Role MUST xóa lựa chọn Resource và Action cũ trước khi tải dữ liệu mới.
- **FR-007**: Kết quả đến muộn của lựa chọn cũ MUST NOT thay thế trạng thái lựa chọn hiện hành.
- **FR-008**: Hệ thống MUST từ chối đọc, cấp hoặc thu hồi khi Role và Resource khác Application.
- **FR-009**: Cấp quyền MUST tạo tối đa một Permission cho mỗi tổ hợp Role–Resource–Action.
- **FR-010**: Thu hồi MUST chỉ xóa Permission của đúng tổ hợp được yêu cầu.
- **FR-011**: Hệ thống MUST xác nhận Role, Resource và Action còn tồn tại trước khi thay đổi.
- **FR-012**: Mỗi thay đổi MUST có trạng thái đang xử lý, thành công hoặc thất bại rõ ràng và ngăn gửi trùng trên cùng Action.
- **FR-013**: Khi thay đổi thất bại, checkbox MUST trở về trạng thái đã cam kết gần nhất và cho phép thử lại.
- **FR-014**: Thay đổi thành công MUST cập nhật phiên bản quyền của Users bị ảnh hưởng để token cũ không tiếp tục mang quyền lỗi thời.
- **FR-015**: Thay đổi thành công MUST ghi nhận thời điểm và danh tính người thao tác.
- **FR-016**: Giao diện MUST dùng được bằng bàn phím, có focus nhìn thấy, nhãn có ý nghĩa và trạng thái tải/rỗng/lỗi.

### Key Entities

- **Role**: Nhóm quyền thuộc một Application.
- **Resource**: Đối tượng được bảo vệ thuộc một Application.
- **Action**: Loại thao tác có thể được cấp trên Resource.
- **Permission**: Liên kết duy nhất giữa Role, Resource và Action.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% Resources hiển thị thuộc đúng Application của Role đã chọn.
- **SC-002**: 100% yêu cầu khác Application hoặc thiếu quyền bị từ chối mà không thay đổi dữ liệu.
- **SC-003**: Không xuất hiện Permission trùng trong kiểm thử đồng thời.
- **SC-004**: Trạng thái checkbox phản ánh dữ liệu đã cam kết trong vòng 2 giây sau thao tác thành công ở điều kiện bình thường.
- **SC-005**: 100% thay đổi quyền làm phiên mang phiên bản quyền cũ bị từ chối ở lần sử dụng tiếp theo.

## Assumptions

- Roles, Resources và Actions được quản lý bởi các tính năng riêng.
- Một Role và Resource chỉ có thể liên kết Permission khi cùng Application.
- Người vận hành đã được cấp quyền quản trị Role Permissions phù hợp.
