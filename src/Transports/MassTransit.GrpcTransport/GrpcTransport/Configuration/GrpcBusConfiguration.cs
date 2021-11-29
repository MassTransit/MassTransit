namespace MassTransit.GrpcTransport.Configuration
{
    using System;
    using MassTransit.Configuration;
    using Observables;
    using Serialization;


    public class GrpcBusConfiguration :
        GrpcEndpointConfiguration,
        IGrpcBusConfiguration
    {
        readonly BusObservable _busObservers;

        public GrpcBusConfiguration(IGrpcTopologyConfiguration topologyConfiguration, Uri baseAddress)
            : base(topologyConfiguration)
        {
            HostConfiguration = new GrpcHostConfiguration(this, baseAddress, topologyConfiguration);

            var factory = new GrpcSerializerFactory();

            Serialization.Clear();
            Serialization.AddSerializer(factory);
            Serialization.AddDeserializer(factory, true);

            BusEndpointConfiguration = CreateEndpointConfiguration();

            _busObservers = new BusObservable();
        }

        IHostConfiguration IBusConfiguration.HostConfiguration => HostConfiguration;
        IEndpointConfiguration IBusConfiguration.BusEndpointConfiguration => BusEndpointConfiguration;
        IBusObserver IBusConfiguration.BusObservers => _busObservers;

        public IGrpcEndpointConfiguration BusEndpointConfiguration { get; }
        public IGrpcHostConfiguration HostConfiguration { get; }

        public ConnectHandle ConnectBusObserver(IBusObserver observer)
        {
            return _busObservers.Connect(observer);
        }

        public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
        {
            return HostConfiguration.ConnectEndpointConfigurationObserver(observer);
        }
    }
}
