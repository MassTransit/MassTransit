namespace UsageInstance
{
    using System;
    using System.Threading.Tasks;
    using UsageConsumer;
    using MassTransit;

    public class Program
    {
        public static async Task Main()
        {
            var submitOrderConsumer = new SubmitOrderConsumer();

            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.ReceiveEndpoint("order-service", e =>
                {
                    e.Instance(submitOrderConsumer);
                });
            });
        }
    }
}