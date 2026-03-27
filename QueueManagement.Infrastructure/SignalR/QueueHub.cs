
using Microsoft.AspNetCore.SignalR;

namespace QueueManagement.Infrastructure.SignalR
{
    public class QueueHub : Hub
    {
        // Hub này hiện tại dùng để quản lý kết nối từ Client.
        // Server sẽ gửi tin nhắn thông qua IHubContext<QueueHub>.
    }
}
