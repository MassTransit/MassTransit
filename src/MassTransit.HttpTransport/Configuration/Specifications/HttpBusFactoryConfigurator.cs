namespace MassTransit.HttpTransport.Specifications
{
    using System;
    using System.Collections.Generic;
    using BusConfigurators;
    using Configuration;
    using Definition;
    using GreenPipes;
    using Hosting;
    using MassTransit.Builders;


    public class HttpBusFactoryConfigurator :
        BusFactoryConfigurator,
        IHttpBusFactoryConfigurator,
        IBusFactory
    {
        readonly IHttpBusConfiguration _configuration;
        readonly IHttpEndpointConfiguration _busEndpointConfiguration;

        public HttpBusFactoryConfigurator(IHttpBusConfiguration configuration, IHttpEndpointConfiguration busEndpointConfiguration)
            : base(configuration, busEndpointConfiguration)
        {
            _configuration = configuration;
            _busEndpointConfiguration = busEndpointConfiguration;
        }

        public IBusControl CreateBus()
        {
            var busReceiveEndpointConfiguration = _configuration.CreateReceiveEndpointConfiguration("bus", _busEndpointConfiguration);

            var builder = new ConfigurationBusBuilder(_configuration, busReceiveEndpointConfiguration, BusObservable);

            ApplySpecifications(builder);

            var bus = builder.Build();

            return bus;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;
        }

        public IHttpHost Host(HttpHostSettings settings)
        {
            var hostConfiguration = _configuration.CreateHostConfiguration(settings);

            return hostConfiguration.Host;
        }

        public override void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<IHttpReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            var configuration = _configuration.CreateReceiveEndpointConfiguration(queueName, _configuration.CreateEndpointConfiguration());

            void Configure(IHttpReceiveEndpointConfigurator configurator)
            {
                definition.Configure(configurator);

                configureEndpoint?.Invoke(configurator);
            }

            ConfigureReceiveEndpoint(configuration, configuration.Configurator, Configure);
        }

        public override void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            ReceiveEndpoint(queueName, configureEndpoint);
        }

        public void ReceiveEndpoint(string queueName, Action<IHttpReceiveEndpointConfigurator> configureEndpoint)
        {
            var configuration = _configuration.CreateReceiveEndpointConfiguration(queueName, _configuration.CreateEndpointConfiguration());

            ConfigureReceiveEndpoint(configuration, configuration.Configurator, configureEndpoint);
        }

        public void ReceiveEndpoint(IHttpHost host, IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IHttpReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            if (!_configuration.Hosts.TryGetHost(host, out var hostConfiguration))
                throw new ArgumentException("The host was not configured on this bus", nameof(host));

            var configuration = hostConfiguration.CreateReceiveEndpointConfiguration(queueName);

            void Configure(IHttpReceiveEndpointConfigurator configurator)
            {
                definition.Configure(configurator);

                configureEndpoint?.Invoke(configurator);
            }

            ConfigureReceiveEndpoint(configuration, configuration.Configurator, Configure);
        }

        public void ReceiveEndpoint(IHttpHost host, string queueName, Action<IHttpReceiveEndpointConfigurator> configure)
        {
            if (!_configuration.Hosts.TryGetHost(host, out var hostConfiguration))
                throw new ArgumentException("The host was not configured on this bus", nameof(host));

            var configuration = hostConfiguration.CreateReceiveEndpointConfiguration(queueName);

            ConfigureReceiveEndpoint(configuration, configuration.Configurator, configure);
        }

        public void ReceiveEndpoint(Action<IHttpReceiveEndpointConfigurator> configure = null)
        {
            var configuration = _configuration.CreateReceiveEndpointConfiguration("", _configuration.CreateEndpointConfiguration());

            ConfigureReceiveEndpoint(configuration, configuration.Configurator, configure);
        }
    }
}
