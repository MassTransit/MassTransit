namespace MassTransit
{
    using System;
    using Conductor.Configuration;
    using Conductor.Configuration.Configurators;
    using Conductor.Server;


    public static class ConductorConfigurationExtensions
    {
        /// <summary>
        /// Configure a service instance, which supports one or more receive endpoints, all of which are managed by conductor.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void ServiceInstance(this IInMemoryBusFactoryConfigurator configurator,
            Action<IServiceInstanceConfigurator<IInMemoryReceiveEndpointConfigurator>> configure)
        {
            var instanceId = NewId.Next();
            var instanceEndpointName = ServiceEndpointNameFormatter.Instance.EndpointName(instanceId);

            configurator.ReceiveEndpoint(instanceEndpointName, endpointConfigurator =>
            {
                var instance = new ServiceInstance(instanceId, endpointConfigurator);

                var instanceConfigurator = new InMemoryServiceInstanceConfigurator(configurator, instance);

                configure?.Invoke(instanceConfigurator);
            });
        }
    }
}
