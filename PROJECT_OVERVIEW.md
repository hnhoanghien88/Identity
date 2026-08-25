# Identity Management Platform

Identity Management Platform là một dự án full-stack mô phỏng hệ thống quản trị danh tính và phân quyền cho nhiều ứng dụng. Dự án không chỉ dừng ở CRUD người dùng: nó quản lý toàn bộ chuỗi **User → Role → Permission → Resource → Action**, cấp access token JWT ngắn hạn cùng refresh session có thể thu hồi, tạo menu động theo quyền và bảo vệ API bằng distributed rate limiting.

## Điểm nổi bật cho portfolio

- Xác thực bằng Code/mật khẩu băm PBKDF2 hoặc Google; phản hồi đăng nhập không làm lộ trạng thái tài khoản.
- Access token ngắn hạn và refresh token rotation theo Application; mỗi Application dùng một HttpOnly cookie và refresh-token family riêng. Logout thu hồi toàn bộ family của Application được chọn, còn access token tự hết hạn.
- Token mang `permissionversion`; khi Role hoặc Permission thay đổi, token cũ bị vô hiệu hóa ở lần sử dụng tiếp theo.
- Phân quyền theo từng thao tác `Read`, `Create`, `Update`, `Delete` thay vì chỉ kiểm tra đã đăng nhập.
- Menu nhiều cấp được lọc theo Permission thực tế của User.
- Runtime authorization nhận `applicationCode` và chỉ trả Roles, Permissions cùng cây menu runtime tối giản thuộc Application đang chạy; Application không tồn tại hoặc inactive trả `404 Problem Details`.
- Frontend dùng HTTP client tập trung theo mô hình interceptor để gắn access token, gửi refresh-token cookie, refresh phiên và retry request khi gặp `401`.
- Distributed rate limiting dùng Redis và Lua script nguyên tử, hỗ trợ Fixed Window, Sliding Window, Token Bucket và Concurrency.
- Optimistic concurrency bằng trường Version, audit metadata và soft-delete cascade cho dữ liệu Identity liên quan.
- Backend phân lớp, CQRS cho use cases và test ở Application, API contract, persistence boundary.
- Giao diện quản trị responsive bằng React và Material UI.

## Tính năng chính

| Nhóm | Khả năng |
|---|---|
| Authentication | Login bằng Code hoặc Google, external identity provisioning, JWT theo audience, refresh-token family và cookie riêng cho từng Application, logout và thu hồi phiên |
| Users | Tìm kiếm, phân trang, tạo, cập nhật, kích hoạt/vô hiệu hóa và xóa User |
| Applications | Quản lý các Application/audience được phục vụ bởi Identity system |
| Resources & Actions | Mô hình hóa đối tượng được bảo vệ và thao tác có thể cấp quyền |
| Roles | Quản lý Role theo từng Application |
| Role Permissions | Cấp/thu hồi Permission theo tổ hợp Role–Resource–Action |
| User Roles | Gán nhiều Users vào Role và gỡ từng quan hệ |
| Menus | Quản lý cây menu nhiều cấp theo Application và Resource |
| Runtime Authorization | Trả Roles, Permissions và menu đã lọc theo User và từng Application cho phiên hiện hành |
| Rate Limiting | Policy động trong database, counter phân tán trong Redis, quản trị qua UI |

## Kiến trúc

```text
React + Material UI
        │ REST / JWT / HttpOnly cookie
        ▼
ASP.NET Core API
  ├─ Authentication & permission middleware
  ├─ Dynamic distributed rate-limit middleware
  └─ Controllers / API contracts
        │
        ▼
Application layer
  ├─ Commands / Queries (CQRS)
  ├─ Validation
  └─ Persistence abstractions
        │
        ▼
Infrastructure layer
  ├─ EF Core + Dapper
  ├─ MySQL: identity data, policies, audit state
  └─ Redis: counters and concurrency leases
```

Dependency direction của backend là `Domain ← Application ← Infrastructure/API`. Domain giữ entities và business concepts; Application giữ use cases cùng interfaces; Infrastructure hiện thực persistence/security; API chịu trách nhiệm HTTP, authentication, authorization và composition.

## Công nghệ

### Backend

- .NET 10, ASP.NET Core Web API
- Entity Framework Core và MySql.EntityFrameworkCore
- Dapper và MySqlConnector
- MediatR cho command/query dispatch, validation pipeline và cảnh báo command/query chạy chậm theo threshold cấu hình
- FluentValidation chạy trước handler qua `ValidationBehavior`; lỗi được chuẩn hóa thành HTTP Validation Problem Details
- Slow MediatR requests được ghi bằng Serilog vào `Identity-api/src/Identity.Api/logs/performance-YYYYMMDD.log`, rolling theo ngày/kích thước và giữ 14 file gần nhất
- Mọi HTTP request được gắn Correlation ID và ghi structured log gồm method, path, status code, elapsed time, Trace ID và User ID vào `Identity-api/src/Identity.Api/logs/application-YYYYMMDD.log`
- JWT Bearer authentication
- StackExchange.Redis và Lua scripts
- Swagger/OpenAPI

### Frontend

- React 19
- Vite 8
- Material UI 9 và Emotion
- Fetch-based `apiClient` dùng chung như interceptor: tự động gắn Bearer token, gửi HttpOnly cookie, xử lý `401`, refresh session và retry request một lần
- Vitest, Testing Library và oxlint

### Data & vận hành

- MySQL cho dữ liệu bền vững
- Redis cho trạng thái rate-limit phân tán
- EF Core migrations kết hợp SQL migration scripts có rollback
- Docker Compose file để chạy Redis khi môi trường có Docker
- Cấu hình phân tách theo environment và hỗ trợ environment variables

## Theo dõi hiệu năng MediatR

`PerformanceBehavior<TRequest, TResponse>` là open pipeline behavior áp dụng cho mọi command/query được gửi bằng `mediator.Send(...)`. Behavior bắt đầu đo trước `ValidationBehavior`, gọi tiếp pipeline trong khối `try/finally` và luôn hoàn tất phép đo kể cả khi validation hoặc handler phát sinh exception.

- Chỉ ghi cảnh báo khi tổng thời gian pipeline lớn hơn hoặc bằng `Observability:SlowRequestThresholdMilliseconds`; mặc định là `500` ms và ứng dụng từ chối khởi động nếu giá trị không lớn hơn `0`.
- Log chỉ chứa tên request, thời gian thực thi và threshold; không serialize request/response nên tránh đưa password, token hoặc dữ liệu định danh vào log.
- Serilog lọc riêng cảnh báo từ `PerformanceBehavior` và ghi vào `Identity-api/src/Identity.Api/logs/performance-YYYYMMDD.log`.
- File được rolling theo ngày hoặc khi đạt `10 MB`, giữ tối đa `14` file gần nhất và thư mục `logs/` không được commit vào Git.
- Phép đo bao gồm validation, handler và repository/database được handler chờ; không bao gồm model binding, authentication, authorization hoặc HTTP response serialization.

```text
PerformanceBehavior → ValidationBehavior → Command/Query Handler → Repository
```

## Correlation ID và Structured Logging

`CorrelationIdMiddleware` chạy ở đầu HTTP pipeline để mọi log phát sinh trong vòng đời request dùng chung một định danh. API nhận Correlation ID qua header `X-Correlation-ID`; nếu header bị thiếu, dài quá `128` ký tự hoặc chứa ký tự không an toàn thì server tự sinh ID mới. Giá trị cuối cùng được gán vào `HttpContext.TraceIdentifier`, đưa vào Serilog `LogContext` và trả lại trong response header `X-Correlation-ID`.

- `CorrelationId` dùng để tìm toàn bộ log thuộc cùng một request; `TraceId` lấy từ W3C `Activity` để sẵn sàng liên kết với distributed tracing.
- Các response lỗi do exception, validation, conflict, `401` hoặc `403` chứa thêm `correlationId` trong Problem Details để đối chiếu với log server.
- Request completion log có các property riêng biệt: `CorrelationId`, `TraceId`, `RequestMethod`, `RequestPath`, `StatusCode`, `ElapsedMs` và `UserId`.
- Request có `StatusCode >= 500` được ghi ở mức `Error`; request có `ElapsedMs > 3000` được ghi ở mức `Warning`; các request còn lại được ghi ở mức `Information`.
- `UserId` lấy từ claim chuẩn `sub` sau authentication. Request login có thể chưa có `UserId`, trong khi các request tiếp theo sử dụng Bearer token sẽ có giá trị này.
- Không ghi request/response body, password, JWT, refresh token hoặc cookie vào log.

Application log được ghi theo định dạng một dòng dễ đọc:

```text
2026-08-25 10:15:20.123 +07:00 [INF] CorrelationId=abc-123 TraceId=... UserId=79 StatusCode=200 ElapsedMs=125.42 | HTTP GET /users responded 200 in 125.420 ms for user 79
```

File nằm tại `Identity-api/src/Identity.Api/logs/application-YYYYMMDD.log`, rolling theo ngày hoặc khi đạt `25 MB`, giữ tối đa `30` file gần nhất và flush xuống đĩa mỗi giây. Vì vậy application log được giới hạn xấp xỉ `750 MB`; performance log vẫn được giữ riêng với giới hạn `10 MB` mỗi file và `14` file.

Có thể tìm một request cụ thể bằng PowerShell:

```powershell
Select-String "CorrelationId=abc-123" Identity-api/src/Identity.Api/logs/application-*.log
```

## Luồng bảo mật tiêu biểu

1. User đăng nhập bằng Code và mật khẩu.
2. API xác minh trạng thái tài khoản và Application, tải Roles rồi cấp access token ngắn hạn với audience của Application.
3. Refresh token được gắn `ApplicationId`; raw token nằm trong HttpOnly cookie riêng của Application với tên `identity_refresh_<hash(applicationCode)>`, còn phía server chỉ lưu hash và quan hệ rotation/family.
4. Mỗi request xác minh chữ ký, issuer, audience/Application, thời hạn, loại token, trạng thái User và `permissionversion`.
5. Permission policy tại controller quyết định thao tác cụ thể có được phép hay không.
6. Khi gán Role hoặc thay đổi Permission, `permissionversion` tăng; access token cũ lập tức trở nên lỗi thời. Logout thu hồi toàn bộ refresh family của Application; access token hiện hành không dùng denylist trong memory và tự hết hạn sau thời gian ngắn.
7. Rate-limit middleware kiểm tra policy động trước khi request đi vào authorization/business handler.

## HTTP client và xử lý phiên ở frontend

- Các feature gọi một `apiFetch` dùng chung thay vì tự lặp lại cấu hình `fetch`.
- Client tự động gắn access token hiện hành vào `Authorization: Bearer ...` và luôn dùng `credentials: "include"` để gửi refresh token trong HttpOnly cookie.
- Khi API trả `401`, client gọi endpoint refresh, cập nhật session rồi retry request ban đầu đúng một lần; nếu refresh thất bại, session được xóa và User phải đăng nhập lại.
- Login gửi `VITE_APPLICATION_CODE`; refresh và logout gọi lần lượt `POST /refresh?applicationCode=...` và `POST /logout?applicationCode=...` để server chọn đúng cookie của Application.
- Refresh request được dùng chung qua một pending promise để tránh nhiều API cùng nhận `401` tạo ra nhiều lần refresh/rotation đồng thời.
- Session được giữ trong memory và đồng bộ giữa các tab bằng `BroadcastChannel`; refresh giữa các tab được điều phối bằng Web Locks API khi trình duyệt hỗ trợ.
- Lỗi Problem Details từ backend được chuẩn hóa nhưng vẫn được ánh xạ sang lớp lỗi riêng của từng feature.

## Runtime authorization theo Application

1. Frontend lấy mã Application đang chạy từ `VITE_APPLICATION_CODE` và gọi `GET /authorization?applicationCode=...` sau login hoặc refresh session.
2. Backend đối chiếu `applicationCode` được yêu cầu với claim `application_code`, sau đó dùng `sub`, `permissionversion` và Application để tải authorization đúng phạm vi. Yêu cầu khác Application trong token trả `403`.
3. Response gồm `roles`, `permissions` và `menus`; menu runtime chỉ chứa `id`, `name`, `route`, `children`. Quyền và metadata quản trị của Application khác không được trộn vào phiên hiện hành.
4. Backend tải cây menu của Application, bỏ các node không active rồi lọc đệ quy theo Permission. Menu gắn Resource yêu cầu Permission `{ResourceCode}.ViewMenu`; menu cha vẫn được giữ khi còn menu con hợp lệ.
5. React lưu kết quả vào `session.authorization`, dựng navigation từ `menus` và dùng `permissions` để ẩn/chặn thao tác trên giao diện. API vẫn là lớp kiểm soát quyền cuối cùng.
6. Authorization cache được phân vùng theo Application, User và `permissionversion`, nên thay đổi Role/Permission làm token và dữ liệu cache cũ mất hiệu lực.

## JWT và refresh session theo Application

- Access token dùng claim chuẩn `sub` cùng `code`, `role`, `permissionversion`, `application_id`, `application_code` và `token_type=access`; token không chứa menu hoặc toàn bộ Permission.
- `aud` lấy từ cấu hình dữ liệu của Application. Identity đối chiếu `application_id`, `application_code` và audience với Application active khi xác thực token.
- Login nhận `applicationCode` tùy chọn để giữ tương thích; frontend luôn gửi `VITE_APPLICATION_CODE`. User phải có ít nhất một Role active trong Application mới được cấp phiên.
- Rotation giữ nguyên `ApplicationId` và `FamilyId`. Refresh không cho client chuyển Application: mã trong query phải khớp Application của refresh token đã lưu.
- Nếu Application inactive, bị xóa hoặc User không còn Role active, refresh family bị thu hồi và cookie tương ứng bị xóa.
- Logout là thao tác idempotent dựa trên HttpOnly cookie nên vẫn đóng được refresh session khi access token đã hết hạn.
- Tên cookie dùng hash SHA-256 đã chuẩn hóa của `applicationCode`, vì vậy Identity và Business có thể duy trì phiên đồng thời trên cùng cookie scope mà không ghi đè nhau.

## Thiết kế rate limiting

- Policy nằm trong MySQL để quản trị, audit và thay đổi mà không triển khai lại.
- Counter nằm trong Redis để nhiều API instances dùng chung trạng thái.
- Lua script đảm bảo kiểm tra và cập nhật counter là một thao tác nguyên tử.
- Key chứa policy ID, policy version và partition để policy mới không dùng nhầm counter cũ.
- Concurrency dùng lease được release khi request hoàn thành và có timeout chống rò slot khi instance dừng bất thường.
- Có `InMemoryRateLimitStore` cho test/đơn instance và `FailureMode` để chọn ưu tiên availability hoặc protection khi Redis lỗi.

## Chất lượng và tài liệu

- 104 backend test cases đang pass trong ba test project: Application, API integration và Infrastructure integration; 39 frontend test cases đang pass.
- Test suite bao phủ Application handlers/validators, API contracts, persistence integration, JWT application scope, cookie isolation và frontend refresh/logout theo Application.
- Mỗi feature nghiệp vụ có specification, acceptance scenarios, functional requirements và measurable outcomes trong thư mục [`specs`](specs).
- Project constitution quy định security-first, dependency boundaries, explicit API contracts, test release gates và accessibility.

## Bản đồ Controller và Specification

| Controller | Specification |
|---|---|
| `AuthController` | [`009-session-authentication`](specs/009-session-authentication/spec.md) |
| `ExternalAuthController` | [`012-google-external-login`](specs/012-google-external-login/spec.md) |
| `AuthorizationController` | [`010-runtime-authorization`](specs/010-runtime-authorization/spec.md) |
| `UsersController` | [`001-manage-users`](specs/001-manage-users/spec.md) |
| `ApplicationsController` | [`002-manage-applications`](specs/002-manage-applications/spec.md) |
| `ResourcesController` | [`003-manage-resources`](specs/003-manage-resources/spec.md) |
| `ActionsController` | [`004-manage-actions`](specs/004-manage-actions/spec.md) |
| `MenusController` | [`005-manage-menus`](specs/005-manage-menus/spec.md) |
| `RolesController` | [`006-manage-roles`](specs/006-manage-roles/spec.md) |
| `RolePermissionsController` | [`007-role-permissions`](specs/007-role-permissions/spec.md) |
| `UserRolesController` | [`008-user-roles`](specs/008-user-roles/spec.md) |
| `RateLimitsController` | [`011-distributed-rate-limiting`](specs/011-distributed-rate-limiting/spec.md) |

`WeatherForecastController` là endpoint mẫu còn lại từ project template, không phải tính năng Identity và không được xem là một phần của portfolio scope. Nên xóa hoặc giới hạn endpoint này trước khi triển khai production.

## Chạy local

Yêu cầu tối thiểu: .NET 10 SDK, Node.js, MySQL và Redis-compatible server.

```powershell
dotnet run --project Identity-api/src/Identity.Api
```

```powershell
cd Identity-client
npm install
npm run dev
```

Nếu có Docker, Redis local có thể được khởi động bằng:

```powershell
cd Identity-api
docker compose -f docker-compose.redis.yml up -d
```

Nếu chưa có Redis, có thể chạy đơn instance bằng cấu hình môi trường:

```powershell
$env:RateLimiting__Store = "InMemory"
dotnet run --project Identity-api/src/Identity.Api
```

## Phạm vi và hướng phát triển tiếp theo

- Bổ sung integration tests chạy trực tiếp với Redis thật/Testcontainers cho cả bốn thuật toán.
- Xóa endpoint Weather Forecast mẫu trước production.
- Đưa JWT key và connection strings vào secret manager của môi trường triển khai.
- Bổ sung observability cho login failures, permission changes, rate-limit rejection và Redis health.
- Bổ sung CI pipeline chạy backend tests, frontend lint/test/build và migration validation.
