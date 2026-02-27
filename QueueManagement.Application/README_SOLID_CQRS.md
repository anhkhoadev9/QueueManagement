# Tài liệu Diễn giải Kiến trúc Application Layer

Tài liệu này giải thích chi tiết về lý do và cách thức áp dụng các Design Pattern và Nguyên tắc thiết kế (SOLID, CQRS, Repository Pattern) trong hệ thống Hair Salon Queue Management (V1 - Mini Project).

## 1. Tại sao lại dùng CQRS (Command Query Responsibility Segregation)?

### Vấn đề:
Trong các dự án CRUD truyền thống (như dùng Service với hàng tá hàm `CreateTicket`, `GetWaitingTickets`, `UpdateTicket`), các hàm đọc (Query) và ghi (Command) bị trộn lẫn vào nhau trong cùng một class `TicketService`.
Khi hệ thống to lên, `TicketService` sẽ bị "phình to" (God Object) và rất khó bảo trì. Đồng thời, cấu trúc dữ liệu trả về cho thao tác Đọc thường khác với thao tác Ghi.

### Áp dụng:
- **Command:** Các thao tác thay đổi trạng thái (Ghi/Update) như `GenerateTicketCommand`, `CallTicketCommand`. Mỗi Command chỉ làm đúng một việc và trả về kết quả thành công/thất bại hoặc ID của đối tượng mới.
- **Query:** Các thao tác lấy dữ liệu (Đọc) như `GetWaitingTicketsQuery`. Query trả về các object DTO (Data Transfer Object) được tối ưu cho việc hiển thị lên UI mà không làm ảnh hưởng đến Entity gốc.
- **MediatR:** Đóng vai trò là một "Người đưa thư" (Mediator). Controller không cần biết ai xử lý logic, nó chỉ việc gửi Command/Query cho MediatR: `_mediator.Send(new GenerateTicketCommand())`. Nhờ đó, Controller cực kỳ "mỏng" và tính lỏng lẻo (Loose Coupling) giữa API và Business Logic được đảm bảo.

## 2. Tại sao dùng Repository Pattern?

### Vấn đề:
Nếu viết trực tiếp `_dbContext.QueueTickets.Add(...)` vào trong Application Layer, Business Logic sẽ bị gắn chặt (tightly coupled) với Entity Framework Core và Database. Nếu sau này muốn đổi Database hoặc viết Unit Test, chúng ta sẽ gặp khó khăn tột cùng vì phải khởi tạo Database thật.

### Áp dụng:
Chúng ta định nghĩa Interface `ITicketRepository` ở tầng Domain và triển khai (Implement) nó ở tầng Infrastructure (`TicketRepository`).
Tầng Application chỉ tương tác thông qua Interface (`ITicketRepository.Add(ticket)`).
=> **Tách biệt hoàn toàn Business Logic khỏi chi tiết truy cập dữ liệu.** (Phù hợp với Dependency Inversion Principle).

## 3. Các Cross-Cutting Concerns (Exception & Logging)

Để hệ thống hoạt động ổn định và dễ scale, việc xử lý lỗi và ghi log phải được tách biệt:

### Custom Exception Middleware
Thay vì dùng khối lệnh `try...catch` ở hàng trăm chỗ trong các Handler hay Controller, chúng ta tạo ra các Exception chuẩn (như `NotFoundException`, `BadRequestException`) trong Application và thiết kế một **`ExceptionMiddleware`**. 
Khi có bất kỳ lỗi nào ném ra từ tầng dưới, Middleware sẽ "tóm" lấy nó, tự động map sang HttpStatusCode tương ứng (ví dụ HTTP 404 cho NotFoundException, HTTP 400 cho BadRequest) và format Response dưới dạng JSON. Giao diện frontend nhờ vậy sẽ nhận được một bộ response code đồng nhất.

### Pipeline Behavior Logging (Tích hợp Serilog/Seq)
Thay vì chèn `_logger.LogInfo` vào đầu và cuối hàng trăm class Handler, chúng ta sử dụng cơ chế **`IPipelineBehavior`** của MediatR (giống như Middleware nhưng chỉ dùng cho các class Request).
`LoggingBehavior` sẽ tự động:
1. In ra tên Request đầu vào.
2. Dùng `Stopwatch` đo đếm thời gian hệ thống xử lý Request đó.
3. Nếu quá 500ms -> In cảnh báo Warning chậm (`LogWarning`).
4. In ra Response kết quả đầu ra.
Log này sau đó có thể được đẩy lên hệ thống **Seq UI** cực kỳ chi tiết (Sẽ được setup cấu hình Sink ở tầng API) để theo dõi tốc độ phần mềm.

## 4. Các nguyên tắc SOLID được áp dụng như thế nào?

Toàn bộ cấu trúc CQRS + MediatR + Repository thực tế là sự thể hiện xuất sắc của bộ 5 nguyên tắc SOLID:

### S (Single Responsibility Principle - Nguyên tắc đơn trách nhiệm)
Mỗi class chỉ nên có 1 lý do duy nhất để thay đổi.
- **Thay vì:** `TicketService` làm mọi thứ (Tạo vé, Gọi số, Hủy vé, Lấy danh sách).
- **Hiện tại:** `GenerateTicketCommandHandler` chỉ chịu trách nhiệm tạo vé. `ExceptionMiddleware` chỉ lo bắt lỗi. Không một class nào ôm đồm đa chức năng.

### O (Open/Closed Principle - Nguyên tắc đóng mở)
Có thể mở rộng thêm hành vi mới mà không cần sửa đổi code cũ.
- Khi cần thêm chức năng Logging hay Caching cho tất cả Request, ta chỉ việc thêm một class Kế thừa `IPipelineBehavior` của MediatR vào hệ thống. Lập tức hàng trăm hàm cũ đều được thừa hưởng tính năng mới mà không hề bị chỉnh sửa.

### L (Liskov Substitution Principle - Nguyên tắc thay thế Liskov)
Các class con (implementation) có thể thay thế class cha (interface) mà không làm hỏng chương trình.
- `TicketRepository` implement `ITicketRepository`. Tầng Application gọi `ITicketRepository.GetByIdAsync()`. Nó không quan tâm bên dưới là SQL Server hay InMemory Database, chỉ cần trả về đúng data.

### I (Interface Segregation Principle - Nguyên tắc phân tách Interface)
Không nên ép một class phải implement những phương thức mà nó không dùng đến.
- Nếu chia Interface bự `IBaseRepository` chứa hàng tá hàm thao tác, nhưng Ticket lại chỉ cần Insert và Read. Ta tạo riêng `ITicketRepository` tập trung vào những hành vi Ticket cần (`GetWaitingTickets`, `GetByTicketNumber`), thay vì ôm đồm.

### D (Dependency Inversion Principle - Nguyên tắc đảo ngược Dependency)
Các module cấp cao (Application) không nên phụ thuộc vào module cấp thấp (Infrastructure). Cả hai nên phụ thuộc vào abstractions (Interface).
- Bằng chứng rõ ràng nhất: Handler tầng Application injection `ITicketRepository` (nằm ở Domain) thông qua Constructor, chứ không new thẳng `TicketRepository` của tầng Infrastructure. Do đó Application hoàn toàn không biết Entity Framework là gì.
