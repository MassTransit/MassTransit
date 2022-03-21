namespace ServiceBusReceiveEndpoint;

using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddMassTransit(x =>
                {
                    x.UsingAzureServiceBus((context, cfg) =>
                    {
                        cfg.Host("connection-string");

                        cfg.ReceiveEndpoint("input-queue", e =>
                        {
                            // all of these are optional!!

                            e.PrefetchCount = 100;

                            // number of "threads" to run concurrently
                            e.MaxConcurrentCalls = 100;

                            // default, but shown for example
                            e.LockDuration = TimeSpan.FromMinutes(5);

                            // lock will be renewed up to 30 minutes
                            e.MaxAutoRenewDuration = TimeSpan.FromMinutes(30);
                        });
                    });
                });
            })
            .Build()
            .RunAsync();
    }
}
