namespace MassTransit.GrpcTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Configuration;
    using Transports;


    public class GrpcRegistrationBusFactory :
        TransportRegistrationBusFactory<IGrpcReceiveEndpointConfigurator>
    {
        readonly GrpcBusConfiguration _busConfiguration;
        readonly Action<IBusRegistrationContext, IGrpcBusFactoryConfigurator> _configure;

        public GrpcRegistrationBusFactory(Uri baseAddress, Action<IBusRegistrationContext, IGrpcBusFactoryConfigurator> configure)
            : this(new GrpcBusConfiguration(new GrpcTopologyConfiguration(GrpcBus.MessageTopology), baseAddress), configure)
        {
        }

        GrpcRegistrationBusFactory(GrpcBusConfiguration busConfiguration,
            Action<IBusRegistrationContext, IGrpcBusFactoryConfigurator> configure)
            : base(busConfiguration.HostConfiguration)
        {
            _configure = configure;

            _busConfiguration = busConfiguration;
        }

        public override IBusInstance CreateBus(IBusRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications, string busName)
        {
            var configurator = new GrpcBusFactoryConfigurator(_busConfiguration);

            return CreateBus(configurator, context, _configure, specifications);
        }
    }
}
