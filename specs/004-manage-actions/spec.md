# Feature Specification: Quản lý Actions

**Feature Branch**: `004-manage-actions`

**Created**: 2026-08-12

**Status**: Draft

**Input**: Màn hình Actions có logic quản lý giống Applications, lưu dữ liệu vào bảng permission_actions và không yêu cầu phân quyền theo từng thao tác.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Xem và tìm danh sách Actions (Priority: P1)

Người quản trị đã đăng nhập mở mục **Actions** để xem, tìm kiếm và duyệt các hành động dùng khi cấu hình Permission.

**Why this priority**: Danh sách là điểm vào cho toàn bộ nghiệp vụ quản lý Actions và mang lại giá trị độc lập trước khi có thao tác thay đổi dữ liệu.

**Independent Test**: Đăng nhập bằng tài khoản có `Actions.Read`, mở **Actions**, rồi xác nhận dữ liệu được hiển thị và có thể lọc, sắp xếp, chuyển trang; tài khoản thiếu quyền bị từ chối.

**Acceptance Scenarios**:

1. **Given** người dùng đã đăng nhập, **When** mở **Actions**, **Then** màn hình hiển thị danh sách gồm Code, Name, ngày tạo và các thao tác quản lý.
2. **Given** có nhiều Actions, **When** người dùng lọc theo Code hoặc Name, **Then** chỉ các bản ghi phù hợp được hiển thị.
3. **Given** kết quả vượt quá kích thước một trang, **When** người dùng đổi trang, kích thước trang hoặc thứ tự sắp xếp, **Then** đúng nhóm kết quả tương ứng được hiển thị.
4. **Given** không có bản ghi phù hợp, **When** danh sách được tải hoặc bộ lọc được áp dụng, **Then** màn hình hiển thị trạng thái rỗng rõ ràng.

---

### User Story 2 - Tạo Action (Priority: P1)

Người quản trị đã đăng nhập tạo Action mới bằng Code và Name để Action có thể được dùng khi cấu hình Permission.

**Why this priority**: Tạo Action là thao tác cốt lõi để mở rộng tập hành động nghiệp vụ của hệ thống.

**Independent Test**: Mở form tạo, nhập Code và Name hợp lệ, lưu, rồi xác nhận bản ghi xuất hiện trong danh sách và vẫn tồn tại sau khi tải lại.

**Acceptance Scenarios**:

1. **Given** người dùng đã đăng nhập, **When** nhập Code và Name hợp lệ rồi lưu, **Then** Action được tạo trong permission_actions và xuất hiện trong danh sách.
2. **Given** thiếu trường, vượt độ dài hoặc Code đã tồn tại, **When** người dùng lưu, **Then** lỗi được chỉ rõ tại trường liên quan và không tạo bản ghi.
3. **Given** yêu cầu tạo đang được xử lý, **When** người dùng tiếp tục bấm lưu, **Then** hệ thống ngăn gửi trùng lặp và thể hiện trạng thái đang xử lý.

---

### User Story 3 - Cập nhật Action (Priority: P2)

Người quản trị đã đăng nhập chỉnh sửa Code hoặc Name của một Action hiện có.

**Why this priority**: Cho phép sửa thông tin nhận diện mà không phải tạo lại Action và cấu hình lại các Permission liên quan.

**Independent Test**: Chọn một Action, sửa dữ liệu hợp lệ, lưu, tải lại màn hình và xác nhận thay đổi được giữ nguyên trong permission_actions.

**Acceptance Scenarios**:

1. **Given** một Action đang tồn tại, **When** người dùng lưu Code và Name hợp lệ, **Then** đúng Action được cập nhật và danh sách phản ánh dữ liệu mới.
2. **Given** dữ liệu không hợp lệ hoặc Code mới trùng Action khác, **When** người dùng lưu, **Then** dữ liệu không thay đổi và lỗi được chỉ rõ.
3. **Given** Action đã bị thay đổi hoặc xóa sau khi form được mở, **When** người dùng lưu dữ liệu cũ, **Then** hệ thống không ghi đè âm thầm và hướng dẫn tải dữ liệu mới nhất.

---

### User Story 4 - Xóa Action (Priority: P3)

Người quản trị đã đăng nhập xóa một Action không còn sử dụng sau khi xác nhận rõ đối tượng và ảnh hưởng của thao tác.

**Why this priority**: Hoàn thiện vòng đời Action nhưng có rủi ro ảnh hưởng các Permission đang tham chiếu.

**Independent Test**: Tạo một Action thử nghiệm không được Permission nào sử dụng, xác nhận xóa và kiểm tra bản ghi không còn trong danh sách hoặc permission_actions.

**Acceptance Scenarios**:

1. **Given** người dùng chọn xóa một Action, **When** hộp thoại xác nhận mở, **Then** Code và Name được hiển thị để nhận diện đúng đối tượng.
2. **Given** hộp thoại xác nhận đang mở, **When** người dùng hủy, **Then** không có dữ liệu nào thay đổi.
3. **Given** Action không được Permission nào sử dụng, **When** người dùng xác nhận, **Then** Action bị xóa và không còn xuất hiện trong danh sách.
4. **Given** Action đang được ít nhất một Permission sử dụng, **When** người dùng xác nhận xóa, **Then** hệ thống giữ nguyên Action và giải thích rằng chưa thể xóa do có dữ liệu phụ thuộc.

### Edge Cases

- Phiên đăng nhập hết hạn trong lúc tải hoặc thay đổi dữ liệu: thao tác thất bại an toàn, không giả định đã lưu và người dùng được hướng dẫn đăng nhập lại.
- Mạng hoặc dịch vụ tạm thời không khả dụng: giữ dữ liệu form chưa gửi, hiển thị lỗi có thể thử lại và không tạo thay đổi trùng lặp.
- Code chỉ khác chữ hoa/chữ thường với bản ghi đã có được xem là trùng.
- Code hoặc Name có khoảng trắng đầu/cuối được loại bỏ trước khi kiểm tra và lưu; giá trị chỉ gồm khoảng trắng được xem là rỗng.
- Hai người cùng sửa hoặc xóa một Action: dữ liệu mới hơn không bị ghi đè âm thầm và người thao tác sau nhận kết quả rõ ràng.
- Action được gắn vào Permission sau khi hộp thoại xóa mở: thao tác xóa bị từ chối và dữ liệu được giữ nguyên.
- Lỗi tải lại danh sách không xóa kết quả hợp lệ đang hiển thị nếu kết quả đó vẫn còn dùng được.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Hệ thống MUST cung cấp mục **Actions** và địa chỉ màn hình riêng làm điểm truy cập chức năng quản lý Actions.
- **FR-002**: Hệ thống MUST yêu cầu quyền `Actions.Read`, `Actions.Create`, `Actions.Update` hoặc `Actions.Delete` tương ứng cho từng thao tác; người chưa đăng nhập hoặc thiếu quyền MUST bị từ chối an toàn.
- **FR-003**: Danh sách MUST đọc từ permission_actions và hiển thị tối thiểu Code, Name, ngày tạo và các thao tác quản lý.
- **FR-004**: Người dùng MUST có thể lọc danh sách theo Code và Name.
- **FR-005**: Người dùng MUST có thể sắp xếp theo các cột được hỗ trợ và chuyển trang bằng các kích thước trang hợp lệ.
- **FR-006**: Màn hình MUST có trạng thái tải, rỗng, lỗi và thử lại rõ ràng; lỗi đọc dữ liệu MUST NOT xóa kết quả hợp lệ đang hiển thị nếu còn dùng được.
- **FR-007**: Người dùng có quyền `Actions.Create` MUST có thể tạo Action với Code và Name.
- **FR-008**: Code MUST bắt buộc, dài từ 1 đến 50 ký tự sau khi loại bỏ khoảng trắng đầu/cuối và duy nhất không phân biệt chữ hoa/chữ thường.
- **FR-009**: Name MUST bắt buộc và dài từ 1 đến 100 ký tự sau khi loại bỏ khoảng trắng đầu/cuối.
- **FR-010**: Hệ thống MUST kiểm tra quy tắc dữ liệu khi tạo và cập nhật, trả lỗi gắn với trường phù hợp và MUST NOT lưu một phần khi có lỗi.
- **FR-011**: Người dùng có quyền `Actions.Update` MUST có thể cập nhật Code và Name của Action hiện có.
- **FR-012**: Mọi thao tác tạo, cập nhật và xóa thành công MUST cập nhật dữ liệu tương ứng trong permission_actions và kết quả đọc tiếp theo MUST phản ánh thay đổi đã cam kết.
- **FR-013**: Trước khi xóa, hệ thống MUST yêu cầu xác nhận với Code và Name của Action.
- **FR-014**: Xóa Action MUST loại bỏ bản ghi khỏi permission_actions khi Action không có Permission phụ thuộc.
- **FR-015**: Hệ thống MUST từ chối xóa Action đang được Permission sử dụng, MUST giữ nguyên Action và MUST cung cấp thông báo an toàn, dễ hiểu.
- **FR-016**: Hệ thống MUST phát hiện thay đổi đồng thời khi cập nhật hoặc xóa và MUST NOT ghi đè âm thầm dữ liệu mới hơn.
- **FR-017**: Màn hình MUST ngăn gửi lặp thao tác thay đổi khi yêu cầu trước đang xử lý và MUST thông báo kết quả thành công hoặc thất bại rõ ràng.
- **FR-018**: Các thao tác thành công MUST ghi nhận thời điểm và danh tính người tạo hoặc cập nhật theo cơ chế audit hiện có.
- **FR-019**: Giao diện MUST hỗ trợ bàn phím, nhãn có ý nghĩa, focus nhìn thấy được và trạng thái disabled/loading/error rõ ràng trên các kích thước desktop và mobile được dự án hỗ trợ.
- **FR-020**: Chức năng quản lý Actions MUST chỉ quản lý danh mục hành động; việc cấp quyền cho Role nằm ngoài phạm vi và được thực hiện qua chức năng Role Permissions.

### Key Entities

- **Action**: Một hành động nghiệp vụ có thể được dùng khi cấu hình Permission; gồm Code duy nhất, Name và thông tin audit, được lưu trong permission_actions.
- **Permission dependency**: Một Permission tham chiếu đến Action; sự tồn tại của liên kết này ngăn Action bị xóa.
- **Authorized administrator**: Người dùng có phiên hợp lệ và quyền Actions tương ứng với thao tác cần thực hiện.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Ít nhất 95% người dùng hoàn thành việc tìm một Action theo Code hoặc Name trong không quá 30 giây ở lần thử đầu tiên.
- **SC-002**: Ít nhất 95% thao tác tạo hoặc cập nhật với dữ liệu hợp lệ được hoàn thành trong không quá 2 phút.
- **SC-003**: 100% dữ liệu không hợp lệ trong các kịch bản chấp nhận bị từ chối mà không tạo thay đổi một phần hoặc Code trùng.
- **SC-004**: 100% yêu cầu thiếu quyền Actions tương ứng bị từ chối an toàn; 100% yêu cầu hợp lệ của người có quyền hoàn thành theo đúng thao tác được cấp.
- **SC-005**: Với tối đa 10.000 Actions, 95% lượt mở, lọc, sắp xếp hoặc chuyển trang hiển thị kết quả trong không quá 2 giây dưới tải vận hành bình thường.
- **SC-006**: 100% thao tác xóa bị hủy, bị từ chối do Permission phụ thuộc hoặc gặp xung đột đồng thời giữ dữ liệu nhất quán và cung cấp kết quả rõ ràng.
- **SC-007**: Tất cả kịch bản CRUD chính có thể hoàn thành chỉ bằng bàn phím và vẫn sử dụng được ở các kích thước màn hình được dự án hỗ trợ.

## Assumptions

- Màn hình mới sử dụng tên **Actions** và địa chỉ `/actions`, đồng thời tuân theo bố cục và hành vi danh sách/form của màn hình Applications.
- “Không cần phân quyền” nghĩa là không định nghĩa hoặc kiểm tra các quyền Actions.View/Create/Update/Delete; yêu cầu đăng nhập hiện có vẫn được giữ để bảo vệ hệ thống quản trị danh tính.
- permission_actions hiện chỉ có Code, Name và audit; feature không bổ sung trạng thái hoạt động hoặc xóa mềm.
- Xóa Action là xóa bản ghi sau khi kiểm tra không có Permission tham chiếu; feature không tự động xóa, chuyển đổi hoặc gán lại Permission.
- Code có thể chỉnh sửa nhưng phải duy nhất không phân biệt chữ hoa/chữ thường.
- Quản lý Permissions, Resources, Applications và gán quyền nằm ngoài phạm vi feature này.
