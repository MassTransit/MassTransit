namespace MassTransit.HttpTransport.Specifications
{
    using System;
    using System.Collections.Generic;
    using BusConfigurators;
    using Configuration;
    using GreenPipes;
    using Hosting;
    using MassTransit.Builders;


    public class HttpBusFactoryConfigurator :
        BusFactoryConfigurator,
        IHttpBusFactoryConfigurator,
        IBusFactory
    {
        readonly IHttpBusConfiguration _busConfiguration;
        readonly IHttpHostConfiguration _hostConfiguration;

        public HttpBusFactoryConfigurator(IHttpBusConfiguration busConfiguration)
            : base(busConfiguration)
        {
            _busConfiguration = busConfiguration;
            _hostConfiguration = busConfiguration.HostConfiguration;
        }

        public IBusControl CreateBus()
        {
            void ConfigureBusEndpoint(IHttpReceiveEndpointConfigurator configurator)
            {
            }

            var busReceiveEndpointConfiguration = _busConfiguration.HostConfiguration
                .CreateReceiveEndpointConfiguration("bus", _busConfiguration.BusEndpointConfiguration, ConfigureBusEndpoint);

            var builder = new ConfigurationBusBuilder(_busConfiguration, busReceiveEndpointConfiguration);

            return builder.Build();
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;
        }

        public void Host(HttpHostSettings settings)
        {
            _busConfiguration.HostConfiguration.Settings = settings;
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IHttpReceiveEndpointConfigurator> configureEndpoint = null)
        {
            _hostConfiguration.ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            _hostConfiguration.ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(IHttpHost host, IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IHttpReceiveEndpointConfigurator> configureEndpoint = null)
        {
            _hostConfiguration.ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(string queueName, Action<IHttpReceiveEndpointConfigurator> configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(queueName, configureEndpoint);
        }

        public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(queueName, configureEndpoint);
        }

        public void ReceiveEndpoint(IHttpHost host, string queueName, Action<IHttpReceiveEndpointConfigurator> configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(queueName, configureEndpoint);
        }
    }
}
