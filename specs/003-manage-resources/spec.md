# Feature Specification: Quản lý Resources

**Feature Branch**: `003-manage-resources`
**Created**: 2026-08-11
**Status**: Draft
**Input**: Chức năng mới Resources tạo giống 002-manage-applications; dữ liệu được lưu vào bảng resources.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Xem và tìm danh sách Resources (Priority: P1)

Người quản trị mở mục **Resources** để xem, tìm kiếm và duyệt tài nguyên của từng Application.

**Why this priority**: Danh sách là điểm vào của toàn bộ nghiệp vụ và cho phép kiểm tra dữ liệu trước khi thay đổi.

**Independent Test**: Mở **Resources**, lọc theo Application và xác nhận dữ liệu từ bảng resources có thể được lọc, sắp xếp và chuyển trang.

**Acceptance Scenarios**:

1. **Given** người dùng có quyền xem, **When** mở **Resources**, **Then** hiển thị Application, Code, Name, Resource Type, trạng thái và hành động được phép.
2. **Given** có nhiều Resources, **When** lọc theo Application, Code, Name, Resource Type hoặc trạng thái, **Then** chỉ bản ghi phù hợp được hiển thị.
3. **Given** kết quả vượt một trang, **When** đổi trang, kích thước hoặc cách sắp xếp, **Then** đúng nhóm kết quả được hiển thị.
4. **Given** không có bản ghi phù hợp, **When** tải hoặc lọc danh sách, **Then** hiển thị trạng thái rỗng rõ ràng.

---

### User Story 2 - Tạo Resource (Priority: P1)

Người quản trị tạo Resource mới cho một Application bằng thông tin nhận diện và phân loại cần thiết.

**Why this priority**: Resource mô tả phạm vi được bảo vệ và là cơ sở gán quyền.

**Independent Test**: Chọn Application, nhập dữ liệu hợp lệ, lưu và xác nhận Resource còn tồn tại sau khi tải lại.

**Acceptance Scenarios**:

1. **Given** người dùng có quyền và Application hợp lệ, **When** nhập Code, Name, Resource Type và Description tùy chọn, **Then** Resource hoạt động được lưu vào bảng resources.
2. **Given** thiếu trường, vượt độ dài hoặc Code trùng trong cùng Application, **When** lưu, **Then** lỗi được chỉ rõ và không tạo bản ghi.
3. **Given** cùng Code tồn tại ở Application khác, **When** tạo Resource hợp lệ, **Then** thao tác được chấp nhận.
4. **Given** yêu cầu đang xử lý, **When** tiếp tục bấm lưu, **Then** hệ thống ngăn gửi trùng.

---

### User Story 3 - Cập nhật Resource (Priority: P2)

Người quản trị chỉnh sửa Application sở hữu, thông tin phân loại, mô tả và trạng thái Resource.

**Why this priority**: Duy trì dữ liệu chính xác mà không làm mất liên kết quyền và menu.

**Independent Test**: Sửa Resource, lưu, tải lại và xác nhận thay đổi được giữ trong bảng resources.

**Acceptance Scenarios**:

1. **Given** người dùng có quyền, **When** lưu dữ liệu hợp lệ, **Then** đúng Resource được cập nhật.
2. **Given** chuyển sang Application khác, **When** tổ hợp Application và Code hợp lệ, **Then** Resource được cập nhật và giữ liên kết hiện có.
3. **Given** dữ liệu không hợp lệ hoặc Code trùng tại Application đích, **When** lưu, **Then** dữ liệu không đổi và lỗi được chỉ rõ.
4. **Given** Resource đã thay đổi sau khi form mở, **When** lưu phiên bản cũ, **Then** không ghi đè âm thầm và yêu cầu tải dữ liệu mới.

---

### User Story 4 - Xóa Resource (Priority: P3)

Người quản trị loại bỏ Resource không còn sử dụng sau khi xác nhận đối tượng và ảnh hưởng.

**Why this priority**: Xóa hoàn thiện vòng đời nhưng có thể ảnh hưởng phân quyền và điều hướng.

**Independent Test**: Xóa Resource không có Permission/Menu liên kết và xác nhận nó không còn trong danh sách mặc định.

**Acceptance Scenarios**:

1. **Given** người dùng có quyền xóa, **When** chọn xóa, **Then** yêu cầu xác nhận với Application, Code và Name.
2. **Given** hộp xác nhận đang mở, **When** hủy, **Then** dữ liệu không đổi.
3. **Given** không có ràng buộc, **When** xác nhận, **Then** Resource bị xóa mềm.
4. **Given** Permission hoặc Menu đang tham chiếu, **When** xác nhận xóa, **Then** dữ liệu được giữ và thông báo chưa thể xóa.

### Edge Cases

- Phiên hết hạn khi tải hoặc lưu: thất bại an toàn và hướng dẫn đăng nhập lại.
- Truy cập trực tiếp nhưng thiếu quyền: không hiển thị dữ liệu hoặc hành động.
- Application bị xóa, không hoạt động hoặc thay đổi trong lúc lưu: từ chối liên kết và giữ dữ liệu.
- Code khác hoa/thường trong cùng Application được xem là trùng; cùng Code ở Application khác hợp lệ.
- Code, Name, Resource Type được trim; giá trị chỉ có khoảng trắng bị xem là rỗng.
- Description trống được chấp nhận.
- Chỉnh sửa đồng thời không được ghi đè dữ liệu mới hơn.
- Lỗi mạng giữ dữ liệu form, cho thử lại và không tạo thay đổi trùng.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Hệ thống MUST cung cấp mục **Resources** trong điều hướng đã xác thực.
- **FR-002**: Người dùng MUST có quyền tương ứng để xem, tạo, cập nhật hoặc xóa; truy cập thiếu quyền MUST bị từ chối ở giao diện và xử lý dữ liệu.
- **FR-003**: Danh sách MUST đọc từ bảng resources và hiển thị Application, Code, Name, Resource Type, trạng thái, ngày tạo và hành động.
- **FR-004**: Người dùng MUST lọc được theo Application, Code, Name, Resource Type và trạng thái.
- **FR-005**: Người dùng MUST sắp xếp và chuyển trang được.
- **FR-006**: Màn hình MUST có trạng thái tải, rỗng, lỗi và thử lại.
- **FR-007**: Người dùng có quyền MUST tạo được Resource với Application, Code, Name, Resource Type, Description tùy chọn và trạng thái hoạt động mặc định.
- **FR-008**: Application MUST tồn tại, chưa xóa và hoạt động khi tạo hoặc chuyển Resource.
- **FR-009**: Code MUST dài 1–120 ký tự sau trim và duy nhất không phân biệt hoa/thường trong từng Application.
- **FR-010**: Name MUST dài 1–150 ký tự sau trim.
- **FR-011**: Resource Type MUST dài 1–30 ký tự sau trim.
- **FR-012**: Description MUST không vượt quá 500 ký tự.
- **FR-013**: Dữ liệu MUST được kiểm tra khi tạo/cập nhật; lỗi phải gắn đúng trường và MUST NOT lưu một phần.
- **FR-014**: Người dùng có quyền MUST cập nhật được Application, Code, Name, Resource Type, Description và trạng thái.
- **FR-015**: Tạo, cập nhật, xóa thành công MUST cập nhật bản ghi trong bảng resources và lần đọc sau MUST phản ánh thay đổi.
- **FR-016**: Xóa MUST yêu cầu xác nhận với Application, Code và Name.
- **FR-017**: Xóa MUST là xóa mềm; bản ghi đã xóa MUST bị ẩn mặc định và không dùng cho hoạt động mới.
- **FR-018**: Hệ thống MUST từ chối xóa Resource được Permission hoặc Menu tham chiếu.
- **FR-019**: Hệ thống MUST phát hiện xung đột đồng thời và MUST NOT ghi đè dữ liệu mới hơn.
- **FR-020**: Màn hình MUST ngăn gửi lặp và thông báo kết quả rõ ràng.
- **FR-021**: Thao tác thành công MUST ghi thời điểm và danh tính người thực hiện theo audit hiện có.
- **FR-022**: Giao diện MUST hỗ trợ bàn phím, focus, nhãn và trạng thái rõ ràng trên desktop/mobile được hỗ trợ.

### Key Entities

- **Resource**: Tài nguyên được bảo vệ; gồm Application, Code duy nhất trong Application, Name, Resource Type, Description, trạng thái và audit.
- **Application**: Phạm vi sở hữu Resource; một Application có nhiều Resources.
- **Resource dependency**: Permission hoặc Menu tham chiếu Resource và có thể ngăn xóa.
- **Authorized administrator**: Người dùng có quyền quản lý Resources.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 95% người dùng có quyền tìm được Resource trong 30 giây.
- **SC-002**: 95% thao tác tạo/cập nhật hợp lệ hoàn thành trong 2 phút.
- **SC-003**: 100% dữ liệu không hợp lệ bị từ chối mà không lưu một phần hoặc tạo trùng trong cùng Application.
- **SC-004**: 100% người dùng thiếu quyền bị ngăn xem hoặc thay đổi Resources.
- **SC-005**: Với 10.000 Resources, 95% lượt mở, lọc, sắp xếp hoặc chuyển trang có kết quả trong 2 giây dưới tải bình thường.
- **SC-006**: 100% thao tác xóa bị hủy, chặn do phụ thuộc hoặc xung đột giữ dữ liệu nhất quán và trả kết quả rõ ràng.
- **SC-007**: Tất cả luồng CRUD chính hoàn thành được bằng bàn phím trên các kích thước màn hình được hỗ trợ.

## Assumptions

- Tạo mục điều hướng **Resources** vì giao diện hiện chưa có mục này.
- Tái sử dụng đăng nhập, phiên, phân quyền và audit hiện có; tên quyền cụ thể thuộc giai đoạn thiết kế.
- Resource Type là chuỗi tự do; quản lý danh mục loại nằm ngoài phạm vi.
- Code có thể sửa và chỉ duy nhất trong một Application.
- Cho phép chuyển Resource sang Application hợp lệ khác; Permission/Menu vẫn gắn với cùng Resource.
- Xóa mềm là mặc định; khôi phục và xem bản ghi đã xóa ngoài phạm vi.
- Quản lý Permission, Menu, Application và gán quyền ngoài phạm vi, trừ kiểm tra phụ thuộc khi xóa.
