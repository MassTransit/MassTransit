namespace UsageConsumerBus
{
    using System;
    using System.Threading.Tasks;
    using UsageConsumer;
    using MassTransit;

    public class Program
    {
        public static async Task Main()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.ReceiveEndpoint("order-service", e =>
                {
                    e.Consumer<SubmitOrderConsumer>();
                });
            });
        }
    }
}