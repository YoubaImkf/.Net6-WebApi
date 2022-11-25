using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using WebApiDemo.Core.Models;
using WebApiDemo.Dtos;
using WebDemo.Core.Interfaces;
using WebDemo.Core.RealTimeModels;

namespace WebDemo.Core.Hubs
{
    //StronglyTypedChatHub
    //https://learn.microsoft.com/fr-fr/aspnet/core/signalr/hubs?view=aspnetcore-6.0
    public class NotificationHub : Hub<INotificationHub>
    {
        public async Task NotifyAll(Notification notification, UserAddOrUpdateDto user) =>
           await Clients.All.ReceiveNotification(notification, user);
//                                                  record   |
        public async Task BackgroundNotification(Notification notification, UserDto user) => 
            await Clients.All.ReceiveBackgroundNotification(notification, user);
    }
}
