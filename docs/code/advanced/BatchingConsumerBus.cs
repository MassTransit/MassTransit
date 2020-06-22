namespace BatchingConsumerBus
{
    using System;
    using System.Threading.Tasks;
    using BatchingConsumer;
    using MassTransit;
    using Microsoft.Extensions.DependencyInjection;

    public class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<OrderAuditConsumer>(typeof(OrderAuditConsumerDefinition));

                x.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));
            });
        }
    }
}