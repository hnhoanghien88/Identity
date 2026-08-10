# Feature Specification: Quản lý Users

**Feature Branch**: `001-manage-users`

**Created**: 2026-08-10

**Updated**: 2026-08-10

**Status**: Draft

**Input**: Cập nhật 001-manage-users: bảng Users hiển thị Code thay cho Email; Code viết liền và không dùng Unicode; form tạo/sửa bắt buộc nhập Email; đăng nhập bằng Code và mật khẩu; loại bỏ các thuộc tính NormalizedEmail và NormalizedCode khỏi User.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Đăng nhập bằng Code (Priority: P1)

Người dùng nhập Code và mật khẩu để được chứng thực, sau đó truy cập các chức năng đúng với vai trò và quyền của tài khoản.

**Why this priority**: Chứng thực là điểm vào bắt buộc và phải xác lập đúng danh tính trước khi cấp quyền truy cập.

**Independent Test**: Nhập Code và mật khẩu hợp lệ của tài khoản đang hoạt động và xác nhận phiên làm việc được thiết lập với đúng quyền.

**Acceptance Scenarios**:

1. **Given** tài khoản đang hoạt động, **When** nhập đúng Code và mật khẩu, **Then** hệ thống chứng thực thành công và thiết lập phiên làm việc.
2. **Given** người dùng nhập Email thay cho Code, Code không tồn tại hoặc mật khẩu sai, **When** gửi form, **Then** hệ thống từ chối bằng thông báo chung và không tiết lộ Code có tồn tại hay không.
3. **Given** tài khoản bị vô hiệu hóa hoặc xóa, **When** nhập đúng Code và mật khẩu, **Then** hệ thống không cấp phiên làm việc.
4. **Given** yêu cầu đang xử lý, **When** tiếp tục bấm đăng nhập, **Then** hệ thống không gửi yêu cầu trùng lặp.

---

### User Story 2 - Xem và tìm danh sách Users (Priority: P1)

Người quản trị chọn mục **Users** ở menu cột trái để mở màn hình quản lý, xem danh sách tài khoản và tìm nhanh tài khoản cần xử lý.

**Why this priority**: Danh sách là điểm vào của toàn bộ nghiệp vụ quản lý và tạo giá trị ngay cả khi chưa có các thao tác thay đổi dữ liệu.

**Independent Test**: Đăng nhập bằng tài khoản có quyền, chọn **Users**, xác nhận danh sách hiển thị đúng các trường và có thể lọc, sắp xếp, chuyển trang.

**Acceptance Scenarios**:

1. **Given** người dùng đã đăng nhập và có quyền xem Users, **When** chọn mục **Users**, **Then** danh sách hiển thị Code, tên, ngày tạo, trạng thái và hành động; Email không thay thế cột Code.
2. **Given** có nhiều Users, **When** lọc theo Code, tên hoặc trạng thái, **Then** chỉ các bản ghi phù hợp được hiển thị.
3. **Given** kết quả vượt quá kích thước một trang, **When** người quản trị đổi trang hoặc kích thước trang, **Then** đúng nhóm kết quả tương ứng được hiển thị.
4. **Given** chưa có User phù hợp, **When** danh sách được tải hoặc áp dụng bộ lọc, **Then** màn hình hiển thị trạng thái rỗng rõ ràng và không báo lỗi giả.

---

### User Story 3 - Tạo User (Priority: P1)

Người quản trị tạo tài khoản bằng Code, Email, tên hiển thị và mật khẩu ban đầu.

**Why this priority**: Tạo tài khoản là thao tác cốt lõi để đưa người dùng mới vào hệ thống.

**Independent Test**: Mở form tạo mới, nhập dữ liệu hợp lệ, lưu và xác nhận User mới xuất hiện trong danh sách mà mật khẩu không bao giờ được hiển thị lại.

**Acceptance Scenarios**:

1. **Given** người quản trị có quyền tạo User, **When** nhập Code ASCII viết liền, Email duy nhất, tên và mật khẩu hợp lệ, **Then** User được tạo và Code mới xuất hiện trong danh sách.
2. **Given** form chứa dữ liệu thiếu hoặc sai định dạng, **When** người quản trị lưu, **Then** từng trường không hợp lệ được chỉ rõ và không có User nào được tạo.
3. **Given** Code chứa khoảng trắng hoặc Unicode, hoặc Code/Email đã tồn tại, **When** lưu, **Then** hệ thống từ chối và chỉ rõ trường lỗi.
4. **Given** yêu cầu đang được xử lý, **When** người quản trị tiếp tục bấm lưu, **Then** hệ thống ngăn gửi trùng lặp và thể hiện trạng thái đang xử lý.

---

### User Story 4 - Cập nhật User (Priority: P2)

Người quản trị mở User hiện có, chỉnh sửa Code, Email hoặc tên hiển thị và lưu thay đổi.

**Why this priority**: Cho phép sửa thông tin sai hoặc thay đổi thông tin nhận diện mà không phải xóa và tạo lại tài khoản.

**Independent Test**: Chọn một User, sửa dữ liệu hợp lệ, lưu rồi tải lại màn hình và xác nhận dữ liệu mới được giữ nguyên.

**Acceptance Scenarios**:

1. **Given** người quản trị có quyền cập nhật, **When** lưu Code, Email và tên hợp lệ, **Then** đúng User được cập nhật và danh sách phản ánh Code mới.
2. **Given** Code sai quy tắc, Email thiếu/sai định dạng, hoặc Code/Email trùng User khác, **When** lưu, **Then** dữ liệu không thay đổi và lỗi được chỉ rõ tại trường liên quan.
3. **Given** User đã bị thay đổi hoặc xóa sau khi form được mở, **When** người quản trị lưu, **Then** hệ thống thông báo dữ liệu không còn khả dụng và cho phép tải lại trạng thái mới nhất.

---

### User Story 5 - Xóa User (Priority: P3)

Quản trị viên xóa một User không còn được phép sử dụng hệ thống sau khi xác nhận rõ đối tượng và hậu quả.

**Why this priority**: Hoàn thiện vòng đời tài khoản nhưng có rủi ro cao hơn các thao tác xem, tạo và sửa nên được ưu tiên sau.

**Independent Test**: Chọn xóa một User thử nghiệm, xác nhận trong hộp thoại và kiểm tra User không còn xuất hiện trong danh sách hoạt động hoặc sử dụng được để truy cập.

**Acceptance Scenarios**:

1. **Given** quản trị viên có quyền xóa, **When** chọn xóa User, **Then** hệ thống yêu cầu xác nhận và hiển thị Code cùng tên để nhận diện đúng đối tượng.
2. **Given** hộp thoại xác nhận đang mở, **When** quản trị viên hủy, **Then** không có dữ liệu nào thay đổi.
3. **Given** quản trị viên xác nhận xóa và thao tác thành công, **When** quay lại danh sách, **Then** User không còn trong danh sách hoạt động và hệ thống hiển thị thông báo thành công.
4. **Given** User không thể xóa do ràng buộc nghiệp vụ hoặc đã bị xử lý trước đó, **When** xác nhận xóa, **Then** hệ thống giữ trạng thái nhất quán và giải thích kết quả mà không tiết lộ chi tiết nội bộ.

### Edge Cases

- Phiên đăng nhập hết hạn trong khi đang tải danh sách hoặc gửi form: thao tác bị từ chối an toàn, dữ liệu nhạy cảm không bị lưu cục bộ và người dùng được hướng dẫn đăng nhập lại.
- Người dùng truy cập trực tiếp địa chỉ màn hình Users nhưng không có quyền: không hiển thị dữ liệu và không cung cấp hành động quản trị.
- Mạng hoặc dịch vụ tạm thời không khả dụng: giữ nguyên dữ liệu form không nhạy cảm, hiển thị lỗi có thể thử lại và không giả định thao tác đã thành công.
- Code chứa khoảng trắng ở bất kỳ vị trí nào, ký tự có dấu, chữ cái ngoài ASCII hoặc emoji: bị từ chối và không tự động biến đổi thành Code khác.
- Code chỉ khác nhau bởi chữ hoa/chữ thường: được xem là trùng.
- Email có khoảng trắng đầu/cuối hoặc khác biệt chữ hoa/chữ thường: khoảng trắng đầu/cuối được loại bỏ khi lưu và việc kiểm tra trùng không phân biệt chữ hoa/thường, không cần lưu thêm một bản sao đã chuẩn hóa.
- Email thay đổi nhưng Code không đổi: đăng nhập vẫn dùng Code; Email không trở thành tên đăng nhập.
- Danh sách có dữ liệu dài hoặc tên dài: nội dung vẫn đọc được, các hành động vẫn truy cập được trên kích thước màn hình được hỗ trợ.
- Người quản trị cố xóa chính tài khoản đang đăng nhập: thao tác bị từ chối để tránh tự khóa quyền quản trị ngoài ý muốn.
- Hai quản trị viên đồng thời sửa hoặc xóa cùng một User: kết quả cuối được thông báo rõ và không ghi đè âm thầm dữ liệu mới hơn.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Hệ thống MUST hiển thị mục **Users** trong menu cột trái cho người dùng có quyền truy cập chức năng quản lý Users.
- **FR-002**: Khi chọn mục **Users**, hệ thống MUST mở màn hình quản lý Users và thể hiện rõ vị trí đang chọn trong điều hướng.
- **FR-003**: Hệ thống MUST giới hạn việc xem, tạo, cập nhật và xóa User theo quyền của người dùng đang đăng nhập; mọi truy cập không được phép MUST thất bại an toàn.
- **FR-004**: Màn hình MUST hiển thị danh sách Users với tối thiểu: Code, tên hiển thị, ngày tạo, trạng thái hoạt động và các hành động được phép; cột nhận diện tài khoản MUST có nhãn **Code**, không phải **Email**.
- **FR-005**: Người quản trị MUST có thể lọc danh sách theo Code, tên hiển thị và trạng thái hoạt động.
- **FR-006**: Người quản trị MUST có thể sắp xếp danh sách theo các cột được hỗ trợ và chuyển trang với kích thước trang hợp lệ.
- **FR-007**: Hệ thống MUST cung cấp các trạng thái tải, rỗng, lỗi và thử lại rõ ràng cho danh sách.
- **FR-008**: Người quản trị có quyền MUST có thể mở form tạo User gồm Code, Email, tên hiển thị và mật khẩu ban đầu.
- **FR-009**: Code MUST có từ 1 đến 50 ký tự, viết liền và chỉ gồm ASCII: chữ cái A-Z/a-z, chữ số 0-9, dấu chấm, gạch dưới hoặc gạch ngang.
- **FR-009A**: Hệ thống MUST từ chối Code chứa khoảng trắng, Unicode hoặc ký tự ngoài tập cho phép và MUST NOT âm thầm chuyển đổi Code không hợp lệ.
- **FR-009B**: Code MUST duy nhất không phân biệt chữ hoa/thường.
- **FR-009C**: Email MUST bắt buộc khi tạo và cập nhật, đúng định dạng, tối đa 254 ký tự và duy nhất không phân biệt chữ hoa/thường.
- **FR-009D**: User MUST chỉ lưu Code và Email gốc phục vụ nghiệp vụ; hệ thống MUST loại bỏ và không phụ thuộc vào các thuộc tính NormalizedCode hoặc NormalizedEmail.
- **FR-009E**: Code và Email MUST độc lập; thay đổi một trường MUST NOT tự động thay đổi trường còn lại.
- **FR-010**: Khi tạo, mật khẩu ban đầu MUST dài từ 8 đến 128 ký tự, được che khi nhập, không xuất hiện trong danh sách, phản hồi hoặc nhật ký, và không được điền lại sau lỗi gửi yêu cầu.
- **FR-011**: Hệ thống MUST hiển thị lỗi xác thực gần trường liên quan và ngăn lưu khi dữ liệu không hợp lệ.
- **FR-012**: Sau khi tạo thành công, hệ thống MUST thông báo kết quả và làm mới danh sách để User mới có thể được tìm thấy.
- **FR-013**: Người quản trị có quyền MUST có thể mở form cập nhật với dữ liệu hiện tại và sửa Code, Email, tên hiển thị; Email là bắt buộc và cập nhật thông tin MUST NOT yêu cầu hoặc thay đổi mật khẩu.
- **FR-014**: Sau khi cập nhật thành công, hệ thống MUST thông báo kết quả và phản ánh dữ liệu mới mà không yêu cầu người dùng tải lại toàn bộ ứng dụng.
- **FR-015**: Chỉ quản trị viên có quyền xóa MUST có thể khởi tạo thao tác xóa User.
- **FR-016**: Trước khi xóa, hệ thống MUST yêu cầu xác nhận, nhận diện rõ User mục tiêu và cho phép hủy mà không thay đổi dữ liệu.
- **FR-017**: Sau khi xóa thành công, User MUST không còn trong danh sách hoạt động và không còn có thể sử dụng tài khoản để truy cập hệ thống.
- **FR-018**: Hệ thống MUST ngăn người quản trị xóa chính tài khoản đang đăng nhập.
- **FR-019**: Mọi thao tác thay đổi MUST ngăn gửi trùng trong khi chờ xử lý và MUST trình bày kết quả thành công hoặc thất bại rõ ràng.
- **FR-020**: Lỗi hiển thị cho người dùng MUST hữu ích nhưng không được tiết lộ mật khẩu, dấu vết lỗi, cấu trúc lưu trữ hoặc thông tin nhạy cảm khác.
- **FR-021**: Các điều khiển và form MUST nhất quán với hệ thống thiết kế hiện có, sử dụng được bằng bàn phím, có nhãn, tiêu điểm nhìn thấy được và trạng thái vô hiệu hóa/tải/lỗi rõ ràng.
- **FR-022**: Màn hình Users MUST sử dụng được ở các độ rộng desktop và mobile mà ứng dụng hiện hỗ trợ.
- **FR-023**: Hệ thống MUST tách biệt các yêu cầu đọc dữ liệu Users khỏi các yêu cầu làm thay đổi dữ liệu để duy trì mô hình xử lý truy vấn và lệnh độc lập.
- **FR-024**: Form đăng nhập MUST yêu cầu Code và mật khẩu; Email MUST NOT được chấp nhận thay cho Code.
- **FR-025**: Khi chứng thực, hệ thống MUST tìm tài khoản theo Code không phân biệt chữ hoa/thường, xác minh mật khẩu, trạng thái hoạt động và trạng thái xóa trước khi cấp phiên.
- **FR-026**: Đăng nhập thất bại MUST dùng thông báo chung, không tiết lộ Code tồn tại, mật khẩu sai, tài khoản bị xóa hay chi tiết nội bộ.
- **FR-027**: Sau khi chứng thực thành công, hệ thống MUST cung cấp danh tính, vai trò và quyền hiện hành cần thiết để điều hướng và bảo vệ chức năng Users.
- **FR-028**: Việc tìm Code khi đăng nhập và kiểm tra trùng Code/Email MUST không phân biệt chữ hoa/thường dù không lưu các bản sao đã chuẩn hóa.

### Key Entities

- **User**: Tài khoản gồm định danh, Code đăng nhập duy nhất, Email bắt buộc, tên hiển thị, thông tin mật khẩu bảo mật, ngày tạo và trạng thái vòng đời. User không có thuộc tính NormalizedCode hoặc NormalizedEmail.
- **Phiên chứng thực**: Kết quả xác minh Code và mật khẩu, gắn với danh tính, vai trò và quyền hiện hành của User.
- **Quyền quản lý User**: Quyết định người đang đăng nhập có thể xem, tạo, cập nhật hay xóa User; quyền xóa có mức hạn chế cao hơn thao tác đọc.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Ít nhất 90% người quản trị thử nghiệm có thể tìm và mở đúng User cần quản lý trong dưới 30 giây ở lần đầu sử dụng.
- **SC-002**: Ít nhất 95% lượt tạo hoặc cập nhật với dữ liệu hợp lệ hoàn tất trong dưới 60 giây tính từ lúc mở form đến khi thấy xác nhận thành công.
- **SC-003**: 100% Code chứa khoảng trắng, Unicode hoặc ký tự ngoài tập cho phép và 100% Email thiếu, sai định dạng hoặc trùng sau chuẩn hóa bị chặn trước khi dữ liệu không hợp lệ được lưu.
- **SC-004**: 100% thao tác xóa yêu cầu xác nhận rõ User mục tiêu và thao tác hủy không gây thay đổi dữ liệu.
- **SC-005**: 100% thử nghiệm truy cập không đủ quyền không hiển thị dữ liệu Users và không thực hiện được thao tác quản trị.
- **SC-006**: Với danh sách đến 10.000 Users, 95% lượt mở, lọc, sắp xếp hoặc chuyển trang cung cấp kết quả nhìn thấy được trong vòng 2 giây ở điều kiện vận hành bình thường.
- **SC-007**: Trong kiểm thử khả dụng, ít nhất 90% người tham gia hoàn thành độc lập cả bốn tác vụ xem, tạo, sửa và xóa mà không cần hỗ trợ.
- **SC-008**: Không có mật khẩu hoặc chi tiết lỗi nội bộ nào xuất hiện trong danh sách, thông báo giao diện hay dữ liệu phản hồi quan sát được trong toàn bộ bộ kiểm thử chấp nhận.
- **SC-009**: 100% tài khoản thử nghiệm hợp lệ đăng nhập được bằng Code và mật khẩu đúng; đăng nhập bằng Email thay cho Code bị từ chối.
- **SC-010**: 100% lần chứng thực thất bại không tiết lộ Code có tồn tại, mật khẩu sai hay trạng thái nội bộ của tài khoản.

## Assumptions

- Chức năng quản lý phục vụ người đã đăng nhập có quyền phù hợp; cơ chế phân quyền hiện có được tái sử dụng sau khi chứng thực bằng Code.
- Tập ký tự Code cho phép là [A-Za-z0-9._-]; Code không cho phép khoảng trắng ở bất kỳ vị trí nào và không cho phép Unicode.
- Code và Email đều duy nhất không phân biệt chữ hoa/thường nhưng không lưu thêm NormalizedCode hoặc NormalizedEmail.
- Email gốc sau khi loại bỏ khoảng trắng đầu/cuối được giữ để hiển thị và chỉnh sửa; Email không được dùng làm tên đăng nhập.
- User hiện có phải được gán Code hợp lệ và có Email hợp lệ trước khi bật luồng đăng nhập mới; không tự suy diễn Code từ Email vì có thể gây trùng hoặc sai ý nghĩa nghiệp vụ.
- Màn hình dùng hệ thống thiết kế Material UI và các quy ước theme, khoảng cách, thành phần dùng chung hiện có; lựa chọn thành phần cụ thể thuộc giai đoạn lập kế hoạch.
- Backend duy trì CQRS: truy vấn dành cho đọc danh sách/chi tiết và command dành cho tạo/cập nhật/xóa; chi tiết tổ chức code thuộc giai đoạn lập kế hoạch.
- Xóa tuân theo cơ chế vòng đời dữ liệu hiện có của hệ thống và vô hiệu hóa quyền truy cập ngay sau thành công; chính sách lưu giữ vật lý không thay đổi trong feature này.
- Quản lý vai trò/quyền, đặt lại hoặc đổi mật khẩu, nhập/xuất hàng loạt và kích hoạt/vô hiệu hóa thủ công không thuộc phạm vi phiên bản này.
- Các API và mô hình User CQRS hiện có là dependency đầu vào; plan sẽ xác định phần nào được tái sử dụng hoặc cần điều chỉnh để đáp ứng đầy đủ spec.
