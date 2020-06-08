namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using Configurators;
    using Registration;


    public class ServiceBusRegistrationBusFactory :
        TransportRegistrationBusFactory
    {
        readonly ServiceBusBusConfiguration _busConfiguration;
        readonly Action<IRegistrationContext, IServiceBusBusFactoryConfigurator> _configure;

        public ServiceBusRegistrationBusFactory(Action<IRegistrationContext, IServiceBusBusFactoryConfigurator> configure)
            : this(new ServiceBusBusConfiguration(new ServiceBusTopologyConfiguration(AzureBusFactory.MessageTopology)), configure)
        {
        }

        ServiceBusRegistrationBusFactory(ServiceBusBusConfiguration busConfiguration,
            Action<IRegistrationContext, IServiceBusBusFactoryConfigurator> configure)
            : base(busConfiguration.HostConfiguration)
        {
            _configure = configure;

            _busConfiguration = busConfiguration;
        }

        public override IBusInstance CreateBus(IRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications)
        {
            var configurator = new ServiceBusBusFactoryConfigurator(_busConfiguration);

            return CreateBus(configurator, context, _configure, specifications);
        }
    }
}
