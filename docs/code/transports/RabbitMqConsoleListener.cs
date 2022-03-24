namespace OrderSystem.Events
{
    using System;

    public interface OrderSubmitted
    {
        Guid OrderId { get; }
    }
}

namespace RabbitMqConsoleListener
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using OrderSystem.Events;
    using MassTransit;
    using Microsoft.Extensions.Hosting;

    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host("localhost", "/", h =>
                            {
                                h.Username("guest");
                                h.Password("guest");
                            });

                            cfg.ReceiveEndpoint("order-events-listener", e =>
                            {
                                e.Consumer<OrderSubmittedEventConsumer>();
                            });
                        });
                    });
                })
                .Build()
                .RunAsync();
        }

        class OrderSubmittedEventConsumer :
            IConsumer<OrderSubmitted>
        {
            public async Task Consume(ConsumeContext<OrderSubmitted> context)
            {
                Console.WriteLine("Order Submitted: {0}", context.Message.OrderId);
            }
        }
    }
}
