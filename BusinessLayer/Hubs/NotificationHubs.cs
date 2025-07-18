using Microsoft.AspNetCore.SignalR;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BusinessLayer.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirst("UserID").Value;

            if (!string.IsNullOrEmpty(userId))
            {
                // Logic quản lý group vẫn giữ nguyên
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                Console.WriteLine($"--> SignalR Client connected: {Context.ConnectionId}, UserID: {userId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User.FindFirst("UserID").Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
                Console.WriteLine($"--> SignalR Client disconnected: {Context.ConnectionId}, UserID: {userId}");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}