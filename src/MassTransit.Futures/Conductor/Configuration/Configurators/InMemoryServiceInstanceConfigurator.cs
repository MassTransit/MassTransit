namespace MassTransit.Conductor.Configurators
{
    using System;
    using Definition;
    using Server;


    public class InMemoryServiceInstanceConfigurator :
        IServiceInstanceConfigurator<IInMemoryReceiveEndpointConfigurator>
    {
        readonly IServiceInstance _instance;
        readonly IInMemoryBusFactoryConfigurator _configurator;
        readonly IInMemoryReceiveEndpointConfigurator _instanceConfigurator;

        public InMemoryServiceInstanceConfigurator(IServiceInstance instance, IInMemoryBusFactoryConfigurator configurator, IInMemoryReceiveEndpointConfigurator
            instanceConfigurator)
        {
            _instance = instance;
            _configurator = configurator;
            _instanceConfigurator = instanceConfigurator;
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter = null,
            Action<IInMemoryReceiveEndpointConfigurator> configureEndpoint = null)
        {
            _configurator.ReceiveEndpoint(definition, endpointNameFormatter, endpointConfigurator =>
            {
                var instanceDefinition = new InstanceEndpointDefinition(definition, _instance);

                _configurator.ReceiveEndpoint(instanceDefinition, endpointNameFormatter, instanceEndpointConfigurator =>
                {
                    IServiceEndpoint serviceEndpoint = new ServiceEndpoint(_instance, endpointConfigurator, instanceEndpointConfigurator);

                    serviceEndpoint.ConnectObservers(endpointConfigurator);
                    configureEndpoint(endpointConfigurator);

                    serviceEndpoint.ConnectObservers(instanceEndpointConfigurator);
                    configureEndpoint(instanceEndpointConfigurator);
                });
            });
        }

        public void ReceiveEndpoint(string queueName, Action<IInMemoryReceiveEndpointConfigurator> configureEndpoint)
        {
            string instanceEndpointName = ServiceEndpointNameFormatter.Instance.EndpointName(_instance.InstanceId, queueName);

            _configurator.ReceiveEndpoint(queueName, endpointConfigurator =>
            {
                _configurator.ReceiveEndpoint(instanceEndpointName, instanceEndpointConfigurator =>
                {
                    var serviceEndpoint = new ServiceEndpoint(_instance, endpointConfigurator, instanceEndpointConfigurator);

                    serviceEndpoint.ConnectObservers(endpointConfigurator);
                    configureEndpoint(endpointConfigurator);

                    serviceEndpoint.ConnectObservers(instanceEndpointConfigurator);
                    configureEndpoint(instanceEndpointConfigurator);
                });
            });
        }
    }
}
