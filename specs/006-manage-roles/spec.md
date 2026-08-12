# Feature Specification: Quản lý Roles

**Feature Branch**: `006-manage-roles`

**Created**: 2026-08-12

**Status**: Draft

**Input**: Chức năng mới Roles, logic chức năng giống Actions, không cần phân quyền.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Xem và tìm danh sách Roles (Priority: P1)

Người quản trị đã đăng nhập mở mục **Roles** để xem, tìm kiếm và duyệt các vai trò theo ứng dụng.

**Why this priority**: Danh sách là điểm vào cho toàn bộ nghiệp vụ quản lý Roles và giúp người dùng nhận biết vai trò thuộc đúng ứng dụng.

**Independent Test**: Đăng nhập, mở **Roles**, xác nhận có thể lọc, sắp xếp và chuyển trang mà không cần quyền Roles riêng.

**Acceptance Scenarios**:

1. **Given** người dùng đã đăng nhập, **When** mở **Roles**, **Then** màn hình hiển thị Application, Code, Name, trạng thái System/Active, ngày tạo và các thao tác quản lý.
2. **Given** có nhiều Roles, **When** lọc theo Application, Code hoặc Name, **Then** chỉ các bản ghi phù hợp được hiển thị.
3. **Given** kết quả vượt quá một trang, **When** đổi trang, kích thước trang hoặc thứ tự sắp xếp, **Then** đúng nhóm kết quả được hiển thị.
4. **Given** không có kết quả, **When** tải hoặc áp dụng bộ lọc, **Then** màn hình hiển thị trạng thái rỗng rõ ràng.

---

### User Story 2 - Tạo Role (Priority: P1)

Người quản trị đã đăng nhập tạo Role cho một Application bằng Code, Name và các trạng thái phù hợp.

**Why this priority**: Tạo Role là thao tác cốt lõi để tổ chức người dùng và Permission theo ứng dụng.

**Independent Test**: Chọn Application, nhập dữ liệu hợp lệ, lưu và xác nhận Role xuất hiện trong danh sách sau khi tải lại.

**Acceptance Scenarios**:

1. **Given** người dùng đã đăng nhập, **When** chọn Application, nhập Code và Name hợp lệ rồi lưu, **Then** Role được tạo và xuất hiện trong danh sách.
2. **Given** thiếu Application/trường bắt buộc, vượt độ dài hoặc Code trùng trong cùng Application, **When** lưu, **Then** lỗi được chỉ rõ và không tạo bản ghi.
3. **Given** yêu cầu tạo đang xử lý, **When** tiếp tục bấm lưu, **Then** hệ thống ngăn gửi trùng lặp.

---

### User Story 3 - Cập nhật Role (Priority: P2)

Người quản trị đã đăng nhập chỉnh sửa Application, Code, Name hoặc trạng thái của Role hiện có.

**Why this priority**: Cho phép duy trì thông tin vai trò mà không phải tạo lại và gán lại các liên kết liên quan.

**Independent Test**: Sửa Role với dữ liệu hợp lệ, lưu, tải lại và xác nhận thay đổi được giữ nguyên.

**Acceptance Scenarios**:

1. **Given** Role đang tồn tại, **When** lưu dữ liệu hợp lệ, **Then** đúng Role được cập nhật và danh sách phản ánh dữ liệu mới.
2. **Given** dữ liệu không hợp lệ hoặc Code trùng Role khác trong Application đích, **When** lưu, **Then** dữ liệu không thay đổi và lỗi được chỉ rõ.
3. **Given** Role đã bị thay đổi hoặc xóa sau khi form mở, **When** lưu phiên bản cũ, **Then** hệ thống không ghi đè âm thầm và hướng dẫn tải dữ liệu mới nhất.

---

### User Story 4 - Xóa Role (Priority: P3)

Người quản trị đã đăng nhập xóa một Role không còn sử dụng sau khi xác nhận rõ đối tượng.

**Why this priority**: Hoàn thiện vòng đời Role nhưng phải bảo vệ liên kết người dùng, Permission và các Role hệ thống.

**Independent Test**: Tạo Role thường không có liên kết, xác nhận xóa và kiểm tra Role không còn trong danh sách.

**Acceptance Scenarios**:

1. **Given** người dùng chọn xóa Role, **When** hộp thoại mở, **Then** Application, Code và Name được hiển thị để nhận diện.
2. **Given** hộp thoại đang mở, **When** hủy, **Then** dữ liệu không thay đổi.
3. **Given** Role không phải Role hệ thống và không có User hoặc Permission liên kết, **When** xác nhận, **Then** Role bị xóa.
4. **Given** Role là Role hệ thống hoặc đang có dữ liệu liên kết, **When** xác nhận xóa, **Then** hệ thống giữ nguyên Role và giải thích lý do.

### Edge Cases

- Phiên đăng nhập hết hạn trong lúc tải hoặc thay đổi dữ liệu: thao tác thất bại an toàn và hướng dẫn đăng nhập lại.
- Mạng hoặc dịch vụ tạm thời không khả dụng: giữ dữ liệu form, cho phép thử lại và không tạo thay đổi trùng.
- Code chỉ khác chữ hoa/chữ thường trong cùng Application được xem là trùng; cùng Code ở Application khác được phép.
- Khoảng trắng đầu/cuối được loại bỏ; giá trị chỉ có khoảng trắng được xem là rỗng.
- Application bị xóa hoặc thay đổi trạng thái sau khi form mở: thao tác không lưu tham chiếu không hợp lệ.
- Hai người cùng sửa hoặc xóa Role: dữ liệu mới hơn không bị ghi đè âm thầm.
- Role được gán cho User/Permission sau khi hộp thoại xóa mở: xóa bị từ chối và dữ liệu được giữ nguyên.
- Lỗi tải lại không xóa kết quả hợp lệ đang hiển thị nếu kết quả đó còn dùng được.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Hệ thống MUST cung cấp mục **Roles** và địa chỉ `/roles` làm điểm truy cập chức năng.
- **FR-002**: Mọi người dùng đã đăng nhập MUST có thể xem, tạo, cập nhật và xóa Roles mà không kiểm tra quyền Roles.View/Create/Update/Delete; người chưa đăng nhập MUST bị từ chối an toàn.
- **FR-003**: Danh sách MUST hiển thị tối thiểu Application, Code, Name, IsSystemRole, IsActive, ngày tạo và thao tác quản lý.
- **FR-004**: Người dùng MUST có thể lọc theo Application, Code và Name, sắp xếp các cột được hỗ trợ và phân trang.
- **FR-005**: Màn hình MUST có trạng thái tải, rỗng, lỗi và thử lại rõ ràng; lỗi đọc MUST NOT xóa kết quả hợp lệ còn dùng được.
- **FR-006**: Người dùng MUST có thể tạo Role bằng Application, Code, Name, IsSystemRole và IsActive; mặc định Role mới không phải Role hệ thống và đang hoạt động.
- **FR-007**: Application MUST tham chiếu một Application đang tồn tại.
- **FR-008**: Code MUST bắt buộc, dài 1-100 ký tự sau khi trim và duy nhất không phân biệt hoa/thường trong từng Application.
- **FR-009**: Name MUST bắt buộc và dài 1-150 ký tự sau khi trim.
- **FR-010**: Validation MUST áp dụng khi tạo/cập nhật, chỉ rõ trường lỗi và không lưu một phần.
- **FR-011**: Người dùng MUST có thể cập nhật Application, Code, Name, IsSystemRole và IsActive của Role hiện có.
- **FR-012**: Tạo, cập nhật và xóa thành công MUST được phản ánh trong lần đọc tiếp theo và ghi nhận audit hiện có.
- **FR-013**: Trước khi xóa, hệ thống MUST yêu cầu xác nhận với Application, Code và Name.
- **FR-014**: Hệ thống MUST từ chối xóa Role hệ thống hoặc Role đang liên kết với User hay Permission, giữ nguyên dữ liệu và trả thông báo dễ hiểu.
- **FR-015**: Role thường không có liên kết MUST được xóa vật lý khi người dùng xác nhận.
- **FR-016**: Hệ thống MUST phát hiện xung đột đồng thời khi cập nhật/xóa và MUST NOT ghi đè dữ liệu mới hơn.
- **FR-017**: Màn hình MUST ngăn gửi lặp khi thao tác đang xử lý và thông báo kết quả rõ ràng.
- **FR-018**: Giao diện MUST hỗ trợ bàn phím, focus nhìn thấy, nhãn có nghĩa và responsive theo chuẩn dự án.
- **FR-019**: Chức năng MUST NOT tạo hoặc gán quyền quản trị mới hay phụ thuộc Permission để quyết định truy cập Roles.

### Key Entities

- **Role**: Vai trò thuộc một Application, gồm Code duy nhất trong Application, Name, cờ hệ thống, trạng thái hoạt động, audit và phiên bản dữ liệu.
- **Application**: Phạm vi sở hữu Role và quyết định ranh giới duy nhất của Code.
- **Role dependency**: Liên kết từ User hoặc Permission tới Role; sự tồn tại của liên kết ngăn Role bị xóa.
- **Authenticated administrator**: Người dùng có phiên hợp lệ, không cần quyền Roles riêng.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Ít nhất 95% người dùng tìm được Role theo Application, Code hoặc Name trong 30 giây ở lần thử đầu.
- **SC-002**: Ít nhất 95% thao tác tạo/cập nhật hợp lệ hoàn thành trong 2 phút.
- **SC-003**: 100% dữ liệu không hợp lệ trong kịch bản chấp nhận bị từ chối mà không tạo thay đổi một phần hoặc Code trùng.
- **SC-004**: 100% người dùng đã đăng nhập dùng đầy đủ Roles không cần quyền Roles riêng; 100% người chưa đăng nhập bị từ chối.
- **SC-005**: Với tối đa 10.000 Roles, 95% lượt mở, lọc, sắp xếp hoặc chuyển trang hiển thị trong 2 giây dưới tải bình thường.
- **SC-006**: 100% thao tác xóa bị hủy, bị chặn do Role hệ thống/dữ liệu liên kết hoặc xung đột giữ dữ liệu nhất quán và có kết quả rõ ràng.
- **SC-007**: Tất cả kịch bản CRUD chính hoàn thành được chỉ bằng bàn phím trên các kích thước màn hình dự án hỗ trợ.

## Assumptions

- Roles dùng `/roles`, bố cục và hành vi CRUD giống Actions; vẫn yêu cầu đăng nhập theo constitution.
- Dữ liệu nằm trong bảng `roles`; Application được chọn từ dữ liệu Applications hiện có.
- `IsSystemRole` và `IsActive` là dữ liệu nghiệp vụ hiển thị/chỉnh sửa; Role hệ thống không được xóa.
- Xóa Role là xóa vật lý sau khi kiểm tra `user_roles` và `role_permissions`; feature không tự động gỡ/gán lại liên kết.
- Quản lý Applications, Users, Permissions và thao tác gán Role nằm ngoài phạm vi.
