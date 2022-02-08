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

    public static class Program
    {
        public static async Task Main()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
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

            using var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await busControl.StartAsync(source.Token);
            try
            {
                Console.WriteLine("Press enter to exit");

                await Task.Run(() => Console.ReadLine());
            }
            finally
            {
                await busControl.StopAsync();
            }
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
