namespace EventHubProducer
{
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.EventHubIntegration;
    using Microsoft.Extensions.DependencyInjection;

    public class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();

            services.AddMassTransit(x =>
            {
                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host("connection-string");

                    cfg.ConfigureEndpoints(context);
                });

                x.AddRider(rider =>
                {
                    rider.UsingEventHub((context, k) =>
                    {
                        k.Host("connection-string");

                        k.Storage("connection-string");
                    });
                });
            });

            var provider = services.BuildServiceProvider(true);

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(new CancellationTokenSource(10000).Token);

            var serviceScope = provider.CreateScope();

            var producerProvider = serviceScope.ServiceProvider.GetRequiredService<IEventHubProducerProvider>();
            var producer = await producerProvider.GetProducer("some-event-hub");

            await producer.Produce<EventHubMessage>(new { Text = "Hello, Computer." });
        }

        public interface EventHubMessage
        {
            string Text { get; }
        }
    }
}
