namespace MicrosoftContainerAddConsumer
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
                // Add a single consumer
                x.AddConsumer<SubmitOrderConsumer>(typeof(SubmitOrderConsumerDefinition));

                // Add a single consumer by type
                x.AddConsumer(typeof(SubmitOrderConsumer), typeof(SubmitOrderConsumerDefinition));

                // Add all consumers in the specified assembly
                x.AddConsumers(typeof(SubmitOrderConsumer).Assembly);

                // Add all consumers in the namespace containing the specified type
                x.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();
            });
        }
    }
}