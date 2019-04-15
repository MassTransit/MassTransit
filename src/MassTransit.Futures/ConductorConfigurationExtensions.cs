namespace MassTransit
{
    using System;
    using Conductor;
    using Conductor.Configurators;
    using Conductor.Server;


    public static class ConductorConfigurationExtensions
    {
        /// <summary>
        /// Configure a service endpoint, allowing services to be configured on the endpoint
        /// </summary>
        /// <param name="busFactoryConfigurator"></param>
        /// <param name="serviceEndpointName">The queue name for the receive endpoint</param>
        /// <param name="configure"></param>
        public static void ServiceEndpoint(this IInMemoryBusFactoryConfigurator busFactoryConfigurator, string serviceEndpointName,
            Action<IInMemoryReceiveEndpointConfigurator> configure)
        {
            NewId instanceId = NewId.Next();
            var instanceEndpointName = ServiceEndpointNameFormatter.Instance.EndpointName(instanceId);

            busFactoryConfigurator.ReceiveEndpoint(instanceEndpointName, configurator =>
            {
                busFactoryConfigurator.ReceiveEndpoint(serviceEndpointName, endpointConfigurator =>
                {
                    string serviceInstanceEndpointName = ServiceEndpointNameFormatter.Instance.EndpointName(instanceId, serviceEndpointName);

                    busFactoryConfigurator.ReceiveEndpoint(serviceInstanceEndpointName, instanceEndpointConfigurator =>
                    {
                        var instance = new ServiceInstance(instanceId, configurator);

                        IServiceEndpoint serviceEndpoint = new ServiceEndpoint(instance, endpointConfigurator, instanceEndpointConfigurator);

                        serviceEndpoint.ConnectObservers(endpointConfigurator);
                        configure(endpointConfigurator);

                        serviceEndpoint.ConnectObservers(instanceEndpointConfigurator);
                        configure(instanceEndpointConfigurator);
                    });
                });
            });
        }

        public static void ServiceInstance(this IInMemoryBusFactoryConfigurator configurator,
            Action<IServiceInstanceConfigurator<IInMemoryReceiveEndpointConfigurator>> configure)
        {
            var instanceId = NewId.Next();
            var instanceEndpointName = ServiceEndpointNameFormatter.Instance.EndpointName(instanceId);

            configurator.ReceiveEndpoint(instanceEndpointName, endpointConfigurator =>
            {
                var instance = new ServiceInstance(instanceId, endpointConfigurator);

                var instanceConfigurator = new InMemoryServiceInstanceConfigurator(instance, configurator, endpointConfigurator);

                configure?.Invoke(instanceConfigurator);
            });
        }
    }
}
