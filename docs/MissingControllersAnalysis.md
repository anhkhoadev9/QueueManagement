# Phân tích API Controllers còn thiếu dựa trên Handler đã triển khai

Dựa vào mã nguồn của dự án (kiến trúc CQRS), dưới đây là phân tích chi tiết về những `Handler` đã được tạo nhưng chưa có điểm cuối (endpoint) tương ứng trong các `Controller`.

## 1. Feature: Auth (Xác thực) 🟢
**Trạng thái:** Hoàn thiện
Tất cả các Handler của Auth đều đã được tích hợp đầy đủ vào `AuthController.cs`.
- `LoginCommandHandler`
- `RegisterCommandHandler`
- `LogoutCommandHandler`
- `RefreshTokenCommandHandler`
- `RevokedAllByUserIdCommandHandler`
- `ForgotPasswordCommandHandler`
- `ChangePasswordCommandHandler`

---

## 2. Feature: Tickets (Vé chờ) 🟡
**Trạng thái:** Đã có nhưng còn thiếu một phần
`TicketsController.cs` đã được tạo và chứa các endpoint cơ bản (Generate, UpdateStatus, GetWaitingTickets, GetCurrentTicket, GetTicketById), tuy nhiên **vẫn còn thiếu** các endpoint để gọi các Handler sau:
1. **CallTicket:** `CallTicketCommandHandler` *(Dùng để nhân viên gọi số vé tiếp theo)*
2. **SubmitFeedback:** `SubmitFeedbackCommandHandler` *(Gửi đánh giá cho một lượt vé)*
3. **GetPaginatedTickets:** `GetPaginatedTicketsQueryHandler` *(Lấy danh sách vé có phân trang dành cho bảng điều khiển)*
4. **GetTicketHistoryByTicketId:** `GetTicketHistoryByTicketIdQueryHandler` *(Lấy chi tiết lịch sử/trạng thái của một vé cụ thể)*

---

## 3. Feature: Service (Dịch vụ) 🔴
**Trạng thái:** Thiếu hoàn toàn
Hiện tại chưa có `ServicesController.cs` hoặc `ServiceController.cs`. Bạn cần tạo Controller này để expose API cho các Handler sau:
1. **CreatedService:** `CreatedServiceCommandHandler` *(Tạo dịch vụ mới)*
2. **DeletedService:** `DeletedServiceCommandHandler` *(Xóa dịch vụ)*
3. **UpdatedService:** `UpdatedServiceCommandHandler` *(Cập nhật thông tin dịch vụ)*
4. **LockService:** `LockServiceCommandHandler` *(Khóa/Tạm dừng cung cấp dịch vụ)*
5. **GetIdService:** `GetIdServiceQueryHandler` *(Lấy thông tin của một dịch vụ qua ID)*
6. **GetPaginatedResultService:** `GetPaginatedResultServiceQueryHandler` *(Lấy ra danh sách toàn bộ dịch vụ có phân trang)*

---

## 4. Feature: FeedBack (Đánh giá) 🔴
**Trạng thái:** Thiếu hoàn toàn
Hiện tại chưa có `FeedBacksController.cs` nào được tạo. Cần phải thiết lập để xử lý các yêu cầu tới các Handler sau:
1. **AddFeedBack:** `AddFeedBackCommandHandler` *(Thêm đánh giá)*
2. **UpdateFeedBack:** `UpdateFeedBackCommandHandler` *(Sửa nội dung đánh giá)*
3. **DeleteFeedBack:** `DeleteFeedBackCommandHandler` *(Xóa đánh giá)*
4. **GetFeedbackByServiceId:** `GetFeedbackByServiceIdQueryHandler` *(Lấy đánh giá dựa trên ID Dịch vụ để tính điểm rating)*
5. **GetPaginateByQueueTicket:** `GetPaginateFeedBackByQueueTicketQueryHandler` *(Lấy danh sách đánh giá có phân trang)*

---

## Tổng kết & Đề xuất công việc tiếp theo
Để mô hình hoàn chỉnh, danh sách các công việc (Task) tiếp theo bao gồm:
1. **Tạo mới `ServicesController.cs`:** Triển khai 6 API endpoints (Tạo, Sửa, Xóa, Khóa, Lấy theo ID, Lấy phân trang).
2. **Tạo mới `FeedBacksController.cs`:** Triển khai 5 API endpoints (Thêm, Sửa, Xóa, Lấy danh sách theo Dịch vụ, Lấy danh sách phân trang).
3. **Cập nhật `TicketsController.cs`:** Bổ sung thêm 4 API endpoints còn thiếu (Gọi vé, Nộp đánh giá từ vé, Lấy danh sách vé phân trang, Lấy lịch sử vé).
