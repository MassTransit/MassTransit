namespace UsageHandler
{
    using System;
    using System.Threading.Tasks;
    using UsageContracts;
    using MassTransit;

    public class Program
    {
        public static async Task Main()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.ReceiveEndpoint("order-service", e =>
                {
                    e.Handler<SubmitOrder>(async context =>
                    {
                        await Console.Out.WriteLineAsync($"Submit Order Received: {context.Message.OrderId}");
                    });
                });
            });
        }
    }
}