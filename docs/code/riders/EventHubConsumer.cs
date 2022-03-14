namespace EventHubConsumer
{
    using System.Threading.Tasks;
    using MassTransit;
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
                    rider.AddConsumer<EventHubMessageConsumer>();

                    rider.UsingEventHub((context, k) =>
                    {
                        k.Host("connection-string");

                        k.Storage("connection-string");

                        k.ReceiveEndpoint("input-event-hub", c =>
                        {
                            c.ConfigureConsumer<EventHubMessageConsumer>(context);
                        });
                    });
                });
            });
        }

        class EventHubMessageConsumer :
            IConsumer<EventHubMessage>
        {
            public Task Consume(ConsumeContext<EventHubMessage> context)
            {
                return Task.CompletedTask;
            }
        }

        public interface EventHubMessage
        {
            string Text { get; }
        }
    }
}
