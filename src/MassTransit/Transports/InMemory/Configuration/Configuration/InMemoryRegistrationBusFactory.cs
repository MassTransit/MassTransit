namespace MassTransit.Transports.InMemory.Configuration
{
    using System;
    using Configurators;
    using Context;
    using Microsoft.Extensions.Logging;
    using Registration;


    public class InMemoryRegistrationBusFactory<TContainerContext> :
        IRegistrationBusFactory<TContainerContext>
        where TContainerContext : class
    {
        readonly InMemoryBusConfiguration _busConfiguration;
        readonly Action<IRegistrationContext<TContainerContext>, IInMemoryBusFactoryConfigurator> _configure;

        public InMemoryRegistrationBusFactory(Uri baseAddress, Action<IRegistrationContext<TContainerContext>, IInMemoryBusFactoryConfigurator> configure)
        {
            _configure = configure;

            var topologyConfiguration = new InMemoryTopologyConfiguration(InMemoryBus.MessageTopology);
            _busConfiguration = new InMemoryBusConfiguration(topologyConfiguration, baseAddress);
        }

        public IBusInstance CreateBus(IRegistrationContext<TContainerContext> context)
        {
            var configurator = new InMemoryBusFactoryConfigurator(_busConfiguration);

            var loggerFactory = context.GetService<ILoggerFactory>();
            if (loggerFactory != null)
                LogContext.ConfigureCurrentLogContext(loggerFactory);

            context.UseHealthCheck(configurator);

            _configure?.Invoke(context, configurator);

            var busControl = configurator.Build();

            return new InMemoryBusInstance(busControl, _busConfiguration.HostConfiguration);
        }
    }
}
