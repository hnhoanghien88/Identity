# Feature Specification: Quản lý Applications

**Feature Branch**: `002-manage-applications`

**Created**: 2026-08-11

**Status**: Draft

**Input**: Chức năng mới Applications: sử dụng menu Applications có sẵn, thay màn hình chính hiện tại bằng màn hình CRUD; dữ liệu được đọc và cập nhật trong tập Applications. Giải pháp triển khai ở giai đoạn thiết kế phải tuân theo CQRS.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Xem và tìm danh sách Applications (Priority: P1)

Người quản trị chọn mục **Application** có sẵn trên menu để xem, tìm kiếm và duyệt các ứng dụng đã đăng ký trong hệ thống.

**Why this priority**: Danh sách là điểm vào của toàn bộ nghiệp vụ quản lý Applications và mang lại giá trị độc lập trước khi có các thao tác thay đổi dữ liệu.

**Independent Test**: Đăng nhập bằng tài khoản có quyền, chọn **Application**, rồi xác nhận dữ liệu từ Applications được hiển thị và có thể lọc, sắp xếp, chuyển trang.

**Acceptance Scenarios**:

1. **Given** người dùng đã đăng nhập và có quyền xem Applications, **When** chọn mục **Application**, **Then** màn hình chính hiển thị danh sách gồm Code, Name, Audience, trạng thái và các hành động được phép thay cho trang chào hiện tại.
2. **Given** có nhiều Applications, **When** người dùng lọc theo Code, Name, Audience hoặc trạng thái, **Then** chỉ các bản ghi phù hợp được hiển thị.
3. **Given** kết quả vượt quá kích thước một trang, **When** người dùng đổi trang, kích thước trang hoặc thứ tự sắp xếp, **Then** màn hình hiển thị đúng nhóm kết quả tương ứng.
4. **Given** không có bản ghi phù hợp, **When** danh sách được tải hoặc bộ lọc được áp dụng, **Then** màn hình hiển thị trạng thái rỗng rõ ràng và không báo lỗi giả.

---

### User Story 2 - Tạo Application (Priority: P1)

Người quản trị có quyền tạo một Application mới bằng thông tin nhận diện và mô tả cần thiết.

**Why this priority**: Tạo Application là thao tác cốt lõi để đăng ký một hệ thống mới vào phạm vi quản lý danh tính và phân quyền.

**Independent Test**: Mở form tạo mới, nhập dữ liệu hợp lệ, lưu và xác nhận Application mới xuất hiện trong danh sách cũng như còn tồn tại sau khi tải lại màn hình.

**Acceptance Scenarios**:

1. **Given** người dùng có quyền tạo Application, **When** nhập Code, Name, Audience hợp lệ và Description tùy chọn rồi lưu, **Then** Application được tạo ở trạng thái hoạt động và xuất hiện trong danh sách.
2. **Given** form thiếu trường bắt buộc, vượt độ dài hoặc Code đã tồn tại, **When** người dùng lưu, **Then** từng trường lỗi được chỉ rõ và không có bản ghi nào được tạo.
3. **Given** yêu cầu tạo đang được xử lý, **When** người dùng tiếp tục bấm lưu, **Then** hệ thống ngăn gửi trùng lặp và thể hiện trạng thái đang xử lý.

---

### User Story 3 - Cập nhật Application (Priority: P2)

Người quản trị có quyền chỉnh sửa thông tin và trạng thái của một Application hiện có.

**Why this priority**: Cho phép sửa thông tin nhận diện, mô tả hoặc ngừng sử dụng Application mà không phải tạo lại và cấu hình lại các liên kết.

**Independent Test**: Chọn một Application, sửa dữ liệu hợp lệ, lưu, tải lại màn hình và xác nhận thay đổi được giữ nguyên trong Applications.

**Acceptance Scenarios**:

1. **Given** người dùng có quyền cập nhật, **When** lưu Name, Audience, Description hoặc trạng thái hợp lệ, **Then** đúng Application được cập nhật và danh sách phản ánh dữ liệu mới.
2. **Given** dữ liệu cập nhật không hợp lệ hoặc Code mới trùng Application khác, **When** lưu, **Then** dữ liệu không thay đổi và lỗi được chỉ rõ tại trường liên quan.
3. **Given** Application đã bị thay đổi hoặc xóa sau khi form được mở, **When** người dùng lưu phiên bản cũ, **Then** hệ thống không ghi đè âm thầm và hướng dẫn tải dữ liệu mới nhất.

---

### User Story 4 - Xóa Application (Priority: P3)

Người quản trị có quyền loại bỏ một Application không còn sử dụng sau khi xác nhận rõ đối tượng và ảnh hưởng của thao tác.

**Why this priority**: Hoàn thiện vòng đời Application nhưng có rủi ro ảnh hưởng dữ liệu liên quan cao hơn thao tác xem, tạo và sửa.

**Independent Test**: Tạo một Application thử nghiệm không có liên kết phụ thuộc, xác nhận xóa và kiểm tra bản ghi không còn xuất hiện trong danh sách mặc định hoặc được sử dụng cho hoạt động mới.

**Acceptance Scenarios**:

1. **Given** người dùng có quyền xóa, **When** chọn xóa một Application, **Then** hệ thống yêu cầu xác nhận và hiển thị Code cùng Name để nhận diện đúng đối tượng.
2. **Given** hộp thoại xác nhận đang mở, **When** người dùng hủy, **Then** không có dữ liệu nào thay đổi.
3. **Given** Application không có ràng buộc ngăn xóa, **When** người dùng xác nhận, **Then** Application được đánh dấu đã xóa, không còn trong danh sách mặc định và hệ thống thông báo thành công.
4. **Given** Application có dữ liệu phụ thuộc khiến thao tác không an toàn, **When** người dùng xác nhận xóa, **Then** hệ thống giữ nguyên dữ liệu và giải thích rằng Application chưa thể xóa mà không tiết lộ chi tiết nội bộ.

### Edge Cases

- Phiên đăng nhập hết hạn trong lúc tải danh sách hoặc gửi form: thao tác thất bại an toàn, không giả định đã lưu và người dùng được hướng dẫn đăng nhập lại.
- Người dùng truy cập trực tiếp `/applications` nhưng không có quyền: không hiển thị dữ liệu và không cung cấp các hành động quản trị.
- Mạng hoặc dịch vụ tạm thời không khả dụng: giữ dữ liệu form chưa gửi, hiển thị lỗi có thể thử lại và không tạo thay đổi trùng lặp.
- Code chỉ khác chữ hoa/chữ thường với bản ghi đã có: được xem là trùng.
- Code, Name hoặc Audience có khoảng trắng đầu/cuối: khoảng trắng đầu/cuối được loại bỏ trước khi kiểm tra và lưu; giá trị chỉ gồm khoảng trắng bị xem là rỗng.
- Description để trống: được chấp nhận và không hiển thị nội dung thay thế gây hiểu nhầm.
- Hai người quản trị đồng thời sửa hoặc xóa cùng một Application: thay đổi mới hơn không bị ghi đè âm thầm và người thao tác sau nhận được kết quả rõ ràng.
- Application đã ngừng hoạt động: vẫn có thể được tìm thấy khi lọc theo trạng thái nhưng không được dùng cho hoạt động nghiệp vụ mới.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Hệ thống MUST sử dụng mục **Application** có sẵn trong menu làm điểm truy cập màn hình quản lý Applications.
- **FR-002**: Khi chọn mục **Application**, hệ thống MUST thay nội dung trang chào hiện tại bằng màn hình quản lý Applications và thể hiện rõ mục điều hướng đang được chọn.
- **FR-003**: Người dùng đã đăng nhập MUST có thể xem, tạo, cập nhật và xóa Application; người chưa đăng nhập MUST bị từ chối an toàn ở cả giao diện và điểm xử lý dữ liệu.
- **FR-004**: Danh sách MUST đọc từ tập Applications và hiển thị tối thiểu Code, Name, Audience, trạng thái hoạt động, ngày tạo và các hành động mà người dùng được phép thực hiện.
- **FR-005**: Người dùng MUST có thể lọc danh sách theo Code, Name, Audience và trạng thái hoạt động.
- **FR-006**: Người dùng MUST có thể sắp xếp danh sách theo các cột được hỗ trợ và chuyển trang bằng các kích thước trang hợp lệ.
- **FR-007**: Màn hình MUST có các trạng thái tải, rỗng, lỗi và thử lại rõ ràng; một lỗi đọc dữ liệu MUST NOT xóa kết quả hợp lệ đang hiển thị nếu kết quả đó vẫn còn dùng được.
- **FR-008**: Người dùng có quyền MUST có thể tạo Application với Code, Name, Audience, Description tùy chọn và trạng thái hoạt động mặc định.
- **FR-009**: Code MUST bắt buộc, dài từ 1 đến 50 ký tự sau khi loại bỏ khoảng trắng đầu/cuối và duy nhất không phân biệt chữ hoa/chữ thường.
- **FR-010**: Name MUST bắt buộc, dài từ 1 đến 150 ký tự sau khi loại bỏ khoảng trắng đầu/cuối.
- **FR-011**: Audience MUST bắt buộc, dài từ 1 đến 150 ký tự sau khi loại bỏ khoảng trắng đầu/cuối.
- **FR-012**: Description MUST không vượt quá 500 ký tự khi được cung cấp.
- **FR-013**: Hệ thống MUST kiểm tra các quy tắc dữ liệu khi tạo và cập nhật, trả lỗi gắn với trường phù hợp và MUST NOT lưu một phần khi có lỗi.
- **FR-014**: Người dùng có quyền MUST có thể cập nhật Code, Name, Audience, Description và trạng thái hoạt động của Application hiện có.
- **FR-015**: Mọi thao tác tạo, cập nhật và xóa thành công MUST cập nhật trực tiếp dữ liệu tương ứng trong Applications và kết quả đọc tiếp theo MUST phản ánh thay đổi đã cam kết.
- **FR-016**: Trước khi xóa, hệ thống MUST yêu cầu xác nhận với Code và Name của Application.
- **FR-017**: Xóa Application MUST là xóa mềm; bản ghi đã xóa MUST bị loại khỏi danh sách mặc định và không được dùng cho hoạt động nghiệp vụ mới.
- **FR-018**: Hệ thống MUST từ chối xóa khi việc xóa làm mất tính toàn vẹn của dữ liệu phụ thuộc và MUST giữ nguyên Application trong trường hợp đó.
- **FR-019**: Hệ thống MUST phát hiện thay đổi đồng thời khi cập nhật hoặc xóa và MUST NOT ghi đè âm thầm dữ liệu mới hơn.
- **FR-020**: Màn hình MUST ngăn gửi lặp thao tác thay đổi khi yêu cầu trước đang xử lý và MUST thông báo kết quả thành công hoặc thất bại rõ ràng.
- **FR-021**: Các thao tác thành công MUST ghi nhận thời điểm và danh tính người tạo hoặc cập nhật theo cơ chế audit hiện có.
- **FR-022**: Giao diện MUST hỗ trợ bàn phím, nhãn có ý nghĩa, focus nhìn thấy được, trạng thái disabled/loading/error rõ ràng và vẫn sử dụng được trên các kích thước desktop và mobile được dự án hỗ trợ.

### Key Entities

- **Application**: Đại diện cho một hệ thống được quản lý danh tính và phân quyền; gồm Code duy nhất, Name, Audience, Description tùy chọn, trạng thái hoạt động/xóa và thông tin audit.
- **Application dependency**: Dữ liệu nghiệp vụ liên kết với một Application, chẳng hạn vai trò, tài nguyên hoặc menu; quyết định liệu Application có thể được xóa an toàn.
- **Authorized administrator**: Người dùng đã đăng nhập có một hoặc nhiều quyền xem, tạo, cập nhật hoặc xóa Applications.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Ít nhất 95% người dùng có quyền hoàn thành việc tìm một Application theo Code, Name hoặc Audience trong không quá 30 giây ở lần thử đầu tiên.
- **SC-002**: Ít nhất 95% thao tác tạo hoặc cập nhật với dữ liệu hợp lệ được người dùng hoàn thành trong không quá 2 phút.
- **SC-003**: 100% dữ liệu không hợp lệ trong các kịch bản chấp nhận bị từ chối mà không tạo ra thay đổi một phần hoặc bản ghi trùng.
- **SC-004**: 100% người dùng không có quyền bị ngăn xem hoặc thay đổi Applications, kể cả khi truy cập trực tiếp địa chỉ màn hình.
- **SC-005**: Với tối đa 10.000 Applications, 95% lượt mở, lọc, sắp xếp hoặc chuyển trang hiển thị kết quả cho người dùng trong không quá 2 giây dưới tải vận hành bình thường.
- **SC-006**: 100% thao tác xóa bị hủy, bị từ chối do phụ thuộc hoặc gặp xung đột đồng thời giữ dữ liệu nhất quán và cung cấp kết quả rõ ràng cho người dùng.
- **SC-007**: Tất cả kịch bản CRUD chính có thể hoàn thành chỉ bằng bàn phím và vẫn sử dụng được ở các kích thước màn hình được dự án hỗ trợ.

## Assumptions

- Menu **Application** và địa chỉ `/applications` hiện có được tái sử dụng; không tạo thêm mục điều hướng mới.
- Cơ chế đăng nhập, phiên làm việc và phân quyền hiện có được tái sử dụng; việc định nghĩa tên quyền cụ thể thuộc giai đoạn thiết kế.
- Code là định danh nghiệp vụ có thể chỉnh sửa nhưng phải duy nhất không phân biệt chữ hoa/chữ thường; không có quy tắc ký tự bổ sung ngoài độ dài và không rỗng trong phạm vi feature này.
- Audience là chuỗi định danh đối tượng nhận hiện có của Application; feature này không quản lý một danh mục Audience riêng và không yêu cầu Audience phải duy nhất.
- Vô hiệu hóa bằng trạng thái hoạt động là thao tác khác với xóa mềm; bản ghi vô hiệu hóa vẫn hiển thị khi người dùng chọn bộ lọc phù hợp.
- Dữ liệu liên kết hiện có được bảo toàn; feature không bao gồm CRUD Roles, Resources, Permissions hoặc Menus.
- Yêu cầu CQRS là ràng buộc thiết kế bắt buộc cho giai đoạn lập kế hoạch và triển khai; hành vi người dùng vẫn được mô tả độc lập với cấu trúc kỹ thuật.
