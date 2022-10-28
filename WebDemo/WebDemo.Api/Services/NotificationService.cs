using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using WebDemo.Core.RealTimeModels;
using WebDemo.Core.Hubs;

namespace WebDemo.Api.Services
{
    public class NotificationService //IHubContext
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext) =>
            _hubContext = hubContext;

        public Task SendNotificationAsync(Notification notification) =>
            notification is not null
                ? _hubContext.Clients.All.SendAsync("NotificationReceived", notification)
                : Task.CompletedTask;
    }
}
