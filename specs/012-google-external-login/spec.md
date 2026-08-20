# Feature Specification: Đăng nhập bằng Google

**Feature Branch**: `012-google-external-login`

**Created**: 2026-08-20

**Status**: Implemented

**Input**: Cho phép đăng nhập Google, bắt buộc email đã xác minh, tự tạo User và gán Role mặc định; lưu liên kết trong external_identities.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Đăng nhập bằng Google lần đầu (Priority: P1)

Người dùng chọn đăng nhập Google, xác nhận tài khoản có email hợp lệ và được tạo tài khoản nội bộ với quyền mặc định.

**Why this priority**: Đây là giá trị chính giúp người dùng truy cập mà không cần mật khẩu nội bộ.

**Independent Test**: Dùng một Google identity chưa từng liên kết, hoàn tất xác thực và xác nhận User, liên kết ngoài, Role mặc định cùng phiên nội bộ được tạo toàn vẹn.

**Acceptance Scenarios**:

1. **Given** Google identity mới có email đã xác minh, **When** callback hợp lệ, **Then** hệ thống tạo User không có mật khẩu, lưu external identity, gán Role mặc định và bắt đầu phiên nội bộ.
2. **Given** Google không trả email hoặc email chưa xác minh, **When** callback hoàn tất, **Then** hệ thống từ chối và không tạo dữ liệu một phần.
3. **Given** Role mặc định hoặc Application không khả dụng, **When** User đăng nhập lần đầu, **Then** hệ thống từ chối an toàn và không tạo User.

---

### User Story 2 - Đăng nhập lại bằng Google (Priority: P1)

Người dùng đã liên kết Google đăng nhập lại vào đúng User nội bộ và nhận Roles/Permissions hiện hành.

**Why this priority**: External identity phải ổn định qua các lần email hoặc hồ sơ Google thay đổi.

**Independent Test**: Đăng nhập lại bằng cùng Google subject sau khi thay đổi tên/email hiển thị và xác nhận vẫn dùng đúng User đã liên kết.

**Acceptance Scenarios**:

1. **Given** external identity đang hoạt động, **When** đăng nhập lại, **Then** hệ thống dùng cặp Provider–Subject để tìm User và cập nhật thời điểm đăng nhập gần nhất.
2. **Given** User hoặc external identity bị vô hiệu hóa/xóa, **When** Google xác thực thành công, **Then** hệ thống vẫn từ chối đăng nhập.
3. **Given** quyền của User đã thay đổi, **When** đăng nhập lại, **Then** token nội bộ phản ánh Roles/Permissions mới nhất.
4. **Given** Google xác thực thành công và phiên nội bộ được phát hành, **When** trình duyệt quay về màn hình ứng dụng, **Then** ứng dụng khôi phục phiên và chuyển người dùng tới màn hình được phép mà không yêu cầu đăng nhập lại.

---

### User Story 3 - Ngăn tự động liên kết email trùng (Priority: P1)

Hệ thống bảo vệ User hiện có khỏi việc bị một Google identity chưa biết chiếm quyền chỉ vì có cùng email.

**Why this priority**: Liên kết ngầm theo email có thể tạo lỗ hổng account takeover.

**Independent Test**: Chuẩn bị User có cùng email nhưng chưa có external identity, đăng nhập Google và xác nhận hệ thống không tự liên kết.

**Acceptance Scenarios**:

1. **Given** email Google đã thuộc User khác nhưng Google subject chưa liên kết, **When** đăng nhập, **Then** hệ thống từ chối và không thay đổi User hiện có.
2. **Given** hai callback đồng thời cho cùng Google subject, **When** provisioning diễn ra, **Then** chỉ một User và một external identity được tạo.

### Edge Cases

- Người dùng hủy consent hoặc provider trả lỗi.
- State/correlation cookie thiếu, hết hạn hoặc không khớp.
- Callback bị gửi lại sau khi đã hoàn tất.
- Email khác biệt chữ hoa/thường so với dữ liệu đã lưu.
- Provider subject vượt giới hạn hoặc claims bắt buộc bị thiếu.
- Gán Role thất bại giữa transaction provisioning.
- User được vô hiệu hóa trong lúc callback đang xử lý.
- Provider trả bằng chứng email đã xác minh dưới một tên trường hợp lệ khác nhau giữa các phiên bản giao thức.
- Người dùng mở màn hình login khi chưa có refresh session; hệ thống trả trạng thái chưa xác thực bình thường, không coi đây là lỗi xử lý nội bộ.
- Frontend và Identity service dùng origin hoặc scheme khác nhau trong môi trường phát triển; session cookie vẫn phải hoạt động đúng theo chính sách an toàn của môi trường đó.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Màn hình login MUST cung cấp lựa chọn đăng nhập Google rõ ràng và có trạng thái lỗi khả dụng.
- **FR-002**: Hệ thống MUST dùng authorization code flow qua backend với state/correlation protection và redirect URI cố định trong allowlist.
- **FR-003**: Hệ thống MUST yêu cầu định danh provider, email và bằng chứng email đã xác minh trước khi provisioning; các biểu diễn hợp lệ của trạng thái xác minh do provider hỗ trợ MUST được diễn giải nhất quán.
- **FR-004**: External identity MUST được nhận diện duy nhất bằng Provider và Provider Subject, không bằng email.
- **FR-005**: Lần đăng nhập đầu MUST tạo User, external identity và quan hệ Role mặc định trong một transaction.
- **FR-006**: Social User MUST không có mật khẩu nội bộ và không thể dùng login Code/password cho đến khi có luồng thiết lập mật khẩu riêng.
- **FR-007**: Code nội bộ của User được tạo từ Google MUST dùng chính email Google đã xác minh, sau khi loại bỏ khoảng trắng ngoài, và MUST duy nhất không phân biệt hoa/thường.
- **FR-008**: Hệ thống MUST không tự liên kết Google identity mới vào User hiện có chỉ vì email trùng.
- **FR-009**: Email trùng với User hiện có MUST trả lỗi an toàn, không tiết lộ chi tiết tài khoản.
- **FR-010**: Role mặc định MUST thuộc đúng Application, đang hoạt động và không bị xóa; hệ thống MUST không mặc định cấp Role quản trị.
- **FR-011**: External identity MUST lưu User, Provider, Provider Subject, email tại thời điểm liên kết, tên hiển thị, avatar tùy chọn và thời điểm đăng nhập gần nhất.
- **FR-012**: Mỗi User MUST có tối đa một external identity Google và mỗi Google subject MUST liên kết tối đa một User.
- **FR-013**: User hoặc external identity không hoạt động/đã xóa MUST bị từ chối dù Google xác thực thành công.
- **FR-014**: Sau external login thành công, hệ thống MUST phát hành refresh session và JWT nội bộ theo cùng quy tắc login hiện có, sau đó cho phép client khôi phục phiên khi quay lại ứng dụng.
- **FR-015**: Provider token, authorization code, client secret và claims nhạy cảm MUST NOT xuất hiện trong URL frontend, log hoặc phản hồi API.
- **FR-016**: Client ID, client secret, Role mặc định, Application và frontend callback MUST được cấu hình theo môi trường.
- **FR-017**: Endpoint bắt đầu/callback external login MUST chịu rate limit và lỗi MUST không tạo dữ liệu một phần.
- **FR-018**: Chính sách session cookie MUST tương thích với origin/scheme được cấu hình cho từng môi trường, đồng thời MUST giữ các thuộc tính bảo vệ bắt buộc trong môi trường triển khai thực tế.
- **FR-019**: Yêu cầu khôi phục phiên khi không có refresh cookie MUST trả trạng thái chưa xác thực có kiểm soát và MUST NOT phát sinh lỗi xử lý nội bộ.
- **FR-020**: Lỗi external login MUST được ghi nhận đủ để phân biệt lỗi xác thực provider, provisioning tài khoản và phát hành phiên, nhưng MUST NOT ghi token, secret hoặc dữ liệu nhận dạng đầy đủ.

### Key Entities

- **User**: Tài khoản nội bộ được tạo với Code bằng email Google đã xác minh và không có mật khẩu ban đầu.
- **External Identity**: Liên kết giữa User nội bộ và định danh ổn định của Google.
- **Default Role Assignment**: Quan hệ cấp quyền tối thiểu cho User mới trong Application cấu hình.
- **Internal Session**: Access token và refresh token do Identity system phát hành sau khi provisioning thành công.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Người dùng mới có email Google hợp lệ hoàn tất đăng nhập và vào màn hình được phép trong dưới 10 giây sau consent ở điều kiện bình thường.
- **SC-002**: 100% callback thiếu email, email chưa xác minh hoặc sai state bị từ chối mà không tạo User.
- **SC-003**: Không xuất hiện User, external identity hoặc quan hệ Role trùng trong kiểm thử callback đồng thời.
- **SC-004**: 100% Google identity đã liên kết đăng nhập lại vào đúng User dựa trên Provider Subject, kể cả khi email hiển thị thay đổi.
- **SC-005**: 100% trường hợp email trùng nhưng chưa liên kết bị từ chối, không tự chiếm User hiện có.
- **SC-006**: Social User mới chỉ nhận đúng Role mặc định đã cấu hình và không nhận quyền quản trị ngoài cấu hình.
- **SC-007**: 100% lần đăng nhập Google thành công trong các môi trường được hỗ trợ khôi phục được phiên nội bộ sau redirect mà không yêu cầu người dùng đăng nhập lần hai.
- **SC-008**: 100% yêu cầu khôi phục phiên không có cookie trả trạng thái chưa xác thực dự kiến và không tạo exception chưa xử lý.

## Assumptions

- Giai đoạn này chỉ hỗ trợ Google; mô hình dữ liệu cho phép thêm provider khác sau này.
- Tài khoản Google phải cung cấp email đã xác minh.
- Role mặc định và Application đã được quản trị viên tạo trước.
- Mỗi môi trường cung cấp frontend URL, callback URL và chính sách cookie phù hợp với scheme/origin thực tế; môi trường production sử dụng kết nối bảo mật.
- Liên kết Google vào User hiện có và thiết lập mật khẩu cho social User nằm ngoài phạm vi phiên bản này.
