#nullable enable
namespace MassTransit.GrpcTransport.Configuration
{
    using System;
    using MassTransit.Configuration;


    public class GrpcBusFactoryConfigurator :
        BusFactoryConfigurator,
        IGrpcBusFactoryConfigurator,
        IBusFactory
    {
        readonly IGrpcBusConfiguration _busConfiguration;
        readonly IGrpcHostConfiguration _hostConfiguration;

        public GrpcBusFactoryConfigurator(IGrpcBusConfiguration busConfiguration)
            : base(busConfiguration)
        {
            _busConfiguration = busConfiguration;
            _hostConfiguration = busConfiguration.HostConfiguration;

            busConfiguration.BusEndpointConfiguration.Consume.Configurator.AutoStart = true;
        }

        public IReceiveEndpointConfiguration CreateBusEndpointConfiguration(Action<IReceiveEndpointConfigurator> configure)
        {
            var hostAddress = _hostConfiguration.HostAddress;

            var queueName = $"bus-{hostAddress.Host}-{hostAddress.Port}";

            return _hostConfiguration.CreateReceiveEndpointConfiguration(queueName, _busConfiguration.BusEndpointConfiguration, configure);
        }

        public override bool AutoStart
        {
            set { }
        }

        public void Publish<T>(Action<IGrpcMessagePublishTopologyConfigurator<T>>? configureTopology)
            where T : class
        {
            IGrpcMessagePublishTopologyConfigurator<T> configurator = _busConfiguration.Topology.Publish.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public void Host(Action<IGrpcHostConfigurator>? configure)
        {
            configure?.Invoke(_hostConfiguration.Configurator);
        }

        public void Host(Uri baseAddress, Action<IGrpcHostConfigurator>? configure)
        {
            _hostConfiguration.BaseAddress = baseAddress;

            configure?.Invoke(_hostConfiguration.Configurator);
        }

        public new IGrpcPublishTopologyConfigurator PublishTopology => _busConfiguration.Topology.Publish;

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter? endpointNameFormatter,
            Action<IGrpcReceiveEndpointConfigurator>? configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter? endpointNameFormatter,
            Action<IReceiveEndpointConfigurator>? configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(string queueName, Action<IGrpcReceiveEndpointConfigurator> configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(queueName, configureEndpoint);
        }

        public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(queueName, configureEndpoint);
        }
    }
}
