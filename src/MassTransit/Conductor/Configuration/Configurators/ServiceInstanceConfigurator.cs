namespace MassTransit.Conductor.Configurators
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Definition;
    using EndpointConfigurators;
    using GreenPipes;
    using GreenPipes.Util;
    using MassTransit.Definition;
    using Server;


    public class ServiceInstanceConfigurator<TEndpointConfigurator> :
        IServiceInstanceConfigurator<TEndpointConfigurator>
        where TEndpointConfigurator : IReceiveEndpointConfigurator
    {
        readonly IBusFactoryConfigurator<TEndpointConfigurator> _configurator;
        readonly IServiceInstance _instance;
        readonly TEndpointConfigurator _instanceEndpointConfigurator;
        readonly ServiceInstanceOptions _options;
        readonly IServiceInstanceTransportConfigurator<TEndpointConfigurator> _transportConfigurator;

        public ServiceInstanceConfigurator(IBusFactoryConfigurator<TEndpointConfigurator> configurator,
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

        public Uri InstanceAddress =>
            _options.InstanceEndpointEnabled
                ? _instanceEndpointConfigurator.InputAddress
                : throw new ConfigurationException("The instance address is not available, InstanceEndpoint is not enabled");

        public IReceiveConfigurator<TEndpointConfigurator> BusConfigurator => _configurator;

        public TEndpointConfigurator InstanceEndpointConfigurator => _instanceEndpointConfigurator;

        public void AddSpecification(ISpecification specification)
        {
            if (_instanceEndpointConfigurator != null)
                _instanceEndpointConfigurator.AddEndpointSpecification(new ValidateSpecification(specification));
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

        public ConnectHandle ConnectBusObserver(IBusObserver observer)
        {
            return _configurator.ConnectBusObserver(observer);
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            if (_instanceEndpointConfigurator != null)
                return _instanceEndpointConfigurator.ConnectReceiveEndpointObserver(observer);

            return new EmptyConnectHandle();
        }

        public T Options<T>(Action<T> configure = null)
            where T : IOptions, new()
        {
            return _options.Options(configure);
        }

        public T Options<T>(T options, Action<T> configure = null)
            where T : IOptions
        {
            return _options.Options(options, configure);
        }

        public bool TryGetOptions<T>(out T options)
            where T : IOptions
        {
            return _options.TryGetOptions(out options);
        }

        public IEnumerable<T> SelectOptions<T>()
            where T : class
        {
            return _options.SelectOptions<T>();
        }

        void ConfigureServiceEndpoint(TEndpointConfigurator endpointConfigurator, TEndpointConfigurator controlEndpointConfigurator,
            Action<TEndpointConfigurator> configureEndpoint)
        {
            var configurator = new ServiceEndpointConfigurator(_instance, _configurator, endpointConfigurator, controlEndpointConfigurator);

            configureEndpoint(endpointConfigurator);
        }

        void ConfigureServiceEndpoint(TEndpointConfigurator endpointConfigurator, Action<TEndpointConfigurator> configureEndpoint)
        {
            var configurator = new ServiceEndpointConfigurator(_instance, _configurator, endpointConfigurator);

            configureEndpoint(endpointConfigurator);
        }


        class ValidateSpecification :
            IReceiveEndpointSpecification
        {
            readonly ISpecification _specification;

            public ValidateSpecification(ISpecification specification)
            {
                _specification = specification;
            }

            public IEnumerable<ValidationResult> Validate()
            {
                return _specification.Validate();
            }

            public void Configure(IReceiveEndpointBuilder builder)
            {
            }
        }
    }
}
