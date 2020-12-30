namespace UsageConsumerTemporaryEndpoint
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
                // ensures the receive endpoint is deleted when the bus is stopped
                cfg.ReceiveEndpoint(new TemporaryEndpointDefinition(), e =>
                {
                    e.Consumer<SubmitOrderConsumer>();
                });
            });
        }
    }
}
