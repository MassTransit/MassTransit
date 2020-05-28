namespace MassTransit.AmazonSqsTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using Configurators;
    using Registration;


    public class AmazonSqsRegistrationBusFactory<TContainerContext> :
        TransportRegistrationBusFactory<TContainerContext>
        where TContainerContext : class
    {
        readonly AmazonSqsBusConfiguration _busConfiguration;
        readonly Action<IRegistrationContext<TContainerContext>, IAmazonSqsBusFactoryConfigurator> _configure;

        public AmazonSqsRegistrationBusFactory(Action<IRegistrationContext<TContainerContext>, IAmazonSqsBusFactoryConfigurator> configure)
            : this(new AmazonSqsBusConfiguration(new AmazonSqsTopologyConfiguration(AmazonSqsBusFactory.MessageTopology)), configure)
        {
        }

        AmazonSqsRegistrationBusFactory(AmazonSqsBusConfiguration busConfiguration,
            Action<IRegistrationContext<TContainerContext>, IAmazonSqsBusFactoryConfigurator> configure)
            : base(busConfiguration.HostConfiguration)
        {
            _configure = configure;

            _busConfiguration = busConfiguration;
        }

        public override IBusInstance CreateBus(IRegistrationContext<TContainerContext> context, IEnumerable<IBusInstanceSpecification> specifications)
        {
            var configurator = new AmazonSqsBusFactoryConfigurator(_busConfiguration);

            return CreateBus(configurator, context, _configure, specifications);
        }
    }
}
