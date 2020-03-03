namespace MassTransit.Conductor.Configuration.Configurators
{
    using System;
    using Definition;
    using EndpointConfigurators;
    using GreenPipes;
    using MassTransit.Definition;
    using Server;


    public class ServiceInstanceConfigurator<TEndpointConfigurator> :
        IServiceInstanceConfigurator<TEndpointConfigurator>
        where TEndpointConfigurator : IReceiveEndpointConfigurator
    {
        readonly IReceiveConfigurator<TEndpointConfigurator> _configurator;
        readonly IServiceInstance _instance;
        readonly ServiceInstanceOptions _options;
        readonly TEndpointConfigurator _instanceEndpointConfigurator;
        readonly IServiceInstanceTransportConfigurator<TEndpointConfigurator> _transportConfigurator;

        public ServiceInstanceConfigurator(IReceiveConfigurator<TEndpointConfigurator> configurator,
            IServiceInstanceTransportConfigurator<TEndpointConfigurator> transportConfigurator,
            IServiceInstance instance,
            ServiceInstanceOptions options,
            TEndpointConfigurator instanceEndpointConfigurator = default)
        {
            _configurator = configurator;
            _transportConfigurator = transportConfigurator;
            _instance = instance;
            _options = options;
            _instanceEndpointConfigurator = instanceEndpointConfigurator;
        }

        public IEndpointNameFormatter EndpointNameFormatter => _options.EndpointNameFormatter;

        ConnectHandle IEndpointConfigurationObserverConnector.ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
        {
            return _configurator.ConnectEndpointConfigurationObserver(observer);
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<TEndpointConfigurator> configureEndpoint)
        {
            endpointNameFormatter ??= EndpointNameFormatter;

            if (_instanceEndpointConfigurator != null)
            {
                _configurator.ReceiveEndpoint(definition, endpointNameFormatter, endpointConfigurator =>
                {
                    _transportConfigurator.ConfigureServiceEndpoint(endpointConfigurator);
                    endpointConfigurator.AddDependency(_instanceEndpointConfigurator);

                    ConfigureServiceEndpoint(endpointConfigurator, _instanceEndpointConfigurator, configureEndpoint);
                });
            }
            else
            {
                var controlEndpointDefinition = new ServiceControlEndpointDefinition(definition);

                _configurator.ReceiveEndpoint(controlEndpointDefinition, endpointNameFormatter, controlEndpointConfigurator =>
                {
                    _transportConfigurator.ConfigureControlEndpoint(controlEndpointConfigurator);

                    _configurator.ReceiveEndpoint(definition, endpointNameFormatter, endpointConfigurator =>
                    {
                        endpointConfigurator.AddDependency(controlEndpointConfigurator);

                        _transportConfigurator.ConfigureServiceEndpoint(endpointConfigurator);

                        ConfigureServiceEndpoint(endpointConfigurator, controlEndpointConfigurator, configureEndpoint);
                    });
                });
            }

            if (_options.InstanceServiceEndpointEnabled)
            {
                var instanceServiceEndpointDefinition = new InstanceServiceEndpointDefinition(definition, _instance);

                _configurator.ReceiveEndpoint(instanceServiceEndpointDefinition, endpointNameFormatter, endpointConfigurator =>
                {
                    _transportConfigurator.ConfigureInstanceServiceEndpoint(endpointConfigurator);

                    ConfigureServiceEndpoint(endpointConfigurator, configureEndpoint);
                });
            }
        }

        public void ReceiveEndpoint(string queueName, Action<TEndpointConfigurator> configureEndpoint)
        {
            var endpointDefinition = new NamedEndpointDefinition(queueName);

            ReceiveEndpoint(endpointDefinition, EndpointNameFormatter, configureEndpoint);
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

        void ConfigureServiceEndpoint(TEndpointConfigurator endpointConfigurator, TEndpointConfigurator controlEndpointConfigurator,
            Action<TEndpointConfigurator> configureEndpoint)
        {
            var configurator = new ServiceEndpointConfigurator(_instance, _configurator, endpointConfigurator,
                controlEndpointConfigurator);

            configureEndpoint(endpointConfigurator);
        }

        void ConfigureServiceEndpoint(TEndpointConfigurator endpointConfigurator, Action<TEndpointConfigurator> configureEndpoint)
        {
            var configurator = new ServiceEndpointConfigurator(_instance, _configurator, endpointConfigurator);

            configureEndpoint(endpointConfigurator);
        }
    }
}
