﻿// Source: https://learn.microsoft.com/en-us/aspnet/core/signalr/streaming?view=aspnetcore-6.0#net-client
// Source: https://www.youtube.com/watch?v=-mWewa_R7-8&ab_channel=WorldofZero
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;
using WebApiDemo.Core.Models;
using WebApiDemo.Dtos;
using WebDemo.Core.RealTimeModels;

try
{
    Console.WriteLine("Trying to connect...");

    HubConnection hubConnection = new HubConnectionBuilder()
               .WithUrl("https://localhost:7271/WebDemoHub") //only one slash
               .WithAutomaticReconnect()
               .Build();

    //Start Connection
    await hubConnection.StartAsync();

    Console.WriteLine("            )       )      )                                (      \r\n   (     ( /(    ( /(   ( /(           (      *   )         )\\ )   \r\n   )\\    )\\())   )\\())  )\\())  (       )\\   ` )  /(   (    (()/(   \r\n (((_)  ((_)\\   ((_)\\  ((_)\\   )\\    (((_)   ( )(_))  )\\    /(_))  \r\n )\\___    ((_)   _((_)  _((_) ((_)   )\\___  (_(_())  ((_)  (_))_   \r\n((/ __|  / _ \\  | \\| | | \\| | | __| ((/ __| |_   _|  | __|  |   \\  \r\n | (__  | (_) | | .` | | .` | | _|   | (__    | |    | _|   | |) | \r\n  \\___|  \\___/  |_|\\_| |_|\\_| |___|   \\___|   |_|    |___|  |___/  \r\n                                                                  ");


    //When you receive this response 
    hubConnection.On<Notification, UserAddOrUpdateDto>("ReceiveNotification", (notification, user) =>
    {
        //Do this
        var jsonUser = JsonSerializer.Serialize<UserAddOrUpdateDto>(user); 
       
        Console.WriteLine($"Notif: {notification}");
        Console.WriteLine($"Update User: {jsonUser}");
    });

    hubConnection.On<Notification, UserDto>("ReceiveBackgroundNotification", (notification, user) =>
    {
        var jsonUser = JsonSerializer.Serialize<UserDto>(user);

        Console.WriteLine($"Notif: {notification}");
        Console.WriteLine($"user found: {jsonUser}");

    });

    Console.Read();

}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

