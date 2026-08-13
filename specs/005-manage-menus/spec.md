# Feature Specification: Quản lý Menus dạng cây

**Feature Branch**: `005-manage-menus`

**Created**: 2026-08-12

**Status**: Draft

**Input**: Chức năng Menus; màn hình index hiển thị tree list nhiều cấp đệ quy dựa vào ParentId; thông tin Application và Resource lấy động từ dữ liệu tương ứng; có đầy đủ CRUD và tuân theo CQRS.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Xem và duyệt cây Menu (Priority: P1)

Người quản trị mở màn hình **Menus** để xem cấu trúc phân cấp của các menu, nhận biết quan hệ cha-con và mở hoặc thu gọn từng nhánh.

**Why this priority**: Cây phân cấp là cách chính để hiểu và quản trị menu; nếu quan hệ cha-con không rõ thì các thao tác còn lại dễ tác động nhầm vị trí.

**Independent Test**: Chuẩn bị Menu có ít nhất ba cấp, mở màn hình Menus, rồi xác nhận tất cả nút xuất hiện đúng cấp, đúng thứ tự và có thể mở/thu gọn độc lập.

**Acceptance Scenarios**:

1. **Given** người dùng có quyền xem Menus và dữ liệu có nhiều cấp, **When** mở màn hình index, **Then** hệ thống hiển thị tree list với mỗi Menu nằm dưới đúng ParentId và thể hiện rõ cấp sâu.
2. **Given** một Menu có nút con, **When** người dùng mở hoặc thu gọn nút đó, **Then** chỉ nhánh tương ứng thay đổi trạng thái hiển thị và cấu trúc cây còn lại được giữ nguyên.
3. **Given** các Menu cùng cấp có SortOrder khác nhau, **When** cây được hiển thị, **Then** chúng được sắp theo SortOrder tăng dần và dùng tiêu chí ổn định để xử lý trường hợp bằng nhau.
4. **Given** người dùng chọn một Application, **When** dữ liệu được tải, **Then** cây chỉ chứa Menu thuộc Application đó và không ghép quan hệ cha-con từ Application khác.
5. **Given** không có Menu phù hợp hoặc tải dữ liệu thất bại, **When** màn hình hoàn tất xử lý, **Then** trạng thái rỗng hoặc lỗi có thể thử lại được hiển thị rõ ràng mà không tạo dữ liệu giả.

---

### User Story 2 - Tạo Menu tại vị trí hợp lệ (Priority: P1)

Người quản trị tạo Menu gốc hoặc Menu con, chọn Application và Resource từ dữ liệu hiện có, rồi đặt thông tin hiển thị và thứ tự.

**Why this priority**: Tạo Menu là thao tác cốt lõi để hình thành và mở rộng cây điều hướng.

**Independent Test**: Tạo một Menu gốc và một Menu con với dữ liệu hợp lệ, tải lại màn hình, rồi xác nhận cả hai xuất hiện đúng Application, quan hệ cha-con và thứ tự.

**Acceptance Scenarios**:

1. **Given** có Application hợp lệ, **When** người dùng tạo Menu không chọn Parent, **Then** Menu được lưu như một nút gốc trong cây của Application đã chọn.
2. **Given** đã chọn Application, **When** người dùng chọn một Parent thuộc cùng Application và lưu dữ liệu hợp lệ, **Then** Menu mới xuất hiện trực tiếp dưới Parent đó.
3. **Given** dữ liệu Applications hoặc Resources thay đổi, **When** người dùng mở hoặc làm mới form, **Then** danh sách lựa chọn phản ánh dữ liệu hiện hành từ nguồn tương ứng thay vì danh sách cố định trên giao diện.
4. **Given** người dùng chọn Resource, **When** danh sách Resource được hiển thị, **Then** chỉ Resource thuộc Application đang chọn và còn khả dụng được phép chọn.
5. **Given** dữ liệu thiếu, vượt giới hạn, trùng Code trong cùng Application hoặc Parent không hợp lệ, **When** người dùng lưu, **Then** lỗi được chỉ rõ và không có thay đổi một phần nào được ghi nhận.

---

### User Story 3 - Cập nhật và di chuyển Menu (Priority: P2)

Người quản trị chỉnh sửa thông tin Menu, trạng thái hiển thị, thứ tự hoặc chuyển Menu cùng toàn bộ nhánh con đến một Parent hợp lệ.

**Why this priority**: Cấu trúc điều hướng thay đổi theo nghiệp vụ và cần được tổ chức lại mà không phải xóa rồi tạo lại các nhánh.

**Independent Test**: Chọn một Menu có nút con, sửa thông tin và Parent, lưu, tải lại, rồi xác nhận toàn bộ nhánh nằm ở vị trí mới mà các quan hệ con cháu được giữ nguyên.

**Acceptance Scenarios**:

1. **Given** Menu đang tồn tại, **When** người dùng cập nhật dữ liệu hợp lệ, **Then** đúng Menu được thay đổi và cây phản ánh kết quả mới.
2. **Given** Menu có các nút con, **When** chuyển Menu sang Parent hợp lệ trong cùng Application, **Then** toàn bộ nhánh được chuyển và quan hệ bên trong nhánh không thay đổi.
3. **Given** người dùng chọn chính Menu hoặc một hậu duệ của nó làm Parent, **When** lưu, **Then** hệ thống từ chối để không tạo chu trình.
4. **Given** người dùng đổi Application của Menu đang có, **When** lưu, **Then** hệ thống từ chối và hướng dẫn tạo cấu trúc phù hợp trong Application đích.
5. **Given** Menu đã bị người khác cập nhật hoặc xóa sau khi form được mở, **When** người dùng lưu dữ liệu cũ, **Then** hệ thống không ghi đè âm thầm và yêu cầu tải dữ liệu mới nhất.
6. **Given** một Menu có cả Resource và Route không được thiết lập, **When** người dùng mở màn hình index, **Then** mỗi giá trị trống được hiển thị bằng dấu gạch ngang dài `—` dễ đọc và thao tác chỉnh sửa của Menu vẫn khả dụng.
7. **Given** một Menu đang hiển thị `—` cho Resource hoặc Route chưa được thiết lập, **When** người dùng chọn chỉnh sửa, nhập hoặc giữ trống các trường tùy chọn rồi lưu dữ liệu hợp lệ, **Then** form mở bình thường, giá trị trống không bị chuyển thành chuỗi ký tự đại diện và Menu được cập nhật thành công.

---

### User Story 4 - Xóa Menu an toàn (Priority: P3)

Người quản trị xóa một Menu không còn sử dụng sau khi thấy rõ đối tượng và tác động đến cấu trúc con.

**Why this priority**: Xóa hoàn thiện vòng đời Menu nhưng có nguy cơ làm hỏng cây nếu nút còn con hoặc liên kết phụ thuộc.

**Independent Test**: Tạo một Menu lá, xác nhận xóa, rồi kiểm tra Menu không còn trong cây mặc định sau khi tải lại.

**Acceptance Scenarios**:

1. **Given** người dùng chọn xóa Menu, **When** hộp thoại xác nhận mở, **Then** Code, Name và thông tin nút con trực tiếp được hiển thị để nhận diện tác động.
2. **Given** Menu là nút lá và không có ràng buộc ngăn xóa, **When** người dùng xác nhận, **Then** Menu được xóa mềm và không còn trong cây mặc định.
3. **Given** Menu còn ít nhất một nút con chưa xóa, **When** người dùng xác nhận, **Then** hệ thống từ chối xóa, giữ nguyên toàn bộ cây và hướng dẫn xử lý các nút con trước.
4. **Given** hộp thoại xác nhận đang mở, **When** người dùng hủy, **Then** không có dữ liệu nào thay đổi.

### Edge Cases

- Resource và Route đồng thời là `NULL`: màn hình index hiển thị `—` riêng cho từng cột, không hiển thị chuỗi lỗi mã hóa như `â€”`, không phát sinh lỗi khi dựng dòng và vẫn cho phép mở thao tác chỉnh sửa.
- Chỉ một trong Resource hoặc Route là `NULL`: trường trống hiển thị `—`; trường còn lại hiển thị đúng dữ liệu thực và thao tác chỉnh sửa vẫn hoạt động.
- Người dùng mở form sửa từ một dòng có giá trị tùy chọn đang hiển thị `—`: form phải nhận giá trị trống thực, không nhận ký tự `—` làm dữ liệu của Resource hoặc Route.

- ParentId không tồn tại, đã xóa hoặc thuộc Application khác: bản ghi không được tạo hoặc cập nhật với quan hệ đó.
- Dữ liệu cũ đã chứa ParentId mồ côi hoặc chu trình: màn hình không lặp vô hạn; hiển thị cảnh báo an toàn và vẫn cho phép người dùng xử lý các phần cây hợp lệ.
- Cây có độ sâu lớn: mọi cấp hợp lệ vẫn được duyệt; giao diện giữ khả năng đọc và thao tác bằng bàn phím mà không cắt mất nút.
- Nhiều nút cùng SortOrder: thứ tự hiển thị ổn định giữa các lần tải.
- Đổi Application trên form tạo mới sau khi đã chọn Parent hoặc Resource: lựa chọn không còn hợp lệ được xóa và người dùng phải chọn lại.
- Resource đã ngừng hoạt động hoặc bị xóa sau khi form mở: lưu bị từ chối, không duy trì liên kết mới đến Resource không còn khả dụng.
- Phiên đăng nhập hết hạn hoặc người dùng mất quyền trong khi thao tác: yêu cầu thất bại an toàn, không giả định đã lưu và hướng dẫn đăng nhập hoặc xin quyền lại.
- Mạng tạm thời không khả dụng: dữ liệu form chưa gửi được giữ lại, lỗi có thể thử lại được hiển thị và thao tác lặp bị ngăn.
- Hai người cùng sửa, di chuyển hoặc xóa một Menu: thay đổi mới hơn không bị ghi đè âm thầm.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Hệ thống MUST cung cấp màn hình **Menus** riêng cho người dùng đã đăng nhập và có quyền quản lý Menu; người không có quyền MUST không được xem dữ liệu hoặc thực hiện thay đổi.
- **FR-002**: Màn hình index MUST hiển thị Menu dưới dạng tree list nhiều cấp được hình thành đệ quy theo ParentId, trong đó ParentId trống biểu thị nút gốc.
- **FR-003**: Người dùng MUST có thể chọn Application để giới hạn cây; mỗi cây MUST chỉ chứa Menu của đúng một Application.
- **FR-004**: Mỗi nút MUST hiển thị tối thiểu Name, Code, Application, Resource liên kết nếu có, Route, SortOrder, trạng thái hiển thị, trạng thái hoạt động và các thao tác được phép.
- **FR-005**: Người dùng MUST có thể mở và thu gọn từng nhánh; màn hình MUST hỗ trợ mở hoặc thu gọn toàn bộ cây.
- **FR-006**: Các nút cùng Parent MUST được sắp theo SortOrder tăng dần, sau đó theo Name và định danh để bảo đảm thứ tự ổn định.
- **FR-007**: Màn hình MUST có trạng thái tải, rỗng, lỗi và thử lại rõ ràng; dữ liệu lỗi cấu trúc MUST NOT khiến màn hình lặp vô hạn hoặc không thể sử dụng.
- **FR-008**: Người dùng có quyền MUST có thể tạo Menu với Application, Parent tùy chọn, Resource tùy chọn, Code, Name, Route tùy chọn, Icon tùy chọn, SortOrder, IsVisible và trạng thái hoạt động.
- **FR-009**: Danh sách Application MUST được tải động từ tập Applications hiện hành và chỉ cho phép chọn bản ghi còn khả dụng.
- **FR-010**: Danh sách Resource MUST được tải động từ tập Resources hiện hành, được lọc theo Application đang chọn và chỉ cho phép chọn bản ghi còn khả dụng.
- **FR-011**: Danh sách Parent MUST được tạo từ các Menu hiện hành của Application đang chọn; khi cập nhật, chính Menu và toàn bộ hậu duệ của nó MUST bị loại khỏi lựa chọn Parent.
- **FR-012**: Code MUST bắt buộc, dài từ 1 đến 120 ký tự sau khi loại bỏ khoảng trắng đầu/cuối và duy nhất không phân biệt chữ hoa/chữ thường trong một Application.
- **FR-013**: Name MUST bắt buộc và dài từ 1 đến 150 ký tự sau khi loại bỏ khoảng trắng đầu/cuối.
- **FR-014**: Route MUST không vượt quá 300 ký tự và Icon MUST không vượt quá 100 ký tự khi được cung cấp.
- **FR-015**: SortOrder MUST là số nguyên; giá trị mặc định MUST là 0. IsVisible và trạng thái hoạt động MUST mặc định là bật khi tạo mới.
- **FR-016**: Hệ thống MUST kiểm tra Parent tồn tại, chưa xóa, thuộc cùng Application và không tạo chu trình trước khi ghi nhận thao tác tạo hoặc cập nhật.
- **FR-017**: Resource được chọn MUST tồn tại, chưa xóa, còn hoạt động và thuộc cùng Application với Menu tại thời điểm lưu.
- **FR-018**: Người dùng có quyền MUST có thể cập nhật Parent, Resource, Code, Name, Route, Icon, SortOrder, IsVisible và trạng thái hoạt động của Menu.
- **FR-019**: Application của Menu đã tồn tại MUST không được thay đổi; việc tổ chức Menu cho Application khác nằm trong thao tác tạo mới tại Application đích.
- **FR-020**: Khi đổi Parent hợp lệ, Menu và toàn bộ hậu duệ MUST giữ nguyên quan hệ nội bộ và xuất hiện tại vị trí mới sau lần đọc tiếp theo.
- **FR-021**: Trước khi xóa, hệ thống MUST yêu cầu xác nhận với Code, Name và số nút con trực tiếp của Menu.
- **FR-022**: Xóa Menu MUST là xóa mềm; Menu đã xóa MUST bị loại khỏi cây, danh sách Parent và các hoạt động nghiệp vụ mới theo mặc định.
- **FR-023**: Hệ thống MUST từ chối xóa Menu còn nút con chưa xóa và MUST giữ nguyên toàn bộ dữ liệu trong trường hợp đó.
- **FR-024**: Mọi thao tác tạo, cập nhật, di chuyển hoặc xóa MUST được áp dụng toàn vẹn; khi một kiểm tra thất bại, không phần thay đổi nào được ghi nhận.
- **FR-025**: Hệ thống MUST phát hiện xung đột thay đổi đồng thời khi cập nhật hoặc xóa và MUST NOT ghi đè âm thầm dữ liệu mới hơn.
- **FR-026**: Màn hình MUST ngăn gửi lặp thao tác thay đổi khi yêu cầu trước đang xử lý và MUST thông báo kết quả thành công hoặc thất bại rõ ràng.
- **FR-027**: Mỗi thao tác thành công MUST ghi nhận thời điểm và danh tính người tạo hoặc cập nhật theo cơ chế audit hiện có.
- **FR-028**: Các nhu cầu đọc dữ liệu và các yêu cầu thay đổi dữ liệu MUST được xác định thành các luồng độc lập trong thiết kế tiếp theo, phù hợp với ràng buộc CQRS của feature.
- **FR-029**: Giao diện MUST hỗ trợ bàn phím, focus nhìn thấy được, nhãn có ý nghĩa, trạng thái mở/thu gọn có thể nhận biết và bố cục dùng được trên các kích thước desktop/mobile được hỗ trợ.
- **FR-030**: Khi Resource hoặc Route của Menu không có giá trị, màn hình index MUST hiển thị ký hiệu thay thế `—` cho trường tương ứng; MUST NOT hiển thị chuỗi lỗi mã hóa như `â€”` hoặc coi ký hiệu thay thế là dữ liệu thực.
- **FR-031**: Menu có Resource và/hoặc Route không có giá trị MUST vẫn cho phép người dùng có quyền mở form chỉnh sửa, xem đúng trạng thái trống, cập nhật các trường hợp lệ và lưu; giá trị `—` dùng để hiển thị MUST NOT được đưa vào dữ liệu chỉnh sửa hoặc dữ liệu lưu.

### Key Entities

- **Menu**: Một nút điều hướng thuộc đúng một Application; có Parent tùy chọn, Resource tùy chọn, Code, Name, Route, Icon, SortOrder, IsVisible, trạng thái hoạt động/xóa và thông tin audit.
- **Menu hierarchy**: Quan hệ tự tham chiếu giữa Menu cha và Menu con trong cùng Application; một Menu có tối đa một cha và có thể có nhiều con, không được có chu trình.
- **Application**: Phạm vi sở hữu và tách biệt cây Menu; cung cấp dữ liệu lựa chọn động cho form.
- **Resource**: Tài nguyên tùy chọn được Menu trỏ tới; chỉ Resource khả dụng thuộc cùng Application mới được liên kết.
- **Authorized administrator**: Người dùng đã đăng nhập và có quyền xem hoặc thay đổi Menus theo chính sách phân quyền hiện có.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% Menu hợp lệ trong bộ kiểm thử nhiều cấp xuất hiện đúng Parent và đúng Application, không có nút bị lặp hoặc thất lạc.
- **SC-002**: Ít nhất 95% người dùng có quyền xác định được vị trí của một Menu trong cây và mở đúng nhánh trong không quá 30 giây ở lần thử đầu tiên.
- **SC-003**: Ít nhất 95% thao tác tạo, cập nhật hoặc di chuyển Menu với dữ liệu hợp lệ được người dùng hoàn thành trong không quá 2 phút.
- **SC-004**: 100% trường hợp tạo chu trình, chọn Parent khác Application hoặc liên kết Resource khác Application bị từ chối mà không ghi nhận thay đổi một phần.
- **SC-005**: Với tối đa 10.000 Menu trong một Application và cây sâu đến 20 cấp, 95% lượt mở cây, đổi Application hoặc mở một nhánh cho kết quả có thể thao tác trong không quá 2 giây dưới tải vận hành bình thường.
- **SC-006**: 100% lựa chọn Application, Resource và Parent trên form phản ánh dữ liệu còn khả dụng tại thời điểm tải hoặc làm mới, không dùng giá trị cấu hình cố định.
- **SC-007**: 100% thao tác xóa bị hủy, bị chặn do còn nút con hoặc gặp xung đột giữ nguyên dữ liệu và cung cấp kết quả dễ hiểu.
- **SC-008**: Toàn bộ luồng CRUD chính và thao tác mở/thu gọn cây có thể hoàn thành chỉ bằng bàn phím trên các kích thước màn hình được hỗ trợ.
- **SC-009**: 100% trường hợp kiểm thử Menu có Resource và/hoặc Route là `NULL` hiển thị đúng `—` trên màn hình index, không xuất hiện `â€”`, và người dùng có quyền mở rồi hoàn thành chỉnh sửa thành công.

## Assumptions

- Dấu — chỉ là ký hiệu trình bày cho giá trị không được thiết lập trên màn hình index; giá trị nghiệp vụ tương ứng vẫn là trống và Resource cùng Route tiếp tục là các trường tùy chọn.

- Feature tái sử dụng đăng nhập, phân quyền và audit hiện có; tên quyền chi tiết sẽ được xác định ở giai đoạn lập kế hoạch theo quy ước dự án.
- Resource là liên kết tùy chọn; một Menu không gắn Resource vẫn hợp lệ, ví dụ nút dùng để nhóm các Menu con.
- Menu con bắt buộc thuộc cùng Application với Menu cha; cây của các Application độc lập với nhau.
- Application của Menu đã tạo là bất biến để tránh di chuyển ngầm cả nhánh và tạo liên kết chéo; sao chép cây giữa Applications nằm ngoài phạm vi.
- Xóa mềm từng Menu là hành vi mặc định; xóa cả nhánh, khôi phục và xem Menu đã xóa nằm ngoài phạm vi feature.
- Cây không áp đặt giới hạn nghiệp vụ cứng về số cấp; mốc 20 cấp là quy mô kiểm chứng chất lượng, không phải giới hạn dữ liệu.
- CQRS là ràng buộc bắt buộc cho thiết kế và triển khai tiếp theo: các trường hợp sử dụng đọc không thực hiện thay đổi, và mỗi thay đổi được biểu diễn bằng một yêu cầu nghiệp vụ riêng.

