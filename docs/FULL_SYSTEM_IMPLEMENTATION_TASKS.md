# Kế hoạch Triển khai Toàn diện Hệ thống (Full System Implementation Plan)

Tài liệu này tổng hợp các hạng mục cần thực hiện để hoàn thiện toàn bộ các Entity trong hệ thống (ngoài `QueueTicket` đã cơ bản hoàn tất).

---

## 🏗️ Tầng 1: Infrastructure (Cấu hình & Persistence)
Mục tiêu: Đảm bảo Database schema và Repositories hoạt động đúng.

### 1.1. Entity Configurations (Fluent API)
- [ ] **Service**: Hoàn thiện `ServiceConfiguration`.
- [ ] **TicketHistory**: Tạo `TicketHistoryConfiguration`.
- [ ] **Feedback**: Tạo `FeedbackConfiguration`.
- [ ] **User & Account**: Tạo cấu hình cho Identity/User data.
- [ ] **RefreshToken**: Cấu hình bảng lưu trữ JWT Refresh Token.

### 1.2. Repositories
- [ ] Cập nhật `IRepositories.cs` (Domain) để bổ sung Interface cho `User`, `Account`, `RefreshToken`.
- [ ] Triển khai Repo thực tế trong `Infrastructure/Persistence/Repositories`.
- [ ] Đăng ký DI trong `InfrastructureServicesRegistration`.

---

## 🚀 Tầng 2: Application (Logic nghiệp vụ)
Mục tiêu: Xử lý dữ liệu thông qua MediatR, DTO và AutoMapper.

### 2.1. Phân vùng Services & Staff
- [ ] **Services Features**: Tạo CRUD cho danh mục dịch vụ (Dịch vụ cắt tóc, gội đầu, v.v.).
- [ ] **Staff/User Features**: Quản lý thông tin nhân viên, phân quyền.

### 2.2. Xử lý Lịch sử & Phản hồi
- [ ] **TicketHistory Features**: Tự động lưu vết khi vé đổi trạng thái (đã có logic, cần đóng gói API nếu cần xem lại).
- [ ] **Feedback Features**: Endpoint cho khách hàng đánh giá dịch vụ sau khi hoàn tất.

### 2.3. Authentication & Authorization (Bảo mật)
- [ ] Triển khai `LoginCommand`, `RegisterCommand`.
- [ ] Logic tạo JWT và Refresh Token.
- [ ] Middleware kiểm tra Role/Permission.

---

## 🌐 Tầng 3: API (Giao tiếp Frontend)
Mục tiêu: Expose các endpoint thông qua Controller.

- [ ] **ServicesController**: Quản lý danh sách dịch vụ.
- [ ] **UsersController**: Quản lý nhân viên và tài khoản.
- [ ] **AuthController**: Xử lý Đăng nhập/Đăng ký/Refresh Token.
- [ ] **FeedbacksController**: Nhận phản hồi từ khách hàng.

---

## 📋 Danh sách kiểm tra theo Entity (Checklist)

| Entity | Domain (Repo Interface) | Infra (Config/Repo) | App (Features/DTO) | API (Controller) |
| :--- | :---: | :---: | :---: | :---: |
| **Service** | [x] | [x] | [ ] | [ ] |
| **TicketHistory** | [x] | [ ] | [ ] | [ ] |
| **Feedback** | [x] | [ ] | [ ] | [ ] |
| **User** | [ ] | [ ] | [ ] | [ ] |
| **Account** | [ ] | [ ] | [ ] | [ ] |
| **RefreshToken** | [ ] | [ ] | [ ] | [ ] |

---
> [!IMPORTANT]
> Các Task nên được thực hiện theo thứ tự: **Infra -> Application -> API** để đảm bảo tính nhất quán của dữ liệu.
