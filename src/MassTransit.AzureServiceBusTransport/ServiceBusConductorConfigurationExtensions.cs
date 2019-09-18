namespace MassTransit
{
    using System;
    using Conductor.Configuration;
    using Conductor.Configuration.Configurators;
    using Conductor.Server;
    using AzureServiceBusTransport;
    using AzureServiceBusTransport.Configurators;


    public static class ServiceBusConductorConfigurationExtensions
    {
        /// <summary>
        /// Configure a service instance, which supports one or more receive endpoints, all of which are managed by conductor.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void ServiceInstance(this IServiceBusBusFactoryConfigurator configurator,
            Action<IServiceInstanceConfigurator<IServiceBusReceiveEndpointConfigurator>> configure)
        {
            var instanceId = NewId.Next();
            var instanceEndpointName = ServiceEndpointNameFormatter.Instance.EndpointName(instanceId);

            configurator.ReceiveEndpoint(instanceEndpointName, endpointConfigurator =>
            {
                var instance = new ServiceInstance(instanceId, endpointConfigurator);

                var instanceConfigurator = new ServiceBusServiceInstanceConfigurator(configurator, instance);

                instanceConfigurator.ConfigureInstanceEndpoint(endpointConfigurator);

                configure?.Invoke(instanceConfigurator);
            });
        }
    }
}
