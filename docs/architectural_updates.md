# Báo cáo Cập nhật Kiến trúc & Tính năng Hệ thống

Tài liệu này tổng hợp các thay đổi mới nhất về kiến trúc và tính năng đã được triển khai trong dự án QueueManagement.

## 1. Bổ dung LoggingMiddleware (Tầng API)

### Mục tiêu:
Theo dõi toàn bộ luồng request/response đi vào và đi ra khỏi hệ thống từ phía HTTP, bổ sung cho `LoggingBehavior` (chỉ theo dõi MediatR).

### Điểm hay & Ứng dụng:
*   **Tầm nhìn toàn diện (Full Visibility):** `LoggingBehavior` giúp ta biết MediatR làm gì, nhưng `LoggingMiddleware` giúp ta biết được các request không đi qua MediatR (VD: Static files, Auth failures, Routing errors).
*   **Đo lường hiệu năng thực tế:** Ghi lại chính xác thời gian từ lúc server nhận request đến lúc trả về cho client.
*   **Cảnh báo Long-Running:** Tự động in `LogWarning` cho các request vượt quá 500ms, giúp nhanh chóng phát hiện các API bị chậm.

```csharp
// Ví dụ log từ Middleware
HTTP Request Completed: GET /api/v1/Tickets responded 200 in 45 ms
```

---

## 2. Hoàn thiện tính năng cho các Entity (Tầng Application)

Tôi đã bổ sung các tính năng còn thiếu để đảm bảo hệ thống có đủ khả năng truy suất dữ liệu cho UI:

### Tickets (Hàng đợi & Lịch sử)
*   **GetPaginatedTicketsQuery:** Cho phép lấy danh sách toàn bộ vé (theo trang), phục vụ cho trang quản lý lịch sử vé.
*   **GetTicketHistoryByTicketIdQuery:** Truy vết toàn bộ quá trình thay đổi trạng thái của một vé (từ Waiting -> Called -> Completed). Điều này rất quan trọng để audit và kiểm tra lỗi.

### FeedBack (Đánh giá)
*   **GetFeedbackByServiceIdQuery:** Cho phép lọc đánh giá theo từng loại dịch vụ (Cắt tóc, Gội đầu, v.v.), giúp quản lý biết được dịch vụ nào đang tốt hoặc cần cải thiện.

---

## 3. Phân tích & Giải thích các điểm cần áp dụng

Dưới đây là những "mẫu mã" (patterns) tốt mà bạn nên tiếp tục áp dụng cho các module sau này:

1.  **Repository Pattern + Specification:** Việc sử dụng `FindAsync` với biểu thức lamba (`h => h.QueueTicketId == id`) giúp code Handler cực kỳ ngắn gọn và dễ đọc.
2.  **Mapping trực tiếp trong Handler:** Đối với các truy vấn đơn giản, việc map sang DTO bằng `.Select(...)` giúp kiểm soát chính xác lượng dữ liệu trả về UI, tránh leak các thông tin dư thừa từ Entity.
3.  **Middlewares vs Behaviors:** 
    *   Dùng **Middleware** cho các logic liên quan đến hạ tầng HTTP (Logging, Exception, Auth).
    *   Dùng **MediatR Behavior** cho các logic liên quan đến nghiệp vụ (Validation, Caching, Transaction).
4.  **Unit of Work:** Luôn sử dụng `_unitOfWork` để đảm bảo tính nhất quán của dữ liệu.

---

*Lưu ý: Các module Đăng ký, Đăng nhập, Đăng xuất đã được giữ lại để bạn tự triển khai theo yêu cầu.*
