# Feature Specification: Quản lý User theo Role

**Feature Branch**: `008-user-roles`

**Created**: 2026-08-12

**Status**: Draft

**Input**: Chức năng user-roles có phân quyền đọc, gán và gỡ; màn hình hai cột gồm danh sách Roles và Users thuộc Role đang active, hỗ trợ thêm nhiều Users qua dialog.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Xem Users theo Role (Priority: P1)

Người dùng đã đăng nhập mở màn hình **User Roles**, xem danh sách Roles ở cột thứ nhất và danh sách Users thuộc Role đang active ở cột thứ hai.

**Why this priority**: Đây là luồng nền tảng để người dùng biết thành viên hiện tại của từng Role trước khi thay đổi.

**Independent Test**: Chuẩn bị nhiều Roles với tập Users đã biết, lần lượt chọn từng Role và xác nhận cột Users chỉ hiển thị đúng thành viên của Role active.

**Acceptance Scenarios**:

1. **Given** có ít nhất một Role, **When** người dùng mở màn hình, **Then** màn hình hiển thị đúng hai cột, tự chọn Role khả dụng đầu tiên và tải Users của Role đó.
2. **Given** một Role đang active, **When** người dùng chọn Role khác, **Then** trạng thái active chuyển sang Role mới và danh sách Users được tải lại theo Role mới.
3. **Given** Role active chưa có thành viên, **When** danh sách Users được tải, **Then** cột Users hiển thị trạng thái rỗng rõ ràng và vẫn cho phép thêm User.
4. **Given** người dùng điều hướng bằng bàn phím, **When** focus và kích hoạt một dòng Role, **Then** dòng active nhìn thấy rõ và danh sách Users cập nhật như khi dùng chuột.
5. **Given** Roles thuộc nhiều Applications hoặc có Code/Name giống nhau, **When** danh sách Roles được hiển thị, **Then** mỗi dòng thể hiện rõ Application Code và Application Name để người dùng chọn đúng phạm vi.

---

### User Story 2 - Thêm nhiều Users vào Role (Priority: P1)

Người dùng nhấn **Add User** tại Role active để mở dialog danh sách Users, chọn một hoặc nhiều Users bằng checkbox và nhấn **Save** để thêm họ vào Role.

**Why this priority**: Thêm thành viên là giá trị nghiệp vụ chính và chọn nhiều giúp giảm thao tác lặp lại.

**Independent Test**: Chọn ba Users chưa thuộc Role trong dialog, lưu, tải lại màn hình và xác nhận cả ba xuất hiện đúng một lần trong Role active.

**Acceptance Scenarios**:

1. **Given** một Role đang active, **When** người dùng nhấn **Add User**, **Then** dialog hiển thị danh sách Users chưa thuộc Role cùng checkbox chọn nhiều.
2. **Given** dialog đang mở và chưa chọn User nào, **When** người dùng xem hành động lưu, **Then** nút **Save** bị vô hiệu hóa.
3. **Given** người dùng đã chọn nhiều Users hợp lệ, **When** nhấn **Save**, **Then** hệ thống lưu toàn bộ quan hệ hợp lệ, đóng dialog và làm mới danh sách Users của Role active.
4. **Given** yêu cầu lưu đang xử lý, **When** người dùng tiếp tục tương tác, **Then** hệ thống ngăn gửi trùng và thể hiện trạng thái đang lưu.
5. **Given** việc lưu thất bại, **When** hệ thống trả lỗi, **Then** dialog giữ lựa chọn để người dùng thử lại, không hiển thị quan hệ chưa được cam kết và đưa ra thông báo có thể hành động.

---

### User Story 3 - Xóa User khỏi Role (Priority: P1)

Người dùng nhấn nút xóa ở một dòng User để gỡ User đó khỏi Role active mà không xóa tài khoản User.

**Why this priority**: Gỡ thành viên sai hoặc không còn phù hợp là thao tác thiết yếu để duy trì cấu hình Role chính xác.

**Independent Test**: Xóa một User khỏi Role, tải lại màn hình và xác nhận User không còn trong Role nhưng tài khoản vẫn tồn tại và có thể xuất hiện trong dialog thêm User.

**Acceptance Scenarios**:

1. **Given** User thuộc Role active, **When** người dùng nhấn nút xóa trên dòng và xác nhận, **Then** chỉ quan hệ giữa User và Role active bị xóa và danh sách được làm mới.
2. **Given** người dùng hủy xác nhận xóa, **When** dialog xác nhận đóng, **Then** không có dữ liệu nào thay đổi.
3. **Given** thao tác xóa thất bại, **When** hệ thống trả lỗi, **Then** User vẫn hiển thị trong Role và người dùng nhận thông báo có thể hành động.

### Edge Cases

- Không có Role: cột Roles hiển thị trạng thái rỗng; cột Users không cho thêm hoặc xóa.
- Role active bị xóa hoặc ngừng khả dụng trong lúc màn hình mở: hệ thống bỏ lựa chọn lỗi thời và chọn lại Role khả dụng đầu tiên nếu có.
- User bị xóa hoặc ngừng khả dụng khi dialog đang mở: hệ thống không tạo quan hệ lỗi thời, báo kết quả và tải lại danh sách lựa chọn.
- User đã được thêm bởi phiên khác trước khi lưu: hệ thống không tạo quan hệ trùng, coi quan hệ đã tồn tại là trạng thái hợp lệ và làm mới danh sách.
- Quan hệ đã bị xóa bởi phiên khác: yêu cầu xóa không gây lỗi dữ liệu và danh sách cuối cùng phản ánh trạng thái hiện tại.
- Người dùng đổi Role liên tiếp khi dữ liệu đang tải: chỉ kết quả của Role được chọn mới nhất được hiển thị.
- Phiên đăng nhập hết hạn khi tải hoặc lưu: thao tác thất bại an toàn, không hiển thị thành công sai và người dùng được hướng dẫn đăng nhập lại.
- Danh sách Roles hoặc Users lớn: người dùng có thể tìm kiếm hoặc duyệt theo cơ chế danh sách chuẩn của dự án mà không phải tải toàn bộ dữ liệu cùng lúc.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Hệ thống MUST cung cấp màn hình **User Roles** gồm đúng hai cột chính: Roles và Users thuộc Role active.
- **FR-002**: Hệ thống MUST yêu cầu `UserRoles.Read` để xem thành viên và ứng viên, `UserRoles.Create` để gán Users, và `UserRoles.Delete` để gỡ User khỏi Role; người chưa đăng nhập hoặc thiếu quyền MUST bị từ chối an toàn.
- **FR-003**: Cột Roles MUST hiển thị Role Code, Role Name, Application Code và Application Name cho từng Role, thể hiện rõ dòng active và cho phép chọn bằng chuột hoặc bàn phím.
- **FR-004**: Khi mở màn hình, hệ thống MUST tự chọn Role khả dụng đầu tiên theo thứ tự hiển thị ổn định nếu có.
- **FR-005**: Cột Users MUST chỉ hiển thị Users đang thuộc Role active và MUST làm mới khi Role active thay đổi.
- **FR-006**: Mỗi dòng User thuộc Role MUST có nút xóa với nhãn truy cập có ý nghĩa.
- **FR-007**: Trước khi xóa, hệ thống MUST yêu cầu người dùng xác nhận và nêu rõ User cùng Role bị ảnh hưởng.
- **FR-008**: Xóa User khỏi Role MUST chỉ xóa quan hệ User–Role tương ứng, không xóa hoặc sửa tài khoản User.
- **FR-009**: Cột Users MUST có nút **Add User**, chỉ khả dụng khi có Role active.
- **FR-010**: Nút **Add User** MUST mở dialog hiển thị các Users chưa thuộc Role active và cho phép chọn nhiều bằng checkbox.
- **FR-011**: Dialog MUST hỗ trợ chọn, bỏ chọn, đóng không lưu và lưu các Users đã chọn; nút **Save** MUST bị vô hiệu khi không có lựa chọn hoặc khi đang lưu.
- **FR-012**: Một lần lưu MUST tạo quan hệ User–Role cho toàn bộ Users hợp lệ đã chọn và MUST không tạo quan hệ trùng.
- **FR-013**: Sau khi thêm hoặc xóa thành công, hệ thống MUST làm mới danh sách của Role active để phản ánh dữ liệu đã cam kết.
- **FR-014**: Hệ thống MUST hiển thị trạng thái tải, rỗng, đang lưu, thành công và lỗi phù hợp cho từng luồng.
- **FR-015**: Khi thao tác thêm nhiều Users chỉ thành công một phần do dữ liệu đồng thời thay đổi, hệ thống MUST báo rõ kết quả, không tạo bản ghi trùng hoặc tham chiếu lỗi thời, và làm mới dữ liệu thực tế.
- **FR-016**: Chức năng MUST giữ khả năng sử dụng bằng bàn phím, focus nhìn thấy được và nhãn điều khiển rõ ràng trên các kích thước màn hình được hỗ trợ.
- **FR-017**: Phạm vi chức năng MUST không bao gồm tạo, sửa, xóa Role hoặc tạo, sửa, xóa tài khoản User.

### Key Entities

- **Role**: Vai trò được hiển thị và chọn ở cột thứ nhất; là phía nhận thành viên trong quan hệ.
- **User**: Tài khoản có thể là thành viên của không, một hoặc nhiều Roles; hiển thị ở cột thứ hai hoặc trong dialog lựa chọn.
- **User Role Membership**: Quan hệ duy nhất giữa một User và một Role; sự tồn tại của quan hệ quyết định User có xuất hiện trong danh sách thành viên của Role hay không.
- **Authorized administrator**: Người vận hành có phiên hợp lệ và quyền UserRoles tương ứng với thao tác cần thực hiện.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Ít nhất 95% người dùng xác định được các thành viên của một Role trong không quá 30 giây ở lần thử đầu tiên.
- **SC-002**: Ít nhất 95% lượt chọn Role hiển thị đúng danh sách thành viên trong không quá 2 giây dưới tải vận hành bình thường.
- **SC-003**: Người dùng có thể thêm ít nhất 20 Users vào một Role trong một lần lưu và hoàn thành trong không quá 60 giây.
- **SC-004**: 100% thao tác thêm hoặc xóa thành công vẫn phản ánh đúng sau khi tải lại màn hình.
- **SC-005**: 100% kịch bản đồng thời được kiểm thử không tạo quan hệ trùng, không xóa tài khoản và không báo thành công sai.
- **SC-006**: 100% luồng chọn Role, mở dialog, chọn nhiều, lưu, xác nhận xóa và hủy có thể hoàn thành chỉ bằng bàn phím.
- **SC-007**: 100% yêu cầu thiếu quyền UserRoles tương ứng bị từ chối an toàn; người có quyền chỉ thực hiện được đúng thao tác đọc, gán hoặc gỡ đã cấp.

## Assumptions

- “Không cần phân quyền” nghĩa là không kiểm tra permission chức năng riêng, nhưng vẫn yêu cầu phiên đăng nhập hợp lệ theo constitution.
- Màn hình dùng tên **User Roles** và địa chỉ `/user-roles` theo quy ước chức năng hiện có.
- Role đầu tiên theo thứ tự danh sách ổn định được chọn mặc định khi mở màn hình.
- Dialog chỉ hiển thị Users chưa thuộc Role active để giảm lỗi chọn trùng.
- Việc xóa yêu cầu xác nhận vì làm thay đổi cấu hình truy cập, dù không xóa tài khoản.
- Bảng quan hệ User–Role hiện có là nguồn dữ liệu chuẩn; mỗi cặp User–Role là duy nhất.
- Danh sách lớn dùng cơ chế tìm kiếm và phân trang theo chuẩn hiện có của dự án; chi tiết được xác định ở bước lập kế hoạch.
