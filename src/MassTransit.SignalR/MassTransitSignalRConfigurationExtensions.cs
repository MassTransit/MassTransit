namespace MassTransit.SignalR
{
    using MassTransit;
    using MassTransit.ExtensionsDependencyInjectionIntegration;
    using MassTransit.SignalR.Contracts;
    using MassTransit.SignalR.Utils;
    using Microsoft.AspNetCore.SignalR;
    using System;
    using Definition;


    public static class MassTransitSignalRConfigurationExtensions
    {
        public static void AddSignalRHubConsumers<THub>(this IServiceCollectionConfigurator configurator)
            where THub : Hub
        {
            configurator.AddRequestClient<GroupManagement<THub>>(TimeSpan.FromSeconds(10));

            var consumers = HubConsumersCache.GetOrAdd<THub>();

            foreach (var consumer in consumers)
            {
                configurator.AddConsumer(consumer);
            }
        }

        public static void AddSignalRHubEndpoints<THub, TEndpointConfigurator>(this IBusFactoryConfigurator configurator,
            IServiceProvider serviceProvider,
            IHost host, Action<TEndpointConfigurator> configureEndpoint = null)
            where TEndpointConfigurator : class, IReceiveEndpointConfigurator
            where THub : Hub
        {
            var consumers = HubConsumersCache.GetOrAdd<THub>();

            var queueNameBase = host.Topology.CreateTemporaryQueueName($"signalRBackplane-{typeof(THub).Name}-");

            // Loop through our 5 hub consumers and create a temporary endpoint for each
            foreach (var consumerType in consumers)
            {
                // remove `1 from generic type
                var name = consumerType.Name;
                int index = name.IndexOf('`');
                if (index > 0)
                    name = name.Remove(index);

                configurator.ReceiveEndpoint(new HubEndpointDefinition<THub>(), null, e =>
                {
                    configureEndpoint?.Invoke((TEndpointConfigurator)e);

                    e.ConfigureConsumer(serviceProvider, consumerType);
                });
            }
        }


        class HubEndpointDefinition<THub> :
            DefaultEndpointDefinition
            where THub : Hub
        {
            public HubEndpointDefinition()
                : base(true)
            {
            }

            public override string GetEndpointName(IEndpointNameFormatter formatter)
            {
                return formatter.TemporaryEndpoint($"signalr_{typeof(THub).Name}");
            }
        }
    }
}
