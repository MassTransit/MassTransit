#nullable enable
namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;


    public class ServiceInstanceConfigurator<TEndpointConfigurator> :
        IServiceInstanceConfigurator<TEndpointConfigurator>
        where TEndpointConfigurator : IReceiveEndpointConfigurator
    {
        readonly ServiceInstanceOptions _options;

        public ServiceInstanceConfigurator(IBusFactoryConfigurator<TEndpointConfigurator> configurator, ServiceInstanceOptions options,
            TEndpointConfigurator instanceEndpointConfigurator)
        {
            if (instanceEndpointConfigurator == null)
                throw new ArgumentNullException(nameof(instanceEndpointConfigurator), "Service instance now requires an instance endpoint");

            BusConfigurator = configurator;
            InstanceEndpointConfigurator = instanceEndpointConfigurator;
            _options = options;
        }

        public Uri InstanceAddress => InstanceEndpointConfigurator.InputAddress;

        IBusFactoryConfigurator IServiceInstanceConfigurator.BusConfigurator => BusConfigurator;
        IReceiveEndpointConfigurator IServiceInstanceConfigurator.InstanceEndpointConfigurator => InstanceEndpointConfigurator;

        public IBusFactoryConfigurator<TEndpointConfigurator> BusConfigurator { get; }
        public TEndpointConfigurator InstanceEndpointConfigurator { get; }

        public void AddSpecification(ISpecification specification)
        {
            InstanceEndpointConfigurator.AddEndpointSpecification(new ValidateSpecification(specification));
        }

        public IEndpointNameFormatter EndpointNameFormatter => _options.EndpointNameFormatter;

        public T Options<T>(Action<T>? configure = null)
            where T : IOptions, new()
        {
            return _options.Options(configure);
        }

        public T Options<T>(T options, Action<T>? configure = null)
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

        void IReceiveConfigurator.ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter? endpointNameFormatter,
            Action<IReceiveEndpointConfigurator>? configureEndpoint)
        {
            ReceiveEndpoint(definition, endpointNameFormatter, x => configureEndpoint?.Invoke(x));
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter? endpointNameFormatter,
            Action<TEndpointConfigurator>? configureEndpoint)
        {
            endpointNameFormatter ??= EndpointNameFormatter;

            BusConfigurator.ReceiveEndpoint(definition, endpointNameFormatter, endpointConfigurator =>
            {
                endpointConfigurator.AddDependency(InstanceEndpointConfigurator);

                configureEndpoint?.Invoke(endpointConfigurator);
            });
        }

        void IReceiveConfigurator.ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            ReceiveEndpoint(queueName, x => configureEndpoint(x));
        }

        public void ReceiveEndpoint(string queueName, Action<TEndpointConfigurator>? configureEndpoint)
        {
            BusConfigurator.ReceiveEndpoint(queueName, endpointConfigurator =>
            {
                endpointConfigurator.AddDependency(InstanceEndpointConfigurator);

                configureEndpoint?.Invoke(endpointConfigurator);
            });
        }

        public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
        {
            return BusConfigurator.ConnectEndpointConfigurationObserver(observer);
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
