namespace MassTransit.AmazonSqsTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using Configurators;
    using Registration;


    public class AmazonSqsRegistrationBusFactory :
        TransportRegistrationBusFactory<IAmazonSqsReceiveEndpointConfigurator>
    {
        readonly AmazonSqsBusConfiguration _busConfiguration;
        readonly Action<IBusRegistrationContext, IAmazonSqsBusFactoryConfigurator> _configure;

        public AmazonSqsRegistrationBusFactory(Action<IBusRegistrationContext, IAmazonSqsBusFactoryConfigurator> configure)
            : this(new AmazonSqsBusConfiguration(new AmazonSqsTopologyConfiguration(AmazonSqsBusFactory.MessageTopology)), configure)
        {
        }

        AmazonSqsRegistrationBusFactory(AmazonSqsBusConfiguration busConfiguration,
            Action<IBusRegistrationContext, IAmazonSqsBusFactoryConfigurator> configure)
            : base(busConfiguration.HostConfiguration)
        {
            _configure = configure;

            _busConfiguration = busConfiguration;
        }

        public override IBusInstance CreateBus(IBusRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications)
        {
            var configurator = new AmazonSqsBusFactoryConfigurator(_busConfiguration);

            return CreateBus(configurator, context, _configure, specifications);
        }
    }
}
