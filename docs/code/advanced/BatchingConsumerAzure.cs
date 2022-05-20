namespace BatchingConsumerAzure
{
    using System;
    using System.Threading.Tasks;
    using BatchingConsumer;
    using MassTransit;

    public class Program
    {
        public static async Task Main()
        {
            var busControl = Bus.Factory.CreateUsingAzureServiceBus(cfg =>
            {
                cfg.ReceiveEndpoint("audit-service", e =>
                {
                    e.PrefetchCount = 100;
                    e.MaxConcurrentCalls = 100;

                    e.Batch<OrderAudit>(b =>
                    {
                        b.MessageLimit = 100;
                        b.TimeLimit = TimeSpan.FromSeconds(3);

                        b.Consumer(() => new OrderAuditConsumer());
                    });
                });
            });
        }
    }
}
