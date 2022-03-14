namespace UsageHandler
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using System.Threading.Tasks;
    using UsageContracts;
    using MassTransit;

    public class Program
    {
        public static async Task Main()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransit(x =>
                {
                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.ReceiveEndpoint("order-service", e =>
                        {
                            e.Handler<SubmitOrder>(async context =>
                            {
                                await Console.Out.WriteLineAsync($"Submit Order Received: {context.Message.OrderId}");
                            });
                        });
                    });
                })
                .BuildServiceProvider();
        }
    }
}
