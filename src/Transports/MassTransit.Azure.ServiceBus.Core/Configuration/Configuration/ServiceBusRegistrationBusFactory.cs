namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using System;
    using Configurators;
    using Registration;


    public class ServiceBusRegistrationBusFactory<TContainerContext> :
        IRegistrationBusFactory<TContainerContext>
        where TContainerContext : class
    {
        readonly ServiceBusBusConfiguration _busConfiguration;
        readonly Action<IRegistrationContext<TContainerContext>, IServiceBusBusFactoryConfigurator> _configure;

        public ServiceBusRegistrationBusFactory(Action<IRegistrationContext<TContainerContext>, IServiceBusBusFactoryConfigurator> configure)
        {
            _configure = configure;

            var topologyConfiguration = new ServiceBusTopologyConfiguration(AzureBusFactory.MessageTopology);
            _busConfiguration = new ServiceBusBusConfiguration(topologyConfiguration);
        }

        public IBusInstance CreateBus(IRegistrationContext<TContainerContext> context)
        {
            var configurator = new ServiceBusBusFactoryConfigurator(_busConfiguration);

            context.UseHealthCheck(configurator);

            _configure?.Invoke(context, configurator);

            var busControl = configurator.Build();

            return new ServiceBusBusInstance(busControl, _busConfiguration.HostConfiguration);
        }
    }
}
