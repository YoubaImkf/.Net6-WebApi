using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebDemo.Core.Hubs;
using WebDemo.Core.Interfaces;
using WebDemo.Core.RealTimeModels;
// source : https://learn.microsoft.com/en-us/aspnet/core/signalr/background-services?view=aspnetcore-6.0

// SECOND WAY https://www.jobsity.com/blog/getting-started-with-background-tasks-in-asp.net-core-webapi#:~:text=When%20to%20Use%20IHostedService%20and%20BackgroundService
public class BgUserService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;  // Object to get services in function of dependancies
    private readonly ILogger<BgUserService> _logger; // Object Logger 
    private readonly IHubContext<NotificationHub, INotificationHub> _hub; // Object Hub

    public BgUserService(ILogger<BgUserService> logger, IHubContext<NotificationHub, INotificationHub> hub, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _hub = hub;
        _serviceScopeFactory = serviceScopeFactory;

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested) // stop when the token is call
        {
            // get UserServic| Create a dependency injection scope in the background thread  
            using (var scope = _serviceScopeFactory.CreateScope())
            { //source https://stackoverflow.com/questions/55381340/create-scope-factory-in-asp-net-core
                var userService = scope.ServiceProvider.GetService<IUserService>();
                var users = await userService.GetAllAsync();

                foreach (var item in users)
                {
                    if(item.FirstName == "Zouhir")
                    {
                        var notif = new Notification(item.FirstName + " user exist", DateTime.Now);
                        // SignalR Notif 
                        await _hub.Clients.All.ReceiveBackgroundNotification(notif, item);
                    }
                }

   
            }

            //_logger.LogInformation($"{ nameof(BgUserService) }"+" running at: {Time}", DateTime.Now);
            await Task.Delay(5000, stoppingToken); // each 5sec do above actions 

        }
    }

}