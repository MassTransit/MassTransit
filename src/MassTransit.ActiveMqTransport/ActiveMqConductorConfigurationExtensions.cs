namespace MassTransit
{
    using System;
    using ActiveMqTransport;
    using ActiveMqTransport.Configurators;
    using Conductor.Configuration;
    using Conductor.Configuration.Configurators;
    using Conductor.Server;


    public static class ActiveMqConductorConfigurationExtensions
    {
        /// <summary>
        /// Configure a service instance, which supports one or more receive endpoints, all of which are managed by conductor.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="host"></param>
        /// <param name="configure"></param>
        public static void ServiceInstance(this IActiveMqBusFactoryConfigurator configurator, IActiveMqHost host,
            Action<IServiceInstanceConfigurator<IActiveMqReceiveEndpointConfigurator>> configure)
        {
            var instanceId = NewId.Next();
            var instanceEndpointName = ServiceEndpointNameFormatter.Instance.EndpointName(instanceId);

            configurator.ReceiveEndpoint(host, instanceEndpointName, endpointConfigurator =>
            {
                var instance = new ServiceInstance(instanceId, endpointConfigurator);

                var instanceConfigurator = new ActiveMqServiceInstanceConfigurator(configurator, host, instance);

                instanceConfigurator.ConfigureInstanceEndpoint(endpointConfigurator);

                configure?.Invoke(instanceConfigurator);
            });
        }
    }
}
