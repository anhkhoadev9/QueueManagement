# Tài liệu Kỹ thuật: Tích hợp SignalR trong Queue Management

Tài liệu này giải thích chi tiết về cách thức triển khai SignalR, lý do đằng sau các quyết định kiến trúc và tư duy lập trình được áp dụng.

## 1. Tại sao cần SignalR?

Trong hệ thống quản lý hàng đợi, tính "tức thời" (real-time) là yếu tố sống còn:
- **Khách hàng**: Cần biết ngay khi số của mình được gọi mà không cần tải lại trang.
- **Nhân viên**: Cần thấy danh sách hàng đợi cập nhật ngay khi có vé mới.
- **Màn hình hiển thị**: Cần nhảy số tự động để mọi người cùng theo dõi.

Nếu dùng cơ chế "Pulling" (trình duyệt cứ 5-10s hỏi server một lần), server sẽ bị quá tải bởi hàng ngàn request vô ích và trải nghiệm người dùng sẽ bị trễ. SignalR giải quyết vấn đề này bằng cơ chế "Push" (Server chủ động đẩy tin nhắn xuống Client).

---

## 2. Tư duy Kiến trúc: Tại sao làm như vậy?

Chúng ta áp dụng **Clean Architecture** để triển khai SignalR nhằm tránh các vấn đề sau:
- **Coupling (Phụ thuộc cứng)**: Nếu chúng ta viết code SignalR trực tiếp trong Handler, sau này nếu muốn đổi sang Socket.io hoặc Firebase, chúng ta phải sửa lại toàn bộ logic nghiệp vụ.
- **Testability (Khả năng kiểm thử)**: Tách Interface giúp chúng ta dễ dàng Mocking khi viết Unit Test.

### Giải pháp các lớp:

#### A. Tầng Application: Interface `IQueueHubService`
- **Tư duy**: Đây là "Hợp đồng". Các Handler chỉ quan tâm là "Tôi muốn thông báo có vé mới", họ không cần biết thông báo đó gửi qua SignalR, Email hay SMS.
- **Vị trí**: `Application/Common/Interfaces`.

#### B. Tầng Infrastructure: Implement `SignalRService` & `QueueHub`
- **Tư duy**: Đây là nơi chứa công nghệ cụ thể. SignalR là một chi tiết kỹ thuật của Infrastructure.
- **Hub**: Là trạm trung chuyển kết nối.
- **Service**: Là lớp bọc (Wrapper) quanh HubContext của SignalR để thực thi các phương thức trong Interface.

#### C. Tầng API: Configuration
- **Tư duy**: API là điểm cuối (Entry point). Đây là nơi chúng ta "ráp" các mảnh ghép lại với nhau và mở ra cổng kết nối (`/queue-hub`) để thế giới bên ngoài truy cập.

---

## 3. Luồng hoạt động (Data Flow)

1.  **Client (Frontend)**: Kết nối tới `/queue-hub`.
2.  **Khách hàng**: Gửi request `GenerateTicket`.
3.  **Handler**: Xử lý logic nghiệp vụ -> Lưu Database thành công.
4.  **Handler**: Gọi `_hubService.NotifyTicketCreated(ticket)`.
5.  **SignalRService**: Nhận lệnh -> Sử dụng `IHubContext` đẩy tin nhắn xuống tất cả Client đang kết nối.
6.  **Client**: Nhận sự kiện `ReceiveTicketCreated` -> Cập nhật UI ngay lập tức.

---

## 4. Các điểm lưu ý cho tương lai

- **Hiệu năng**: Hiện tại dùng `Clients.All` để đơn giản hóa. Trong thực tế, có thể dùng `Groups` (ví dụ: Group theo Chi nhánh/Salon) để chỉ gửi thông báo cho đúng đối tượng cần nhận.
- **Bảo mật**: Hub có thể được bảo vệ bằng JWT Token nếu cần phân quyền AI/Staff mới được kết nối.
- **Mất kết nối**: Phía Frontend cần có cơ chế `retry` khi mất kết nối mạng (SignalR có sẵn `withAutomaticReconnect`).

---
*Tài liệu này được soạn thảo để giúp đội ngũ phát triển nắm vững tư duy hệ thống thay vì chỉ copy-paste code.*
