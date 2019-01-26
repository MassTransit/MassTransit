namespace MassTransit.SignalR
{
    using MassTransit;
    using Scoping;
    using System;
    using System.Collections.Generic;
    using ExtensionsDependencyInjectionIntegration.ScopeProviders;
    using Registration;


    public static class MassTransitSignalRConfigurationExtensions
    {
        public static void CreateBackplaneEndpoints<TEndpointConfigurator>(this IBusFactoryConfigurator configurator,
            IServiceProvider serviceProvider,
            IHost host, IReadOnlyDictionary<Type, IReadOnlyList<Type>> hubConsumers, Action<TEndpointConfigurator> configureEndpoint = null)
            where TEndpointConfigurator : class, IReceiveEndpointConfigurator
        {
            IConsumerScopeProvider scopeProvider = new DependencyInjectionConsumerScopeProvider(serviceProvider);

            foreach (var hub in hubConsumers)
            {
                var queueNameBase = host.Topology.CreateTemporaryQueueName($"signalRBackplane-{hub.Key.Name}-");

                // Loop through our 5 hub consumers and create a temporary endpoint for each
                foreach (var consumerType in hub.Value)
                {
                    // remove `1 from generic type
                    var name = consumerType.Name;
                    int index = name.IndexOf('`');
                    if (index > 0)
                        name = name.Remove(index);

                    configurator.ReceiveEndpoint($"{queueNameBase}{name}-", e =>
                    {
                        configureEndpoint?.Invoke((TEndpointConfigurator)e);

                        ConsumerConfiguratorCache.Configure(consumerType, e, scopeProvider);
                    });
                }
            }
        }
    }
}
