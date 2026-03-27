# Kế hoạch Phát triển Tầng API (API Layer Development Plan)

Tài liệu này đóng vai trò như một bản "Job Description" và danh sách Task chi tiết để dev (bạn) có thể tự hoàn thiện tầng giao tiếp của hệ thống.

---

## 📋 Danh sách Task (Backlog)

### Task 1: Thiết lập CORS (Cơ sở hạ tầng)
**Mục tiêu**: Cho phép Frontend truy cập API và SignalR từ các domain khác nhau.
- [ ] Mở file `Program.cs`.
- [ ] Định nghĩa chính sách CORS (ví dụ: cho phép `localhost:3000`).
- [ ] Áp dụng `app.UseCors()` trước `app.UseAuthorization()`.
- [ ] Kiểm tra SignalR có hỗ trợ `AllowCredentials()` không (Bắt buộc cho SignalR).

### Task 2: Triển khai BaseApiController
**Mục tiêu**: Tránh lặp lại logic inject MediatR trong mỗi Controller.
- [ ] Tạo file `BaseApiController.cs` trong thư mục `Controllers`.
- [ ] Sử dụng thuộc tính `[ApiController]` và `[Route("api/v[controller]")]`.
- [ ] Tạo thuộc tính protected `IMediator Mediator` bằng cách lấy từ `HttpContext` (Service Locator pattern) hoặc inject qua constructor.

### Task 3: Triển khai TicketsController
**Mục tiêu**: Cung cấp các Endpoint để xử lý vé.
- [ ] Tạo `TicketsController` kế thừa từ `BaseApiController`.
- [ ] **Endpoint 1 (POST)**: `Generate` - Nhận `GenerateTicketCommand`, trả về `201 Created`.
- [ ] **Endpoint 2 (PATCH)**: `UpdateStatus` - Nhận `UpdateTicketStatusCommand`, trả về `204 No Content`.
- [ ] **Endpoint 3 (GET)**: `Waiting` - Gọi `GetWaitingTicketsQuery`.
- [ ] **Endpoint 4 (GET)**: `Current` - Gọi `GetCurrentlyCalledTicketQuery`.

### Task 4: Làm sạch Swagger & API Documentation
**Mục tiêu**: Giúp tài liệu API chuyên nghiệp và dễ đọc.
- [ ] Thêm file XML documentation (tùy chọn).
- [ ] Cấu hình `AddSwaggerGen` trong `Program.cs` để hiển thị các tiêu đề và phiên bản API rõ ràng hơn.

---

## 🛠 Hướng dẫn tư duy: "Làm thế nào để đúng chuẩn chuyên nghiệp?"

### 1. Phân tách Request Body và Path Variable
- Với `UpdateStatus`, `id` của vé nên nằm ở URL (Path), còn các thông tin khác (`Status`, `StaffId`) nên nằm ở Body.
- Tại sao? Vì nó tuân thủ chuẩn RESTful API.

### 2. Sử dụng đúng HTTP Methods
- **POST**: Để tạo mới dữ liệu (Generate Ticket).
- **PATCH**: Để cập nhật một phần dữ liệu (Chỉ cập nhật Status). Không nên dùng PUT nếu bạn không thay thế toàn bộ Entity.
- **GET**: Để lấy dữ liệu (Queries).

### 3. Xử lý lỗi (Global Exception Handling)
- Hãy tận dụng `ExceptionMiddleware` đã có để trả về mã lỗi `400 Bad Request` hoặc `404 Not Found` thay vì để hệ thống crash và trả về `500`.

### 4. Kết nối với SignalR
- Nhớ rằng Hub đã được map tại `/queue-hub`. Khi viết logic Controller, bạn không cần gọi Hub nữa vì chúng ta đã tích hợp việc đó vào các **Handlers** ở tầng Application rồi. Controller chỉ việc gọi Command, logic bắn tin nhắn sẽ tự chạy ngầm.

---
> [!TIP]
> Hãy hoàn thành từng Task một, kiểm tra kỹ với Swagger (`/swagger`) sau mỗi bước. Chúc bạn code vui vẻ!
