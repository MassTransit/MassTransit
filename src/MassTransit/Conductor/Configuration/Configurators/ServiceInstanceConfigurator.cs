namespace MassTransit.Conductor.Configuration.Configurators
{
    using System;
    using Definition;
    using EndpointConfigurators;
    using GreenPipes;
    using Server;


    public class ServiceInstanceConfigurator<THost, TEndpointConfigurator> :
        IServiceInstanceConfigurator<TEndpointConfigurator>
        where TEndpointConfigurator : IReceiveEndpointConfigurator
        where THost : IHost
    {
        readonly IReceiveConfigurator<THost, TEndpointConfigurator> _configurator;
        readonly THost _host;
        readonly IServiceInstance _instance;

        public ServiceInstanceConfigurator(IReceiveConfigurator<THost, TEndpointConfigurator> configurator, THost host, IServiceInstance instance)
        {
            _configurator = configurator;
            _host = host;
            _instance = instance;
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter
            endpointNameFormatter, Action<TEndpointConfigurator> configureEndpoint)
        {
            _configurator.ReceiveEndpoint(_host, definition, endpointNameFormatter, endpointConfigurator =>
            {
                var instanceDefinition = new InstanceEndpointDefinition(definition, _instance);

                _configurator.ReceiveEndpoint(_host, instanceDefinition, endpointNameFormatter, instanceEndpointConfigurator =>
                {
                    ConfigureInstanceEndpoint(instanceEndpointConfigurator);

                    IServiceEndpoint serviceEndpoint = new ServiceEndpoint(_instance, endpointConfigurator, instanceEndpointConfigurator);

                    ConfigureServiceEndpoint(serviceEndpoint, endpointConfigurator, configureEndpoint);
                    ConfigureServiceEndpoint(serviceEndpoint, instanceEndpointConfigurator, configureEndpoint);
                });
            });
        }

        public void ReceiveEndpoint(string queueName, Action<TEndpointConfigurator> configureEndpoint)
        {
            string instanceEndpointName = ServiceEndpointNameFormatter.Instance.EndpointName(_instance.InstanceId, queueName);

            _configurator.ReceiveEndpoint(_host, queueName, endpointConfigurator =>
            {
                _configurator.ReceiveEndpoint(_host, instanceEndpointName, instanceEndpointConfigurator =>
                {
                    ConfigureInstanceEndpoint(instanceEndpointConfigurator);

                    var serviceEndpoint = new ServiceEndpoint(_instance, endpointConfigurator, instanceEndpointConfigurator);

                    ConfigureServiceEndpoint(serviceEndpoint, endpointConfigurator, configureEndpoint);
                    ConfigureServiceEndpoint(serviceEndpoint, instanceEndpointConfigurator, configureEndpoint);
                });
            });
        }

        void ConfigureServiceEndpoint(IServiceEndpoint serviceEndpoint, TEndpointConfigurator endpointConfigurator,
            Action<TEndpointConfigurator> configureEndpoint)
        {
            ConfigureServiceEndpoint(endpointConfigurator);

            serviceEndpoint.ConnectConfigurationObserver(endpointConfigurator);
            configureEndpoint(endpointConfigurator);
        }

        ConnectHandle IEndpointConfigurationObserverConnector.ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
        {
            return _configurator.ConnectEndpointConfigurationObserver(observer);
        }

        void IReceiveConfigurator.ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            ReceiveEndpoint(definition, endpointNameFormatter, x => configureEndpoint(x));
        }

        void IReceiveConfigurator.ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            ReceiveEndpoint(queueName, x => configureEndpoint(x));
        }

        public virtual void ConfigureInstanceEndpoint(TEndpointConfigurator configurator)
        {
        }

        protected virtual void ConfigureServiceEndpoint(TEndpointConfigurator configurator)
        {
        }
    }
}
