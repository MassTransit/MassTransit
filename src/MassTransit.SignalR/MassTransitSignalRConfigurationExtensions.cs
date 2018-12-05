namespace MassTransit.SignalR
{
    using MassTransit;
    using MassTransit.ConsumeConfigurators;
    using MassTransit.ExtensionsDependencyInjectionIntegration;
    using MassTransit.Scoping;
    using System;
    using System.Collections.Generic;

    public static class MassTransitSignalRConfigurationExtensions
    {
        public static void CreateBackplaneEndpoints<T>(this IBusFactoryConfigurator configurator, IServiceProvider serviceProvider, IHost host, IReadOnlyDictionary<Type, IReadOnlyList<Type>> hubConsumers, Action<T> configureEndpoint = null)
            where T : class, IReceiveEndpointConfigurator
        {
            var factoryType = typeof(ScopeConsumerFactory<>);
            var consumerConfiguratorType = typeof(ConsumerConfigurator<>);

            foreach (var hub in hubConsumers)
            {
                var queueNameBase = host.Topology.CreateTemporaryQueueName($"signalRBackplane-{hub.Key.Name}-");

                // Loop through our 5 hub consumers and create a temporary endpoint for each
                foreach (var consumer in hub.Value)
                {
                    // remove `1 from generic type
                    var name = consumer.Name;
                    int index = name.IndexOf('`');
                    if (index > 0)
                        name = name.Remove(index);

                    configurator.ReceiveEndpoint($"{queueNameBase}{name}-", e =>
                    {
                        configureEndpoint?.Invoke((T)e);

                        IConsumerScopeProvider scopeProvider = new DependencyInjectionConsumerScopeProvider(serviceProvider);

                        var concreteFactoryType = factoryType.MakeGenericType(consumer);

                        var consumerFactory = Activator.CreateInstance(concreteFactoryType, scopeProvider);

                        var concreteConsumerConfiguratorType = consumerConfiguratorType.MakeGenericType(consumer);

                        var consumerConfigurator = Activator.CreateInstance(concreteConsumerConfiguratorType, consumerFactory, e);

                        e.AddEndpointSpecification((IReceiveEndpointSpecification)consumerConfigurator);
                    });
                }
            }

        }
    }
}
