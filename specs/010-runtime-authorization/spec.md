# Feature Specification: Ngữ cảnh phân quyền runtime

**Feature Branch**: `010-runtime-authorization`

**Created**: 2026-08-20

**Status**: Implemented

**Input**: Cung cấp Roles, Permissions và menu khả dụng cho User đang đăng nhập.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Tải quyền và điều hướng cá nhân hóa (Priority: P1)

Người dùng đã đăng nhập tải Roles, Permissions và cây menu phù hợp với quyền hiện hành.

**Why this priority**: Client cần một nguồn nhất quán để hiển thị điều hướng và kiểm soát thao tác.

**Independent Test**: Chuẩn bị User với quyền và menu đã biết, gọi ngữ cảnh phân quyền rồi xác nhận kết quả chỉ chứa nội dung khả dụng.

**Acceptance Scenarios**:

1. **Given** access token hợp lệ, **When** tải authorization context, **Then** hệ thống trả Roles, Permissions và cây menu của Application hiện hành.
2. **Given** menu yêu cầu quyền mà User không có, **When** tạo cây menu, **Then** menu đó không được trả về trừ khi có hậu duệ hợp lệ cần giữ cấu trúc.
3. **Given** token có phiên bản quyền cũ, **When** yêu cầu context, **Then** hệ thống từ chối và yêu cầu phiên mới.

### Edge Cases

- Application cấu hình không tồn tại hoặc không hoạt động.
- Menu cha không hiển thị nhưng có menu con được phép.
- Menu hoặc Permission thay đổi trong thời gian cache còn hiệu lực.
- Claim User hoặc phiên bản quyền thiếu/sai định dạng.
- Cây menu có nhiều cấp hoặc nhánh không hoạt động.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Endpoint authorization context MUST yêu cầu access token hợp lệ.
- **FR-002**: Hệ thống MUST xác định User và phiên bản quyền từ danh tính đã xác thực, từ chối claim thiếu hoặc không hợp lệ.
- **FR-003**: Kết quả MUST chứa danh sách Role codes, Permission codes và cây menu khả dụng.
- **FR-004**: Roles và Permissions MUST thuộc Application hiện hành và phản ánh dữ liệu đã cam kết mới nhất trong giới hạn cache công bố.
- **FR-005**: Chỉ menu hoạt động MUST được xem xét trong kết quả.
- **FR-006**: Menu nghiệp vụ MUST yêu cầu Permission `ResourceCode.ViewMenu` tương ứng.
- **FR-007**: Menu nhóm không trực tiếp hiển thị MUST có thể giữ lại khi chứa hậu duệ hợp lệ; cấu trúc con MUST được lọc đệ quy.
- **FR-008**: Khi không tìm thấy Application hiện hành, hệ thống MUST trả Roles và Permissions hợp lệ cùng cây menu rỗng thay vì dữ liệu Application khác.
- **FR-009**: Thay đổi Role hoặc Permission MUST làm phiên bản quyền của User bị ảnh hưởng tăng lên để token cũ bị từ chối.
- **FR-010**: Cache MUST được phân tách theo User và phiên bản quyền để không dùng nhầm dữ liệu giữa Users hoặc giữa hai phiên bản.
- **FR-011**: Phản hồi MUST không chứa dữ liệu User khác, token hoặc chi tiết lưu trữ.

### Key Entities

- **Authorization Context**: Roles, Permissions và menu hiệu lực của User.
- **Permission Version**: Phiên bản dùng để phát hiện token mang quyền lỗi thời.
- **Menu**: Nút điều hướng có trạng thái, quan hệ cha–con và Resource tùy chọn.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% menu nghiệp vụ trả về có quyền xem tương ứng hoặc chứa hậu duệ hợp lệ.
- **SC-002**: 100% token có phiên bản quyền cũ bị từ chối ở yêu cầu tiếp theo.
- **SC-003**: Không có dữ liệu authorization của User khác xuất hiện trong kiểm thử đồng thời và cache.
- **SC-004**: Người dùng nhận context trong dưới 1 giây ở điều kiện hoạt động bình thường.

## Assumptions

- Application hiện hành được xác định từ cấu hình tin cậy của API.
- Client dùng context để điều hướng nhưng API vẫn là lớp thực thi quyền cuối cùng.
- Quản lý Menu và Role Permissions thuộc các feature riêng.
