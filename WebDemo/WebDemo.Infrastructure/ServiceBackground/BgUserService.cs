using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using System;
using WebDemo.Core.Hubs;
using WebDemo.Core.Interfaces;
using WebDemo.Core.RealTimeModels;
//source : https://learn.microsoft.com/en-us/aspnet/core/signalr/background-services?view=aspnetcore-6.0
public class BgUserService : BackgroundService
{
    private readonly ILogger<BgUserService> _logger;
    private readonly IHubContext<NotificationHub,INotificationHub> _hub;

    public BgUserService(ILogger<BgUserService> logger, IHubContext<NotificationHub, INotificationHub> hub)
    {
        _logger = logger;
        _hub = hub;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            
            var notif = new Notification("Youba user exist", DateTime.Now);
            
            await _hub.Clients.All.ReceiveBackgroundNotification(notif);
            _logger.LogInformation($"{ nameof(BgUserService) }"+" running at: {Time}", DateTime.Now);
            await Task.Delay(5000, stoppingToken);
        }
    }
}