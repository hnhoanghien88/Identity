# Feature Specification: Xác thực và quản lý phiên

**Feature Branch**: `009-session-authentication`

**Created**: 2026-08-20

**Status**: Implemented

**Input**: Đặc tả các luồng login, refresh và logout của Identity API.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Đăng nhập an toàn bằng Code (Priority: P1)

Người dùng nhập Code và mật khẩu để nhận phiên truy cập phản ánh vai trò và quyền hiện hành.

**Why this priority**: Đây là cửa ngõ của toàn bộ chức năng được bảo vệ.

**Independent Test**: Đăng nhập bằng tài khoản hợp lệ, sai mật khẩu, vô hiệu hóa và đã xóa; xác nhận chỉ trường hợp hợp lệ nhận phiên.

**Acceptance Scenarios**:

1. **Given** tài khoản hoạt động và thông tin đúng, **When** đăng nhập, **Then** người dùng nhận access token và refresh token được lưu trong cookie bảo vệ.
2. **Given** thông tin sai hoặc tài khoản không khả dụng, **When** đăng nhập, **Then** hệ thống trả thông báo chung và không tiết lộ nguyên nhân nhận diện tài khoản.
3. **Given** một địa chỉ nguồn gửi quá số lần cho phép, **When** tiếp tục đăng nhập, **Then** hệ thống tạm từ chối và cho biết thời điểm thử lại.

---

### User Story 2 - Làm mới phiên (Priority: P1)

Người dùng duy trì phiên mà không nhập lại mật khẩu khi refresh token còn hợp lệ.

**Why this priority**: Phiên truy cập ngắn hạn cần được làm mới an toàn để cân bằng bảo mật và trải nghiệm.

**Independent Test**: Dùng refresh token hợp lệ một lần, xác nhận token được xoay vòng và token cũ không thể dùng lại.

**Acceptance Scenarios**:

1. **Given** refresh token hợp lệ, **When** làm mới, **Then** hệ thống thu hồi token cũ, cấp token mới và access token chứa quyền hiện hành.
2. **Given** token thiếu, hết hạn, đã thu hồi hoặc đã thay thế, **When** làm mới, **Then** hệ thống từ chối an toàn.

---

### User Story 3 - Đăng xuất (Priority: P1)

Người dùng kết thúc phiên hiện tại và không thể tiếp tục dùng các token của phiên đó.

**Why this priority**: Đăng xuất phải thực sự kết thúc quyền truy cập, không chỉ xóa trạng thái giao diện.

**Independent Test**: Đăng xuất rồi thử dùng lại access token và refresh token hiện tại; cả hai đều bị từ chối.

**Acceptance Scenarios**:

1. **Given** người dùng có phiên hợp lệ, **When** đăng xuất, **Then** refresh token bị thu hồi, access token hiện tại bị vô hiệu hóa và cookie bị xóa.
2. **Given** cookie không tồn tại, **When** đăng xuất, **Then** hệ thống vẫn kết thúc yêu cầu an toàn mà không tiết lộ chi tiết phiên.

### Edge Cases

- Refresh token bị sử dụng đồng thời bởi hai yêu cầu.
- User bị vô hiệu hóa sau khi login nhưng trước khi refresh.
- Quyền hoặc Role thay đổi khi access token cũ còn hạn.
- Cookie bị thiếu, sai hoặc gửi qua kết nối không phù hợp.
- Client ngắt kết nối trong khi token đang được xoay vòng.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Login và refresh MUST cho phép truy cập không cần access token; logout MUST xử lý được phiên hiện hành.
- **FR-002**: Login MUST nhận Code và mật khẩu, đối chiếu Code không phân biệt hoa/thường và không chấp nhận Email thay Code.
- **FR-003**: Hệ thống MUST kiểm tra mật khẩu bằng cơ chế băm được phê duyệt và MUST NOT ghi log hoặc trả mật khẩu.
- **FR-004**: Hệ thống MUST từ chối tài khoản không tồn tại, không hoạt động hoặc đã xóa bằng thông báo chung.
- **FR-005**: Access token MUST có thời hạn ngắn và chứa định danh User, Application, Roles, Permissions, loại token và phiên bản quyền.
- **FR-006**: Refresh token MUST là giá trị ngẫu nhiên một lần, chỉ lưu bản băm phía máy chủ và được gửi qua cookie chỉ dành cho HTTP với SameSite nghiêm ngặt.
- **FR-007**: Refresh MUST xoay vòng token trong một thao tác nhất quán và phát hiện sử dụng lại token cũ.
- **FR-008**: Refresh MUST tải trạng thái User, Roles và Permissions mới nhất trước khi cấp access token.
- **FR-009**: Logout MUST thu hồi refresh token hiện tại, vô hiệu hóa access token hiện tại và xóa cookie.
- **FR-010**: Access token MUST được kiểm tra issuer, audience, chữ ký, thời hạn, loại token, trạng thái thu hồi, trạng thái User và phiên bản quyền.
- **FR-011**: Phản hồi lỗi MUST không tiết lộ token, mật khẩu, trạng thái tồn tại của tài khoản hoặc chi tiết lưu trữ.
- **FR-012**: Login MUST chịu rate limit độc lập theo nguồn yêu cầu.

### Key Entities

- **User**: Danh tính đăng nhập và trạng thái tài khoản.
- **Access Token**: Bằng chứng truy cập ngắn hạn mang phiên bản quyền.
- **Refresh Token**: Thông tin phiên dài hơn, có thể xoay vòng và thu hồi.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% trường hợp thông tin sai hoặc tài khoản không khả dụng không nhận được token.
- **SC-002**: Refresh token đã dùng không thể tạo thêm phiên trong kiểm thử sử dụng lại và đồng thời.
- **SC-003**: Sau logout, cả access token và refresh token hiện hành bị từ chối ở lần sử dụng tiếp theo.
- **SC-004**: 100% token có phiên bản quyền lỗi thời bị từ chối.
- **SC-005**: Luồng login, refresh và logout hoàn thành trong dưới 2 giây ở điều kiện hoạt động bình thường.

## Assumptions

- Tài khoản được tạo bởi quản trị viên; đăng ký công khai và khôi phục mật khẩu nằm ngoài phạm vi.
- Mỗi phiên refresh thuộc một User và một Application.
- Client hỗ trợ cookie bảo mật và lưu access token theo hướng dẫn của ứng dụng.
