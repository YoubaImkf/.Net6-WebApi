using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using System;
using WebDemo.Core.RealTimeModels;
using Microsoft.AspNetCore.SignalR;
using WebDemo.Core.Hubs;
using WebDemo.Core.Interfaces;

//source:https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-6.0&tabs=visual-studio
public class UserManagerService : IHostedService, IDisposable
{
    private int executionCount = 0;
    private readonly IHubContext<NotificationHub, INotificationHub> _hubContext;
    private readonly ILogger<UserManagerService> _logger;
    private Timer? _timer = null;

    public UserManagerService(ILogger<UserManagerService> logger, IHubContext<NotificationHub, INotificationHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");
        _timer = new Timer(DoWork, null, TimeSpan.Zero,
             TimeSpan.FromSeconds(10));

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        var notif = new Notification("Youba user exist", DateTime.Now);
        var count = Interlocked.Increment(ref executionCount);

        _logger.LogInformation(
            "Timed Hosted Service is working. Count: {Count}", count);
        _hubContext.Clients.All.ReceiveBackgroundNotification(notif);
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}