namespace MicrosoftContainerAddConsumerEndpoint
{
    using System;
    using System.Threading.Tasks;
    using ContainerConsumers;
    using MassTransit;
    using Microsoft.Extensions.DependencyInjection;

    public class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<SubmitOrderConsumer>(typeof(SubmitOrderConsumerDefinition))
                    .Endpoint(e =>
                    {
                        // override the default endpoint name
                        e.Name = "order-service-extreme";

                        // specify the endpoint as temporary (may be non-durable, auto-delete, etc.)
                        e.Temporary = false;

                        // specify an optional concurrent message limit for the consumer
                        e.ConcurrentMessageLimit = 8;

                        // only use if needed, a sensible default is provided, and a reasonable
                        // value is automatically calculated based upon ConcurrentMessageLimit if 
                        // the transport supports it.
                        e.PrefetchCount = 16;

                        // set if each service instance should have its own endpoint for the consumer
                        // so that messages fan out to each instance.
                        e.InstanceId = "something-unique";
                    });

                x.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));
            });
        }
    }
}