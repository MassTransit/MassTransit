namespace BatchingConsumerExplicit;

using System;
using System.Threading.Tasks;
using BatchingConsumer;
using MassTransit;

public class Program
{
    public static async Task Main()
    {
        var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.ReceiveEndpoint("audit-service", e =>
            {
                e.PrefetchCount = 1000;

                e.Batch<OrderAudit>(b =>
                {
                    b.MessageLimit = 100;
                    b.ConcurrencyLimit = 10;
                    b.TimeLimit = TimeSpan.FromSeconds(1);

                    b.Consumer(() => new OrderAuditConsumer());
                });
            });
        });
    }
}
