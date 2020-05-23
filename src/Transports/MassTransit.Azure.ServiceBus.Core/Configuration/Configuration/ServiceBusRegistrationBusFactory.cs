namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using System;
    using Configurators;
    using Registration;


    public class ServiceBusRegistrationBusFactory<TContainerContext> :
        TransportRegistrationBusFactory<TContainerContext>
        where TContainerContext : class
    {
        readonly ServiceBusBusConfiguration _busConfiguration;
        readonly Action<IRegistrationContext<TContainerContext>, IServiceBusBusFactoryConfigurator> _configure;

        public ServiceBusRegistrationBusFactory(Action<IRegistrationContext<TContainerContext>, IServiceBusBusFactoryConfigurator> configure)
            : this(new ServiceBusBusConfiguration(new ServiceBusTopologyConfiguration(AzureBusFactory.MessageTopology)), configure)
        {
        }

        ServiceBusRegistrationBusFactory(ServiceBusBusConfiguration busConfiguration,
            Action<IRegistrationContext<TContainerContext>, IServiceBusBusFactoryConfigurator> configure)
            : base(busConfiguration.HostConfiguration)
        {
            _configure = configure;

            _busConfiguration = busConfiguration;
        }

        public override IBusInstance CreateBus(IRegistrationContext<TContainerContext> context)
        {
            var configurator = new ServiceBusBusFactoryConfigurator(_busConfiguration);

            return CreateBus(configurator, context, _configure);
        }
    }
}
