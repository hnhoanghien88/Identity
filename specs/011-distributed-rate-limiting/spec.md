# Feature Specification: Distributed Rate Limiting

**Feature Branch**: `011-distributed-rate-limiting`

**Created**: 2026-08-20

**Status**: Implemented

**Input**: Quản trị policy động và áp dụng giới hạn yêu cầu nhất quán trên nhiều API instances.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Bảo vệ endpoint bằng policy động (Priority: P1)

Hệ thống áp dụng các policy đang hoạt động theo route, method, Application và các chiều phân vùng đã cấu hình.

**Why this priority**: Giảm brute force và quá tải phải hoạt động trước khi cung cấp giao diện quản trị.

**Independent Test**: Gửi yêu cầu vượt giới hạn vào login và API được bảo vệ qua nhiều instances, xác nhận giới hạn được thực thi dùng chung.

**Acceptance Scenarios**:

1. **Given** policy phù hợp, **When** số yêu cầu vượt giới hạn, **Then** hệ thống trả 429 cùng giới hạn, số còn lại, thời điểm reset và Retry-After.
2. **Given** nhiều API instances, **When** yêu cầu được phân phối giữa các instances, **Then** chúng vẫn dùng chung một giới hạn logic.
3. **Given** nhiều policy cùng khớp, **When** xử lý request, **Then** hệ thống áp dụng theo thứ tự ưu tiên và request phải thỏa tất cả policy phù hợp.

---

### User Story 2 - Quản trị rate-limit policies (Priority: P1)

Người quản trị có quyền xem endpoints và tạo, cập nhật hoặc xóa policy mà không triển khai lại ứng dụng.

**Why this priority**: Vận hành cần điều chỉnh bảo vệ theo rủi ro thực tế.

**Independent Test**: Tạo policy, xác nhận có hiệu lực sau thời gian cache; cập nhật tạo trạng thái counter mới và xóa làm policy ngừng áp dụng.

**Acceptance Scenarios**:

1. **Given** người có quyền phù hợp, **When** tạo hoặc cập nhật policy hợp lệ, **Then** policy được lưu và có hiệu lực sau khi cache được làm mới.
2. **Given** phiên bản policy đã cũ, **When** cập nhật hoặc xóa, **Then** hệ thống trả xung đột và không ghi đè.
3. **Given** thuật toán không hỗ trợ, **When** lưu policy, **Then** hệ thống từ chối thay vì tự chuyển sang thuật toán khác.

---

### User Story 3 - Giới hạn xử lý đồng thời (Priority: P2)

Người vận hành giới hạn số request đang xử lý đồng thời cho endpoint tốn tài nguyên.

**Why this priority**: Bảo vệ tài nguyên mà giới hạn theo thời gian không giải quyết được.

**Independent Test**: Giữ đủ request đang chạy, xác nhận request tiếp theo bị từ chối; hoàn tất một request và xác nhận slot được giải phóng.

**Acceptance Scenarios**:

1. **Given** đủ concurrency leases đang hoạt động, **When** request mới đến, **Then** request bị từ chối.
2. **Given** request hoàn thành hoặc lease hết timeout, **When** request mới đến, **Then** slot có thể được cấp lại.

### Edge Cases

- Redis không khả dụng ở chế độ fail-open hoặc fail-closed.
- Policy thay đổi khi counter phiên bản cũ vẫn tồn tại.
- Request bị hủy hoặc API instance dừng trước khi release concurrency lease.
- Route không có endpoint metadata hoặc thuật toán không hỗ trợ.
- Nhiều request cập nhật cùng counter trong cùng thời điểm.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Policy MUST được lưu bền vững và gồm tên, Application tùy chọn, route pattern, methods, partition dimensions, algorithm, permit limit, window, burst, priority, trạng thái và version.
- **FR-002**: Hệ thống MUST hỗ trợ Fixed Window, Sliding Window, Token Bucket và Concurrency.
- **FR-003**: Thao tác kiểm tra và cập nhật distributed counter MUST nguyên tử dưới tải đồng thời.
- **FR-004**: Counter MUST dùng chung giữa các API instances và được tách theo policy version cùng partition key.
- **FR-005**: Partition MUST hỗ trợ User, IP address, Application và Endpoint.
- **FR-006**: Policy hoạt động MUST được áp dụng theo priority giảm dần cho route và method phù hợp.
- **FR-007**: Yêu cầu vượt giới hạn MUST nhận HTTP 429 cùng các header giới hạn và thời gian thử lại.
- **FR-008**: Concurrency MUST acquire trước xử lý, release sau xử lý và có timeout tự phục hồi khi tiến trình lỗi.
- **FR-009**: Hệ thống MUST hỗ trợ store Redis và store in-memory cho kiểm thử hoặc môi trường đơn instance.
- **FR-010**: Khi store lỗi, FailureMode Open MUST cho request đi qua và ghi log; FailureMode Closed MUST không bỏ qua giới hạn.
- **FR-011**: Chỉ người có `RateLimiting.Read/Create/Update/Delete` tương ứng mới được quản trị policy.
- **FR-012**: Hệ thống MUST cung cấp danh sách endpoints để giảm lỗi cấu hình route.
- **FR-013**: Tạo/cập nhật MUST kiểm tra trường bắt buộc, giới hạn dương và thuật toán hợp lệ.
- **FR-014**: Cập nhật/xóa MUST dùng version để phát hiện xung đột.
- **FR-015**: Policy cache MUST được invalidate sau thay đổi và có thời hạn hữu hạn.

### Key Entities

- **Rate Limit Policy**: Quy tắc khớp request và giới hạn được phép.
- **Counter State**: Trạng thái ngắn hạn của thuật toán theo partition.
- **Concurrency Lease**: Slot xử lý có định danh và thời điểm hết hạn.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Không có request vượt capacity được chấp nhận trong kiểm thử đồng thời trên nhiều instances.
- **SC-002**: 100% phản hồi bị giới hạn có 429, Retry-After và thông tin limit/reset hợp lệ.
- **SC-003**: Concurrency slot được tái sử dụng ngay sau khi request hoàn thành hoặc chậm nhất khi lease timeout.
- **SC-004**: Policy hợp lệ mới có hiệu lực trong tối đa thời gian cache đã cấu hình.
- **SC-005**: Người vận hành có quyền hoàn thành tạo hoặc chỉnh policy trong dưới 2 phút qua giao diện quản trị.

## Assumptions

- Policy configuration cần tính bền vững; counter là trạng thái tạm thời có TTL.
- Redis là store mặc định cho môi trường nhiều instances.
- WindowSeconds của Concurrency được dùng làm lease safety timeout.
