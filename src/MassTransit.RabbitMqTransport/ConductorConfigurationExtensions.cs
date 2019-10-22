namespace MassTransit
{
    using System;
    using Conductor.Configuration;
    using Conductor.Configuration.Configurators;
    using Conductor.Server;
    using RabbitMqTransport;
    using RabbitMqTransport.Configurators;


    public static class RabbitMqConductorConfigurationExtensions
    {
        /// <summary>
        /// Configure a service instance, which supports one or more receive endpoints, all of which are managed by conductor.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void ServiceInstance(this IRabbitMqBusFactoryConfigurator configurator,
            Action<IServiceInstanceConfigurator<IRabbitMqReceiveEndpointConfigurator>> configure)
        {
            var instanceId = NewId.Next();
            var instanceEndpointName = ServiceEndpointNameFormatter.Instance.EndpointName(instanceId);

            configurator.ReceiveEndpoint(instanceEndpointName, endpointConfigurator =>
            {
                var instance = new ServiceInstance(instanceId, endpointConfigurator);

                var instanceConfigurator = new RabbitMqServiceInstanceConfigurator(configurator, instance);

                instanceConfigurator.ConfigureInstanceEndpoint(endpointConfigurator);

                configure?.Invoke(instanceConfigurator);
            });
        }
    }
}
