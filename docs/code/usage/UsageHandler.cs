namespace UsageHandler;

using System;
using System.Threading.Tasks;
using UsageContracts;
using MassTransit;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddMassTransit(x =>
                {
                    x.UsingInMemory((cxt, cfg) =>
                    {
                        cfg.ReceiveEndpoint("order-service", e =>
                        {
                            e.Handler<SubmitOrder>(async context =>
                            {
                                await Console.Out.WriteLineAsync($"Submit Order Received: {context.Message.OrderId}");
                            });
                        });
                    });
                });
            })
            .Build()
            .RunAsync();
    }
}
