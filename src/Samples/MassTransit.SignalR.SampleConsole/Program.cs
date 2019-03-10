using MassTransit.SignalR.Contracts;
using MassTransit.SignalR.Sample.Hubs;
using Microsoft.AspNetCore.SignalR.Protocol;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit.SignalR.Utils;
using Microsoft.AspNetCore.SignalR;

namespace MassTransit.SignalR.SampleConsole
{
    static class Program
    {
        internal static async Task Main(string[] args)
        {
            IReadOnlyList<IHubProtocol> protocols = new IHubProtocol[] { new JsonHubProtocol() };
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });

            // Important! The bus must be started before using it!
            await busControl.StartAsync();

            do
            {
                Console.WriteLine("Enter message (or quit to exit)");
                Console.Write("> ");
                string value = Console.ReadLine();

                if ("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
                    break;

                await busControl.Publish<All<ChatHub>>(new
                {
                    Messages = protocols.ToProtocolDictionary("broadcastMessage", new object[] { "backend-process", value })
                });
            }
            while (true);

            await busControl.StopAsync();
        }
    }
}

namespace MassTransit.SignalR.Sample.Hubs
{
    public class ChatHub : Hub
    {
        // Actual implementation in the other project, but MT Needs the hub for the generic message type
    }
}
